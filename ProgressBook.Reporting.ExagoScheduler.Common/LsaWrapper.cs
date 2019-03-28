using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ProgressBook.Reporting.ExagoScheduler.Common
{

    public class LocalSecurityAuthorityController
    {
        private const int Access = (int)(
                                                LocalSecurityAuthorityAccessPolicy.POLICY_AUDIT_LOG_ADMIN |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_CREATE_ACCOUNT |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_CREATE_PRIVILEGE |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_CREATE_SECRET |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_GET_PRIVATE_INFORMATION |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_LOOKUP_NAMES |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_NOTIFICATION |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_SERVER_ADMIN |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_SET_AUDIT_REQUIREMENTS |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_SET_DEFAULT_QUOTA_LIMITS |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_TRUST_ADMIN |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_VIEW_AUDIT_INFORMATION |
                                                LocalSecurityAuthorityAccessPolicy.POLICY_VIEW_LOCAL_INFORMATION
                                            );

        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern UInt32 LsaOpenPolicy(ref LSA_UNICODE_STRING SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes, Int32 DesiredAccess, out IntPtr PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern uint LsaAddAccountRights(IntPtr PolicyHandle, IntPtr AccountSid, LSA_UNICODE_STRING[] UserRights, int CountOfRights);

        [DllImport("advapi32")]
        public static extern void FreeSid(IntPtr pSid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
        private static extern bool LookupAccountName(string lpSystemName, string lpAccountName, IntPtr psid, ref int cbsid, StringBuilder domainName, ref int cbdomainLength, ref int use);

        [DllImport("advapi32.dll")]
        private static extern bool IsValidSid(IntPtr pSid);

        [DllImport("advapi32.dll")]
        private static extern int LsaClose(IntPtr ObjectHandle);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("advapi32.dll")]
        private static extern int LsaNtStatusToWinError(uint status);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern uint LsaEnumerateAccountRights(IntPtr PolicyHandle, IntPtr AccountSid, out IntPtr UserRightsPtr, out int CountOfRights);

        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_OBJECT_ATTRIBUTES
        {
            public int Length;
            public IntPtr RootDirectory;
            public LSA_UNICODE_STRING ObjectName;
            public UInt32 Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [Flags]
        private enum LocalSecurityAuthorityAccessPolicy : long
        {
            POLICY_VIEW_LOCAL_INFORMATION = 0x00000001L,
            POLICY_VIEW_AUDIT_INFORMATION = 0x00000002L,
            POLICY_GET_PRIVATE_INFORMATION = 0x00000004L,
            POLICY_TRUST_ADMIN = 0x00000008L,
            POLICY_CREATE_ACCOUNT = 0x00000010L,
            POLICY_CREATE_SECRET = 0x00000020L,
            POLICY_CREATE_PRIVILEGE = 0x00000040L,
            POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080L,
            POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100L,
            POLICY_AUDIT_LOG_ADMIN = 0x00000200L,
            POLICY_SERVER_ADMIN = 0x00000400L,
            POLICY_LOOKUP_NAMES = 0x00000800L,
            POLICY_NOTIFICATION = 0x00001000L
        }

        public IList<string> GetRights(string accountName)
        {
            IList<string> rights = new List<string>();
            string errorMsg = string.Empty;

            long winErrorCode = 0;
            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            StringBuilder domainName = new StringBuilder();
            int nameSize = 0;
            int accountType = 0;

            LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType);

            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            if (!LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType))
            {
                winErrorCode = GetLastError();
                errorMsg = ("LookupAccountName failed: " + winErrorCode);
                throw new Win32Exception((int)winErrorCode, errorMsg);
            }
            else
            {
                LSA_UNICODE_STRING systemName = new LSA_UNICODE_STRING();

                IntPtr policyHandle = IntPtr.Zero;
                IntPtr userRightsPtr = IntPtr.Zero;
                int countOfRights = 0;

                LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObject();

                uint policyStatus = LsaOpenPolicy(ref systemName, ref objectAttributes, Access, out policyHandle);
                winErrorCode = LsaNtStatusToWinError(policyStatus);

                if (winErrorCode != 0)
                {
                    errorMsg = string.Format("OpenPolicy failed: {0}.", winErrorCode);
                    throw new Win32Exception((int)winErrorCode, errorMsg);
                }
                else
                {
                    try
                    {
                        uint result = LsaEnumerateAccountRights(policyHandle, sid, out userRightsPtr, out countOfRights);
                        winErrorCode = LsaNtStatusToWinError(result);
                        if (winErrorCode != 0)
                        {
                            if (winErrorCode == 2)
                            {
                                return new List<string>();
                            }
                            errorMsg = string.Format("LsaEnumerateAccountRights failed: {0}", winErrorCode);
                            throw new Win32Exception((int)winErrorCode, errorMsg);
                        }

                        Int32 ptr = userRightsPtr.ToInt32();
                        LSA_UNICODE_STRING userRight;

                        for (int i = 0; i < countOfRights; i++)
                        {
                            userRight = (LSA_UNICODE_STRING)Marshal.PtrToStructure(new IntPtr(ptr), typeof(LSA_UNICODE_STRING));
                            string userRightStr = Marshal.PtrToStringAuto(userRight.Buffer);
                            rights.Add(userRightStr);
                            ptr += Marshal.SizeOf(userRight);
                        }
                    }
                    finally
                    {
                        LsaClose(policyHandle);
                    }
                }
                FreeSid(sid);
            }
            return rights;
        }

        public void SetRightToAccount(string acctName, string privilegeName)
        {
            long winErrorCode = 0;
            string errorMsg = string.Empty;

            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            StringBuilder domainName = new StringBuilder();
            int nameSize = 0;
            int accountType = 0;

            LookupAccountName(String.Empty, acctName, sid, ref sidSize, domainName, ref nameSize, ref accountType);

            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            if (!LookupAccountName(string.Empty, acctName, sid, ref sidSize, domainName, ref nameSize, ref accountType))
            {
                winErrorCode = GetLastError();
                errorMsg = string.Format("LookupAccountName failed: {0}", winErrorCode);
                throw new Win32Exception((int)winErrorCode, errorMsg);
            }
            else
            {
                LSA_UNICODE_STRING systemName = new LSA_UNICODE_STRING();
                IntPtr policyHandle = IntPtr.Zero;
                LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObject();

                uint resultPolicy = LsaOpenPolicy(ref systemName, ref objectAttributes, Access, out policyHandle);
                winErrorCode = LsaNtStatusToWinError(resultPolicy);

                if (winErrorCode != 0)
                {
                    errorMsg = string.Format("OpenPolicy failed: {0} ", winErrorCode);
                    throw new Win32Exception((int)winErrorCode, errorMsg);
                }
                else
                {
                    try
                    {
                        LSA_UNICODE_STRING[] userRights = new LSA_UNICODE_STRING[1];
                        userRights[0] = new LSA_UNICODE_STRING();
                        userRights[0].Buffer = Marshal.StringToHGlobalUni(privilegeName);
                        userRights[0].Length = (UInt16)(privilegeName.Length * UnicodeEncoding.CharSize);
                        userRights[0].MaximumLength = (UInt16)((privilegeName.Length + 1) * UnicodeEncoding.CharSize);

                        uint res = LsaAddAccountRights(policyHandle, sid, userRights, 1);
                        winErrorCode = LsaNtStatusToWinError(res);
                        if (winErrorCode != 0)
                        {
                            errorMsg = string.Format("LsaAddAccountRights failed: {0}", winErrorCode);
                            throw new Win32Exception((int)winErrorCode, errorMsg);
                        }
                    }
                    finally
                    {
                        LsaClose(policyHandle);
                    }
                }
                FreeSid(sid);
            }
        }

        private static LSA_OBJECT_ATTRIBUTES CreateLSAObject()
        {
            LSA_OBJECT_ATTRIBUTES newInstance = new LSA_OBJECT_ATTRIBUTES();

            newInstance.Length = 0;
            newInstance.RootDirectory = IntPtr.Zero;
            newInstance.Attributes = 0;
            newInstance.SecurityDescriptor = IntPtr.Zero;
            newInstance.SecurityQualityOfService = IntPtr.Zero;

            return newInstance;
        }
    }


    public class LocalSecurityAuthorityRights
    {
        public const string LogonAsService = "SeServiceLogonRight";
    }

    public class LocalSecurityAuthorityWrapper
    {
        public static IList<string> GetRightsForAccount(string acctName)
        {
            return new LocalSecurityAuthorityController().GetRights(acctName);
        }

        public static void SetRightByAccount(string acctName, string privilegeName)
        {
            new LocalSecurityAuthorityController().SetRightToAccount(acctName, privilegeName);
        }
    }
}

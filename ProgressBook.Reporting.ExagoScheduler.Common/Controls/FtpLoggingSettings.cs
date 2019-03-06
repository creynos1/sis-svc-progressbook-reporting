using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    public partial class FtpLoggingSettings : BaseControl
    {
        public FtpLoggingSettings()
        {
            InitializeComponent();
        }

        public int NumberofLogDaysHistoryToMaintain
        {
            get { return int.Parse(numberOfDaysHistoryToMaintain.Text); }
            set { numberOfDaysHistoryToMaintain.Text = value.ToString(); }
        }

        public bool EnableFtpSessionLogging
        {
            get { return enableFtpSessionLogging.Checked; }
            set { enableFtpSessionLogging.Checked = value; }
        }

        public string FtpSessionLogPath
        {
            get { return ftpSessionLogPath.Text; }
            set { ftpSessionLogPath.Text = value; }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if(!((CheckBox)sender).Checked)
            {
                numberOfDaysHistoryToMaintain.Text = "0";
                numberOfDaysHistoryToMaintain.Enabled = false;

                ftpSessionLogPath.Text = "";
                ftpSessionLogPath.Enabled = false;

            }
            else
            {
                numberOfDaysHistoryToMaintain.Text = "10";
                numberOfDaysHistoryToMaintain.Enabled = true;

                ftpSessionLogPath.Text = @"c:\temp\logs\";
                ftpSessionLogPath.Enabled = true;
            }
        }
    }
}

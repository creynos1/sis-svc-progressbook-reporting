namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    using System;
    using System.Windows.Forms;

    public partial class AdvancedSettings : BaseControl
    {
        public AdvancedSettings()
        {
            InitializeComponent();

            flushTimeToolTip.SetToolTip(flushTimeTextBox, "The number of hours that a completed, deleted, or aborted job will be saved for viewing in the schedule reports manager. Set to 0 to flush jobs immediately upon completion.");
            emailRetryTimeToolTip.SetToolTip(emailRetryTimeTextBox, "In case and email fails to send, the number of minutes to wait before retrying.");

            reportPathTextBox.Validating += RequiredFieldHandler;
            defaultJobTimeoutTextBox.Validating += NumericRequiredFieldHandler;
            sleepTimeTextBox.Validating += NumericRequiredFieldHandler;
            simultaneousJobMaxUpDown.Validating += NumericRequiredFieldHandler;
            flushTimeTextBox.Validating += NumericRequiredFieldHandler;
            emailRetryTimeTextBox.Validating += NumericRequiredFieldHandler;
            errorEmailTextBox.Validating += OptionalEmailFieldHandler;

            ErrorProvider.SetIconPadding(reportPathTextBox, 29);
        }

        public string ReportPath
        {
            get { return reportPathTextBox.Text; }
            set { reportPathTextBox.Text = value; }
        }

        public string ErrorEmail
        {
            get { return errorEmailTextBox.Text; }
            set { errorEmailTextBox.Text = value; }
        }

        public int DefaultJobTimeout
        {
            get { return int.Parse(defaultJobTimeoutTextBox.Text); }
            set { defaultJobTimeoutTextBox.Text = value.ToString(); }
        }

        public int SleepTime
        {
            get { return int.Parse(sleepTimeTextBox.Text); }
            set { sleepTimeTextBox.Text = value.ToString(); }
        }

        public int SimultaneousJobMax
        {
            get { return int.Parse(simultaneousJobMaxUpDown.Text); }
            set { simultaneousJobMaxUpDown.Text = value.ToString(); }
        }

        public int FlushTime
        {
            get { return int.Parse(flushTimeTextBox.Text); }
            set { flushTimeTextBox.Text = value.ToString(); }
        }

        public int EmailRetryTime
        {
            get { return int.Parse(emailRetryTimeTextBox.Text); }
            set { emailRetryTimeTextBox.Text = value.ToString(); }
        }

        public bool StopScheduleOnError
        {
            get { return stopScheduleOnErrorCheckBox.Checked; }
            set { stopScheduleOnErrorCheckBox.Checked = value; }
        }

        private void disableAutoFlushCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            flushTimeTextBox.Enabled = !disableAutoFlushCheckBox.Checked;
            flushTimeTextBox.Text = disableAutoFlushCheckBox.Checked ? "-1" : "0";
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                reportPathTextBox.Text = dialog.SelectedPath;
            }
        }
    }
}
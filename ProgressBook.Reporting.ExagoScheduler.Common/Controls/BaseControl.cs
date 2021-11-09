namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class BaseControl : UserControl
    {
        public BaseControl()
        {
            InitializeComponent();
        }

        protected void RequiredFieldHandler(object sender, CancelEventArgs e)
        {
            var textbox = sender as Control;
            if (textbox == null)
                return;

            var isValid = !string.IsNullOrEmpty(textbox.Text.Trim());
            ErrorProvider.SetError(textbox, isValid ? string.Empty : "Required");
            e.Cancel = !isValid;
        }

        protected void RequiredEmailFieldHandler(object sender, CancelEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null)
                return;

            var isValid = !string.IsNullOrEmpty(textbox.Text.Trim()) && IsValidEmail(textbox.Text.Trim());
            ErrorProvider.SetError(textbox, isValid ? string.Empty : "Valid email required");
            e.Cancel = !isValid;
        }

        protected void NumericRequiredFieldHandler(object sender, CancelEventArgs e)
        {
            var textbox = sender as Control;
            if (textbox == null)
                return;

            int parsedValue;
            var isValid = !string.IsNullOrEmpty(textbox.Text) && int.TryParse(textbox.Text, out parsedValue);
            ErrorProvider.SetError(textbox, isValid ? string.Empty : "Numeric value required");
            e.Cancel = !isValid;
        }

        protected void OptionalEmailFieldHandler(object sender, CancelEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null)
                return;

            if (string.IsNullOrEmpty(textbox.Text.Trim()))
                return;

            var isValid = IsValidEmail(textbox.Text.Trim());
            ErrorProvider.SetError(textbox, isValid ? string.Empty : "Valid email required");
            e.Cancel = !isValid;
        }

        private bool IsValidEmail(string email)
        {
            // a more practical implementation of RFC 5322 from http://www.regular-expressions.info/email.html
            const string emailRegex = "[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?";

            return Regex.IsMatch(email, emailRegex);
        }
    }
}
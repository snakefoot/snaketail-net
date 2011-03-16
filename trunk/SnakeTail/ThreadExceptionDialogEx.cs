#region License statement
/* SnakeTail is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SnakeTail
{
    public partial class ThreadExceptionDialogEx : Form
    {
        string _emailHost;
        int _emailPort;
        string _emailUsername;
        string _emailPassword;
        string _emailToAddress;
        string _emailFromAddress;
        string _emailSubject;
        bool _emailSSL;

        public ThreadExceptionDialogEx(Exception exception)
        {
            InitializeComponent();

            Text = Application.ProductName + " - Error Report";

            ShowInTaskbar = Application.OpenForms.Count == 0;

            _pictureBox.Image = SystemIcons.Error.ToBitmap();

            _reportText.Text = "Unhandled exception has occurred in the application:";
            _reportText.Text += Environment.NewLine;
            _reportText.Text += Environment.NewLine + exception.Message;
            _reportText.Text += Environment.NewLine;
            _reportText.Text += Environment.NewLine + "Please press 'Send Report' to notify " + Application.CompanyName;

            _reportListBox.Items.Add(new ExceptionReport(exception));
            _reportListBox.Items.Add(new ApplicationReport());
            _reportListBox.Items.Add(new SystemReport());
        }

        public virtual string EmailHost { get { return _emailHost; } set { _emailHost = value; } }
        public virtual int EmailPort { get { return _emailPort; } set { _emailPort = value; } }
        public virtual string EmailUsername { get { return _emailUsername; } set { _emailUsername = value; } }
        public virtual string EmailPassword { get { return _emailPassword; } set { _emailPassword = value; } }
        public virtual string EmailToAddress { get { return _emailToAddress; } set { _emailToAddress = value; } }
        public virtual string EmailFromAddress { get { return _emailFromAddress; } set { _emailFromAddress = value; } }
        public virtual string EmailSubject { get { return _emailSubject; } set { _emailSubject = value; } }
        public virtual bool EmailSSL { get { return _emailSSL; } set { _emailSSL = value; } }
        public virtual string EmailBody
        {
            get
            {
                StringBuilder body = new StringBuilder();
                foreach (object item in _reportListBox.Items)
                {
                    ReportItem reportItem = item as ReportItem;
                    if (reportItem != null)
                        body.AppendLine(reportItem.Details);
                }
                return body.ToString();
            }
        }

        private void _detailsBtn_Click(object sender, EventArgs e)
        {
            _reportListBox.Visible = !_reportListBox.Visible;
            if (_reportListBox.Visible)
                this.Height += 150;
            else
                this.Height -= 150;
        }

        private void _abortBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void _sendReportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (new HourGlass(this))
                using (MailMessage msg = new MailMessage())
                {
                    msg.From = new MailAddress(EmailFromAddress);
                    foreach (string s in EmailToAddress.Split(";".ToCharArray()))
                    {
                        msg.To.Add(s);
                    }
                    if (String.IsNullOrEmpty(EmailSubject))
                        msg.Subject = Text;
                    else
                        msg.Subject = EmailSubject;

                    msg.Body = EmailBody;

                    SmtpClient smtp = null;
                    if (String.IsNullOrEmpty(EmailHost))
                    {
                        smtp = new SmtpClient();
                    }
                    else
                    {
                        if (EmailPort == 0)
                            smtp = new SmtpClient(EmailHost);
                        else
                            smtp = new SmtpClient(EmailHost, EmailPort);
                    }
                    if (String.IsNullOrEmpty(EmailUsername) && String.IsNullOrEmpty(EmailPassword))
                        smtp.UseDefaultCredentials = true;
                    else
                        smtp.Credentials = new System.Net.NetworkCredential(EmailUsername, EmailPassword);
                    smtp.EnableSsl = EmailSSL;
                    smtp.Send(msg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to send report");
            }
            DialogResult = DialogResult.Ignore;
            Close();
        }

        private void ThreadExceptionDialogEx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                Clipboard.SetText(_reportText.Text);
            }
        }

        private void _reportListBox_DoubleClick(object sender, EventArgs e)
        {
            ReportItem reportItem = _reportListBox.SelectedItem as ReportItem;
            if (reportItem != null)
                MessageBox.Show(reportItem.Details, reportItem.Title);
        }

        private void _reportListBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Return)
                e.IsInputKey = true;    // Steal the key-event from parent from
        }

        private void _reportListBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                _reportListBox_DoubleClick(sender, e);
                e.Handled = true;
            }
        }
    }

    class ReportItem
    {
        string _title;
        string _details;

        public ReportItem(string title, string details)
        {
            _title = title;
            _details = details;
        }

        public override string ToString()
        {
            return _title;
        }

        public string Details
        {
            get { return _details; }
        }

        public string Title
        {
            get { return _title; }
        }
    }

    class ExceptionReport : ReportItem
    {
        public ExceptionReport(Exception exception)
            : base("Exception Details", GetExceptionReport(exception))
        {
        }

        static string GetExceptionReport(Exception exception)
        {
            StringBuilder exceptionReport = new StringBuilder();
            exceptionReport.AppendLine("Message Stack:");
            Exception innerException = exception;
            while (innerException != null)
            {
                exceptionReport.Append("  ");
                exceptionReport.Append(innerException.Message);
                exceptionReport.Append(" (");
                exceptionReport.Append(innerException.GetType().ToString());
                exceptionReport.AppendLine(")");
                innerException = innerException.InnerException;
            }

            exceptionReport.AppendLine();
            exceptionReport.AppendLine("Stack Trace:");
            exceptionReport.Append(exception.StackTrace);
            return exceptionReport.ToString();
        }
    }

    class ApplicationReport : ReportItem
    {
        public ApplicationReport()
            : base("Application Details", GetApplicationDetails())
        {
        }

        static string GetApplicationDetails()
        {
            StringBuilder appReport = new StringBuilder();
            appReport.Append("Title: ");
            appReport.AppendLine(AssemblyTitle);
            appReport.Append("Version: ");
            appReport.AppendLine(Application.ProductVersion);
            appReport.Append("Product: ");
            appReport.AppendLine(Application.ProductName);
            appReport.Append("Company: ");
            appReport.AppendLine(Application.CompanyName);
            return appReport.ToString();
        }

        static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
    }

    class SystemReport : ReportItem
    {
        public SystemReport()
            :base("System Details", GetSystemDetails())
        {
        }

        static string GetSystemDetails()
        {
            StringBuilder systemReport = new StringBuilder();
            OperatingSystem os = Environment.OSVersion;
            systemReport.Append("Operating System: ");
            systemReport.AppendLine(os.VersionString);
            if (IntPtr.Size == 4)
                systemReport.AppendLine("Platform: 32 bit");
            else
                systemReport.AppendLine("Platform: 64 bit");
            systemReport.Append(".NET: ");
            systemReport.AppendLine(System.Environment.Version.ToString());
            systemReport.Append("Language: ");
            systemReport.AppendLine(Application.CurrentCulture.EnglishName);
            return systemReport.ToString();
        }
    }
}

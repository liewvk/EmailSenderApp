using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace EmailSenderApp
{
    public partial class Form1 : Form
    {
        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
        private bool ValidateInput(out int port)
        {
            port = 0;

            if (txtSmtpServer.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the SMTP server.",
                                "Missing SMTP Server",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtSmtpServer.Focus();
                return false;
            }

            if (!int.TryParse(txtPort.Text.Trim(), out port))
            {
                MessageBox.Show("Please enter a valid SMTP port.",
                                "Invalid Port",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPort.Focus();
                return false;
            }

            if (port <= 0)
            {
                MessageBox.Show("SMTP port must be greater than zero.",
                                "Invalid Port",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPort.Focus();
                return false;
            }

            if (txtSenderEmail.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the sender email.",
                                "Missing Sender Email",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtSenderEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtSenderEmail.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid sender email.",
                                "Invalid Sender Email",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtSenderEmail.Focus();
                return false;
            }

            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the sender password or app password.",
                                "Missing Password",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPassword.Focus();
                return false;
            }

            if (txtRecipientEmail.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the recipient email.",
                                "Missing Recipient Email",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtRecipientEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtRecipientEmail.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid recipient email.",
                                "Invalid Recipient Email",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtRecipientEmail.Focus();
                return false;
            }

            if (txtSubject.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the email subject.",
                                "Missing Subject",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtSubject.Focus();
                return false;
            }

            if (rtbMessage.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the email message.",
                                "Missing Message",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                rtbMessage.Focus();
                return false;
            }

            return true;
        }
        private void ResetAllFields()
        {
            txtSmtpServer.Clear();
            txtPort.Text = "587";
            txtSenderEmail.Clear();
            txtPassword.Clear();
            txtRecipientEmail.Clear();
            txtSubject.Clear();
            rtbMessage.Clear();

            chkShowPassword.Checked = false;
            lblStatus.Text = "Ready";

            txtSmtpServer.Focus();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            txtSmtpServer.Text = "your-real-smtp-server.com";
            txtPort.Text = "465";

            lblStatus.Text = "Ready";
            txtSmtpServer.Focus();

        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {

            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int port))
            {
                return;
            }

            string smtpServer = txtSmtpServer.Text.Trim();
            string senderEmail = txtSenderEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string recipientEmail = txtRecipientEmail.Text.Trim();
            string subject = txtSubject.Text.Trim();
            string messageBody = rtbMessage.Text.Trim();

            try
            {
                btnSend.Enabled = false;
                lblStatus.Text = "Sending email...";

                MimeMessage message = new MimeMessage();

                message.From.Add(new MailboxAddress("Sender", senderEmail));
                message.To.Add(new MailboxAddress("Recipient", recipientEmail));
                message.Subject = subject;

                message.Body = new TextPart("plain")
                {
                    Text = messageBody
                };

                using (SmtpClient client = new SmtpClient())
                {
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync(
    smtpServer,
    port,
    SecureSocketOptions.SslOnConnect);

                    await client.AuthenticateAsync(senderEmail, password);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }

                lblStatus.Text = "Email sent successfully.";

                MessageBox.Show("Email sent successfully.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Failed to send email.";

                MessageBox.Show("Failed to send email.\n\n" + ex.Message,
                                "Email Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                btnSend.Enabled = true;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRecipientEmail.Clear();
            txtSubject.Clear();
            rtbMessage.Clear();

            lblStatus.Text = "Ready";

            txtRecipientEmail.Focus();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?",
                                      "Confirm Exit",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }

        }
    }
}

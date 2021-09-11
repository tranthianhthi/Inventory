using System;
using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    public class ACFCEmailSender
    {
        private string smtpServer;
        private string smtpTarget;
        private int smtpPort;
        private bool useDefaultCredential;
        private bool enableSSL;
        private string username;
        private string password;

        public ACFCEmailSender(string smtpServer, string smtpTarget, int smtpPort, bool enableSSL = true)
        {
            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new ArgumentException("message", nameof(smtpServer));
            }

            if (string.IsNullOrEmpty(smtpTarget))
            {
                throw new ArgumentException("message", nameof(smtpTarget));
            }

            this.smtpServer = smtpServer;
            this.smtpTarget = smtpTarget;
            this.smtpPort = smtpPort;
            this.enableSSL = enableSSL;
            this.useDefaultCredential = false;
            this.username = "";
            this.password = "";
        }

        public ACFCEmailSender(string smtpServer, string smtpTarget, int smtpPort, string username, string password, bool enableSSL = true) : this(smtpServer, smtpTarget, smtpPort, enableSSL)
        {
            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new ArgumentException("message", nameof(smtpServer));
            }

            if (string.IsNullOrEmpty(smtpTarget))
            {
                throw new ArgumentException("message", nameof(smtpTarget));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("message", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("message", nameof(password));
            }

            this.useDefaultCredential = true;
            this.username = username;
            this.password = password;
        }



        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="from">Địa chỉ gửi</param>
        /// <param name="emailCredential">User name</param>
        /// <param name="password">Password</param>
        /// <param name="to">Danh sách mail nhận</param>
        /// <param name="subject">Chủ đề</param>
        /// <param name="mailmessage">Nội dung</param>

        /// <param name="fromAlias">Display name của email</param>
        /// <param name="cc">Danh sách cc</param>
        /// <param name="method">Smtp Delivery method</param>
        /// <param name="isBodyHTML">HTML body</param>

        public void SendMail(string from, string to, string subject, string mailmessage,
            bool isBodyHTML = false,
            string fromAlias = "", string cc = "", string emailCredential = "", string emailPassword = "",
            SmtpDeliveryMethod method = SmtpDeliveryMethod.Network,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Tls)
        {

            try
            {
                MailMessage mail = new MailMessage()
                {
                    Subject = subject,
                    Body = mailmessage,
                    IsBodyHtml = isBodyHTML,
                };
                mail.From = new MailAddress(from, fromAlias);// "it.support@acfc.com.vn", "ACFC IT Dept.");
                mail.To.Add(to);
                if (!string.IsNullOrEmpty(cc))
                    mail.CC.Add(cc);

                SmtpClient SmtpServer = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    TargetName = smtpTarget,
                    DeliveryMethod = method,
                    EnableSsl = enableSSL,
                    UseDefaultCredentials = useDefaultCredential

                }; // "smtp.office365.com");

                //mail.Subject = subject; //"ACFC Sale Recon Files Interfaced.";
                //mail.Body = mailmessage;

                ServicePointManager.SecurityProtocol = securityProtocol; //SecurityProtocolType.Tls;
                //SmtpServer.TargetName = smtpTarget; //"STARTTLS/smtp.office365.com";
                //SmtpServer.EnableSsl = true;

                //SmtpServer.Port = smtpPort;
                //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                //SmtpServer.UseDefaultCredentials = false;

                //mail.IsBodyHtml = false;
                if (useDefaultCredential)
                {
                    SmtpServer.Credentials = new NetworkCredential(this.username, this.password);
                }
                else
                {
                    SmtpServer.Credentials = new NetworkCredential(emailCredential, emailPassword);// "it.support@acfc.com.vn", "itACFC@2020");
                }
                
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

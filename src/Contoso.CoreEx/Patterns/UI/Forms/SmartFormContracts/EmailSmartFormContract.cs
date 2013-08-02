#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Collections;
using Contoso.Net.Mail;
namespace Contoso.Patterns.UI.Forms.SmartFormContracts
{
    /// <summary>
    /// IContract implementation that implements the ability to send a SmartForm-defined email.
    /// </summary>
    //+ dont like the mergetext function should just take a string and merge, like the one in POM
    public class EmailSmartFormContract : ISmartFormContract
    {
        private static object[] _defaultArgs = new object[] { null, string.Empty };
        private IEmailSmartFormBodyBuilder _bodyBuilder;
        private SmtpClientEx _smtpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartForm"/> class.
        /// </summary>
        public EmailSmartFormContract()
            : this(new EmailSmartFormBodyBuilder(), new SmtpClientEx()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSmartFormContract"/> class.
        /// </summary>
        /// <param name="bodyBuilder">The body builder.</param>
        public EmailSmartFormContract(IEmailSmartFormBodyBuilder bodyBuilder)
            : this(bodyBuilder, new SmtpClientEx()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSmartFormContract"/> class.
        /// </summary>
        /// <param name="smtpClient">The SMTP client.</param>
        public EmailSmartFormContract(SmtpClientEx smtpClient)
            : this(new EmailSmartFormBodyBuilder(), smtpClient) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSmartFormContract"/> class.
        /// </summary>
        /// <param name="bodyBuilder">The body builder.</param>
        /// <param name="smtpClient">The SMTP client.</param>
        public EmailSmartFormContract(IEmailSmartFormBodyBuilder bodyBuilder, SmtpClientEx smtpClient)
        {
            if (bodyBuilder == null)
                throw new ArgumentNullException("bodyBuilder");
            if (smtpClient == null)
                throw new ArgumentNullException("smtpClient");
            _bodyBuilder = bodyBuilder;
            _smtpClient = smtpClient;
        }

        /// <summary>
        /// Executes the specified method.
        /// </summary>
        /// <param name="smartForm">The smart form.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public object Execute(SmartForm smartForm, string method, params object[] args)
        {
            if (smartForm == null)
                throw new ArgumentNullException("smartForm");
            if (args == null || args.Length == 0)
                args = _defaultArgs;
            // send email
            var emailsSent = 0;
            var usedToEmails = new List<string>();
            for (var argIndex = 0; argIndex < args.Length; argIndex++)
            {
                var scopeKey = (args[argIndex] as string);
                if (scopeKey == null)
                    throw new ArgumentNullException(string.Format("args[{0}]", argIndex));
                if (scopeKey.Length > 0)
                    scopeKey += "::";
                foreach (var toEmail2 in smartForm[scopeKey + "toEmail"].Replace(";", ",").Split(','))
                {
                    var toEmail = toEmail2.Trim();
                    if (!string.IsNullOrEmpty(toEmail) && !usedToEmails.Contains(toEmail.ToLowerInvariant()))
                    {
                        var fromEmail = smartForm[scopeKey + "fromEmail"];
                        if (!string.IsNullOrEmpty(fromEmail))
                        {
                            // execute
                            var fromName = smartForm.CreateMergedText(scopeKey + "fromName");
                            var mailMessage = new MailMessage
                            {
                                From = (!string.IsNullOrEmpty(fromName) ? new MailAddress(fromEmail, fromName) : new MailAddress(fromEmail)),
                                Subject = smartForm.CreateMergedText(scopeKey + "subject"),
                            };
                            var replyToEmail = smartForm[scopeKey + "replyToEmail"];
                            if (!string.IsNullOrEmpty(replyToEmail))
                            {
                                var replyToName = smartForm[scopeKey + "replyToName"];
#if CLR4
                                mailMessage.ReplyToList.Add(!string.IsNullOrEmpty(replyToName) ? new MailAddress(replyToEmail, replyToName) : new MailAddress(replyToEmail));
#else
                                mailMessage.ReplyTo = (!string.IsNullOrEmpty(replyToName) ? new MailAddress(replyToEmail, replyToName) : new MailAddress(replyToEmail));
#endif
                            }
                            var toName = smartForm[scopeKey + "toName"];
                            mailMessage.To.Add(!string.IsNullOrEmpty(toName) ? new MailAddress(toEmail, toName) : new MailAddress(toEmail));
                            var ccEmail = smartForm[scopeKey + "ccEmail"];
                            if (!string.IsNullOrEmpty(ccEmail))
                                mailMessage.CC.Add(ccEmail.Replace(";", ","));
                            var bccEmail = smartForm[scopeKey + "bccEmail"];
                            if (!string.IsNullOrEmpty(bccEmail))
                                mailMessage.Bcc.Add(bccEmail.Replace(";", ","));
                            if (smartForm.ContainsKey(scopeKey + "attachments"))
                                HandleAttachments(smartForm, scopeKey + "attachments", mailMessage);
                            _bodyBuilder.Execute(smartForm, mailMessage, scopeKey);
                            if (!smartForm.ContainsKey(scopeKey + "mhtmlBody"))
                                _smtpClient.Send(mailMessage);
                            else
                                _smtpClient.SendMhtml(mailMessage);
                            emailsSent++;
                        }
                        // prevent resends
                        usedToEmails.Add(toEmail.ToLowerInvariant());
                    }
                }
                usedToEmails.Clear();
            }
            return emailsSent;
        }

        private static void HandleAttachments(SmartForm smartForm, string key, MailMessage mailMessage)
        {
            var attachments = smartForm.Get(key);
            if (attachments == null)
                return;
            // String
            var attachmentsAsString = (attachments as string);
            if (!string.IsNullOrEmpty(attachmentsAsString))
            {
                foreach (var attachment2 in attachmentsAsString.Split(';'))
                    if (!string.IsNullOrEmpty(attachment2) && File.Exists(attachment2))
                        mailMessage.Attachments.Add(new Attachment(attachment2));
                return;
            }
            // IEnumerable<string>
            var attachmentsAsEnumerable = (attachments as IEnumerable<string>);
            if (attachmentsAsEnumerable != null)
            {
                foreach (var attachment2 in attachmentsAsEnumerable)
                    if (!string.IsNullOrEmpty(attachment2) && File.Exists(attachment2))
                        mailMessage.Attachments.Add(new Attachment(attachment2));
                return;
            }
            // Attachment
            var attachment = (attachments as Attachment);
            if (attachment != null)
            {
                mailMessage.Attachments.Add(attachment);
                return;
            }
            // IEnumerable<Attachment>
            var attachmentsAsEnumerable2 = (attachments as IEnumerable<Attachment>);
            if (attachmentsAsEnumerable2 != null)
            {
                foreach (var attachment2 in attachmentsAsEnumerable2)
                    mailMessage.Attachments.Add(attachment2);
                return;
            }
        }
    }
}
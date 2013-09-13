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
using System.Linq;
using System.Net.Mail;

namespace Contoso.Net.Mail
{
    /// <summary>
    /// SmtpClientEx
    /// </summary>
    public class SmtpClientEx : SmtpClient
    {
        static readonly object _lock = new object();

        /// <summary>
        /// Sends the email MHTML.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void SendMhtml(MailMessage message)
        {
            lock (_lock)
                try
                {
                    var m = new CDO.Message();

                    // set message
                    var s = new ADODB.Stream();
                    s.Charset = "ascii";
                    s.Open();
                    s.WriteText(message.Body);
                    m.DataSource.OpenObject(s, "_Stream");

                    // set configuration
                    var f = m.Configuration.Fields;
                    switch (DeliveryMethod)
                    {
                        case SmtpDeliveryMethod.Network:
                            f["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = CDO.CdoSendUsing.cdoSendUsingPort;
                            f["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = Host;
                            break;
                        case SmtpDeliveryMethod.SpecifiedPickupDirectory:
                            f["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = CDO.CdoSendUsing.cdoSendUsingPickup;
                            f["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"].Value = PickupDirectoryLocation;
                            break;
                        default: throw new NotSupportedException();
                    }
                    f.Update();

                    // set other values
                    m.MimeFormatted = true;
                    m.Subject = message.Subject;
                    if (message.From != null) m.From = message.From.ToString();
                    var to = (message.To != null ? string.Join(",", message.To.Select(x => x.ToString()).ToArray()) : null);
                    if (!string.IsNullOrEmpty(to)) m.To = to;
                    var bcc = (message.Bcc != null ? string.Join(",", message.Bcc.Select(x => x.ToString()).ToArray()) : null);
                    if (!string.IsNullOrEmpty(to)) m.BCC = bcc;
                    var cc = (message.CC != null ? string.Join(",", message.CC.Select(x => x.ToString()).ToArray()) : null);
                    if (!string.IsNullOrEmpty(to)) m.CC = cc;
                    if (message.Attachments != null)
                        foreach (var attachment in message.Attachments.Where(x => x != null))
                            AddAttachement(m, attachment, false);
                    m.Send();
                }
                catch (Exception ex) { throw ex; }
        }

        private void AddAttachement(CDO.Message m, Attachment attachment, bool allowUnicode)
        {
            // set message
            var s = new ADODB.Stream();
            s.Charset = "UTF-8";
            s.Open();
            s.Type = ADODB.StreamTypeEnum.adTypeBinary;
            int bytesRead;
            var buffer = new byte[0x4400];
            using (var acs = attachment.ContentStream)
                while ((bytesRead = acs.Read(buffer, 0, 0x4400)) > 0)
                    if (bytesRead == 0x4400)
                        s.Write(buffer);
                    else
                    {
                        Array.Resize(ref buffer, bytesRead);
                        s.Write(buffer);
                        break;
                    }
            s.Flush();
            s.Position = 0;
            //
            var p = m.Attachments.Add();
            p.ContentMediaType = attachment.ContentType.ToString();
            p.ContentTransferEncoding = "base64";
            var ds = p.GetDecodedContentStream();
            s.CopyTo(ds);
            ds.Flush();
        }
    }
}

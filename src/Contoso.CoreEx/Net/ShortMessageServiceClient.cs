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
using System.Net;
using System.Net.Mail;
using System;
namespace Contoso.Net
{
    /// <summary>
    /// IShortMessageServiceClient
    /// </summary>
    public interface IShortMessageServiceClient
    {
        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <param name="ex">The ex.</param>
        void TrySend(ShortMessageServiceMessage m, out Exception ex);
    }

    /// <summary>
    /// ShortMessageServiceClient
    /// </summary>
    public class ShortMessageServiceClient : IShortMessageServiceClient
    {
        private SmtpClient _smtpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortMessageServiceClient"/> class.
        /// </summary>
        /// <param name="smtpClient">The SMTP client.</param>
        public ShortMessageServiceClient(SmtpClient smtpClient)
        {
            if (smtpClient == null)
                throw new ArgumentNullException("smtpClient");
            _smtpClient = smtpClient;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <param name="ex"></param>
        public virtual void TrySend(ShortMessageServiceMessage m, out Exception ex)
        {
            if (From == null)
                throw new NullReferenceException("From");
            var emailMessage = new MailMessage
            {
                From = From,
                IsBodyHtml = false,
                Body = m.Body,
            };
            emailMessage.To.Add(new MailAddress(GetCarrierEmail(m.CarrierID, m.Phone)));
            try { _smtpClient.Send(emailMessage); ex = null; }
            catch (SmtpException e) { ex = e; }
        }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public MailAddress From { get; set; }

        /// <summary>
        /// Gets the carrier email.
        /// </summary>
        /// <param name="carrierID">The carrier id.</param>
        /// <param name="phone">The phone.</param>
        /// <returns></returns>
        protected virtual string GetCarrierEmail(ShortMessageServiceCarrierID carrierID, string phone)
        {
            phone = StringEx.ExtractString.ExtractDigit(phone);
            switch (carrierID)
            {
                case ShortMessageServiceCarrierID.Verizon:
                    return phone + "@vtext.com";
                case ShortMessageServiceCarrierID.Sprint:
                    return phone + "@messaging.sprintpcs.com";
                case ShortMessageServiceCarrierID.ATT:
                    return phone + "@txt.att.net";
                case ShortMessageServiceCarrierID.TMobile:
                    return phone + "@tmomail.net";
                case ShortMessageServiceCarrierID.AllTel:
                    return phone + "@message.alltel.com";
                case ShortMessageServiceCarrierID.Cricket:
                    return phone + "@mms.mycricket.com";
                case ShortMessageServiceCarrierID.Cingular:
                    return phone + "@mobile.mycingular.com";
                case ShortMessageServiceCarrierID.Nextel:
                    return phone + "@messaging.nextel.com";
                case ShortMessageServiceCarrierID.Unicel:
                    return phone + "@utext.com";
                case ShortMessageServiceCarrierID.VirginMobile:
                    return phone + "@vmobl.com";
                case ShortMessageServiceCarrierID.NorthwestMissouriCellular:
                    return phone + "@mynwmcell.com";
                case ShortMessageServiceCarrierID.USCellular:
                    return phone + "@email.uscc.net";
                default:
                    throw new IndexOutOfRangeException(carrierID.ToString());
            }
        }
    }
}
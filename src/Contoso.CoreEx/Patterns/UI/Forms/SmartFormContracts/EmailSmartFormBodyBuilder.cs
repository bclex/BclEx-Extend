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
using System.Patterns.Schema;
using System.Net.Mime;
namespace Contoso.Patterns.UI.Forms.SmartFormContracts
{
    /// <summary>
    /// IEmailSmartFormBodyBuilder
    /// </summary>
    public interface IEmailSmartFormBodyBuilder
    {
        /// <summary>
        /// Executes the specified smart form.
        /// </summary>
        /// <param name="smartForm">The smart form.</param>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="scopeKey">The scope key.</param>
        void Execute(SmartForm smartForm, MailMessage emailMessage, string scopeKey);
    }

    /// <summary>
    /// IContract implementation that implements the ability to send a SmartForm-defined email.
    /// </summary>
    //+ dont like the mergetext function should just take a string and merge, like the one in POM
    public class EmailSmartFormBodyBuilder : IEmailSmartFormBodyBuilder
    {
        private HtmlSchemaBase _htmlSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSmartFormBodyBuilder"/> class.
        /// </summary>
        public EmailSmartFormBodyBuilder()
            : this(null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSmartFormBodyBuilder"/> class.
        /// </summary>
        /// <param name="htmlSchema">The HTML schema.</param>
        public EmailSmartFormBodyBuilder(HtmlSchemaBase htmlSchema)
        {
            //if (htmlSchema == null)
            //    throw new ArgumentNullException("htmlSchema");
            _htmlSchema = htmlSchema;
        }

        /// <summary>
        /// Executes the specified method.
        /// </summary>
        /// <param name="smartForm">The smart form.</param>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="scopeKey">The scope key.</param>
        public void Execute(SmartForm smartForm, MailMessage emailMessage, string scopeKey)
        {
            if (smartForm == null)
                throw new ArgumentNullException("smartForm");
            if (emailMessage == null)
                throw new ArgumentNullException("emailMessage");
            if (scopeKey == null)
                throw new ArgumentNullException("scopeKey");
            // htmlBody
            var htmlBody = smartForm.CreateMergedText(scopeKey + "htmlBody");
            if (!string.IsNullOrEmpty(htmlBody))
            {
                if (_htmlSchema != null && htmlBody.IndexOf("<html>", StringComparison.OrdinalIgnoreCase) == -1)
                    htmlBody = "<html><body>" + _htmlSchema.DecodeHtml(htmlBody, (int)HtmlSchemaBase.DecodeFlags.CrLfToBr) + "</body></html>";
                emailMessage.IsBodyHtml = true;
                emailMessage.Body = htmlBody;
                return;
            }
            // mhtmlBody
            var mhtmlBody = smartForm.CreateMergedText(scopeKey + "mhtmlBody");
            if (!string.IsNullOrEmpty(mhtmlBody))
            {
                emailMessage.Body = mhtmlBody;
                //var view = AlternateView.CreateAlternateViewFromString(mhtmlBody, new ContentType("text/plain"));
                //view.TransferEncoding = TransferEncoding.SevenBit;
                //emailMessage.AlternateViews.Add(view);
                return;
            }
            // textBody
            var textBody = smartForm.CreateMergedText(scopeKey + "textBody");
            {
                emailMessage.IsBodyHtml = false;
                emailMessage.Body = textBody;
            }
        }
    }
}
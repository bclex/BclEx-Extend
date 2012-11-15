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
using System.Collections;
using System.Net;
using System.Net.Mail;
using Contoso.Net;
namespace Contoso.Patterns.UI.Forms.SmartFormContracts
{
    /// <summary>
    /// IContract implementation that implements the ability to send a SmartForm-defined email.
    /// </summary>
    //+ dont like the mergetext function should just take a string and merge, like the one in POM

    public class SmsSmartFormContract : ISmartFormContract
    {
        private static object[] _defaultArgs = new object[] { null, string.Empty };
        private IShortMessageServiceClient _smsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartForm"/> class.
        /// </summary>
        public SmsSmartFormContract(IShortMessageServiceClient smsClient)
        {
            if (smsClient == null)
                throw new ArgumentNullException("smsClient");
            _smsClient = smsClient;
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
            if ((args == null) || (args.Length == 0))
                args = _defaultArgs;
            // send email
            int smsSent = 0;
            var usedPhoneIDs = new List<string>();
            for (int argIndex = 1; argIndex < args.Length; argIndex++)
            {
                string scopeKey = (args[argIndex] as string);
                if (scopeKey == null)
                    throw new ArgumentNullException(string.Format("args[{0}]", argIndex));
                if (scopeKey.Length > 0)
                    scopeKey += "::";
                foreach (string phone2 in smartForm[scopeKey + "phone"].Replace(";", ",").Split(','))
                {
                    string phone = phone2.Trim();
                    if (!string.IsNullOrEmpty(phone) && !usedPhoneIDs.Contains(phone.ToLowerInvariant()))
                    {
                        string carrierIDAsText = smartForm[scopeKey + "carrierID"];
                        ShortMessageServiceCarrierID carrierID;
                        if (!string.IsNullOrEmpty(carrierIDAsText) && EnumEx.TryParse<ShortMessageServiceCarrierID>(carrierIDAsText, out carrierID))
                        {
                            // execute
                            var message = new ShortMessageServiceMessage
                            {
                                Phone = phone,
                                CarrierID = carrierID,
                                Body = smartForm.CreateMergedText(scopeKey + "textBody"),
                            };
                            Exception ex;
                            _smsClient.TrySend(message, out ex);
                            smsSent++;
                        }
                        // prevent resends
                        usedPhoneIDs.Add(phone.ToLowerInvariant());
                    }
                }
                usedPhoneIDs.Clear();
            }
            return smsSent;
        }
    }
}
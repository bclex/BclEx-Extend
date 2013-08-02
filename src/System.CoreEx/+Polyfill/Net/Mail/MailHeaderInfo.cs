#region Foreign-License
// .Net40 Polyfill
#endregion
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.Mail
{
    internal static class MailHeaderInfo
    {
        private static readonly Dictionary<string, int> m_HeaderDictionary = new Dictionary<string, int>(0x21, StringComparer.OrdinalIgnoreCase);
        private static readonly HeaderInfo[] m_HeaderInfo = new HeaderInfo[] { 
            new HeaderInfo(MailHeaderID.Bcc, "Bcc", true, false, true), new HeaderInfo(MailHeaderID.Cc, "Cc", true, false, true), new HeaderInfo(MailHeaderID.Comments, "Comments", false, true, true), new HeaderInfo(MailHeaderID.ContentDescription, "Content-Description", true, true, true), new HeaderInfo(MailHeaderID.ContentDisposition, "Content-Disposition", true, true, true), new HeaderInfo(MailHeaderID.ContentID, "Content-ID", true, false, false), new HeaderInfo(MailHeaderID.ContentLocation, "Content-Location", true, false, true), new HeaderInfo(MailHeaderID.ContentTransferEncoding, "Content-Transfer-Encoding", true, false, false), new HeaderInfo(MailHeaderID.ContentType, "Content-Type", true, false, false), new HeaderInfo(MailHeaderID.Date, "Date", true, false, false), new HeaderInfo(MailHeaderID.From, "From", true, false, true), new HeaderInfo(MailHeaderID.Importance, "Importance", true, false, false), new HeaderInfo(MailHeaderID.InReplyTo, "In-Reply-To", true, true, false), new HeaderInfo(MailHeaderID.Keywords, "Keywords", false, true, true), new HeaderInfo(MailHeaderID.Max, "Max", false, true, false), new HeaderInfo(MailHeaderID.MessageID, "Message-ID", true, true, false), 
            new HeaderInfo(MailHeaderID.MimeVersion, "MIME-Version", true, false, false), new HeaderInfo(MailHeaderID.Priority, "Priority", true, false, false), new HeaderInfo(MailHeaderID.References, "References", true, true, false), new HeaderInfo(MailHeaderID.ReplyTo, "Reply-To", true, false, true), new HeaderInfo(MailHeaderID.ResentBcc, "Resent-Bcc", false, true, true), new HeaderInfo(MailHeaderID.ResentCc, "Resent-Cc", false, true, true), new HeaderInfo(MailHeaderID.ResentDate, "Resent-Date", false, true, false), new HeaderInfo(MailHeaderID.ResentFrom, "Resent-From", false, true, true), new HeaderInfo(MailHeaderID.ResentMessageID, "Resent-Message-ID", false, true, false), new HeaderInfo(MailHeaderID.ResentSender, "Resent-Sender", false, true, true), new HeaderInfo(MailHeaderID.ResentTo, "Resent-To", false, true, true), new HeaderInfo(MailHeaderID.Sender, "Sender", true, false, true), new HeaderInfo(MailHeaderID.Subject, "Subject", true, false, true), new HeaderInfo(MailHeaderID.To, "To", true, false, true), new HeaderInfo(MailHeaderID.XPriority, "X-Priority", true, false, false), new HeaderInfo(MailHeaderID.XReceiver, "X-Receiver", false, true, true), 
            new HeaderInfo(MailHeaderID.XSender, "X-Sender", true, true, true)
         };

        [StructLayout(LayoutKind.Sequential)]
        private struct HeaderInfo
        {
            public readonly string NormalizedName;
            public readonly bool IsSingleton;
            public readonly MailHeaderID ID;
            public readonly bool IsUserSettable;
            public readonly bool AllowsUnicode;
            public HeaderInfo(MailHeaderID id, string name, bool isSingleton, bool isUserSettable, bool allowsUnicode)
            {
                ID = id;
                NormalizedName = name;
                IsSingleton = isSingleton;
                IsUserSettable = isUserSettable;
                AllowsUnicode = allowsUnicode;
            }
        }

        static MailHeaderInfo()
        {
            for (int i = 0; i < m_HeaderInfo.Length; i++)
                m_HeaderDictionary.Add(m_HeaderInfo[i].NormalizedName, i);
        }

        internal static bool AllowsUnicode(string name)
        {
            int num;
            if (m_HeaderDictionary.TryGetValue(name, out num))
                return m_HeaderInfo[num].AllowsUnicode;
            return true;
        }

        internal static MailHeaderID GetID(string name)
        {
            int num;
            if (m_HeaderDictionary.TryGetValue(name, out num))
                return (MailHeaderID)num;
            return MailHeaderID.Unknown;
        }

        internal static string GetString(MailHeaderID id)
        {
            var rid = id;
            if (rid != MailHeaderID.Unknown && rid != (MailHeaderID.XSender | MailHeaderID.Cc))
                return m_HeaderInfo[(int)id].NormalizedName;
            return null;
        }

        internal static bool IsMatch(string name, MailHeaderID header)
        {
            int num;
            return (m_HeaderDictionary.TryGetValue(name, out num) && num == (int)header);
        }

        internal static bool IsSingleton(string name)
        {
            int num;
            return (m_HeaderDictionary.TryGetValue(name, out num) && m_HeaderInfo[num].IsSingleton);
        }

        internal static bool IsUserSettable(string name)
        {
            int num;
            if (m_HeaderDictionary.TryGetValue(name, out num))
                return m_HeaderInfo[num].IsUserSettable;
            return true;
        }

        internal static bool IsWellKnown(string name)
        {
            int num;
            return m_HeaderDictionary.TryGetValue(name, out num);
        }

        internal static string NormalizeCase(string name)
        {
            int num;
            if (m_HeaderDictionary.TryGetValue(name, out num))
                return m_HeaderInfo[num].NormalizedName;
            return name;
        }
    }
}


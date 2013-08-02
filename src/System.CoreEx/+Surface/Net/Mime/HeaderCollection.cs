#region Foreign-License
// .Net40 Surface
#endregion
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace System.Net.Mime
{
    internal class HeaderCollection : NameValueCollection
    {
        private MimeBasePart _part = null;

        internal HeaderCollection()
            : base(StringComparer.OrdinalIgnoreCase) { }

        public override void Add(string name, string value)
        {
            //if (Logging.On)
            //    Logging.PrintInfo(Logging.Web, this, "Add", name.ToString() + "=" + value.ToString());
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            if (name == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "name" }), "name");
            if (value == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "value" }), "name");
            MailBnfHelper.ValidateHeaderName(name);
            name = MailHeaderInfo.NormalizeCase(name);
            MailHeaderID iD = MailHeaderInfo.GetID(name);
            value = value.Normalize(NormalizationForm.FormC);
            if (iD == MailHeaderID.ContentType && _part != null)
                _part.ContentType.Set(value.ToLower(CultureInfo.InvariantCulture), this);
            else if (iD == MailHeaderID.ContentDisposition && _part is MimePart)
                ((MimePart)_part).ContentDisposition.Set(value.ToLower(CultureInfo.InvariantCulture), this);
            else
                InternalAdd(name, value);
        }

        public override string Get(string name)
        {
            //if (Logging.On)
            //    Logging.PrintInfo(Logging.Web, this, "Get", name);
            if (name == null)
                throw new ArgumentNullException("name");
            if (name == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "name" }), "name");
            var iD = MailHeaderInfo.GetID(name);
            if (iD == MailHeaderID.ContentType && _part != null)
                _part.ContentType.PersistIfNeeded(this, false);
            else if (iD == MailHeaderID.ContentDisposition && _part is MimePart)
                ((MimePart)_part).ContentDisposition.PersistIfNeeded(this, false);
            return base.Get(name);
        }

        public override string[] GetValues(string name)
        {
            //if (Logging.On)
            //    Logging.PrintInfo(Logging.Web, this, "Get", name);
            if (name == null)
                throw new ArgumentNullException("name");
            if (name == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "name" }), "name");
            var iD = MailHeaderInfo.GetID(name);
            if (iD == MailHeaderID.ContentType && _part != null)
                _part.ContentType.PersistIfNeeded(this, false);
            else if (iD == MailHeaderID.ContentDisposition && _part is MimePart)
                ((MimePart)_part).ContentDisposition.PersistIfNeeded(this, false);
            return base.GetValues(name);
        }

        internal void InternalAdd(string name, string value)
        {
            if (MailHeaderInfo.IsSingleton(name))
                base.Set(name, value);
            else
                base.Add(name, value);
        }

        internal void InternalRemove(string name)
        {
            base.Remove(name);
        }

        internal void InternalSet(string name, string value)
        {
            base.Set(name, value);
        }

        public override void Remove(string name)
        {
            //if (Logging.On)
            //    Logging.PrintInfo(Logging.Web, this, "Remove", name);
            if (name == null)
                throw new ArgumentNullException("name");
            if (name == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "name" }), "name");
            var iD = MailHeaderInfo.GetID(name);
            if (iD == MailHeaderID.ContentType && _part != null)
                _part.ContentType = null;
            else if (iD == MailHeaderID.ContentDisposition && _part is MimePart)
                ((MimePart)_part).ContentDisposition = null;
            base.Remove(name);
        }

        public override void Set(string name, string value)
        {
            //if (Logging.On)
            //    Logging.PrintInfo(Logging.Web, this, "Set", name.ToString() + "=" + value.ToString());
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            if (name == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "name" }), "name");
            if (value == string.Empty)
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "value" }), "name");
            if (!MimeBasePart.IsAscii(name, false))
                throw new FormatException(SR.GetString("InvalidHeaderName"));
            name = MailHeaderInfo.NormalizeCase(name);
            MailHeaderID iD = MailHeaderInfo.GetID(name);
            value = value.Normalize(NormalizationForm.FormC);
            if (iD == MailHeaderID.ContentType && _part != null)
                _part.ContentType.Set(value.ToLower(CultureInfo.InvariantCulture), this);
            else if (iD == MailHeaderID.ContentDisposition && _part is MimePart)
                ((MimePart)_part).ContentDisposition.Set(value.ToLower(CultureInfo.InvariantCulture), this);
            else
                base.Set(name, value);
        }
    }
}


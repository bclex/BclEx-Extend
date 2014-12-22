using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Win32.TaskScheduler
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Triggers", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task", IsNullable = false)]
    public class LocalTriggerCollection : List<Trigger>, IXmlSerializable
    {
        static readonly ICollection<Trigger> Daily = new LocalTriggerCollection { new DailyTrigger(1) { StartBoundary = DateTime.MinValue } };
        static readonly ICollection<Trigger> Weekly = new LocalTriggerCollection { new WeeklyTrigger(DaysOfTheWeek.Sunday, 1) { StartBoundary = DateTime.MinValue } };
        static readonly ICollection<Trigger> Monthly = new LocalTriggerCollection { new MonthlyTrigger(1, MonthsOfTheYear.AllMonths) { StartBoundary = DateTime.MinValue } };
        static readonly ICollection<Trigger> LastDayOfMonth = new LocalTriggerCollection { new MonthlyTrigger(1, MonthsOfTheYear.AllMonths) { StartBoundary = DateTime.MinValue, RunOnLastDayOfMonth = true } };
        static readonly ICollection<Trigger> Quarterly = new LocalTriggerCollection { new MonthlyTrigger(1, MonthsOfTheYear.January | MonthsOfTheYear.April | MonthsOfTheYear.July | MonthsOfTheYear.October) { StartBoundary = DateTime.MinValue } };
        static readonly ICollection<Trigger> Yearly = new LocalTriggerCollection { new MonthlyTrigger(1, MonthsOfTheYear.January) { StartBoundary = DateTime.MinValue } };

        /// <summary>
        /// Parses the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">s</exception>
        public static ICollection<Trigger> Parse(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            else if (string.Equals("daily", s, StringComparison.OrdinalIgnoreCase)) return Daily;
            else if (string.Equals("weekly", s, StringComparison.OrdinalIgnoreCase)) return Weekly;
            else if (string.Equals("monthly", s, StringComparison.OrdinalIgnoreCase)) return Monthly;
            else if (string.Equals("lastdayofmonth", s, StringComparison.OrdinalIgnoreCase)) return LastDayOfMonth;
            else if (string.Equals("quarterly", s, StringComparison.OrdinalIgnoreCase)) return Quarterly;
            else if (string.Equals("yearly", s, StringComparison.OrdinalIgnoreCase)) return Yearly;
            else if (s[0] == '<')
            {
                var triggers = new LocalTriggerCollection();
                using (var reader = new StringReader(s))
                using (var reader2 = XmlReader.Create(reader))
                {
                    reader2.MoveToContent();
                    ((IXmlSerializable)triggers).ReadXml(reader2);
                }
                return triggers;
            }
            throw new ArgumentOutOfRangeException("s", s);
        }

        internal static Trigger CreateTrigger(TaskTriggerType taskTriggerType)
        {
            switch (taskTriggerType)
            {
                case TaskTriggerType.Event: return new EventTrigger();
                case TaskTriggerType.Time: return new TimeTrigger();
                case TaskTriggerType.Daily: return new DailyTrigger();
                case TaskTriggerType.Weekly: return new WeeklyTrigger();
                case TaskTriggerType.Monthly: return new MonthlyTrigger();
                case TaskTriggerType.MonthlyDOW: return new MonthlyDOWTrigger();
                case TaskTriggerType.Idle: return new IdleTrigger();
                case TaskTriggerType.Registration: return new RegistrationTrigger();
                case TaskTriggerType.Boot: return new BootTrigger();
                case TaskTriggerType.Logon: return new LogonTrigger();
                case TaskTriggerType.SessionStateChange: return new SessionStateChangeTrigger();
                case TaskTriggerType.Custom: throw new NotSupportedException();
            }
            return null;
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="taskTriggerType">Type of the task trigger.</param>
        /// <returns></returns>
        public Trigger AddNew(TaskTriggerType taskTriggerType)
        {
            var trigger = CreateTrigger(taskTriggerType);
            trigger.StartBoundary = DateTime.MinValue;
            Add(trigger);
            return trigger;
        }

        #region IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Triggers", "http://schemas.microsoft.com/windows/2004/02/mit/task");
            while (reader.MoveToContent() == XmlNodeType.Element)
                switch (reader.LocalName)
                {
                    case "BootTrigger": XmlSerializationHelper.ReadObject(reader, AddNew(TaskTriggerType.Boot)); break;
                    case "IdleTrigger": XmlSerializationHelper.ReadObject(reader, AddNew(TaskTriggerType.Idle)); break;
                    case "TimeTrigger": XmlSerializationHelper.ReadObject(reader, AddNew(TaskTriggerType.Time)); break;
                    case "LogonTrigger": XmlSerializationHelper.ReadObject(reader, AddNew(TaskTriggerType.Logon)); break;
                    case "CalendarTrigger": Add(CalendarTrigger.GetTriggerFromXml(reader)); break;
                    default: reader.Skip(); break;
                }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var trigger in this)
                XmlSerializationHelper.WriteObject(writer, trigger, null);
        }

        #endregion
    }
}
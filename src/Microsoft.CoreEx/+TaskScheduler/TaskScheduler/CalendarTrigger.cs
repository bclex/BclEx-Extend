using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Win32.TaskScheduler
{
    internal static class CalendarTrigger
    {
        public static Trigger GetTriggerFromXml(XmlReader reader)
        {
            Trigger trigger = null;
            var xml = reader.ReadOuterXml();
            var match = System.Text.RegularExpressions.Regex.Match(xml, @"\<(?<T>ScheduleBy.+)\>");
            if (match.Success && match.Groups.Count == 2)
            {
                switch (match.Groups[1].Value)
                {
                    case "ScheduleByDay": trigger = new DailyTrigger(); break;
                    case "ScheduleByWeek": trigger = new WeeklyTrigger(); break;
                    case "ScheduleByMonth": trigger = new MonthlyTrigger(); break;
                    case "ScheduleByMonthDayOfWeek": trigger = new MonthlyDOWTrigger(); break;
                    default: break;
                }
                if (trigger != null)
                    using (var ms = new StringReader(xml))
                    using (var xr = XmlReader.Create(ms))
                        ((IXmlSerializable)trigger).ReadXml(xr);
            }
            return trigger;
        }

        public static void ReadXml(XmlReader reader, Trigger trigger, Action<XmlReader> readerProc)
        {
            reader.ReadStartElement("CalendarTrigger", "http://schemas.microsoft.com/windows/2004/02/mit/task");
            while (reader.MoveToContent() == XmlNodeType.Element)
                switch (reader.LocalName)
                {
                    case "Enabled": trigger.Enabled = reader.ReadElementContentAsBoolean(); break;
                    case "EndBoundary": trigger.EndBoundary = reader.ReadElementContentAsDateTime(); break;
                    case "RandomDelay": ((ITriggerDelay)trigger).Delay = StringToTimeSpan(reader.ReadElementContentAsString()); break;
                    case "StartBoundary": trigger.StartBoundary = reader.ReadElementContentAsDateTime(); break;
                    case "Repetition": XmlSerializationHelper.ReadObject(reader, trigger.Repetition); break;
                    case "ScheduleByDay":
                    case "ScheduleByWeek":
                    case "ScheduleByMonth":
                    case "ScheduleByMonthDayOfWeek": readerProc(reader); break;
                    default: reader.Skip(); break;
                }
            reader.ReadEndElement();
        }

        public static void WriteXml(XmlWriter writer, Trigger trigger, Action<XmlWriter> writerProc)
        {
            if (!trigger.Enabled)
                writer.WriteElementString("Enabled", XmlConvert.ToString(trigger.Enabled));
            if (trigger.EndBoundary != DateTime.MaxValue)
                writer.WriteElementString("EndBoundary", XmlConvert.ToString(trigger.EndBoundary, XmlDateTimeSerializationMode.RoundtripKind));
            XmlSerializationHelper.WriteObject(writer, trigger.Repetition, null);
            writer.WriteElementString("StartBoundary", XmlConvert.ToString(trigger.StartBoundary, XmlDateTimeSerializationMode.RoundtripKind));
            writerProc(writer);
        }

        public static TimeSpan StringToTimeSpan(string input)
        {
            if (!string.IsNullOrEmpty(input))
                try { return XmlConvert.ToTimeSpan(input); }
                catch { }
            return TimeSpan.Zero;
        }
    }
}
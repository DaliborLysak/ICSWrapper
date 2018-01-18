using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace ICSWrapper
{
    [ICSMember(Name = "VEVENT")]
    public class ICSEvent : ICSMember
    {
        private const string StartKeyName = "DTSTART;VALUE=DATE";
        private const string EndKeyName = "DTEND;VALUE=DATE";
        private const string SummaryKeyName = "SUMMARY";
        private const string DTStampKeyName = "DTSTAMP";
        private const string UIDKeyName = "UID";

        public ICSEvent()
        {
            Definition = new Dictionary<string, string>()
            {
                [StartKeyName] = String.Empty,
                [EndKeyName] = String.Empty,
                ["DTSTAMP"] = String.Empty,
                ["UID"] = String.Empty,
                [SummaryKeyName] = String.Empty
            };
        }

        protected override List<string> ExportDefinition()
        {
            var result = base.ExportDefinition();
            result.AddRange(Definition.Keys.ToList().Select(k => GetDefinitionItem(k)));
            return result;
        }

        protected bool IsValid()
        {
            var valid = Definition.Values.Where(v => !String.IsNullOrEmpty(v)).Count() == Definition.Count();
            Trace.TraceInformation($"Event is {(valid ? String.Empty : "not")} valid");
            return valid;
        }

        private DateTime ParseDate(string key)
        {
            DateTime date;
            DateTime.TryParseExact(Definition[key], "yyyyMMdd", null, DateTimeStyles.None, out date);
            return date;
        }

        public DateTime StartDate { get { return ParseDate(StartKeyName); } set { Definition[StartKeyName] = value.ToString(); } }

        public DateTime EndDate { get { return ParseDate(EndKeyName); } set { Definition[EndKeyName] = value.ToString(); } }

        public string Summary { get { return Definition[SummaryKeyName]; } set { Definition[SummaryKeyName] = value; } }
    }
}

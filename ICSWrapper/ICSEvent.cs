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

        public DateTime StartDate { get { return ParseDate(StartKeyName); } }

        public DateTime EndDate { get { return ParseDate(EndKeyName); } }

        public string Summary { get { return Definition[SummaryKeyName]; } }
    }
}

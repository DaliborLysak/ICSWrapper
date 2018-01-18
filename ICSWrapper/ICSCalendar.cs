using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ICSWrapper
{
    [ICSMember(Name = "VCALENDAR")]
    public class ICSCalendar : ICSMember
    {
        public ICSCalendar()
        {
            Trace.Listeners.Add(new DefaultTraceListener() { TraceOutputOptions = TraceOptions.DateTime });

            Definition = new Dictionary<string, string>()
            {
                ["PRODID"] = String.Empty,
                ["VERSION"] = String.Empty,
                ["CALSCALE"] = String.Empty,
                ["METHOD"] = String.Empty,
                ["X-WR-CALNAME"] = String.Empty,
                ["X-WR-TIMEZONE"] = String.Empty,
                ["X-WR-CALDESC"] = String.Empty,
            };
        }

        public List<ICSEvent> Events = new List<ICSEvent>();

        public ICSCalendar Get(string path)
        {
            if (File.Exists(path))
            {
                var data = File.ReadAllLines(path);
                Trace.TraceInformation($"File {path} loaded.");
                Parse(data);
            }
            else
            {
                Trace.TraceInformation($"File {path} not exist.");
            }

            return this;
        }

        public void Set(string path)
        {
            File.WriteAllLines(path, Export().ToArray());
            Trace.TraceInformation($"File {path} saved.");
        }

        protected override List<string> ExportDefinition()
        {
            var result = base.ExportDefinition();
            result.AddRange(Definition.Keys.ToList().Select(k => GetDefinitionItem(k)));
            Events.ForEach(e => result.AddRange(e.Export()));
            return result;
        }

        private void Parse(string[] data)
        {
            if (IsValid(data[0], data[data.Length - 1]))
            {
                Events.Clear();
                int index = 1;
                ICSEvent icsEvent = null;
                while (index < data.Length - 1)
                {
                    var line = data[index];
                    (var key, var value) = ReadLine(line);
                    if (WrapState == LineWrapType.Definition)
                    {
                        Read(key, value);
                    }
                    if (WrapState == LineWrapType.Timezone)
                    {
                        // Timezone not supported
                    }
                    if (WrapState == LineWrapType.Alarm)
                    {
                        if (icsEvent != null)
                        {
                            Events.Add(icsEvent);
                            icsEvent = null;
                        }
                        // Alarm not supported
                    }
                    if (WrapState == LineWrapType.Event)
                    {
                        // only processing easy events with no cycles and hiearchy and with defined start and end
                        if (icsEvent == null)
                        {
                            icsEvent = new ICSEvent();
                        }
                        icsEvent.Read(key, value);
                        if (!icsEvent.IsEndLine(line))
                        {
                            icsEvent.Read(key, value);
                        }
                        else
                        {
                            Events.Add(icsEvent);
                            icsEvent = null;
                        }
                    }
                    index++;
                }
            }
        }

        protected bool IsValid(string startLine, string endLine)
        {
            var valid = startLine.Equals(StartLine) && IsEndLine(endLine);
            Trace.TraceInformation($"Calendar is {(valid ? String.Empty : "not")} valid");
            return valid;
        }
    }
}

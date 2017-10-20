﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

        private void Parse(string[] data)
        {
            if (IsValid(data[0], data[data.Length - 1]))
            {
                Events.Clear();
                int index = 1;
                ICSEvent icsEvent = null;
                while (index < data.Length - 2)
                {
                    var line = data[index];
                    (var key, var value) = ReadLine(line);
                    if (WrapState == LineWrapType.Definition)
                    {
                        Read(key, value);
                    }
                    if (WrapState == LineWrapType.Timezone)
                    {
                        // not supported
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
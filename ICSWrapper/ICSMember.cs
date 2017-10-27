using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSWrapper
{
    public class ICSMember
    {
        public ICSMember()
        {
            RecordTypes = new Dictionary<string, LineWrapType>()
            {
                [GetName(typeof(ICSEvent))] = LineWrapType.Event,
                [GetName(typeof(ICSTimezone))] = LineWrapType.Timezone,
                [GetName(typeof(ICSAlarm))] = LineWrapType.Alarm,
            };
        }

        protected const string StartLinePattern = "BEGIN";
        protected const string EndLinePattern = "END";

        private (string key, string value) SplitLine(string line)
        {
            var items = line.Split(':');
            return (items[0], items.Length > 1 ? items[1] : String.Empty);
        }

        protected (string key, string value) ReadLine(string line)
        {
            (var key, var value) = SplitLine(line);
            if (key.Equals(StartLinePattern) && RecordTypes.ContainsKey(value))
            {
                WrapState = RecordTypes[value];
                Trace.WriteLine($"WrapState = {WrapState.ToString()} ");
            }

            return (key, value);
        }

        public bool IsEndLine(string line)
        {
            return EndLine.Equals(line);
        }

        protected LineWrapType WrapState = LineWrapType.Definition;

        protected enum LineWrapType
        {
            Definition,
            Event,
            Timezone,
            Alarm
        }

        private Dictionary<string, LineWrapType> RecordTypes;

        protected string GetName(Type type)
        {
            string name = null;
            var memberType = typeof(ICSMemberAttribute);
            if (Attribute.IsDefined(type, memberType))
            {
                var attrName = Attribute.GetCustomAttribute(type, memberType) as ICSMemberAttribute;
                if (attrName != null)
                {
                    name = attrName?.Name ?? String.Empty;
                }
                else
                {
                    Trace.TraceInformation($"The description could not be retrieved for {nameof(type)}.");
                }

            }
            else
            {
                Trace.TraceInformation($"The AssemblyDescription attribute is not defined for assembly {type}.");
            }

            return name;
        }

        protected string StartLine { get { return $"{StartLinePattern}:{GetName(GetType())}"; } }
        protected string EndLine { get { return $"{EndLinePattern}:{GetName(GetType())}"; } }

        protected Dictionary<string, string> Definition;

        public void Read(string key, string value)
        {
            if (Definition.ContainsKey(key))
            {
                Definition[key] = value;
                Trace.WriteLine($"{key} = {value}");
            }
            else
            {
                Trace.TraceInformation($"{key} is missing.");
            }
        }
    }
}

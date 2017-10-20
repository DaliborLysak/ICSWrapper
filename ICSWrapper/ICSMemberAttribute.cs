using System;
using System.Diagnostics;

namespace ICSWrapper
{
    [DebuggerDisplay("{ICSMemberAttribute.Name}")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ICSMemberAttribute : Attribute
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
    }
}

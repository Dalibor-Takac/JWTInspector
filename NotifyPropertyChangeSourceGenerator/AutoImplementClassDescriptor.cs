using System;
using System.Collections.Generic;
using System.Text;

namespace NotifyPropertyChangeSourceGenerator
{
    public class AutoImplementClassDescriptor
    {
        public string ClassModifiers { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public IEnumerable<AutoImplementFieldDescriptor> AutoImplementFields { get; set; }
        public IEnumerable<string> ClassUsings { get; set; }
        public bool NeedsProtectedEventTrigerMethod { get; set; }
    }

    public class AutoImplementFieldDescriptor
    {
        public string PropertyAccessibility { get; set; }
        public string PropertyName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
    }
}

using Quidgest.Persistence;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSGenio.business.async
{
    public class AsyncProcessArgument
    {
        public AsyncProcessArgument(ICollection<string> value)
        {
            Value = value;
            Hide = false;
        }

        public ICollection<String> Value { get; set; }
        public string Name { get; set; }
        public FieldRef Field { get; set; }
        public bool Hide { get; set; }
        public bool Docum { get; set; }
        public ArrayInfo Array { get; set; }
        public string KeyName { get; set; }
        public MethodInfo Function { get; set; }

    }

}

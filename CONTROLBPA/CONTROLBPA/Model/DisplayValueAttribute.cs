using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLBPA.Model
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DisplayValueAttribute : Attribute
    {
        public DisplayValueAttribute(string value__1)
        {
            Value = value__1;
        }

        public string Value
        {
            get
            {
                return m_Value;
            }
            private set
            {
                m_Value = value;
            }
        }
        private string m_Value;
    }

}

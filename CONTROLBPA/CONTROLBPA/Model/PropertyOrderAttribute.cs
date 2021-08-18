using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace CONTROLBPA.Model
{
    public enum UsageContextEnum
    {
        Alphabetical,
        Categorized,
        Both
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class PropertyOrderAttribute : Attribute
    {
        public int Order
        {
            get
            {
                return m_Order;
            }
            set
            {
                m_Order = value;
            }
        }
        private int m_Order;

        public UsageContextEnum UsageContext
        {
            get
            {
                return m_UsageContext;
            }
            set
            {
                m_UsageContext = value;
            }
        }
        private UsageContextEnum m_UsageContext;

        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        public PropertyOrderAttribute(int order) : this(order, UsageContextEnum.Both)
        {
        }

        public PropertyOrderAttribute(int order__1, UsageContextEnum usageContext__2)
        {
            Order = order__1;
            UsageContext = usageContext__2;
        }
    }

}

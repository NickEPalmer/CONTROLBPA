using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLBPA
{
    public class modCommondefs
    {
        public enum ItemStatus : int
        {
            ItemCompliant = 1,
            ItemWarning = 2,
            ItemError = 3
        }

        public enum ItemCategory
        {
            Configuration = 1,
            Performance = 2
        }

        public enum VisiblePages
        {
            Home = 1,
            Parameters = 2,
            Progress = 3,
            Report = 4
        }

    }
}

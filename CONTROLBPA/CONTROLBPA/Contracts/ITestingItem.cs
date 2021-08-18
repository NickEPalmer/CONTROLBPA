using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CONTROLBPA.Model;

namespace CONTROLBPA.Contracts
{
    public interface ITestingItem
    {
        BaseLineConfigItem Run();
    }
}

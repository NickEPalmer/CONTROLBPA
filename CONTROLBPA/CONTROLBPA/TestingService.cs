using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CONTROLBPA.Contracts;
using CONTROLBPA.Model;

namespace CONTROLBPA
{
    public class TestingService
    {
        public BaseLineConfigItem RunTest(ITestingItem testingItem)
        {
            System.Threading.Thread.Sleep(500);
            return testingItem.Run();
        }
    }
}

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
using CONTROLBPA;
using Microsoft.VisualBasic;
using CONTROLBPA.Contracts;
using CONTROLBPA.Model;

namespace CONTROLBPA.Testers
{
    public class CheckCONTROLVersion : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            string infoDate;
            string infoVersion;
            modCommonUtil.BuildInfo = string.Empty;
            string build;

            res = new BaseLineConfigItem("CONTROL® Version");
            res.Category = modCommondefs.ItemCategory.Configuration;

            if (File.Exists(modCommonUtil.ControlDir + @"\Version.txt"))
            {
                try
                {
                    // Open the file using a stream reader.
                    using (StreamReader sr = new StreamReader(modCommonUtil.ControlDir + @"\Version.txt"))
                    {
                        infoDate = sr.ReadLine();
                        infoVersion = sr.ReadLine();
                        modCommonUtil.BuildInfo = "Build Number ( " + infoVersion + ") Build Date (" + infoDate + ")";
                    }
                }
                catch (Exception e)
                {
                    modCommonUtil.BuildInfo = string.Empty;
                }
            }

            build = modCommonUtil.BuildInfo;
            if (build.Trim().Length > 0)
                res.Issue = "Detected " + modCommonUtil.CONTROLName + " version: " + build.Trim();
            else
            {
                res.Status = modCommondefs.ItemStatus.ItemWarning;
                res.Issue = "Unable to determine " + modCommonUtil.CONTROLName + " version information";
                res.Impact = "Without the proper version information " + modCommonUtil.CONTROLName + " will not work correctly";
                res.Resolution = "Verify that " + modCommonUtil.CONTROLName + " has been installed correcctly on this system";
            }
            return res;
        }
    }
}

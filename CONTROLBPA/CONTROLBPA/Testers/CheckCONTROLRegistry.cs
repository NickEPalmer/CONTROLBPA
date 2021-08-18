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
    public class CheckCONTROLRegistry : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            modCommonUtil.ControlDir = string.Empty;

            res = new BaseLineConfigItem("CONTROL® Registry settings");
            res.Category = modCommondefs.ItemCategory.Configuration;

            // check for multiple versions of COntrol
            string cdir = string.Empty;
            bool foundControl = false;
            int max10Version = 0;
            for (int i = 1; i < modCommonUtil.MaxCONTROL10Version + 1; i++) 
            {
                if (0 == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, modCommonUtil.CONTROLRegistryName + i.ToString(), "InstallDir", ref cdir))
                {
                    //System.Windows.Forms.MessageBox.Show(cdir, modCommonUtil.CONTROLRegistryName + i.ToString() + "InstallDir");
                    max10Version = i;
                    foundControl = true;
                    modCommonUtil.ControlDir = cdir;
                }                    
            }

            if (!foundControl)
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Unable to locate " + modCommonUtil.CONTROLName + " registry settings";
                res.Impact = "Without the proper registry settings " + modCommonUtil.CONTROLName + " will not work correctly";
                res.Resolution = "Verify that " + modCommonUtil.CONTROLName + " has been installed on this system";
            }
            else
                //modCommonUtil.ControlDir = cdir;
                modCommonUtil.ControlName = "CONTROL 10." + max10Version.ToString();
                modCommonUtil.ControlRegistryValue = modCommonUtil.CONTROLRegistryName + max10Version.ToString();
                res.Issue = "Detected " + modCommonUtil.ControlName + " registry settings. CONTROL® install directory: " + modCommonUtil.ControlDir;
            return res;
        }
    }
}

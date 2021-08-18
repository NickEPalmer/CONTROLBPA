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
    public class CheckDisabledAddins : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            string controlDir = string.Empty;

            res = new BaseLineConfigItem("CONTROL Disabled Excel add-ins");
            res.Category = modCommondefs.ItemCategory.Configuration;

            if (0 != modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, modCommonUtil.CONTROLRegistryName, "InstallDir", ref controlDir))
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Unable to locate " + modCommonUtil.CONTROLName + " registry settings";
                res.Impact = "Without the proper registry settings " + modCommonUtil.CONTROLName + " will not work correctly";
                res.Resolution = "Verify that " + modCommonUtil.CONTROLName + " has been installed on this system";
            }
            else
                res.Issue = "Detected " + modCommonUtil.CONTROLName + " registry settings";

            // ' Turn ON the CONTROL .NET add-in as well - this applies to all versions
            // Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\InfoNavSupport.AddinModule", "ADXStartMode", "NORMAL", RegistryValueKind.String)
            // Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\InfoNavSupport.AddinModule", "LoadBehavior", 3, RegistryValueKind.DWord)

            // ' Related to BUG04265:
            // ' If we have been terminated previously then we need to clean up the registry
            // ' so we can launch Excel successfully again.  So we need to delete the
            // ' HKCU\SOFTWARE\Microsoft\Office\11.0\Excel\Resiliency key
            // ' This does Office 2007, 2010 and 2013
            // ' And now for do the same for Office 2007
            // RegDeleteKeys(HKEY_CURRENT_USER, "SOFTWARE\Microsoft\Office\12.0\Excel\Resiliency")
            // ' And now for do the same for Office 2010
            // RegDeleteKeys(HKEY_CURRENT_USER, "SOFTWARE\Microsoft\Office\14.0\Excel\Resiliency")
            // ' And now for do the same for Office 2013
            // RegDeleteKeys(HKEY_CURRENT_USER, "SOFTWARE\Microsoft\Office\15.0\Excel\Resiliency")

            return res;
        }
    }
}

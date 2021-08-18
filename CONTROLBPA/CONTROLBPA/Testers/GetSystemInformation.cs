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
using System.Management;

namespace CONTROLBPA.Testers
{
    class GetSystemInformation : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            List<string> info = new List<string>();

            res = new BaseLineConfigItem("System Information");
            res.Category = modCommondefs.ItemCategory.Configuration;


            Microsoft.VisualBasic.Devices.Computer cc = new Microsoft.VisualBasic.Devices.Computer();

            info.Add(cc.Info.OSFullName.ToString());
            info.Add(cc.Info.OSPlatform.ToString());
            info.Add(cc.Info.OSVersion.ToString());
            if (Environment.Is64BitOperatingSystem)
                info.Add("64 bit");
            else
                info.Add("32 bit");

            info.Add("Computer Name: " + cc.Name.ToString());
            info.Add("Computer Language: " + System.Globalization.CultureInfo.CurrentCulture.DisplayName);

            WMI objWMI = new WMI();
            info.Add("Computer Manufacturer = " + objWMI.Manufacturer);
            info.Add("Computer Model = " + objWMI.Model);
            info.Add("OS Version = " + objWMI.OSVersion);
            info.Add("System Type = " + objWMI.SystemType);
            info.Add("");

            double ramsize;
            ramsize = cc.Info.TotalPhysicalMemory / (double)1024 / (double)1024;
            info.Add("Memory: " + ((int)ramsize).ToString() + "MB RAM");
            info.Add("");
            string VGA = string.Empty;
            ManagementObjectSearcher WmiSelect = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach (var WmiResults in WmiSelect.Get())
                VGA = WmiResults.GetPropertyValue("Name").ToString();
            info.Add("Computer Display Info: " + VGA);

            int intX = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int intY = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            info.Add("Screen Resolution: " + intX + " X " + intY);
            info.Add("");
            info.Add("Total Physical Memory: " + cc.Info.TotalPhysicalMemory.ToString());
            info.Add("Total Virtual Memory: " + cc.Info.TotalVirtualMemory.ToString());
            info.Add("Available Physical Memory: " + cc.Info.AvailablePhysicalMemory.ToString());
            info.Add("");
            info.Add("CPU: " + modCommonUtil.GetProcessorId());

            string inf = string.Empty;
            foreach (var item in info)
                inf = inf + item + Constants.vbCrLf;

            res.Issue = inf;

            return res;
        }
    }
}

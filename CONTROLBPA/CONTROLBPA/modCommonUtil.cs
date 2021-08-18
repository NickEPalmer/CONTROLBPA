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
using System.Management;
using System.Xml.Linq;
using Microsoft.Win32;


namespace CONTROLBPA
{
    public static class modCommonUtil
    {
        public static bool bExcel16; // Excel 2016
        public static bool bExcel16O365; // Excel 2016 365

        public static string excel16Path;
        public static string excel16MSOPath;

        internal static string excel16InstallRoot = @"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot";

        public static string excel16VersionInfoString = string.Empty;

        public static string excel16VersionInfo = string.Empty;

        public static string excel16MSOVersionInfo = string.Empty;

        public static bool bExcel2016OK = false;
        public static string err2016Message = string.Empty;

        public static int ExcelPlatform = 64;

        private static string[] LongMonths = new string[13];

        public static string CC2016Version = "16.0.7967.2161";
        public static string DF2016Version = "16.0.6965.2150";
        public static string FRCC2016Version = "16.0.7766.2084";
        public static string FRDF2016Version = "16.0.7766.2084";

        public static string ORACLEDLL1 = "oci.dll";
        //internal static string ODBC32DLL1 = "odbc32.dll";
        //internal static string ODBC32DLL2 = "odbccp32.dll";
        public static string SQL2008DLL1 = "sqlncli10.dll";
        public static string SQL2012DLL1 = "sqlncli11.dll";
        public static string ODBC13DLL1 = "msodbcsql13.dll";
        public static string ODBC17DLL1 = "msodbcsql17.dll";

        public static string CONTROLName = "CONTROL® 10";
        public static string CONTROLRegistryName = "SOFTWARE\\KCI\\Control 10.";
        public static int MaxCONTROL10Version = 6;

        public static uint HKEY_CURRENT_USER = 0x80000001;
        public static uint HKEY_CLASSES_ROOT = 0x80000000;
        public static uint HKEY_LOCAL_MACHINE = 0x80000002;

        public static short ERROR_SUCCESS = 0;

        internal static string _controlDir;
        internal static string _controlRegistryValue;
        internal static string buildInfo = string.Empty;
        internal static string _controlName = "CONTROL10.1";

        public static string ControlDir
        {
            get => _controlDir;
            set
            {
                if ((_controlDir != value))
                {
                    _controlDir = value;
                }
            }
        }

        public static string ControlName
        {
            get => _controlName;
            set
            {
                if ((_controlName != value))
                {
                    _controlName = value;
                }
            }
        }

        public static string ControlRegistryValue
        {
            get => _controlRegistryValue;
            set
            {
                if ((_controlRegistryValue != value))
                {
                    _controlRegistryValue = value;
                }
            }
        }

        public static string BuildInfo
        {
            get => buildInfo;
            set
            {
                if ((buildInfo != value))
                {
                    buildInfo = value;
                }
            }
        }

        // Returns the value of RegName from key hKey\RegKey.  Returns a 0 if it succeeded, and a non-zero
        // value if there was any problem
        public static int RegGetValue(uint hKey, string RegKey, string RegName, ref string Value)
        {
            object result = null;

            switch (hKey)
            {
                case 0x80000001:
                    {
                        result = Registry.GetValue(@"HKEY_CURRENT_USER\" + RegKey, RegName, null);
                        break;
                    }

                case 0x80000002:
                    {
                        result = Registry.GetValue(@"HKEY_LOCAL_MACHINE\" + RegKey, RegName, null);
                        break;
                    }

                case 0x80000000:
                    {
                        result = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + RegKey, RegName, null);
                        break;
                    }
            }

            if (result != null)
            {
                Value = result as string;
                return 0;
            }
            else
            {
                Value = string.Empty;
                return 1;
            }
        }

        public static int RegGetValueDWORD(uint hKey, string RegKey, string RegName, ref int Value)
        {
            object result = null;

            switch (hKey)
            {
                case 0x80000001:
                    {
                        result = Registry.GetValue(@"HKEY_CURRENT_USER\" + RegKey, RegName, null);
                        break;
                    }

                case 0x80000002:
                    {
                        result = Registry.GetValue(@"HKEY_LOCAL_MACHINE\" + RegKey, RegName, null);
                        break;
                    }

                case 0x80000000:
                    {
                        result = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + RegKey, RegName, null);
                        break;
                    }
            }


            if (result != null)
            {
                if (result is int)
                {
                    Value = System.Convert.ToInt32(result);
                    return 0;
                }
                else
                {
                    Value = 0;
                    return 0;
                }
            }
            else
            {
                Value = 0;
                return 1;
            }
        }

        internal static string GetProcessorId()
        {
            string processorId = string.Empty;
            string clockSpeed = string.Empty;
            string procName = string.Empty;
            string manufacturer = string.Empty;
            string version = string.Empty;
            string numberOfCores = string.Empty;
            string numberOfLogicalProcessors = string.Empty;
            string numberOfPhysicalProcessors = string.Empty;

            SelectQuery query = new SelectQuery("Win32_processor");
            ManagementObjectSearcher search = new ManagementObjectSearcher(query);
            //ManagementObject info;

            try
            {
                foreach (var info in search.Get())
                {
                    processorId = info.GetPropertyValue("processorId").ToString();
                    clockSpeed = info.GetPropertyValue("CurrentClockSpeed").ToString();
                    procName = info.GetPropertyValue("Name").ToString();
                    manufacturer = info.GetPropertyValue("Manufacturer").ToString();
                    version = info.GetPropertyValue("Version").ToString();
                    numberOfLogicalProcessors = info.GetPropertyValue("NumberOfLogicalProcessors").ToString();
                    numberOfCores = info.GetPropertyValue("NumberOfCores").ToString();
                }
            }
            catch (Exception ex)
            {
            }

            query = new SelectQuery("Win32_ComputerSystem");
            search = new ManagementObjectSearcher(query);
            try
            {
                foreach (var info in search.Get())
                    numberOfPhysicalProcessors = info.GetPropertyValue("NumberOfProcessors").ToString();
            }
            catch (Exception ex)
            {
            }

            return procName + " - Physical Processors(" + numberOfPhysicalProcessors + ") Cores(" + numberOfCores + ") Logical Processors(" + numberOfLogicalProcessors + ")";
        }

        public static string GetMajorVersion(string _path)
        {
            string toReturn = string.Empty;
            if (File.Exists(_path))
            {
                try
                {
                    FileVersionInfo _fileVersion = FileVersionInfo.GetVersionInfo(_path);
                    toReturn = _fileVersion.FileVersion;
                }
                catch
                {
                }
            }
            return toReturn;
        }

        public static Boolean CheckFileVersion(string FilePath, string MinVersion)
        {
            Boolean bValue;
            string svVersionNumber;

            bValue = false;

            var versInfo = FileVersionInfo.GetVersionInfo(FilePath);
            svVersionNumber = versInfo.FileVersion;

            var minVer = new Version(MinVersion);
            var fileVer = new Version(svVersionNumber);
            var result = fileVer.CompareTo(minVer);

            if (result > 0)
                bValue = true;
            else if (result < 0)
                bValue = false;
            else if (result == 0)
                bValue = true;

            return bValue;
        }

        public static int VerCompare(string version1, string version2)
        {
            var minVer = new Version(version1);
            var fileVer = new Version(version2);
            var result = fileVer.CompareTo(minVer);

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////
        //                                                                           //
        // Function: CheckForInstalledUpdate		                                 //
        //                                                                           //
        //  Purpose: This function checks whether the specified update is installed  //
        //  		 on this system and returns a boolean							 //
        //                                                                           //
        //                                                                           //
        ///////////////////////////////////////////////////////////////////////////////
        public static Boolean CheckForInstalledUpdate(string updateTitle)
        {
            Boolean bValue = false;
            int i;
            string updateEntryTitle;

            try
            {
                //create the object
                Type updateSession = Type.GetTypeFromProgID("Microsoft.Update.Session");
                dynamic updateSessionInst = Activator.CreateInstance(updateSession);

                if (updateSessionInst != null)
                {
                    var updateSearcher = updateSessionInst.CreateUpdateSearcher();
                    var updateHistory = updateSearcher.QueryHistory(1, updateSearcher.GetTotalHistoryCount);

                    for (i = 0; i <= (updateHistory.Count - 1); i++) 
                    {
                        var updateEntry = updateHistory(i);
                        updateEntryTitle = updateEntry.Title;
                        //nLocation = StrFind (updateEntryTitle, updateTitle);
                        if (updateEntryTitle == updateTitle)
                            bValue = true;
                    }
                }
            }
            catch {
                bValue = false;
            }
            return (bValue);
        }

        public static string MakeDateTime()
        {
            string sHour;
            string sMinute;
            string sSecond;
            DateTime CurrentDate;

            SetupMonths();
            CurrentDate = DateTime.Now;

            // now format it
            sHour = System.Convert.ToString(CurrentDate.Hour);
            if (Conversion.Val(sHour) > 12)
                sHour = Strings.Trim(Conversion.Str(Conversion.Val(sHour) - 12));
            if (Conversion.Val(sHour) < 10)
                sHour = Strings.Trim("0" + sHour);
            sMinute = System.Convert.ToString(CurrentDate.Minute);
            if (Conversion.Val(sMinute) < 10)
                sMinute = Strings.Trim("0" + sMinute);

            sSecond = System.Convert.ToString(CurrentDate.Second);
            if (Conversion.Val(sSecond) < 10)
                sSecond = Strings.Trim("0" + sSecond);

            return LongMonths[CurrentDate.Month] + CurrentDate.Day + CurrentDate.Year + "_" + sHour + sMinute + sSecond;
        }

        public static void SetupMonths()
        {
            LongMonths[1] = "Jan";
            LongMonths[2] = "Feb";
            LongMonths[3] = "Mar";
            LongMonths[4] = "Apr";
            LongMonths[5] = "May";
            LongMonths[6] = "Jun";
            LongMonths[7] = "Jul";
            LongMonths[8] = "Aug";
            LongMonths[9] = "Sep";
            LongMonths[10] = "Oct";
            LongMonths[11] = "Nov";
            LongMonths[12] = "Dec";
        }
    }
}

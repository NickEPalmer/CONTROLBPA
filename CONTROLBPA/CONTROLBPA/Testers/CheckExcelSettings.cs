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
    public class CheckExcelSettings : ITestingItem
    {
        private bool b32BitOffice;
        private int ExcelPlatform = 32;

        private bool bExcel12; // Excel 2007
        private bool bExcel14; // Excel 2010
        private bool bExcel15; // Excel 2013
        private bool bExcel15O365; // Excel 2013 365

        private string excel12Path;
        private string excel14Path;
        private string excel15Path;
        private string excel15MSOPath;
        //private string excel16Path;
        //private string excel16MSOPath;

        private bool bExcel2013OK = false;
        //private bool bExcel2016OK = false;
        private string err2013Message = string.Empty;
        //private string err2016Message = string.Empty;

        internal static string excel12InstallRoot = @"SOFTWARE\Microsoft\Office\12.0\Excel\InstallRoot";
        internal static string excel14InstallRoot = @"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot";
        internal static string excel15InstallRoot = @"SOFTWARE\Microsoft\Office\15.0\Excel\InstallRoot";
        //internal static string excel16InstallRoot = @"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot";

        private string excel12VersionInfoString = string.Empty;
        private string excel14VersionInfoString = string.Empty;
        private string excel15VersionInfoString = string.Empty;
        //private string excel16VersionInfoString = string.Empty;

        private string excel12VersionInfo = string.Empty;
        private string excel14VersionInfo = string.Empty;
        private string excel15VersionInfo = string.Empty;
        //private string excel16VersionInfo = string.Empty;

        private string excel12MSOVersionInfo = string.Empty;
        private string excel14MSOVersionInfo = string.Empty;
        private string excel15MSOVersionInfo = string.Empty;
        //private string excel16MSOVersionInfo = string.Empty;

        private string[] LongMonths = new string[13];

        internal static string CC2013Version = "15.0.7967.2161";
        internal static string DF2013Version = "15.0.6965.2150";
        internal static string FRCC2013Version = "15.0.7766.2084";
        internal static string FRDF2013Version = "15.0.7766.2084";

        //internal static string CC2016Version = "16.0.7967.2161";
        //internal static string DF2016Version = "16.0.6965.2150";
        //internal static string FRCC2016Version = "16.0.7766.2084";
        //internal static string FRDF2016Version = "16.0.7766.2084";

        public BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;

            res = new BaseLineConfigItem("Excel Configuration");
            res.Category = modCommondefs.ItemCategory.Configuration;

            CheckForExcelVersions();
            // make sure there is an Excel installed
            if ((bExcel12 == false) & (bExcel14 == false) & (bExcel15 == false) & (modCommonUtil.bExcel16 == false))
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Unable to locate Microsoft Excel on this system";
                res.Impact = "Without Microsoft Excel installed " + modCommonUtil.CONTROLName + " will not work correctly";
                res.Resolution = "Verify that Microsoft Office and Excel has been installed on this system";
            }
            else
            {
                if (modCommonUtil.ExcelPlatform == 32)
                    res.Issue = "Detected Microsoft Office and Excel (MSI) 32-bit";
                else
                    res.Issue = "Detected Microsoft Office and Excel (MSI) 64-bit";

                if ((bExcel15 == true) | (modCommonUtil.bExcel16 == true))
                {
                    if ((bExcel15O365 == true) | (modCommonUtil.bExcel16O365 == true))
                        if (modCommonUtil.ExcelPlatform == 32)
                            res.Issue = "Detected Microsoft Office and Excel 365 32-bit";
                        else
                            res.Issue = "Detected Microsoft Office and Excel 365 64-bit";
                }
            }

            return res;
        }

        private void CheckForExcelVersions()
        {
            string RegValue = string.Empty;
            bool bExcel2013Patch1 = false;
            bool bExcel2013Patch2 = false;
            bool bExcel2016Patch1 = false;
            bool bExcel2016Patch2 = false;
            bool bExcel2016Patch3 = false;
            bool bExcel2016Patch4 = false;
            string sOfficeChannel;
            string sOfficeVersion;

            b32BitOffice = false; // assume we are running 32 bit Office to start with

            // start as always false
            bExcel12 = false;
            bExcel14 = false;
            bExcel15 = false;
            modCommonUtil.bExcel16 = false;
            bExcel15O365 = false;
            modCommonUtil.bExcel16O365 = false;
            bExcel2013OK = false;
            modCommonUtil.bExcel2016OK = false;

            excel12Path = "";
            excel14Path = "";
            excel15Path = "";
            modCommonUtil.excel16Path = "";
            excel15MSOPath = "";
            modCommonUtil.excel16MSOPath = "";

            excel12VersionInfoString = string.Empty;
            excel14VersionInfoString = string.Empty;
            excel15VersionInfoString = string.Empty;
            modCommonUtil.excel16VersionInfoString = string.Empty;

            excel12VersionInfo = string.Empty;
            excel14VersionInfo = string.Empty;
            excel15VersionInfo = string.Empty;
            modCommonUtil.excel16VersionInfo = string.Empty;

            excel12MSOVersionInfo = string.Empty;
            excel14MSOVersionInfo = string.Empty;
            excel15MSOVersionInfo = string.Empty;
            modCommonUtil.excel16MSOVersionInfo = string.Empty;

            err2013Message = string.Empty;
            modCommonUtil.err2016Message = string.Empty;

            // Also check if version 2007 exists - but only for 32 bit systems
            if (!Environment.Is64BitProcess)
            {
                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\12.0\Excel\InstallRoot", "Path", ref RegValue))
                {
                    bExcel12 = true;
                    excel12Path = RegValue;
                    b32BitOffice = true;
                    excel12InstallRoot = @"SOFTWARE\Microsoft\Office\12.0\Excel\InstallRoot";

                    // Get version info as well
                    excel12VersionInfo = GetMajorVersion(excel12Path + "Excel.exe");
                    excel12VersionInfoString = "Microsoft® Excel® 2007 (" + excel12VersionInfo + ")";

                    if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\12.0\Common\FilesPaths", "mso.dll", ref RegValue))
                    {
                        excel12MSOVersionInfo = GetMajorVersion(RegValue);
                        excel12VersionInfoString = excel12VersionInfoString + " MSO (" + excel12MSOVersionInfo + ")";
                    }
                }
            }

            // Also check if version 2010 exists
            if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\14.0\Excel\InstallRoot", "Path", ref RegValue))
            {
                bExcel14 = true;
                excel14Path = RegValue;
                if (!Environment.Is64BitProcess)
                    b32BitOffice = true;
                excel14InstallRoot = @"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot";

                // Get version info as well
                excel14VersionInfo = GetMajorVersion(excel14Path + "Excel.exe");
                excel14VersionInfoString = "Microsoft® Excel® 2010 (" + excel14VersionInfo + ")";

                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\14.0\Common\FilesPaths", "mso.dll", ref RegValue))
                {
                    excel14MSOVersionInfo = GetMajorVersion(RegValue);
                    excel14VersionInfoString = excel14VersionInfoString + " MSO (" + excel14MSOVersionInfo + ")";
                }

                if (Environment.Is64BitProcess)
                    excel14VersionInfoString = excel14VersionInfoString + " 64-bit";
            }

            // Also check if version if 32 2010 exists on a 64 bit system
            if (Environment.Is64BitProcess)
            {
                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\14.0\Excel\InstallRoot", "Path", ref RegValue))
                {
                    bExcel14 = true;
                    excel14Path = RegValue;
                    b32BitOffice = true;
                    excel14InstallRoot = @"SOFTWARE\Wow6432Node\Microsoft\Office\14.0\Excel\InstallRoot";

                    // Get version info as well
                    excel14VersionInfo = GetMajorVersion(excel14Path + "Excel.exe");
                    excel14VersionInfoString = "Microsoft® Excel® 2010 (" + excel14VersionInfo + ")";

                    if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\14.0\Common\FilesPaths", "mso.dll", ref RegValue))
                    {
                        excel14MSOVersionInfo = GetMajorVersion(RegValue);
                        excel14VersionInfoString = excel14VersionInfoString + " MSO (" + excel14MSOVersionInfo + ")";
                    }
                }
            }

            // Also check if version 2013 exists
            if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\15.0\Excel\InstallRoot", "Path", ref RegValue))
            {
                bExcel15 = true;
                excel15Path = RegValue;
                if (!Environment.Is64BitProcess)
                    b32BitOffice = true;
                excel15InstallRoot = @"SOFTWARE\Microsoft\Office\15.0\Excel\InstallRoot";

                // Get version info as well
                excel15VersionInfo = GetMajorVersion(excel15Path + "Excel.exe");
                excel15VersionInfoString = "Microsoft® Excel® 2013 (" + excel15VersionInfo + ")";

                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\15.0\Common\FilesPaths", "mso.dll", ref RegValue))
                {
                    excel15MSOPath = RegValue;
                    excel15MSOVersionInfo = GetMajorVersion(RegValue);
                    excel15VersionInfoString = excel15VersionInfoString + " MSO (" + excel15MSOVersionInfo + ")";
                }

                if (Environment.Is64BitProcess)
                    excel15VersionInfoString = excel15VersionInfoString + " 64-bit";
            }

            // Also check if version if 32 2013 exists on a 64 bit system
            if (Environment.Is64BitProcess)
            {
                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\15.0\Excel\InstallRoot", "Path", ref RegValue))
                {
                    bExcel15 = true;
                    excel15Path = RegValue;
                    b32BitOffice = true;
                    excel15InstallRoot = @"SOFTWARE\Wow6432Node\Microsoft\Office\15.0\Excel\InstallRoot";

                    // Get version info as well
                    excel15VersionInfo = GetMajorVersion(excel15Path + "Excel.exe");
                    excel15VersionInfoString = "Microsoft® Excel® 2013 (" + excel15VersionInfo + ")";

                    if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\15.0\Common\FilesPaths", "mso.dll", ref RegValue))
                    {
                        excel15MSOPath = RegValue;
                        excel15MSOVersionInfo = GetMajorVersion(RegValue);
                        excel15VersionInfoString = excel15VersionInfoString + " MSO (" + excel15MSOVersionInfo + ")";
                    }
                }
            }

            // Also check if version 2016 exists
            if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\16.0\Excel\InstallRoot", "Path", ref RegValue))
            {
                modCommonUtil.bExcel16 = true;
                modCommonUtil.excel16Path = RegValue;
                if (!Environment.Is64BitProcess)
                    b32BitOffice = true;
                modCommonUtil.excel16InstallRoot = @"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot";

                // Get version info as well
                modCommonUtil.excel16VersionInfo = GetMajorVersion(modCommonUtil.excel16Path + "Excel.exe");
                modCommonUtil.excel16VersionInfoString = "Microsoft® Excel® 2016 (" + modCommonUtil.excel16VersionInfo + ")";

                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Office\16.0\Common\FilesPaths", "mso.dll", ref RegValue))
                {
                    modCommonUtil.excel16MSOPath = RegValue;
                    modCommonUtil.excel16MSOVersionInfo = GetMajorVersion(RegValue);
                    modCommonUtil.excel16VersionInfoString = modCommonUtil.excel16VersionInfoString + " MSO (" + modCommonUtil.excel16MSOVersionInfo + ")";
                }

                if (Environment.Is64BitProcess)
                    modCommonUtil.excel16VersionInfoString = modCommonUtil.excel16VersionInfoString + " 64-bit";
            }

            // Also check if version if 32 2013 exists on a 64 bit system
            if (Environment.Is64BitProcess)
            {
                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\16.0\Excel\InstallRoot", "Path", ref RegValue))
                {
                    modCommonUtil.bExcel16 = true;
                    modCommonUtil.excel16Path = RegValue;

                    b32BitOffice = true;
                    modCommonUtil.excel16InstallRoot = @"SOFTWARE\Wow6432Node\Microsoft\Office\16.0\Excel\InstallRoot";

                    // Get version info as well
                    modCommonUtil.excel16VersionInfo = GetMajorVersion(modCommonUtil.excel16Path + "Excel.exe");
                    modCommonUtil.excel16VersionInfoString = "Microsoft® Excel® 2016 (" + modCommonUtil.excel16VersionInfo + ")";

                    if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"Software\Wow6432Node\Microsoft\Office\16.0\Common\FilesPaths", "mso.dll", ref RegValue))
                    {
                        modCommonUtil.excel16MSOPath = RegValue;
                        modCommonUtil.excel16MSOVersionInfo = GetMajorVersion(RegValue);
                        modCommonUtil.excel16VersionInfoString = modCommonUtil.excel16VersionInfoString + " MSO (" + modCommonUtil.excel16MSOVersionInfo + ")";
                    }
                }
            }

            if (b32BitOffice)
            {
                ExcelPlatform = 32;
                modCommonUtil.ExcelPlatform = 32;
            }
            else
            {
                ExcelPlatform = 64;
                modCommonUtil.ExcelPlatform = 64;
            }

            // // Now check for the propery installed Excel 2013 and 2016 updates
            if ((bExcel15 | modCommonUtil.bExcel16))
            {
                // // so 2013 or 2016 is installed so now check to see if the MSI or O365 version is installed
                //if (bExcel15)
                //{
                //    // // check for O365 version vs. MSI version
                //    if (excel15Path.Contains("root"))
                //    {
                //        // // O365 version
                //        bExcel15O365 = true;
                //        sOfficeChannel = GetO365Channel("15.0");
                //        sOfficeVersion = GetO365Version();

                //        excel15VersionInfoString = "Microsoft® Excel® 2013 MSO (" + sOfficeVersion + ")";
                //        if (ExcelPlatform == 64)
                //            excel15VersionInfoString = excel15VersionInfoString + " 64-bit";
                //        excel15VersionInfoString = excel15VersionInfoString + " " + GetO365ChannelName(sOfficeChannel);

                //        if ((ValidateO365Version(sOfficeChannel, sOfficeVersion, 15) == true))
                //            bExcel2013OK = true;
                //        else
                //        {
                //            err2013Message = "Failed to detect the minimum required version of Microsoft Office 365® 2013 on this system." + "#";
                //            err2013Message = err2013Message + "KCI Computing strongly recommends upgrading to the most current version of Microsoft Office 365® 2013 for your channel to improve the performance of " + modCommonUtil.CONTROLName + " " + "#";
                //            err2013Message = err2013Message + "Please upgrade to the most current version of Microsoft Office 365® 2013 for your channel before attempting to run " + modCommonUtil.CONTROLName + " with Microsoft Office 365® 2013." + "#";
                //        }
                //    }
                //    else
                //    {
                //        // Detected Office 2013 MSI
                //        // // Check for the recommended updates KB3172542 and KB3191885
                //        if ((modCommonUtil.CheckForInstalledUpdate("KB3172542") == true))
                //            bExcel2013Patch1 = true;
                //        else
                //            bExcel2013Patch1 = false;

                //        // // Check for the recommended updates KB3172542 and KB3191885
                //        if ((modCommonUtil.CheckForInstalledUpdate("KB3191885") == true))
                //            bExcel2013Patch2 = true;
                //        else
                //            bExcel2013Patch2 = false;

                //        if (((bExcel2013Patch1 == true) & (bExcel2013Patch2 == true)))
                //            bExcel2013OK = true;
                //        else
                //        {
                //            // // Attempt to validate the required Excel components by file version
                //            bExcel2013Patch1 = false;
                //            bExcel2013Patch2 = false;
                //            if ((modCommonUtil.CheckFileVersion(excel15Path + "Excel.exe", "15.0.4937.1000") == true))
                //                bExcel2013Patch1 = true;
                //            else
                //            {
                //            }

                //            if ((modCommonUtil.CheckFileVersion(excel15MSOPath, "15.0.4937.1000") == true))
                //                bExcel2013Patch2 = true;
                //            else
                //            {
                //            }

                //            if (((bExcel2013Patch1 == true) & (bExcel2013Patch2 == true)))
                //                bExcel2013OK = true;
                //            else
                //            {
                //                err2013Message = "Failed to detect either the KB3172542 or KB3191885 update for Microsoft Excel® 2013 installed on this system." + "#";
                //                err2013Message = err2013Message + "KCI Computing strongly recommends that both KB3172542 and KB3191885 be installed when running " + modCommonUtil.CONTROLName + " with Microsoft Excel® 2013 to improve the performance of " + modCommonUtil.CONTROLName + " " + "#";
                //                err2013Message = err2013Message + "Please install KB3172542 and KB3191885 before attempting to run " + modCommonUtil.CONTROLName + " with Microsoft Excel® 2013.";
                //            }
                //        }
                //    }
                //}

                if (modCommonUtil.bExcel16)
                {
                    // // check for O365 version vs. MSI version
                    if (modCommonUtil.excel16Path.Contains("root"))
                    {
                        // // O365 version
                        modCommonUtil.bExcel16O365 = true;
                        sOfficeChannel = GetO365Channel("16.0");
                        sOfficeVersion = GetO365Version();

                        modCommonUtil.excel16VersionInfoString = "Microsoft® Excel® 2016 MSO (" + sOfficeVersion + ")";
                        if (ExcelPlatform == 64)
                            modCommonUtil.excel16VersionInfoString = modCommonUtil.excel16VersionInfoString + " 64-bit";
                        modCommonUtil.excel16VersionInfoString = modCommonUtil.excel16VersionInfoString + " " + GetO365ChannelName(sOfficeChannel);

                        if ((ValidateO365Version(sOfficeChannel, sOfficeVersion, 16) == true))
                            modCommonUtil.bExcel2016OK = true;
                        else
                        {
                            modCommonUtil.err2016Message = "Failed to detect the minimum required version of Microsoft Office 365® 2016 on this system." + "#";
                            modCommonUtil.err2016Message = modCommonUtil.err2016Message + "KCI Computing strongly recommends upgrading to the most current version of Microsoft Office 365® 2016 for your channel to improve the performance of " + modCommonUtil.CONTROLName + " " + "#";
                            modCommonUtil.err2016Message = modCommonUtil.err2016Message + "Please upgrade to the most current version of Microsoft Office 365® 2016 for your channel before attempting to run " + modCommonUtil.CONTROLName + " with Microsoft Office 365® 2016." + "#";
                        }
                    }
                    else
                    {
                        // Detected Office 2016 MSI
                        // // Check for the recommended updates KB3178719 and KB3191943
                        if ((modCommonUtil.CheckForInstalledUpdate("KB3178719") == true))
                            bExcel2016Patch1 = true;
                        else
                            bExcel2016Patch1 = false;

                        // // Check for the recommended updates KB3178719 and KB3191943
                        if ((modCommonUtil.CheckForInstalledUpdate("KB3191943") == true))
                            bExcel2016Patch2 = true;
                        else
                            bExcel2016Patch2 = false;

                        // // Check for the recommended updates KB3178719 and KB3191943
                        if ((modCommonUtil.CheckForInstalledUpdate("KB3191943") == true))
                            bExcel2016Patch2 = true;
                        else
                            bExcel2016Patch2 = false;

                        // // Check for the recommended updates KB3178719 and KB3191943
                        if ((modCommonUtil.CheckForInstalledUpdate("KB4462115") == true))
                            bExcel2016Patch4 = true;
                        else
                            bExcel2016Patch4 = false;

                        if (((bExcel2016Patch1 == true) & (bExcel2016Patch2 == true) & (bExcel2016Patch3 == true) & (bExcel2016Patch4 == true)))
                            modCommonUtil.bExcel2016OK = true;
                        else
                        {
                            // // Attempt to validate the required Excel components by file version
                            bExcel2016Patch1 = false;
                            bExcel2016Patch2 = false;
                            if ((modCommonUtil.CheckFileVersion(modCommonUtil.excel16Path + "Excel.exe", "16.0.4549.1000") == true))
                                bExcel2016Patch1 = true;
                            else
                            {
                            }

                            if ((modCommonUtil.CheckFileVersion(modCommonUtil.excel16MSOPath, "16.0.4549.1001") == true))
                                bExcel2016Patch2 = true;
                            else
                            {
                            }

                            if (((bExcel2016Patch1 == true) & (bExcel2016Patch2 == true)))
                                modCommonUtil.bExcel2016OK = true;
                            else
                            {
                                modCommonUtil.err2016Message = "Failed to detect either the KB3178719 or KB3191943 or KB4022174 or KB4462115 or KB4484338 updates for Microsoft Excel® 2016 installed on this system." + "#";
                                modCommonUtil.err2016Message = modCommonUtil.err2016Message + "KCI Computing strongly recommends that KB3178719, KB3191943, KB4022174, KB4462115 and KB4484338 be installed when running " + modCommonUtil.CONTROLName + " with Microsoft Excel® 2016 to improve the performance of " + modCommonUtil.CONTROLName + " " + "#";
                                modCommonUtil.err2016Message = modCommonUtil.err2016Message + "Please install KB3178719, KB3191943, KB4022174, KB4462115 and KB4484338 before attempting to run " + modCommonUtil.CONTROLName + " with Microsoft Excel® 2016.";
                            }
                        }
                    }
                }
            }
        }

        // ///////////////////////////////////////////////////////////////////////////////
        // //                                                                           //
        // // Function: GetO365Channel		                                         	 //
        // //                                                                           //
        // //  Purpose: This function determines what channel of O365 is installed		 //
        // //			 for the specified version										 //
        // //                                                                           //
        // //			 Return values are:												 //
        // //			 CC = Current Channel											 //
        // //			 DF = Deferred Channel											 //
        // //			 FRCC = First Release for Current Channel						 //
        // //			 FRDF = First Release for Deferred Channel						 //
        // //                                                                           //
        // ///////////////////////////////////////////////////////////////////////////////
        public string GetO365Channel(string sVersion)
        {
            string svExcelTemp = string.Empty;
            string sRes;

            // // HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\office\16.0\common\officeupdate
            // // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\ClickToRun\Configuration

            sRes = "CC";
            // // Check for 32 Bit Office 2016 on a 64 bit Windows system
            if (Environment.Is64BitProcess)
            {
                if ((ExcelPlatform == 32))
                {
                    // //if (RegDBGetKeyValueExLog("SOFTWARE\\Wow6432Node\\Policies\\Microsoft\\Office\\" + sVersion + "\\Common\\OfficeUpdate",
                    if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Policies\\Microsoft\\Office\\" + sVersion + @"\\Common\\OfficeUpdate", "UpdateBranch", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                    {
                        if (string.Compare(svExcelTemp, "FirstReleaseCurrent", true) == 0)
                            sRes = "FRCC";
                        else if (string.Compare(svExcelTemp, "Current", true) == 0)
                            sRes = "CC";
                        else if (string.Compare(svExcelTemp, "Validation", true) == 0)
                            sRes = "FRDF";
                        else if (string.Compare(svExcelTemp, "Business", true) == 0)
                            sRes = "DF";
                    }
                    else
                       // //if (RegDBGetKeyValueExLog("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\ClickToRun\\Configuration",
                       if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "UpdateChannel", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                    {
                        if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/64256afe-f5d9-4f86-8936-8840a6a4f5be", true) == 0)
                            sRes = "FRCC";
                        else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/492350f6-3a01-4f97-b9c0-c7c6ddf67d60", true) == 0)
                            sRes = "CC";
                        else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/b8f9b850-328d-4355-9145-c59439a0c4cf", true) == 0)
                            sRes = "FRDF";
                        else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/7ffbc6bf-bc32-4f92-8982-f9dd17fd3114", true) == 0)
                            sRes = "DF";
                    }
                }
                else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Policies\\Microsoft\\Office\\" + sVersion + @"\\Common\\OfficeUpdate", "UpdateBranch", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                {
                    if (string.Compare(svExcelTemp, "FirstReleaseCurrent", true) == 0)
                        sRes = "FRCC";
                    else if (string.Compare(svExcelTemp, "Current", true) == 0)
                        sRes = "CC";
                    else if (string.Compare(svExcelTemp, "Validation", true) == 0)
                        sRes = "FRDF";
                    else if (string.Compare(svExcelTemp, "Business", true) == 0)
                        sRes = "DF";
                }
                else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "UpdateChannel", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                {
                    if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/64256afe-f5d9-4f86-8936-8840a6a4f5be", true) == 0)
                        sRes = "FRCC";
                    else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/492350f6-3a01-4f97-b9c0-c7c6ddf67d60", true) == 0)
                        sRes = "CC";
                    else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/b8f9b850-328d-4355-9145-c59439a0c4cf", true) == 0)
                        sRes = "FRDF";
                    else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/7ffbc6bf-bc32-4f92-8982-f9dd17fd3114", true) == 0)
                        sRes = "DF";
                }
            }
            else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Policies\\Microsoft\\Office\\" + sVersion + @"\\Common\\OfficeUpdate", "UpdateBranch", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
            {
                if (string.Compare(svExcelTemp, "FirstReleaseCurrent", true) == 0)
                    sRes = "FRCC";
                else if (string.Compare(svExcelTemp, "Current", true) == 0)
                    sRes = "CC";
                else if (string.Compare(svExcelTemp, "Validation", true) == 0)
                    sRes = "FRDF";
                else if (string.Compare(svExcelTemp, "Business", true) == 0)
                    sRes = "DF";
            }
            else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "UpdateChannel", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
            {
                if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/64256afe-f5d9-4f86-8936-8840a6a4f5be", true) == 0)
                    sRes = "FRCC";
                else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/492350f6-3a01-4f97-b9c0-c7c6ddf67d60", true) == 0)
                    sRes = "CC";
                else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/b8f9b850-328d-4355-9145-c59439a0c4cf", true) == 0)
                    sRes = "FRDF";
                else if (string.Compare(svExcelTemp, "http://officecdn.microsoft.com/pr/7ffbc6bf-bc32-4f92-8982-f9dd17fd3114", true) == 0)
                    sRes = "DF";
            }

            return sRes;
        }

        // ///////////////////////////////////////////////////////////////////////////////
        // //                                                                           //
        // // Function: GetO365Version		                                         	 //
        // //                                                                           //
        // //  Purpose: This function determines the version of O365 installed		 	 //
        // //                                                                           //
        // //			 Returns the version number as string							 //
        // //                                                                           //
        // ///////////////////////////////////////////////////////////////////////////////
        public string GetO365Version()
        {
            string svExcelTemp = string.Empty;
            string sRes;

            // // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\ClickToRun\Configuration						  

            sRes = "0.0.0.0";
            // // Check for 32 Bit Office 2016 on a 64 bit Windows system
            if (Environment.Is64BitProcess)
            {
                // // set the flag to allow us to access the 64 bit registry

                if ((ExcelPlatform == 32))
                {
                    // //if (RegDBGetKeyValueExLog("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\ClickToRun\\Configuration",
                    if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "VersionToReport", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                        sRes = svExcelTemp;
                }
                else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "VersionToReport", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                    sRes = svExcelTemp;
            }
            else if ((modCommonUtil.RegGetValue(modCommonUtil.HKEY_LOCAL_MACHINE, @"SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration", "VersionToReport", ref svExcelTemp) == modCommonUtil.ERROR_SUCCESS))
                sRes = svExcelTemp;

            return sRes;
        }


        // ///////////////////////////////////////////////////////////////////////////////
        // //                                                                           //
        // // Function: ValidateO365Version		                                     //
        // //                                                                           //
        // //  Purpose: This function validates the version of O365 installed		 	 //
        // //                                                                           //
        // //			 Returns TRUE or FALSE											 //
        // //                                                                           //
        // ///////////////////////////////////////////////////////////////////////////////
        public bool ValidateO365Version(string sChannel, string sVersion, int nVersion)
        {
            int nResult = 0;
            bool bValue;

            bValue = false;
            // // for the specified version check with our minimum version numbers
            if ((nVersion == 15))
            {
                if (string.Compare(sChannel, "FRCC", true) == 0)
                    nResult = modCommonUtil.VerCompare(FRCC2013Version, sVersion);
                else if (string.Compare(sChannel, "CC", true) == 0)
                    nResult = modCommonUtil.VerCompare(CC2013Version, sVersion);
                else if (string.Compare(sChannel, "FRDF", true) == 0)
                    nResult = modCommonUtil.VerCompare(FRDF2013Version, sVersion);
                else if (string.Compare(sChannel, "DF", true) == 0)
                    nResult = modCommonUtil.VerCompare(DF2013Version, sVersion);
            }
            else if ((nVersion == 16))
            {
                if (string.Compare(sChannel, "FRCC", true) == 0)
                    nResult = modCommonUtil.VerCompare(modCommonUtil.FRCC2016Version, sVersion);
                else if (string.Compare(sChannel, "CC", true) == 0)
                    nResult = modCommonUtil.VerCompare(modCommonUtil.CC2016Version, sVersion);
                else if (string.Compare(sChannel, "FRDF", true) == 0)
                    nResult = modCommonUtil.VerCompare(modCommonUtil.FRDF2016Version, sVersion);
                else if (string.Compare(sChannel, "DF", true) == 0)
                    nResult = modCommonUtil.VerCompare(modCommonUtil.DF2016Version, sVersion);
            }

            // // now figure out how we did
            if (nResult > 0)
                bValue = true;
            else if (nResult < 0)
                bValue = false;
            else if (nResult == 0)
                bValue = true;

            return bValue;
        }

        // ///////////////////////////////////////////////////////////////////////////////
        // //                                                                           //
        // // Function: GetO365ChannelName		                                          //
        // //                                                                           //
        // //  Purpose: This function returns the pretty for name for the supplied      //
        // //			 O365 channel                                                     //
        // //                                                                           //
        // //			 Return values are:												            //
        // //			 CC = Monthly Channel									    	            //
        // //			 DF = Semi-Annual Channel										            //
        // //			 FRCC = Monthly Channel (Targeted)						               //
        // //			 FRDF = Semi-Annual Channel (Targeted)	                           //
        // //                                                                           //
        // ///////////////////////////////////////////////////////////////////////////////
        public string GetO365ChannelName(string sChannel)
        {
            string sRes = "Monthly Channel";

            if (string.Compare(sChannel, "FRCC", true) == 0)
                sRes = "Monthly Channel (Targeted)";
            else if (string.Compare(sChannel, "CC", true) == 0)
                sRes = "Monthly Channel";
            else if (string.Compare(sChannel, "FRDF", true) == 0)
                sRes = "Semi-Annual Channel (Targeted)";
            else if (string.Compare(sChannel, "DF", true) == 0)
                sRes = "Semi-Annual Channel";
            return sRes;
        }

        public string GetMajorVersion(string _path)
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

    }
}

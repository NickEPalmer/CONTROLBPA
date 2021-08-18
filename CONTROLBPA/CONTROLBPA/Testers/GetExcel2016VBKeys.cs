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
    class GetExcel2016VBKeys : ITestingItem
    {
        public BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            int RegValue = 0;
            bool bExcel2016Key = false;
            bool[] bExcel2016Keys= new bool[9];
            string ExcelLoc64 = @"Software\Microsoft\Office\16.0\Excel\Options";
            string ExcelLoc32 = @"Software\Wow6432Node\Microsoft\Office\16.0\Excel\Options";

            // This allows access to the VBA Object Model which CONTROL requires    	                                        
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Security "AccessVBOM", REGDB_NUMBER, "1", -1);
            //  This enables the fix in MS Update KB3172542    	                                        
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v NoActivateHidden / d 1 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v RDPUsePrinterMF / d 1 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v ReplaceCFOnPaste / d 1 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v OLEVisibilityResizeOnScale / d 0 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v MultiSheetPrint / d 1 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v LegacyAnchorResize / d 1 / f
            //  HKCU\Software\Microsoft\Office\16.0\Excel\Options / t REG_DWORD / v OneCellPasteFixup / d 1 / f
            // 8 HKCU\Software\Microsoft\Office\16.0\Common\General / t REG_DWORD / v AcbControl / d 2147483648 / f

            res = new BaseLineConfigItem("Microsoft® Excel® 2016 registry configuration keys");
            res.Category = modCommondefs.ItemCategory.Configuration;

            // make sure there is an Excel installed
            if ((modCommonUtil.bExcel16 == false))
                res = null;
            else if (modCommonUtil.bExcel2016OK)
            {
                // They have the correct version of Excel installed so check for the registry key
                if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValueDWORD(modCommonUtil.HKEY_CURRENT_USER, ExcelLoc64, "NoActivateHidden", ref RegValue))
                {
                    if (RegValue == 1)
                        bExcel2016Key = true;
                }

                if (Environment.Is64BitProcess)
                {
                    if (modCommonUtil.ERROR_SUCCESS == modCommonUtil.RegGetValueDWORD(modCommonUtil.HKEY_CURRENT_USER, ExcelLoc32, "NoActivateHidden", ref RegValue))
                    {
                        if (RegValue == 1)
                            bExcel2016Key = true;
                    }
                }
                if (bExcel2016Key)
                    res.Issue = "Detected the NoActivateHidden registry key with the correct value required for Microsoft® Excel® 2016 KB3178719";
                else
                {
                    res.Status = modCommondefs.ItemStatus.ItemError;
                    res.Issue = "Failed to detect the NoActivateHidden registry key with correct value required for Microsoft® Excel® 2016 KB3178719 to take effect";
                    res.Impact = "Without the correctly configured NoActivateHidden registry key, the KB3178719 update will not take effect and the performance of " + modCommonUtil.CONTROLName + " can be negatively affected.";
                    res.Resolution = "Refer to https://support.microsoft.com/en-us/help/3178719/april-4-2017-update-for-excel-2016-kb3178719 for instructions";
                }
            }
            else
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Failed to detect the Microsoft® Excel® 2016 KB3178719 update";
                res.Impact = "Without the KB3178719 update and the correctly configured NoActivateHidden registry key, the performance of " + modCommonUtil.CONTROLName + " can be negatively affected.";
                res.Resolution = "Refer to https://support.microsoft.com/en-us/help/3178719/april-4-2017-update-for-excel-2016-kb3178719 for instructions";
            }

            return res;
        }
    }
}

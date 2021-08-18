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
    class GetExcel2016Information : ITestingItem
    {
        public BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;

            res = new BaseLineConfigItem("Microsoft® Excel® 2016 Configuration");
            res.Category = modCommondefs.ItemCategory.Configuration;

            // make sure there is an Excel installed
            if ((modCommonUtil.bExcel16 == false))
                res = null/* TODO Change to default(_) if this is not a reference type */;
            else if (modCommonUtil.bExcel2016OK)
                res.Issue = "Detected " + modCommonUtil.excel16VersionInfoString;
            else
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                string[] errorMessage;
                errorMessage = modCommonUtil.err2016Message.Split('#');
                res.Issue = "Detected " + modCommonUtil.excel16VersionInfoString + ". " + errorMessage[0];
                res.Impact = errorMessage[1];
                res.Resolution = errorMessage[2];
            }

            return res;
        }

    }
}

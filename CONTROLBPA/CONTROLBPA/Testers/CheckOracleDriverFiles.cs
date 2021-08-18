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
    class CheckOracleDriverFiles : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            modCommonUtil.ControlDir = string.Empty;

            bool foundOCI = false;
            res = new BaseLineConfigItem("Oracle client (OCI) drivers");
            res.Category = modCommondefs.ItemCategory.Configuration;

            string path;
            string[] sysPath;
            string curFile;
            FileVersionInfo _fileVersion = null;
            path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process | EnvironmentVariableTarget.Machine);
            if (path.Length > 0)
            {
                sysPath = path.Split(';');
                if (sysPath.Count() > 0)
                {
                    // look for the SQL Server Native client drivers first
                    // start with the 2008/2008 R2 driver
                    int i;
                    for (i = 0; i <= sysPath.Count() - 1; i++)
                    {
                        curFile = sysPath[i].Trim() + @"\" + modCommonUtil.ORACLEDLL1;
                        if ((File.Exists(curFile)))
                        {
                            foundOCI = true;
                            _fileVersion = FileVersionInfo.GetVersionInfo(curFile);
                            break;
                        }
                    }

                    if (foundOCI)
                        res.Issue = "Detected Oracle (OCI) 64-bit client software version " + _fileVersion.FileVersion.ToString();
                    else
                    {
                        res.Issue = "Did not detect the Oracle (OCI) 64-bit client software";
                        res.Status = modCommondefs.ItemStatus.ItemError;
                        res.Impact = "For Oracle clients this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                        res.Resolution = "Install the correct Oracle (OCI) 64-bit client software";
                    }
                }
                else
                {
                    res.Status = modCommondefs.ItemStatus.ItemError;
                    res.Issue = "Unable to get system path information";
                    res.Impact = "Test for Oracle (OCI) 64-bit client software cannot be completed";
                    res.Resolution = "Verify system confiruation and re-run tests";
                }
            }
            else
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Unable to get system path information";
                res.Impact = "Test for Oracle (OCI) 64-bit client software cannot be completed";
                res.Resolution = "Verify system confiruation and re-run tests";
            }
            return res;
        }
    }
}

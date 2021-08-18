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
    class CheckSQLServerDriverFiles : ITestingItem
    {
        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            modCommonUtil.ControlDir = string.Empty;

            bool found2008 = false;
            bool found2012 = false;
            bool found2013 = false;
            bool found2017 = false;

            res = new BaseLineConfigItem("SQL Server Native Client drivers");
            res.Category = modCommondefs.ItemCategory.Configuration;

            string path;
            string[] sysPath;
            string curFile;
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
                        curFile = sysPath[i].Trim() + @"\" + modCommonUtil.SQL2008DLL1;
                        if ((File.Exists(curFile)))
                        {
                            if (curFile.ToUpper().Contains("SYSTEM32"))
                            {
                                found2008 = true;
                                break;
                            }
                        }
                    }

                    // now look for SQL 2012 driver
                    for (i = 0; i <= sysPath.Count() - 1; i++)
                    {
                        curFile = sysPath[i].Trim() + @"\" + modCommonUtil.SQL2012DLL1;
                        if ((File.Exists(curFile)))
                        {
                            if (curFile.ToUpper().Contains("SYSTEM32"))
                            {
                                found2012 = true;
                                break;
                            }
                        }
                    }

                    // now look for ODBC 13 driver
                    for (i = 0; i <= sysPath.Count() - 1; i++)
                    {
                        curFile = sysPath[i].Trim() + @"\" + modCommonUtil.ODBC13DLL1;
                        if ((File.Exists(curFile)))
                        {
                            if (curFile.ToUpper().Contains("SYSTEM32"))
                            {
                                found2013 = true;
                                break;
                            }
                        }
                    }

                    // now look for ODBC 17 driver
                    for (i = 0; i <= sysPath.Count() - 1; i++)
                    {
                        curFile = sysPath[i].Trim() + @"\" + modCommonUtil.ODBC17DLL1;
                        if ((File.Exists(curFile)))
                        {
                            if (curFile.ToUpper().Contains("SYSTEM32"))
                            {
                                found2017 = true;
                                break;
                            }
                        }
                    }

                    string result = "";
                    if (found2008)
                        result += "SQL Server 2008/2008 R2 64-bit";

                    if (found2012)
                        if (result.Length > 0)
                            result += ", SQL Server 2012/2014 64-bit";
                        else
                            result += "SQL Server 2012/2014 64-bit";

                    if (found2013)
                        if (result.Length > 0)
                            result += ", ODBC Driver 13 for SQL SQL Server 64-bit";
                        else
                            result += "ODBC Driver 13 for SQL SQL Server 64-bit";

                    if (found2017)
                        if (result.Length > 0)
                            result += ", ODBC Driver 17 for SQL SQL Server 64-bit";
                        else
                            result += "ODBC Driver 17 for SQL SQL Server 64-bit";

                    if (result.Length > 0)
                        res.Issue = "Detected " + result + " drivers";
                    else
                    {
                        res.Issue = "Did not detect any 64-bit SQL Server Native Client drivers installed on this system";
                        res.Status = modCommondefs.ItemStatus.ItemError;
                        res.Impact = "For SQL Server clients this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                        res.Resolution = "Install the correct 64-bit SQL Server Native Client driver";
                    }
                }
                else
                {
                    res.Status = modCommondefs.ItemStatus.ItemError;
                    res.Issue = "Unable to get system path information";
                    res.Impact = "Test for 64-bit SQL Server Native Client drivers cannot be completed";
                    res.Resolution = "Verify system confiruation and re-run tests";
                }
            }
            else
            {
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Issue = "Unable to get system path information";
                res.Impact = "Test for 64-bit SQL Server Native Client drivers cannot be completed";
                res.Resolution = "Verify system confiruation and re-run tests";
            }
            return res;
        }
    }
}

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
using System.Data.Odbc;
using Microsoft.Win32;
using CONTROLBPA.ViewModel;

namespace CONTROLBPA.Testers
{ 
    class CheckODBCDriver : ITestingItem
    {
        private DatabaseStoreViewModel databaseInfo;

        public CheckODBCDriver(DatabaseStoreViewModel _databaseStore)
        {
            databaseInfo = _databaseStore;
        }

        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            modCommonUtil.ControlDir = string.Empty;

            res = new BaseLineConfigItem("ODBC Driver check");
            res.Category = modCommondefs.ItemCategory.Configuration;

            var ODBCDriverList = GetOdbcDataSources();
            if (ODBCDriverList.Contains(databaseInfo.Database.ToUpper()))
            {
                // Get the name of the driver
                var driverName = GetOdbcDataSourceDriver(databaseInfo.Database);
                res.Issue = "Detected 64-bit ODBC data source using the driver: " + driverName;
            }
            else 
            {
                res.Issue = "Did not locate the ODBC data source: " + databaseInfo.Database;
                res.Status = modCommondefs.ItemStatus.ItemError;
                res.Impact = "For clients using an ODBC connection this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                res.Resolution = "Setup the the correct 64-bit ODBC driver";
            }

            return res;
        }

        /// <summary>
        /// Gets the ODBC driver names from the registry.
        /// </summary>
        /// <returns>a string array containing the ODBC driver names, if the registry key is present; null, otherwise.</returns>
        public static string[] GetOdbcDriverNames()
        {
            string[] odbcDriverNames = null;
            using (RegistryKey localMachineHive = Registry.LocalMachine)
            using (RegistryKey odbcDriversKey = localMachineHive.OpenSubKey(@"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"))
            {
                if (odbcDriversKey != null)
                {
                    odbcDriverNames = odbcDriversKey.GetValueNames();
                }
            }

            return odbcDriverNames;
        }

        //public static string[] GetOdbcDataSources()
        //{
        //    string[] OdbcDataSources = null;
        //    using (RegistryKey localMachineHive = Registry.LocalMachine)
        //    using (RegistryKey odbcDriversKey = localMachineHive.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources"))
        //    {
        //        if (odbcDriversKey != null)
        //        {
        //            OdbcDataSources = odbcDriversKey.GetValueNames();
        //        }
        //    }

        //    return OdbcDataSources;
        //}

        public static List<String> GetOdbcDataSources()
        {
            List<string> names = new List<string>();
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Data Sources");
                        if (reg != null)
                        {
                            // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                            foreach (string sName in reg.GetValueNames())
                            {
                                names.Add(sName.ToUpper());
                            }
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { /* ignore this exception if we couldn't close */ }
                    }
                }
            }

            return names;
        }

        public static String GetOdbcDataSourceDriver(string dataSourceName)
        {
            string driverName = "";
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Data Sources");
                        if (reg != null)
                        {
                            driverName = (string)reg.GetValue(dataSourceName);
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { /* ignore this exception if we couldn't close */ }
                    }
                }
            }

            return driverName;
        }
    }
}

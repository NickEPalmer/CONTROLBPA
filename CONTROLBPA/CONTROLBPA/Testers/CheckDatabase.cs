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
    class CheckDatabase : ITestingItem
    {
        private DatabaseStoreViewModel databaseInfo;

        public CheckDatabase(DatabaseStoreViewModel _databaseStore)
        {
            databaseInfo = _databaseStore;
        }

        public Model.BaseLineConfigItem Run()
        {
            BaseLineConfigItem res;
            modCommonUtil.ControlDir = string.Empty;

            res = new BaseLineConfigItem("Database Connection check");
            res.Category = modCommondefs.ItemCategory.Configuration;

            // attempt to connect using the provided database informaiton
            if (databaseInfo.DatabaseType == DatabaseTypeEnum.SQLServer)
            {
                try
                {
                    string password = SecureStringToString(databaseInfo.Password);
                    string sConnString = $"DSN={databaseInfo.Database};uid={databaseInfo.UserID};pwd={password};";
                    var oOdbcConnection = new System.Data.Odbc.OdbcConnection(sConnString);

                    string queryString = $"SELECT * FROM {databaseInfo.UserID}.OBJECTS";
                    OdbcCommand command = new OdbcCommand(queryString);

                    command.Connection = oOdbcConnection;
                    oOdbcConnection.Open();

                    oOdbcConnection.Close();

                    res.Issue = "Successfully connected to " + databaseInfo.DatabaseType.ToString() + " database: " + databaseInfo.Database;
                }
                catch (Exception e)
                {
                    res.Issue = "Could not connect to the " + databaseInfo.DatabaseType.ToString() + "database: " + databaseInfo.Database + " - Error: " + e.Message;
                    res.Status = modCommondefs.ItemStatus.ItemError;
                    res.Impact = "For clients using an ODBC connection this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                    res.Resolution = "Verify connection configuration";
                }
            }
            else if(databaseInfo.DatabaseType == DatabaseTypeEnum.Oracle)
            {
                try
                {
                    var OracleDriverList = GetOracleOdbcDrivers();
                    if (OracleDriverList.Count > 0)
                    {
                        foreach (string driverName in OracleDriverList)
                        {
                            string password = SecureStringToString(databaseInfo.Password);
                            string sConnString = "DRIVER={" + driverName + "};DBQ=" + databaseInfo.Database;
                            sConnString += $";uid={databaseInfo.UserID};pwd={password};";
                            var oOdbcConnection = new System.Data.Odbc.OdbcConnection(sConnString);

                            string queryString = $"SELECT * FROM {databaseInfo.UserID}.OBJECTS";
                            OdbcCommand command = new OdbcCommand(queryString);

                            command.Connection = oOdbcConnection;
                            oOdbcConnection.Open();

                            oOdbcConnection.Close();

                            res.Issue = "Successfully connected to " + databaseInfo.DatabaseType.ToString() + " database: " + databaseInfo.Database;
                        }

                    }
                    else
                    {
                        //res.Issue = "Could not connect to the " + databaseInfo.DatabaseType.ToString() + " database: " + databaseInfo.Database + " - Error: " + e.Message;
                        res.Status = modCommondefs.ItemStatus.ItemError;
                        res.Impact = "For clients using an ODBC connection this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                        res.Resolution = "Verify connection configuration";
                    }
                }
                catch (Exception e)
                {
                    res.Issue = "Unable to verify database connCould not connect to the " + databaseInfo.DatabaseType.ToString() + " database: " + databaseInfo.Database + " - Error: " + e.Message;
                    res.Status = modCommondefs.ItemStatus.ItemError;
                    res.Impact = "For clients using an ODBC connection this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                    res.Resolution = "Verify connection configuration";
                }
            }
            else
            {
                res.Issue = "Could not connect to the " + databaseInfo.DatabaseType.ToString() + " database: " + databaseInfo.Database + " - Error: " + e.Message;
                res.Status = modCommondefs.ItemStatus.ItemWarning;
                res.Impact = "For clients using an ODBC connection this will prevent " + modCommonUtil.CONTROLName + " from connecting to the database";
                res.Resolution = "Verify connection configuration";
            }

            return res;
        }

        static String SecureStringToString(SecureString value)
        {
            IntPtr bstr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(value);

            try
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeBSTR(bstr);
            }
        }

        public static List<String> GetOracleOdbcDrivers()
        {
            List<string> names = new List<string>();
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBCINST.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Drivers");
                        if (reg != null)
                        {
                            // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                            foreach (string sName in reg.GetValueNames())
                            {
                                if (sName.StartsWith("Oracle"))
                                    names.Add(sName);
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

    }
}

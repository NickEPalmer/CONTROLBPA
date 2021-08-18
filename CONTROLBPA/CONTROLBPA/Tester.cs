//' Our main testing class
//'-	Check database driver files x
//'-	Check Excel settings x 
//'-	Get Excel version information x
//'- Check Excel version and patches x
//'-	Get CPU and memory x
//'- Check Control registry x
//'- Get installed Control Version x

//'- Check for disabled Add-ins
//'- CHeck dyalog runtime files
//'- Checkf for Excel registry flashing key
//'- Check InfoNavSupport COM object
//'-	Check ODBC settings - parameter would be CONTROL data source name
//'-	Check Engine and login - also capture time to login  - parameter would be CONTROL data source name, ID and password
//   Check that the ODBC data source is the correct type

//'			RegDBSetKeyValueExLog ("Software\\Wow6432Node\\Microsoft\\Office\\14.0\\Excel\\Security\\","AccessVBOM", REGDB_NUMBER, "1", -1);		  	    	    	  


//' Analyze Environent (Windows)
//' Analyze Excel Configuration - versions and patches and disabled add-ins
//' Analyze CONTROL Configuration

//' Configuration
//' Performance
//' Environment

//'http://www.c-sharpcorner.com/UploadFile/mahesh/using-xaml-expander-in-wpf/
//'https://wpf.2000things.com/tag/expander/
//'https://github.com/chrisparnin/wpfPlusMinusExpander/tree/master/Demo
//'http://blog.ninlabs.com/2011/04/keeping-it-simple-customizing-the-wpf-expander-icon/
//'https://blogs.msdn.microsoft.com/pigscanfly/2010/03/28/wpf-styling-the-expander-control/

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
using CONTROLBPA.Model;
using System.Management;
using System.Windows;
using System.Windows.Shapes;
using CONTROLBPA.ViewModel;
using Microsoft.Win32;
using CONTROLBPA.Testers;
using CONTROLBPA.Contracts;

namespace CONTROLBPA
{
    public class Tester
    {
        private string _userId;
        private string _password;
        private int _databaseType;
        private string _connectionString;
        private object oLibrary;
        private object LoginSupportLibrary;                   // Reference to our 32 bit shim for creating Engine objects in 64 bit Windows with 32 bit Office
        private List<BaseLineConfigItem> _results = new List<BaseLineConfigItem>();

        private bool _analyzeWindows = true;
        private bool _analyzeOffice = true;
        private bool _analyzeCONTROL = true;
        private bool _analyzeODBC = false;
        private bool _analyzeDatabase = false;
        private bool _analyzeDatabaseClient = false;
        private DatabaseStoreViewModel _databaseInfo;

        public Tester(bool windows, bool office, bool control, bool odbc, bool database, bool databaseclient, DatabaseStoreViewModel _databaseStore)
        {
            _analyzeWindows = windows;
            _analyzeOffice = office;
            _analyzeCONTROL = control;
            _analyzeODBC = odbc;
            _analyzeDatabase = database;
            _databaseInfo = _databaseStore;
            _analyzeDatabaseClient = databaseclient;
        }

        public int PerformTests()
        {
            int i;
            BaseLineConfigItem testResult;

            List<ITestingItem> tests = new List<ITestingItem>();
            if (_analyzeWindows)
                tests.Add(new Testers.GetSystemInformation());
            if (_analyzeOffice)
            {
                tests.Add(new Testers.CheckExcelSettings());
                //tests.Add(new Testers.GetExcel2010Information());
                //tests.Add(new Testers.GetExcel2013Information());
                //tests.Add(new Testers.GetExcel2013KBKeys());
                tests.Add(new Testers.GetExcel2016Information());
                tests.Add(new Testers.GetExcel2016VBKeys());
            }
            if (_analyzeCONTROL)
            {
                tests.Add(new Testers.CheckCONTROLRegistry());
                tests.Add(new Testers.CheckCONTROLVersion());
            }
            if (_analyzeDatabaseClient)
            {
                tests.Add(new Testers.CheckSQLServerDriverFiles());
                tests.Add(new Testers.CheckOracleDriverFiles());
            }
            if (_analyzeODBC)
            {
                tests.Add(new Testers.CheckODBCDriver(_databaseInfo));
            }
            if (_analyzeDatabase)
            {
                tests.Add(new Testers.CheckDatabase(_databaseInfo));
            }

            TestingService testing = new TestingService();
            foreach (var test in tests)
            {
                testResult = testing.RunTest(test);
                if (testResult != null)
                    _results.Add(testResult);
            }

            // testing only
            BaseLineConfigItem res;
            //res = new BaseLineConfigItem("Test Error");
            //res.Category = modCommondefs.ItemCategory.Configuration;

            //res.Status = modCommondefs.ItemStatus.ItemError;
            //res.Issue = "Unable to locate Microsoft Excel on this system";
            //res.Impact = "Without Microsoft Excel installed" + modCommonUtil.CONTROLName + " will not work correctly";
            //res.Resolution = "Verify that Microsoft Excel has been installed on this system";
            //_results.Add(res);

            res = new BaseLineConfigItem("Test Warning");
            res.Category = modCommondefs.ItemCategory.Configuration;

            res.Status = modCommondefs.ItemStatus.ItemWarning;
            res.Issue = "Unable to locate Microsoft Excel on this system";
            res.Impact = "Without Microsoft Excel installed" + modCommonUtil.CONTROLName + " will not work correctly";
            res.Resolution = "Verify that Microsoft Excel has been installed on this system";
            _results.Add(res);

            return 0;
        }

        public List<BaseLineConfigItem> Results
        {
            get
            {
                return _results;
            }
        }
    }
}

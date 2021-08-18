using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Security;
using CONTROLBPA.Model;

namespace CONTROLBPA.ViewModel
{
    public class ControlBPAViewModel : INotifyPropertyChanged
    {
        private bool _home;
        private bool _enterParams;
        private bool _viewReport;
        private RelayCommand _homeCommand;
        private RelayCommand _startCommand;
        private RelayCommand _startScanning;
        private RelayCommand _exportCommand;
        private modCommondefs.VisiblePages _currentPage;
        private List<BaseLineConfigItem> _testResults;
        private ObservableCollection<BaseLineConfigItem> _errorResults = new ObservableCollection<BaseLineConfigItem>();
        private ObservableCollection<BaseLineConfigItem> _warningResults = new ObservableCollection<BaseLineConfigItem>();
        private ObservableCollection<BaseLineConfigItem> _compliantResults = new ObservableCollection<BaseLineConfigItem>();
        private Tester _tester;
        public event PropertyChangedEventHandler PropertyChanged;

        private System.ComponentModel.BackgroundWorker _backGroundWorker1 = new System.ComponentModel.BackgroundWorker();

        private bool _analyzeWindows = true;
        private bool _analyzeOffice = true;
        private bool _analyzeCONTROL = true;
        private bool _analyzeODBC = false;
        private bool _analyzeDatabase = false;
        private bool _analyzeDatabaseClient = false;
        private DatabaseStoreViewModel _databaseStore = null;
        private bool _scanningEnabled = true;

        public ControlBPAViewModel() : base()
        {
            _home = true;
            _enterParams = true;
            _viewReport = false;
            _currentPage = modCommondefs.VisiblePages.Home;

            _homeCommand = new RelayCommand(param =>
            {
                HomeClicked(param);
            }, param => true);

            _startCommand = new RelayCommand(param =>
            {
                StartClicked(param);
            }, param => true);

            _startScanning = new RelayCommand(param =>
            {
                ScanningClicked(param);
            }, param => true);

            _exportCommand = new RelayCommand(param =>
            {
                ExportResults(param);
            }, param => true);
        }

        public void HomeClicked(object args)
        {
            _home = true;
            _enterParams = true;
            _viewReport = false;
            _currentPage = modCommondefs.VisiblePages.Home;
            UpdateTabs();
            UpdatePages();
        }

        public void StartClicked(object args)
        {
            _home = true;
            _enterParams = true;
            _viewReport = false;
            _currentPage = modCommondefs.VisiblePages.Parameters;

            _databaseStore = new DatabaseStoreViewModel();
            _databaseStore.UserID = "CNTADM";
            _databaseStore.DatabaseType = DatabaseTypeEnum.SQLServer;

            _databaseStore.Password = new SecureString();
            _databaseStore.Database = "";

            _databaseStore.PropertyChanged += DatabasePropertyChanged;

            UpdateTabs();
            UpdatePages();
        }

        public void ScanningClicked(object args)
        {
            _home = false;
            _enterParams = false;
            _viewReport = false;
            _currentPage = modCommondefs.VisiblePages.Progress;
            UpdateTabs();
            UpdatePages();
            // http://www.wpf-tutorial.com/misc-controls/the-progressbar-control/

            // Now actually start the testing process
            _tester = new Tester(AnalyzeWindows, AnalyzeOffice, AnalyzeCONTROL, AnalyzeODBC, AnalyzeDatabase, AnalyzeDatabaseClient, DatabaseStore);
            _backGroundWorker1 = new System.ComponentModel.BackgroundWorker();

            _backGroundWorker1.DoWork += backgroundWorker1_DoWork;
            //_backGroundWorker1.ProgressChanged += Worker_ProgressChanged;
            //_backGroundWorker1.WorkerReportsProgress = true;
            _backGroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            _backGroundWorker1.RunWorkerAsync();
        }

        public void ExportResults(object args)
        {
            string sHome;
            string exportFile;

            // Export the results to a text file
            sHome = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            exportFile = sHome + (@"\" + modCommonUtil._controlName + "_BPA_Report" + "_" + modCommonUtil.MakeDateTime() + ".html");

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.CheckFileExists = false;
            openFileDialog1.Title = modCommonUtil.CONTROLName + " BPA report file export";

            openFileDialog1.FileName = System.IO.Path.GetFileName(exportFile);
            openFileDialog1.InitialDirectory = sHome;

            openFileDialog1.Filter = "Export file (*.html)|*.html" + "|All Files (*.*)|*.*";

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;// no exporting              // cmndlgFileOpen.filename <> sEMPTY

            exportFile = openFileDialog1.FileName;

            // create the file content
            string[] headerText = new string[8];
            headerText[0] = "<html>";
            headerText[1] = "<head>";
            headerText[2] = "<META http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-16\">";
            headerText[3] = "<title>" + modCommonUtil.CONTROLName + " Best Practice Analyzer Report</title>";
            headerText[4] = "<style>";
            headerText[5] = Properties.Settings.Default.htmlStyle; // "body {font-family: verdana, arial, helvetica, sans-serif;background: white;font-size: 9pt;color: black;}table {font-size: 9pt;}th {background-color: #0A6CCD;color: white;padding-left: 5px;padding-right: 5px;}td {padding-left: 5px;padding-right: 5px;}td.hilite {font-weight: bold;text-align: left;padding-left: 5px;padding-right: 5px;}";
            headerText[6] = "</style>";
            headerText[7] = "</head>";

            File.WriteAllLines(exportFile, headerText, Encoding.UTF8);

            string appendText = "<body lang=EN-US>" + Environment.NewLine;            
            appendText = "<div class=WordSection1>" + Environment.NewLine;
            appendText += "<h1><span style='font-size:36.0pt;line-height:107%'>" + modCommonUtil.CONTROLName + " Best Practice Analyzer Report</span></h1>";
            appendText += "<h1>&nbsp;</h1>";
            appendText += "<p class=MsoNormal>Report Date: " + DateTime.Now.ToString() + "</p>";
            appendText += "<p class=MsoNormal>&nbsp;</p>";
            appendText += "</div>";

            //appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;

            appendText += "<h2><span class=Heading2Char>Compliant: </span></h2>" + Environment.NewLine;
            appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;

            if (_compliantResults.Count > 0)
            {
                //appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
                appendText += "<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 width=0" + Environment.NewLine;
                appendText += "style='width:22.0in;border-collapse:collapse;border:none'>" + Environment.NewLine;

                appendText += GenHeaderRow("#2cf278", false);
                foreach (var item in _compliantResults)
                {
                    appendText += item.Print();
                }

                appendText += "</table>" + Environment.NewLine;
                appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
            }

            appendText += "<h2><span class=Heading2Char>Warning: </span></h2>" + Environment.NewLine;
            appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;

            if (_warningResults.Count > 0)
            {
                //appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
                appendText += "<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 width=0" + Environment.NewLine;
                appendText += "style='width:22.0in;border-collapse:collapse;border:none'>" + Environment.NewLine;

                appendText += GenHeaderRow("#eef207", true);
                foreach (var item in _warningResults)
                {
                    appendText += item.Print();
                }

                appendText += "</table>" + Environment.NewLine;
                appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
            }

            appendText += "<h2><span class=Heading2Char>Error: </span></h2>" + Environment.NewLine;
            appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;

            if (_errorResults.Count > 0)
            {
                //appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
                appendText += "<table class=MsoNormalTable border=1 cellspacing=0 cellpadding=0 width=0" + Environment.NewLine;
                appendText += "style='width:22.0in;border-collapse:collapse;border:none'>" + Environment.NewLine;

                appendText += GenHeaderRow("#f50000", true);
                foreach (var item in _errorResults)
                {
                    appendText += item.Print();
                }

                appendText += "</table>" + Environment.NewLine;
                appendText += "<p class=MsoNormal>&nbsp;</p>" + Environment.NewLine;
            }

            appendText += "</body>" + Environment.NewLine;
            appendText += "</html>" + Environment.NewLine;

            File.AppendAllText(exportFile, appendText, Encoding.UTF8);

            //MsgBox("Report export complete", MsgBoxStyle.OkOnly, modCommonUtil.CONTROLName + " BPA report export");
        }

        private string GenHeaderRow(string color, bool warningOrError) 
        {
            string result = "";

            result += "<tr style='height:15.0pt'>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:50.7pt;border:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "background:" + color + "; padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal align=left style='margin-bottom:0in;margin-bottom:.0001pt;" + Environment.NewLine;
            //result += "text-align:left;line-height:normal'><span style='color:black'>" & CStr(vText(j, 1)) & "</span></p>" + Environment.NewLine;
            result += "text-align:left;line-height:normal'><span style='color:black'>Name</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:37.45pt;border:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;background:" + color + ";padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal align=left style='margin-bottom:0in;margin-bottom:.0001pt;" + Environment.NewLine;
            //result += "text-align:left;line-height:normal'><span style='color:black'>" & sDate & "</span></p>" + Environment.NewLine;
            result += "text-align:left;line-height:normal'><span style='color:black'>Category</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:33.5pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "background:" + color + ";padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
            //result += "normal'><span style='color:black'>" & vText(j, 3) & "</span></p>" + Environment.NewLine;
            result += "normal'><span style='color:black'>Source</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:317.1pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "background:" + color + ";padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
            //result += "normal'><span style='color:black'>" & vText(j, 4) & "</span></p>" + Environment.NewLine;
            result += "normal'><span style='color:black'>Information</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:435.25pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "background:" + color + ";padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
            //result += "normal'><span style='color:black'>" & sText & "</span></p>" + Environment.NewLine;
            if (!warningOrError)
                result += "normal'><span style='color:black'>Status</span></p>" + Environment.NewLine;
            else
                result += "normal'><span style='color:black'>Impact</span></p>" + Environment.NewLine;

            if (warningOrError)
            {
                result += "<td nowrap valign=bottom style='width:435.25pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "background:" + color + ";padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
                result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
                //result += "normal'><span style='color:black'>" & sText & "</span></p>" + Environment.NewLine;
                result += "normal'><span style='color:black'>Resolution</span></p>" + Environment.NewLine;
            }

            result += "</td>" + Environment.NewLine;
            result += "</tr>" + Environment.NewLine;

            return result;
        }

        // This event handler is where the actual work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            // Get the BackgroundWorker object that raised this event.
            BackgroundWorker worker = (BackgroundWorker)sender;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = _tester.PerformTests();
        } // backgroundWorker1_DoWork

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            // First, handle the case where an exception was thrown.
            if ((e.Error != null))
                System.Windows.Forms.MessageBox.Show("Error Encountered: " + e.Error.Message);
            else if (e.Cancelled)
            {
            //    ' Next, handle the case where the user canceled the 
            //' operation.
            //' Note that due to a race condition in 
            //' the DoWork event handler, the Cancelled
            //' flag may not have been set, even though
            //' CancelAsync was called.
            //'         resultLabel.Text = "Canceled"
            }
            else
            {
                // Finally, handle the case where the operation succeeded.
                if (_tester.Results.Count > 0)
                    PopulateResults();
            }

            _home = true;
            _enterParams = true;
            _viewReport = true;
            _currentPage = modCommondefs.VisiblePages.Report;
            UpdateTabs();
            UpdatePages();
        } // backgroundWorker1_RunWorkerCompleted

        private void PopulateResults()
        {
            _errorResults.Clear();
            _warningResults.Clear();
            _compliantResults.Clear();
            foreach (BaseLineConfigItem item in _tester.Results)
            {
                if (item.Status == modCommondefs.ItemStatus.ItemError)
                    _errorResults.Add(item);
                else if (item.Status == modCommondefs.ItemStatus.ItemWarning)
                    _warningResults.Add(item);
                else
                    _compliantResults.Add(item);
            }
            Notify("ErrorResults");
            Notify("WarningResults");
            Notify("CompliantResults");
            Notify("ItemsFound");
            Notify("ErrorsFound");
            Notify("WarningsFound");
            Notify("CompliantsFound");
        }

        public bool ScanningEnabled
        {
            get
            {
                //         private bool _analyzeODBC = false;
                //private bool _analyzeDatabase = false;
                if (_analyzeODBC)
                {
                    if (_databaseStore.Database.Trim().Length > 0)
                        return true;
                    else
                        return false;
                }
                if (_analyzeDatabase)
                {
                    if ((_databaseStore.Database.Trim().Length > 0) && (_databaseStore.UserID.Trim().Length > 0) && (_databaseStore.DatabaseType > 0))
                        return true;
                    else
                        return false;
                }
                return _scanningEnabled;
            }
            set
            {
                _scanningEnabled = value;
            }
        }

        public bool AnalyzeWindows
        {
            get
            {
                return _analyzeWindows;
            }
            set
            {
                _analyzeWindows = value;
            }
        }

        public bool AnalyzeOffice
        {
            get
            {
                return _analyzeOffice;
            }
            set
            {
                _analyzeOffice = value;
            }
        }

        public bool AnalyzeCONTROL
        {
            get
            {
                return _analyzeCONTROL;
            }
            set
            {
                _analyzeCONTROL = value;
            }
        }

        public bool AnalyzeDatabaseClient
        {
            get
            {
                return _analyzeDatabaseClient;
            }
            set
            {
                _analyzeDatabaseClient = value;
            }
        }

        public bool AnalyzeODBC
        {
            get
            {
                return _analyzeODBC;
            }
            set
            {
                _analyzeODBC = value;
                if (_analyzeODBC)
                    _analyzeDatabase = !value;
                else
                    _analyzeDatabase = false;

                ScanningEnabled = !value;
                Notify("ScanningEnabled");
                Notify("ODBCOptionsVisible");
                Notify("DatabaseOptionsVisible");
                Notify("AnalyzeDatabase");

            }
        }

        public bool AnalyzeDatabase
        {
            get
            {
                return _analyzeDatabase;
            }
            set
            {
                _analyzeDatabase = value;
                if (_analyzeDatabase)
                    _analyzeODBC = !value;
                else
                    _analyzeODBC = false;

                ScanningEnabled = !value;
                Notify("ScanningEnabled");
                Notify("ODBCOptionsVisible");
                Notify("DatabaseOptionsVisible");
                Notify("AnalyzeODBC");
            }
        }

        public DatabaseStoreViewModel DatabaseStore
        {
            get
            {
                return _databaseStore;
            }
        }

        private void DatabasePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            //if (propertyChangedEventArgs.PropertyName == "Database")
                OnPropertyChanged(nameof(ScanningEnabled));
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Visibility DatabaseOptionsVisible
        {
            get
            {
                if (_analyzeDatabase)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility ODBCOptionsVisible
        {
            get
            {
                if (_analyzeODBC)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        private DatabaseTypeEnum _selectedDatabaseTypeEnum;

        public DatabaseTypeEnum SelectedDatabaseType
        {
            get
            {
                return _selectedDatabaseTypeEnum;
            }
            set
            {
                _selectedDatabaseTypeEnum = value;
            }
        }

        public IEnumerable<DatabaseTypeEnum> DatabaseTypeTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(DatabaseTypeEnum)).Cast<DatabaseTypeEnum>();
            }
        }

        public string CurrentDateTime
        {
            get
            {
                return DateTime.Now.ToString();
            }
        }

        public ObservableCollection<BaseLineConfigItem> ErrorResults
        {
            get
            {
                return _errorResults;
            }
        }

        public ObservableCollection<BaseLineConfigItem> WarningResults
        {
            get
            {
                return _warningResults;
            }
        }

        public ObservableCollection<BaseLineConfigItem> CompliantResults
        {
            get
            {
                return _compliantResults;
            }
        }

        public string ItemsFound
        {
            get
            {
                if (_tester != null)
                {
                    if (_tester.Results.Count > 0)
                        return "Items Found: " + _tester.Results.Count.ToString();
                    else
                        return "Items Found: 0";
                }
                else
                    return "Items Found: 0";
            }
        }

        public string ErrorsFound
        {
            get
            {
                if (_errorResults.Count > 0)
                    return "Error (" + _errorResults.Count.ToString() + ")";
                else
                    return "Error ( 0 )";
            }
        }

        public string WarningsFound
        {
            get
            {
                if (_warningResults.Count > 0)
                    return "Warning (" + _warningResults.Count.ToString() + ")";
                else
                    return "Warning ( 0 )";
            }
        }

        public string CompliantsFound
        {
            get
            {
                if (_compliantResults.Count > 0)
                    return "Compliant (" + _compliantResults.Count.ToString() + ")";
                else
                    return "Compliant ( 0 )";
            }
        }

        public bool Home
        {
            get
            {
                return _home;
            }
        }

        public bool EnterParams
        {
            get
            {
                return _enterParams;
            }
        }

        public bool ViewReport
        {
            get
            {
                return _viewReport;
            }
        }

        public ICommand HomeCommand
        {
            get
            {
                return _homeCommand;
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return _startCommand;
            }
        }

        public ICommand StartScanning
        {
            get
            {
                return _startScanning;
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand;
            }
        }

        private void UpdateTabs()
        {
            Notify("Home");
            Notify("EnterParams");
            Notify("ViewReport");
        }

        private void UpdatePages()
        {
            Notify("HomeVisible");
            Notify("ParametersVisible");
            Notify("ProgressVisible");
            Notify("ReportVisible");

            Notify("DatabaseStore");
            Notify("ScanningEnabled");
        }

        public string ApplicationTitle
        {
            get
            {
                return modCommonUtil.CONTROLName + " Best Practices Analyzer";
            }
        }

        public Visibility HomeVisible
        {
            get
            {
                if (_currentPage == modCommondefs.VisiblePages.Home)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public Visibility ParametersVisible
        {
            get
            {
                if (_currentPage == modCommondefs.VisiblePages.Parameters)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public Visibility ProgressVisible
        {
            get
            {
                if (_currentPage == modCommondefs.VisiblePages.Progress)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public Visibility ReportVisible
        {
            get
            {
                if (_currentPage == modCommondefs.VisiblePages.Report)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        private void Notify(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}

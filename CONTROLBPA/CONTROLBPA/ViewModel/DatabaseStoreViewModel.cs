using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using CONTROLBPA.Model;

namespace CONTROLBPA.ViewModel
{
    public enum DatabaseTypeEnum : int
    {
        [Description("ODBC")]
        ODBC = 1,
        [Description("SQL Server")]
        [DisplayValue("SQL Server")]
        SQLServer = 2,
        [Description("Oracle")]
        Oracle = 3,
        [Description("Sybase")]
        Sybase = 4,
        [Description("Vertica")]
        Vertica = 5,
        [Description("UDB")]
        UDB = 6
    }

    public class DatabaseStoreViewModel : INotifyPropertyChanged
    {
        [Category("Connection Information")]
        [DisplayName("User ID")]
        [PropertyOrder(1)]
        public string UserID
        {
            get
            {
                return m_UserID;
            }
            set
            {
                m_UserID = value;
            }
        }
        private string m_UserID;

        [Category("Connection Information")]
        [DisplayName("Password")]
        [PasswordPropertyText(true)]
        [PropertyOrder(2)]
        public SecureString Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
            }
        }
        private SecureString m_Password;

        [Category("Connection Information")]
        [DisplayName("Database")]
        [PropertyOrder(3)]
        public string Database
        {
            get
            {
                return m_Database;
            }
            set
            {
                if (value != Database)
                {
                    m_Database = value;
                    //Whenever property value is changes
                    //PropertyChanged event is triggered
                    OnPropertyChanged("Database");
                }
            }
        }
        private string m_Database;

        [Category("Connection Information")]
        [DisplayName("Database Type")]
        [PropertyOrder(4)]
        public DatabaseTypeEnum DatabaseType
        {
            get
            {
                return m_DatabaseType;
            }
            set
            {
                m_DatabaseType = value;
            }
        }
        private DatabaseTypeEnum m_DatabaseType;


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        //Create OnPropertyChanged method to raise event
        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}

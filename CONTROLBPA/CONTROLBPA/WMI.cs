using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace CONTROLBPA
{
    public class WMI
    {
        private System.Management.ManagementObjectSearcher objOS;
        private System.Management.ManagementObjectSearcher objCS;
        private System.Management.ManagementObject objMgmt;
        private string m_strComputerName;
        private string m_strManufacturer;
        private string m_StrModel;
        private string m_strOSName;
        private string m_strOSVersion;
        private string m_strSystemType;
        private string m_strTPM;
        private string m_strWindowsDir;

        public WMI()
        {
            objOS = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            objCS = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (var objMgmt in objOS.Get())
            {
                if (objMgmt != null)
                {
                    m_strOSName = objMgmt.GetPropertyValue("name").ToString();
                    m_strOSVersion = objMgmt.GetPropertyValue("version").ToString();
                    m_strComputerName = objMgmt.GetPropertyValue("csname").ToString();
                    m_strWindowsDir = objMgmt.GetPropertyValue("windowsdirectory").ToString();
                }
            }
            foreach (var objMgmt in objCS.Get())
            {
                if (objMgmt != null)
                {
                    m_strManufacturer = objMgmt.GetPropertyValue("manufacturer").ToString();
                    m_StrModel = objMgmt.GetPropertyValue("model").ToString();
                    m_strSystemType = objMgmt.GetPropertyValue("systemtype").ToString();
                    m_strTPM = objMgmt.GetPropertyValue("totalphysicalmemory").ToString();
                }
            }
        }
        public string ComputerName
        {
            get
            {
                return m_strComputerName;
            }
        }
        public string Manufacturer
        {
            get
            {
                return m_strManufacturer;
            }
        }
        public string Model
        {
            get
            {
                return m_StrModel;
            }
        }
        public string OsName
        {
            get
            {
                return m_strOSName;
            }
        }
        public string OSVersion
        {
            get
            {
                return m_strOSVersion;
            }
        }
        public string SystemType
        {
            get
            {
                return m_strSystemType;
            }
        }
        public string TotalPhysicalMemory
        {
            get
            {
                return m_strTPM;
            }
        }
        public string WindowsDirectory
        {
            get
            {
                return m_strWindowsDir;
            }
        }
    }

}

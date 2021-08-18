using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CONTROLBPA.Model
{
    public class BaseLineConfigItem : INotifyPropertyChanged
    {
        private modCommondefs.ItemStatus _status;

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseLineConfigItem(string name)
        {
            Name = name;
            _status = modCommondefs.ItemStatus.ItemCompliant;
            Category = modCommondefs.ItemCategory.Configuration;
            Source = Environment.MachineName;
            Issue = string.Empty;
            Impact = string.Empty;
            Resolution = string.Empty;
        }

        public modCommondefs.ItemStatus Status
        {
            get => _status;
            set
            {
                if ((_status != value))
                {
                    _status = value;
                    Notify("Status");
                }
            }
        }

        public modCommondefs.ItemCategory Category { get; set; }

        public string Name { get; set; }

        public string Source { get; set; }

        public string Issue { get; set; }

        public string Impact { get; set; }

        public string Resolution { get; set; }

        private void Notify(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public string Print()
        {
            string result = "";

            result += "<tr style='height:15.0pt'>" + Environment.NewLine;
            result += "<td nowrap valign=bottom style='width:50.7pt;border:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal align=left style='margin-bottom:0in;margin-bottom:.0001pt;" + Environment.NewLine;
            result += "text-align:left;line-height:normal'><span style='color:black'>" + Name + "</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:37.45pt;border:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal align=left style='margin-bottom:0in;margin-bottom:.0001pt;" + Environment.NewLine;
            result += "text-align:left;line-height:normal'><span style='color:black'>" + Category.ToString() + "</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:33.5pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
            result += "normal'><span style='color:black'>" + Source.ToString() + "</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            result += "<td nowrap valign=bottom style='width:317.1pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
            result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
            result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
            result += "normal'><span style='color:black'>" + Issue + "</span></p>" + Environment.NewLine;
            result += "</td>" + Environment.NewLine;

            if (_status == modCommondefs.ItemStatus.ItemCompliant) {
                result += "<td nowrap valign=bottom style='width:435.25pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
                result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
                result += "normal'><span style='color:black'>CONTROL® Baseline Configuration Analyzer has determined that you are in compliance with this best practice</span></p>" + Environment.NewLine;
                result += "</td>" + Environment.NewLine;
            }
            else
            {
                result += "<td nowrap valign=bottom style='width:435.25pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
                result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
                result += "normal'><span style='color:black'>" + Impact + "</span></p>" + Environment.NewLine;
                result += "</td>" + Environment.NewLine;

                result += "<td nowrap valign=bottom style='width:435.25pt;border-top:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "border-left:none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;" + Environment.NewLine;
                result += "padding:0in 5.4pt 0in 5.4pt;height:15.0pt'>" + Environment.NewLine;
                result += "<p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:" + Environment.NewLine;
                result += "normal'><span style='color:black'>" + Resolution + "</span></p>" + Environment.NewLine;
                result += "</td>" + Environment.NewLine;
            }

            result += "</tr>" + Environment.NewLine;

            //result = "-------------------------------------------------------------------------------------------------" + Environment.NewLine;
            //result = result + "Name: " + Name + Environment.NewLine;
            //result = result + "Category: " + Category.ToString() + Environment.NewLine;
            //result = result + "Source: " + Source.ToString() + Environment.NewLine;
            //result = result + "Issue: " + Issue + Environment.NewLine;
            //if (_status == modCommondefs.ItemStatus.ItemCompliant)
            //    result = result + "CONTROL® Baseline Configuration Analyzer has determined that you are in compliance with this best practice" + Environment.NewLine;
            //else
            //{
            //    result = result + "Impact: " + Impact + Environment.NewLine;
            //    result = result + "Resolution: " + Resolution + Environment.NewLine;
            //}
            //result = result + "-------------------------------------------------------------------------------------------------" + Environment.NewLine;

            return result;
        }

    }
}

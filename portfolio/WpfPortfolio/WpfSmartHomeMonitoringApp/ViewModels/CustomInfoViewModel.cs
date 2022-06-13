using Caliburn.Micro;
using System;
using System.Reflection;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class CustomInfoViewModel : Conductor<object>
    {
        private string applicationInfo;

        public string ApplicationInfo
        {
            get => applicationInfo; set
            {
                applicationInfo = value;
                NotifyOfPropertyChange(() => ApplicationInfo);
            }
        }

        public CustomInfoViewModel(string title)
        {
            this.DisplayName = title;
            setApplicationInfo();
        }

        private void setApplicationInfo()
        {
            ApplicationInfo = AssemblyTitle + " Ver." + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ApplicationInfo += "\n\nAssemblyDescription : " + AssemblyDescription;
            ApplicationInfo += "\nProduct : " + AssemblyProduct;
            ApplicationInfo += "\n" + AssemblyCopyright;
            ApplicationInfo += "\nCompany : " + AssemblyCompany;
        }

        public void AcceptClose()
        {
            // 창 닫기
            TryCloseAsync(true);
        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

    }
}

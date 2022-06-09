using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using uPLibrary.Networking.M2Mqtt;

namespace WpfSmartHomeMonitoringApp.Helpers
{
    public class Commons
    {
        public static string BROKERHOST { get; set; }
        public static string PUB_TOPIC { get; set; }
        public static MqttClient MQTT_CLIENT { get; set; }
        public static string CONNSTRING { get; set; }
        public static bool IS_CONNECT { get; set; }
    }

    //public class NotEmptyValidationRule : ValidationRule
    //{
    //    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    //    {
    //        return string.IsNullOrWhiteSpace((value ??))
    //    }
    //}
}

using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{

    public class RealTimeViewModel : Screen
    {

        private string livingTempVal;
        private string diningTempVal;
        private string bedTempVal;
        private string bathTempVal;
        private double livingHumidVal;
        private double diningHumidVal;
        private double bedHumidVal;
        private double bathHumidVal;
        public string LivingTempVal
        {
            get => livingTempVal; set
            {
                livingTempVal = value;
                NotifyOfPropertyChange(() => LivingTempVal);
            }
        }
        public string DiningTempVal
        {
            get => diningTempVal; set
            {
                diningTempVal = value;
                NotifyOfPropertyChange(() => DiningTempVal);
            }
        }
        public string BedTempVal
        {
            get => bedTempVal; set
            {
                bedTempVal = value;
                NotifyOfPropertyChange(() => BedTempVal);
            }
        }
        public string BathTempVal
        {
            get => bathTempVal; set
            {
                bathTempVal = value;
                NotifyOfPropertyChange(() => BathTempVal);
            }
        }
        public double LivingHumidVal
        {
            get => livingHumidVal; set
            {
                livingHumidVal = value;
                NotifyOfPropertyChange(() => LivingHumidVal);
            }
        }
        public double DiningHumidVal
        {
            get => diningHumidVal; set
            {
                diningHumidVal = value;
                NotifyOfPropertyChange(() => DiningHumidVal);
            }
        }
        public double BedHumidVal
        {
            get => bedHumidVal; set
            {
                bedHumidVal = value;
                NotifyOfPropertyChange(() => BedHumidVal);
            }
        }
        public double BathHumidVal
        {
            get => bathHumidVal; set
            {
                bathHumidVal = value;
                NotifyOfPropertyChange(() => BathHumidVal);
            }
        }

        public RealTimeViewModel()
        {
            Commons.BROKERHOST = "127.0.0.1";
            Commons.PUB_TOPIC = "home/device/#";

            LivingTempVal = DiningTempVal = BedTempVal = BathTempVal = string.Empty;
            LivingHumidVal = DiningHumidVal = BedHumidVal = BathHumidVal = 0;

            if (Commons.MQTT_CLIENT != null && Commons.MQTT_CLIENT.IsConnected)
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
            else  // 접속이 안되어 있으면
            {
                Commons.MQTT_CLIENT = new MqttClient(Commons.BROKERHOST);
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                Commons.MQTT_CLIENT.Connect("MONITOR");
                Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                    new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                Commons.IS_CONNECT = true;

            }
        }

        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            switch (currDatas["DevId"].ToString())
            {
                case "LIVING":
                    LivingTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    LivingHumidVal = double.Parse(currDatas["Humid"]);
                    break;

                case "DINING":
                    DiningTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    DiningHumidVal = double.Parse(currDatas["Humid"]);
                    break;

                case "BED":
                    BedTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    BedHumidVal = double.Parse(currDatas["Humid"]);
                    break;

                case "BATH":
                    BathTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    BathHumidVal = double.Parse(currDatas["Humid"]);
                    break;

                default:
                    break;
            }
        }
    }
}

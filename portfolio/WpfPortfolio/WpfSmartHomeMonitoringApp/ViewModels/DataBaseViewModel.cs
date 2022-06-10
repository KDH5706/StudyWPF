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
    public class DataBaseViewModel : Screen
    {
        // Alt+Enter : 필드 캡슐화 활용
        private string brokerUrl;
        private string topic;
        private string connString;
        private string dbLog;
        private bool isConnected;

        public string BrokerUrl
        {
            get => brokerUrl;
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }
        public string Topic
        {
            get => topic;
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }
        public string ConnString
        {
            get => connString;
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }
        public string DbLog
        {
            get => dbLog;
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }
        public bool IsConnected
        {
            get => isConnected; set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "210.119.12.70";   // MQTT Broker IP 설정
            Topic = Commons.PUB_TOPIC = "home/device/#";    //Multi topic
            //Single Level wildcarrd +
            //Multi Level wildcard #
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

            if (Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }
        }

        /// <summary>
        /// DB연결 + MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if (IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);    //Port : 1883(Default)

                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> MQTT Broker Connected");
                        IsConnected = Commons.IS_CONNECT = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // Pass
                }
            }
            else // 접속 끄기
            {
                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;   // 이벤트 연결 삭제
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected...");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // Pass
                }
            }
        }

        private void MQTT_CLIENT_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }

        /// <summary>
        /// Subscribe한 메시지 처리해주는 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.Default.GetString(e.Message);
            UpdateText(message);    // 센서데이터 출력
            SetDataBase(message, e.Topic);   // DB에 저장
        }

        private void SetDataBase(string message, string topic)
        {
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            //Debug.WriteLine(currDatas);

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();
                // Verbatim string : 보간된 축자 문자열
                string strInQuery = @"INSERT INTO TblSmartHome
                                               (DevId
                                               ,CurrTime
                                               ,Temp
                                               ,Humid)
                                         VALUES
                                               (@DevId
                                               ,@CurrTime
                                               ,@Temp
                                               ,@Humid)";

                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);
                    SqlParameter parmDevId = new SqlParameter("@DevId", currDatas["DevId"]);
                    cmd.Parameters.Add(parmDevId);
                    SqlParameter parmCurrTime = new SqlParameter("@CurrTime", DateTime.Parse(currDatas["CurrTime"]));   //날짜형으로 변환 필요
                    cmd.Parameters.Add(parmCurrTime);
                    SqlParameter parmTemp = new SqlParameter("@Temp", currDatas["Temp"]);
                    cmd.Parameters.Add(parmTemp);
                    SqlParameter parmHumid = new SqlParameter("@Humid", currDatas["Humid"]);
                    cmd.Parameters.Add(parmHumid);

                    if(cmd.ExecuteNonQuery() == 1)
                        UpdateText(">>> DB Inserted."); // 저장 성공
                    else
                        UpdateText(">>> DB Failed!!!!!"); //저장 실패

                }
                catch (Exception ex)
                {
                    UpdateText($">>> DB Error! {ex.Message}");
                }

            }   //conn.Close() 불필요
        }
    }
}

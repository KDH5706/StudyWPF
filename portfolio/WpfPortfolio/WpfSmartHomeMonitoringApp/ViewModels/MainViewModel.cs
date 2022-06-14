using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class MainViewModel : Conductor<object>  // Screen에는 ActivateItemAsync메서드가 존재하지 않음
    {
        public MainViewModel()
        {
            DisplayName = "SmartHome Monitoring v2.0";  //윈도우 타이틀, 제목
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (Commons.MQTT_CLIENT.IsConnected)
            {
                Commons.MQTT_CLIENT.Disconnect();
                Commons.MQTT_CLIENT = null;
            }  // 비활성화 처리

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        //TODO
        public void LoadDataBaseView()
        {
            //if (Commons.MQTT_CLIENT != null)
            ActivateItemAsync(new DataBaseViewModel());
            //else
            //    var windowManager = new WindowManager();
            //windowManager.ShowDialog(new ErrorPopupViewModel("Report|MQTT doesn't start, yet"));
        }
        public void LoadRealTimeView()
        {
            ActivateItemAsync(new RealTimeViewModel());
        }
        public void LoadHistoryView()
        {
            ActivateItemAsync(new HistoryViewModel());
        }
        public void ExitProgram()
        {
            Environment.Exit(0);
        }
        public void ExitToolbar()
        {
            Environment.Exit(0);
        }
        public void ToolBarStopSubscribe()
        {
            StopSubscribe();
        }

        public void MenuStopSubscribe()
        {
            StopSubscribe();
        }

        private void StopSubscribe()
        {
            if(this.ActiveItem is DataBaseViewModel)
            {
                DataBaseViewModel activeModel = (DataBaseViewModel)this.ActiveItem;
                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= activeModel.MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        activeModel.IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception)
                {
                    //pass
                }
                DeactivateItemAsync(this.ActiveItem, true);
            }
        }


        // Start 메뉴, 아이콘 눌렀을때 처리할 이벤트
        public void PopInfoDialog()
        {
            TaskPupup();
        }
        public void StartSubscribe()
        {
            TaskPupup();
        }
        private void TaskPupup()
        {
            // CustomPopupView
            var windowManager = new WindowManager();

            Task<bool?> result = windowManager.ShowDialogAsync(new CustomPopupViewModel("New Network"));

            if (result.Result == true)  //if (result.IsCompleted)
            {
                ActivateItemAsync(new DataBaseViewModel()); // 화면전환
            }
        }
        public void PopInfoView()
        {
            var windowManager = new WindowManager();
            windowManager.ShowDialogAsync(new CustomInfoViewModel("About"));
        }

    }
}

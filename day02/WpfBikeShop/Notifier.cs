using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBikeShop
{
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //사용자가 메시지 보내고 받을 때 모든 목록 내용 제거하고 다시 추가
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

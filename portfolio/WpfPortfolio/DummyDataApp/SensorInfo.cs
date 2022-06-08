using System;

namespace DummyDataApp
{
    public class SensorInfo
    {
        public string DevId { get; set; }
        public DateTime CurrTime { get; set; }
        public float Temp { get; set; }     //온도
        public float Humid { get; set; }    //습도
    }
}

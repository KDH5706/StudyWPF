using Bogus;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace DummyDataApp
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var Rooms = new[] { "DINNING", "LIVING", "BATH", "BED" };   //부엌, 거실, 욕실, 침실

                var sensorFaker = new Faker<SensorInfo>()
                    .RuleFor(r => r.DevId, f => f.PickRandom(Rooms))
                    .RuleFor(r => r.CurrTime, f => f.Date.Past(0))
                    .RuleFor(r => r.Temp, f => f.Random.Float(19.0f, 30.9f))
                    .RuleFor(r => r.Humid, f => f.Random.Float(40.0f, 63.9f));

                var currValue = sensorFaker.Generate();

                Console.WriteLine(JsonConvert.SerializeObject(currValue, Formatting.Indented));

                Thread.Sleep(1000);
            }
        }
    }
}

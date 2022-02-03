using System;
// To get the dotnet/io packages:
// install-Package System.Device.Gpio -Version 1.1.0-prerelease.20153
// Install-Package Iot.Device.Bindings -Version 1.1.0-prerelease.20153.1
using System.Device.Gpio;
using Iot.Device;
using Iot.Device.CpuTemperature;
using System.Device.I2c;
using Iot.Device.Bmp180;
using Iot.Device.DHTxx;
using System.Threading;
using System.Device.Pwm.Drivers;
using System.IO;

namespace DNETCoreGPIO.TRIGGERcmdData
{
    public static class TRIGGERcmd
    {
        const int MaxNumTries = 20; //For DHT22-i-Wire 

        /// <summary>
        /// Methods here are only for a single pass.
        /// DHT22-i-Wire can get invalids so do multiple tries.
        /// </summary>
        /// <param name="index"></param>
        public static void Trigger(int index)
        {

            switch (index)
            {
                case 11:
                    Relay(true);
                    break;
                case 12:
                    Relay(false);
                    break;
                case 13:
                    //Console.WriteLine("Doing Get-Temp with DHTxx");
                    //GetTempDHTxx();
                    break;
                case 14:
                    Console.WriteLine("Doing Get-Temp with DHT22-i-Wire One result only");
                    GetTempDHTxx1Wire(MaxNumTries);
                    break;
                case 15:
                    Console.WriteLine("Doing Get-Temp with BME280 One result only");
                    DotNetCoreCoreGPIO.BME280Sampler.Get();
                    break;
                case > 19:
                    int state = index - 20;
                    Console.WriteLine($"Doing MotorControl single pass state:{state}.");
                    MotorControl(state);
                    break;
            }
        }

        static void GetTempDHTxx1Wire(int maxNumTries)
        {
            int numTries = 0;
            int delayMs = 2000;
            //1-Wire:
            int pin = 26;
            Console.WriteLine("Using DH22-1-Wire1");
            bool lastResult = true;
            using (Dht22 dht = new Dht22(pin))
            {
                if (dht == null)
                {
                    Console.WriteLine("Dht22 instantiation failed");
                    return;

                }
                else
                {
                    while (true)
                    {
                        UnitsNet.Temperature temp;
                        UnitsNet.RelativeHumidity humid;
                        bool result1 = dht.TryReadTemperature(out temp);
                        bool result2 = dht.TryReadHumidity(out humid);
                        if (!result1 || !result2)
                        {
                            Console.Write(".");
                            lastResult = false;
                            if (++numTries >= maxNumTries)
                                return;
                        }
                        else
                        {
                            //Sanity Check
                            bool resultIsValid = true;
                            if (temp.DegreesCelsius is double.NaN)
                                resultIsValid = false;
                            else if (humid.Percent is double.NaN)
                                resultIsValid = false;
                            if (!resultIsValid)
                            {
                                Console.Write("#");
                                lastResult = false;
                                if (++numTries >= maxNumTries)
                                    return;
                            }
                            else
                            {
                                bool resultIsSane = true;
                                if ((temp.DegreesCelsius > 100) || (temp.DegreesCelsius < -20))
                                    resultIsSane = false;
                                else if ((humid.Percent > 100) || (humid.Percent < 0))
                                    resultIsSane = false;
                                if (!resultIsSane)
                                {
                                    Console.Write("x");
                                    lastResult = false;
                                    if (++numTries >= maxNumTries)
                                        return;
                                }
                                else
                                {

                                    if (!lastResult)
                                    {
                                        Console.WriteLine("");
                                    }
                                    lastResult = true;
                                    Console.Write($"Temperature: {temp.DegreesCelsius.ToString("0.0")} °C ");
                                    Console.Write($"Humidity: { humid.Percent.ToString("0.0")} % ");
                                    Console.WriteLine("");
                                    string result = $"Temperature equals {temp.DegreesCelsius.ToString("0.0")} °C ";
                                    result += $"and Humidity equals { humid.Percent.ToString("0.0")} % ";
                                    WriteT2S(result);
                                    return;
                                }
                            }
                        }
                        Thread.Sleep(delayMs);
                    }
                }
            }
        }

        public static void MotorControl(int state)
        {
            //GPIO Pin numbers:
            //=================
            var pinFwd = 17; // <- Brd Pin 11            If hi and pinBack is lo motor goes fwd
            var pinRev = 27; // <- Brd Pin 13             if hi and pinFwd is lo motor goes back (reverse)
            var pinEn = 22;  // <- Brd Pin 15            Overall enable/disable  hi/lo

            //Nb: if pinFwd=pinRev hi or lo then its brake
            Console.WriteLine(state);

            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(pinEn, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Enable): {pinEn}");
                controller.OpenPin(pinRev, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Reverse): {pinRev}");
                controller.OpenPin(pinFwd, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Forward): {pinFwd}");

                Console.WriteLine($"2 Doing MotorControl single pass state:{state}.");
                string stateStr = "";
                switch (state)
                {
                    case 0: // Partial off
                        controller.Write(pinRev, PinValue.Low);
                        controller.Write(pinFwd, PinValue.Low);
                        stateStr = "Partial Off";
                        break;
                    case 1: //Forward
                        controller.Write(pinRev, PinValue.Low);
                        controller.Write(pinFwd, PinValue.High);
                        stateStr = "Forward";
                        break;
                    case 2: //Reverse
                        controller.Write(pinFwd, PinValue.Low);
                        controller.Write(pinRev, PinValue.High);
                        stateStr = "Reverse";
                        break;
                    case 3: //Disable 
                        controller.Write(pinEn, PinValue.Low);
                        stateStr = "Disable";
                        break;
                    case 4: //Enable
                        controller.Write(pinEn, PinValue.High);
                        stateStr = "Enable";
                        break;
                    case 5: // Off
                        controller.Write(pinEn, PinValue.Low);
                        controller.Write(pinRev, PinValue.Low);
                        controller.Write(pinFwd, PinValue.Low);
                        stateStr = "Off";
                        break;

                }
                string result = $"Done Motor Control. Command equals ({stateStr})";
                Console.WriteLine(result);
                WriteT2S(result);
            }
        }

        public static void WriteT2S(string txt)
        {
            if (File.Exists("/tmp/temperature.txt"))
            {
                // If file found, delete it    
                File.Delete("/tmp/temperature.txt");
                Console.WriteLine("File deleted.");
            }
            File.WriteAllText("/tmp/temperature.txt", txt);
            return;
        }

        private static void Relay(bool on)
        {
            var pinRelay = 19;  
            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(pinRelay, System.Device.Gpio.PinMode.Output);
                if (on)
                {
                    Console.WriteLine($"Setting Relay to ON");
                    controller.Write(pinRelay, System.Device.Gpio.PinValue.High);
                }
                else
                {
                    Console.WriteLine($"Setting Relay to OFF");
                    controller.Write(pinRelay, System.Device.Gpio.PinValue.Low);
                }
            }

        }
    }
}

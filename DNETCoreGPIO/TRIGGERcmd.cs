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
using DotNetCoreCoreGPIO;

namespace DNETCoreGPIO.TRIGGERcmdData
{


    public static class TRIGGERcmd
    {
        const string saythisFile = "/tmp/saythis.txt";
        const int MaxNumTries = 20; //For DHT22-i-Wire 

        /// <summary>
        /// Methods here are only for a single pass.
        /// DHT22-i-Wire can get invalids so do multiple tries.
        /// </summary>
        /// <param name="index"></param>
        public static void Trigger(int index, int[] gpios)
        {

            switch (index)
            {
                case 11:
                    Relay(true, gpios);
                    break;
                case 12:
                    Relay(false, gpios);
                    break;
                case 13:
                    //Console.WriteLine("Doing Get-Temp with DHTxx");
                    //GetTempDHTxx();
                    break;
                case 14:
                    Console.WriteLine("Doing Get-Temp with DHT22-i-Wire One result only");
                    GetTempDHTxx1Wire(MaxNumTries, gpios);
                    break;
                case 15:
                    Console.WriteLine("Doing Get-Temp with BME280 One result only");
                    DotNetCoreCoreGPIO.BME280Sampler.Get();
                    break;
                case 16:
                    SetLEDState(gpios, true);
                    break;
                case 17:
                    SetLEDState(gpios, false);
                    break;
                case 18:
                    Console.WriteLine($"Toggle LED/GPIO{gpios[(int)PinGPIOs.led]}");
                    SetLEDState(gpios, null);
                    break;
                case > 19:
                    int state = index - 20;
                    Console.WriteLine($"Doing MotorControl single pass state:{state}.");
                    MotorControl(state, gpios);
                    break;
            }
        }

        static void GetTempDHTxx1Wire(int maxNumTries, int[] gpios)
        {
            int numTries = 0;
            int delayMs = 2000;
            //1-Wire:
            int pin = gpios[(int)PinGPIOs.dht22]; //26;
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
                                    string result = $"Temperature equals {temp.DegreesCelsius.ToString("0.0")} °C ,";
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

        public static void MotorControl(int state, int[] gpios)
        {
            //GPIO Pin numbers:
            //=================
            var pinFwd = gpios[(int)PinGPIOs.motorfwd];// 17; // <- Brd Pin 11            If hi and pinBack is lo motor goes fwd
            var pinRev = gpios[(int)PinGPIOs.motorrev]; //27; // <- Brd Pin 13             if hi and pinFwd is lo motor goes back (reverse)
            var pinEn = gpios[(int)PinGPIOs.motoren]; // 22;  // <- Brd Pin 15            Overall enable/disable  hi/lo

            //Nb: if pinFwd=pinRev hi or lo then its brake
            Console.WriteLine(state);

            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(pinEn, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Enable): {pinEn}");
                controller.OpenPin(pinFwd, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Forward): {pinFwd}");
                controller.OpenPin(pinRev, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Reverse): {pinRev}");


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
                    case 3: //Enable
                        controller.Write(pinEn, PinValue.High);
                        stateStr = "Enable";
                        break;
                    case 4: //Disable 
                        controller.Write(pinEn, PinValue.Low);
                        stateStr = "Disable";
                        break;
                    case 5: // Off
                        controller.Write(pinEn, PinValue.Low);
                        controller.Write(pinRev, PinValue.Low);
                        controller.Write(pinFwd, PinValue.Low);
                        stateStr = "Off";
                        break;

                }
                string result = $"Done Motor Control. Command ({stateStr})";
                Console.WriteLine(result);
                WriteT2S(result);
            }
        }

        public static void WriteT2S(string txt)
        {
            if (File.Exists(saythisFile))
            {
                // If file found, delete it    
                File.Delete(saythisFile);
                Console.WriteLine(saythisFile);
            }
            string[] lines = txt.Split(',');
            File.WriteAllLines(saythisFile, lines);
            return;
        }


        private static void Relay(bool on, int[] gpios)
        {
            var pinRelay = gpios[(int)PinGPIOs.relay]; //19;  
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

        public static bool LEDState = false;
        private static void SetLEDState(int[] gpios, bool? on)
        {
            var pinRLED = gpios[(int)PinGPIOs.led];
            using (GpioController controller = new GpioController())
            {
                // This doesn't work for Toggle (Read then set state)
                controller.OpenPin(pinRLED, System.Device.Gpio.PinMode.Input);
                LEDState = (PinValue.High == controller.Read(pinRLED));
                controller.ClosePin(pinRLED);

                controller.OpenPin(pinRLED, System.Device.Gpio.PinMode.Output);
                if (on == null)
                    LEDState = !LEDState;
                else
                    LEDState = (bool)on;
                if (LEDState)
                {
                    Console.WriteLine($"Setting LED/GPIO {gpios[(int)PinGPIOs.led]} to ON");
                    controller.Write(pinRLED, System.Device.Gpio.PinValue.High);
                }
                else
                {
                    Console.WriteLine($"Setting LED/GPIO {gpios[(int)PinGPIOs.led]} to OFF");
                    controller.Write(pinRLED, System.Device.Gpio.PinValue.Low);
                }
            }

        }
    }
}

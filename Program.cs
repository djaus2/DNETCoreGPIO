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
using DNETCoreGPIO.TRIGGERcmdData;

namespace DotNetCoreCoreGPIO
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine(Figgle.FiggleFonts.Standard.Render("DNETCoreGPIO"));
            Console.WriteLine("Starting DJz DNETCoreGPIO App...A DotNet/IO Sampler!");
            Console.WriteLine("Based upon the https://github.com/dotnet/iot repository.");
            Console.WriteLine("Uses 2 Nuget packages from there: Iot.Device.Bindings and System.Device.Gpio ");
            Console.WriteLine("See in app how to include them in your .NET Core projects.");
            Console.WriteLine("This version has option 7: Run app once for each state change. See MotorControl().");
            Console.WriteLine();
            int state = -1;
            if (args.Length > 0)
            {
                int index;
                if (int.TryParse(args[0], out index))
                {
                    switch (index)
                    {
                        case 1:
                            Console.WriteLine("Doing Blink-LED");
                            Blinkled();
                            break;
                        case 2:
                            Console.WriteLine("Doing Get-Temp with BMP180");
                            BMP180Sampler.Run();
                            break;
                        case 3:
                            Console.WriteLine("Doing Get-Temp with DHTxx");
                            GetTempDHTxx();
                            break;
                        case 4:
                            Console.WriteLine("Doing Get-Temp with DHT22-i-Wire");
                            GetTempDHTxx1Wire();
                            break;
                        case 5:
                            Console.WriteLine("Doing LED PWM");
                            SoftwarePWM();
                            break;
                        case 6:
                            Console.WriteLine("Doing Motor");
                            Motor();
                            break;
                        case >9:
                            TRIGGERcmd.Trigger(index);
                            break;
                        default:
                            Console.WriteLine("Command line is DNETCoreGPIO n where n is:");
                            Console.WriteLine("========================================");
                            Console.WriteLine("1. Doing Blink-LED                       ... Works on Raspian AND IoT-Core");
                            Console.WriteLine("2. Doing Get-Temp with BMP180            ... Not tested yet.");
                            Console.WriteLine("3. Doing Get-Temp with DHTxx             ... Not tested yet");
                            Console.WriteLine("4. Doing Get-Temp with DHT22-i-Wire      ... Only works on Raspian");
                            Console.WriteLine("5. Doing LED PWM                         ... Works on Raspian AND IoT-Core");
                            Console.WriteLine("6. Doing Motor H-Bridge style with L293D ... Works on Raspian ... should work on IoT-Core.");
                            Console.WriteLine("10. to 15.  Doing Motor H-Bridge style with L293D. . See MotorControl(()");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Command line is DNETCoreGPIO n where n is:");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Doing Blink-LED                       ... Works on Raspian AND IoT-Core");
                Console.WriteLine("2. Doing Get-Temp with BMP180            ... Not tested yet.");
                Console.WriteLine("3. Doing Get-Temp with DHTxx             ... Not tested yet");
                Console.WriteLine("4. Doing Get-Temp with DHT22-i-Wire      ... Only works on Raspian");
                Console.WriteLine("6. Doing Motor H-Bridge style with L293D ... Works on Raspian ... should work on IoT-Core.");
                Console.WriteLine("10. to 15.  Doing Motor H-Bridge style with L293D. . See MotorControl(()");
            }


            Console.WriteLine("Done!");
            if (state < 0)
            {
                // Skip press for headless
                Console.WriteLine("Press [Return] to exit");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Press and release press buton.
        /// Then LED will flash until pressed again (may need to hold press for a moment).
        /// </summary>
        static void Blinkled()
        {
            var pinOut = 17;// <- Pin 11            Connect to LED Grounded through led and resitor (1kish)
            var pinIn = 4;  // <- Actual Pin 7     Connect to press button, pulled hi
            var lightTimeInMilliseconds = 1000;
            var dimTimeInMilliseconds = 200;

            Console.WriteLine($"Let's blink an LED! Some button presses as well.");
            using (GpioController controller = new GpioController())
            {
               
                controller.OpenPin(pinOut, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for Output (The LED): {pinOut}");
                controller.OpenPin(pinIn, PinMode.Input);
                Console.WriteLine($"GPIO pin enabled for Input (The button): {pinIn}");

                PinValue state =  controller.Read(pinIn);
                PinValue start = state;


                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    controller.Dispose();
                };

                while (state == start)
                {
                    state = controller.Read(pinIn);
                    Console.WriteLine(state);
                    if (state == start)
                        Thread.Sleep(lightTimeInMilliseconds);
                }
                Console.WriteLine("Button Press detected.");
                while (state != start)
                {
                    state = controller.Read(pinIn);
                    Console.WriteLine(state);
                    if (state != start)
                        Thread.Sleep(lightTimeInMilliseconds);
                }
                Console.WriteLine("Button Release detected.");

                state = controller.Read(pinIn);
                start = state;
                Console.WriteLine("Press and hold button to exit");
                Console.WriteLine("");
                while (state == start)
                {
                    Console.WriteLine($"Light for {lightTimeInMilliseconds}ms");
                    controller.Write(pinOut, PinValue.High);
                    Thread.Sleep(lightTimeInMilliseconds);

                    state = controller.Read(pinIn);
                    Console.WriteLine(state);
                    if (state != start)
                        Console.WriteLine("Keeo holding down til app exits");

                    Console.WriteLine($"Dim for {dimTimeInMilliseconds}ms");
                    controller.Write(pinOut, PinValue.Low);
                    Thread.Sleep(dimTimeInMilliseconds);

                    state = controller.Read(pinIn);
                    Console.WriteLine(state);
                }
            }
        }

        static void GetTempBMP280()
        {
            CpuTemperature temp = new CpuTemperature();
            while (true)
            {
                    Console.WriteLine(
                        $"Temperature: {temp.Temperature.DegreesCelsius.ToString("0.0")} °C");

                Thread.Sleep(2000);
            }
        }

        static void GetTempDHTxx()
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(1, Dht10.DefaultI2cAddress);
            I2cDevice device = I2cDevice.Create(settings);
            using (Dht10 dht = new Dht10(device))
            {
                while (true)
                {
                    UnitsNet.Temperature temp;
                    UnitsNet.RelativeHumidity humid;
                    if (dht.TryReadTemperature(out temp))
                        if (dht.TryReadHumidity(out humid))
                            {
                            Console.WriteLine(
                                $"Temperature: {temp.DegreesCelsius.ToString("0.0")} °C, Humidity: {humid.Percent.ToString("0.0")} %");
                        }
                        else
                        {
                            Console.WriteLine("Humidity reading failed);");
                        }
                    else
                    {
                        Console.WriteLine("Temperature reading failed.");
                    }

                    Thread.Sleep(2000);
                }
            }
        }

        static void GetTempDHTxx1Wire()
        {
            int delayMs = 2000;
            //1-Wire:
            int  pin = 26;
            Console.WriteLine("Using DH22-1-Wire1");
            bool lastResult = true;
            using (Dht22 dht = new Dht22(pin))
            {
                if (dht == null)
                {
                    Console.WriteLine("Dht22 instantiation failed");




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
                            }
                            else
                            {
                                bool resultIsSane = true;
                                if ((temp.DegreesCelsius > 100) || (temp.DegreesCelsius < -20))
                                    resultIsSane = false;
                                else if ((humid.Percent > 100) || (humid.Percent <0))
                                    resultIsSane = false;
                                if (!resultIsSane)
                                {
                                    Console.Write("x");
                                    lastResult = false;
                                }
                                else
                                {

                                    if (!lastResult)
                                        Console.WriteLine("");
                                    lastResult = true;
                                    Console.Write($"Temperature: {temp.DegreesCelsius.ToString("0.0")} °C ");
                                    Console.Write($"Humidity: { humid.Percent.ToString("0.0")} % ");
                                    Console.WriteLine("");
                                }
                            }
                        }
                        Thread.Sleep(delayMs);
                    }
                }
            }
        }

        static void SoftwarePWM()
        {
            Console.WriteLine("Connect LED to GND. Connect hi end through resistor to GPIO 17 Pin 11.");
            //var softwarePwmChannelWithPrecisionTimer = new SoftwarePwmChannel(17, frequency: 50, dutyCyclePercentage = 0.5, usePrecisionTimer: true);
            using (var pwmChannel = new SoftwarePwmChannel(17, 200, 0))
            {
                Console.WriteLine("Starting");
                pwmChannel.Start();
                for (double fill = 0.0; fill <= 1.0; fill += 0.01)
                {
                    pwmChannel.DutyCycle = fill;
                    Thread.Sleep(100);
                }
            }
            Console.WriteLine("Done");
        }
        
        /// <summary>
        /// Drive a DC Motor forwards and back.
        /// Effectively a H-Bridge (but no one of pinFwd and pinRev active and other disabled combination)
        /// Using L293D: See http://www.robotplatform.com/howto/L293/motor_driver_1.html
        /// More: https://www.alldatasheet.com/datasheet-pdf/pdf/22432/STMICROELECTRONICS/L293D.html
        /// 2Do: Add PWM
        /// </summary>
        static void Motor()
        {
            //GPIO Pin numbers:
            //=================
            var pinFwd = 17; // <- Brd Pin 11            If hi and pinBack is lo motor goes fwd
            var pinRev = 27; // <- Brd Pin 13             if hi and pinFwd is lo motor goes back (reverse)
            var pinEn = 22;  // <- Brd Pin 15            Overall enable/disable  hi/lo

            //Nb: if pinFwd=pinRev hi or lo then its brake

            Console.WriteLine($"Let control a DC motor!");
            using (GpioController controller = new GpioController())
            {

                controller.OpenPin(pinEn, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Enable): {pinEn}");
                controller.OpenPin(pinRev, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Reverse): {pinRev}");
                controller.OpenPin(pinFwd, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use (Output:Forward): {pinFwd}");

                controller.Write(pinEn, PinValue.Low);
                controller.Write(pinFwd, PinValue.Low);
                controller.Write(pinRev, PinValue.Low);

                Console.WriteLine();
                Console.WriteLine("Motor Commands:");
                Console.WriteLine("===============");
                Console.WriteLine("E: Enable");
                Console.WriteLine("D: Disable");
                Console.WriteLine("F: Forwards");
                Console.WriteLine("R: Reverse");
                Console.WriteLine("B: Brake");
                Console.WriteLine("Fwd, Rev and Brake don't apply  until enabled.");
                Console.WriteLine("Q: Quit");

                bool exitNow = false;
                while (!exitNow)
                {
                    var chrk = Console.ReadKey(false);
                    bool fwdState = (bool)controller.Read(pinFwd);
                    bool revState = (bool)controller.Read(pinRev);
                    char ch = chrk.KeyChar;
                    switch (char.ToUpper(ch))
                    {
                        case '0':
                            controller.Write(pinFwd, PinValue.Low);
                            break;
                        case '1':
                            controller.Write(pinFwd, PinValue.High);
                            break;
                        case '2':
                            controller.Write(pinRev, PinValue.Low);
                            break;
                        case '3':
                            controller.Write(pinRev, PinValue.High);
                            break;
                        case '4':
                            controller.Write(pinEn, PinValue.Low);
                            break;
                        case '5':
                            controller.Write(pinEn, PinValue.High);
                            break;

                        case 'F': //Forward
                                  //Fwd: Take action so as to eliminate undesirable intermediate state/s
                            if (fwdState && revState)
                            {
                                //Is braked (hi)
                                controller.Write(pinRev, PinValue.Low);
                            }
                            else if (!fwdState && revState)
                            {
                                //Is Rev. Brake first
                                controller.Write(pinRev, PinValue.Low);
                                controller.Write(pinFwd, PinValue.High);
                            }
                            else if (!fwdState && !revState)
                            {
                                //Is braked (lo)
                                controller.Write(pinFwd, PinValue.High);
                            }
                            else if (fwdState && !revState)
                            {
                                //Is fwd
                            }

                            break;
                        case 'R': // Reverse
                            if (fwdState && revState)
                            {
                                //Is braked (hi)
                                controller.Write(pinFwd, PinValue.Low);
                            }
                            else if (!fwdState && revState)
                            {
                                //Is reverse
                            }
                            else if (!fwdState && !revState)
                            {
                                //Is braked (lo)
                                controller.Write(pinRev, PinValue.High);
                            }
                            else if (fwdState && !revState)
                            {
                                //Is fwd: Brake first
                                controller.Write(pinFwd, PinValue.Low);
                                controller.Write(pinRev, PinValue.High);
                            }
                            break;
                        case 'B': //Brake
                            if (fwdState && revState)
                            {
                                //Is braked (hi)
                            }
                            else if (!fwdState && revState)
                            {
                                //Is Rev: Brake lo
                                controller.Write(pinRev, PinValue.Low);
                            }
                            else if (!fwdState && !revState)
                            {
                                //Is braked (lo)
                            }
                            else if (fwdState && !revState)
                            {
                                //Is fwd: Brake lo
                                controller.Write(pinFwd, PinValue.Low);
                            }
                            break;
                        case 'E': //Enable
                            controller.Write(pinEn, PinValue.High);
                            break;
                        case 'D': //Disable
                            controller.Write(pinEn, PinValue.Low);
                            break;
                        case 'Q':
                            exitNow = true;
                                break;
                    }

                }

            }
        }




    }
}

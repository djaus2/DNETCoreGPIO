using System;
// install-Package System.Device.Gpio -Version 1.1.0-prerelease.20153
using System.Device.Gpio;
// Install-Package Iot.Device.Bindings -Version 1.1.0-prerelease.20153.1
using Iot.Device;
using Iot.Device.CpuTemperature;
using System.Device.I2c;
using Iot.Device.Bmp180;
using Iot.Device.DHTxx;
using System.Threading;
using System.Device.Pwm.Drivers;

namespace DotNetCoreCoreGPIO
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting DNETCoreGPIO App");
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
                            Console.WriteLine("Doing Get-Tep with BMP280");
                            GetTempBMP280();
                            break;
                        case 3:
                            Console.WriteLine("Doing Get-Temp with DHTxx");
                            GetTempDHTxx();
                            break;
                        case 4:
                            Console.WriteLine("Doing Get-Temp with DHTxx-i-Wire");
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
                        default:
                            Console.WriteLine("Doing Blink-LED");
                            Blinkled();
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Doing Blink-LED");
                Blinkled();
            }


            Console.WriteLine("Done!");
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

            Console.WriteLine($"Let's blink an LED!");
            using (GpioController controller = new GpioController())
            {
               
                controller.OpenPin(pinOut, PinMode.Output);
                Console.WriteLine($"GPIO pin enabled for use: {pinOut}");
                controller.OpenPin(pinIn, PinMode.Input);
                Console.WriteLine($"GPIO pin enabled for use: {pinIn}");

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
                        $"Temperature: {temp.Temperature.Celsius.ToString("0.0")} °C");

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
                    Console.WriteLine(
                        $"Temperature: {dht.Temperature.Celsius.ToString("0.0")} °C, Humidity: {dht.Humidity.ToString("0.0")} %");

                    Thread.Sleep(2000);
                }
            }
        }

        static void GetTempDHTxx1Wire()
        {
            //1-Wire:
            int  pin = 26;
            Console.WriteLine("Using DH22-1-Wire1");
            bool lastResult = true;
            using (Dht22 dht = new Dht22(pin))
            {
                while (true)
                {
                    var temp = dht.Temperature.Celsius;
                    bool result1 = dht.IsLastReadSuccessful;
                    var humid = dht.Humidity;
                    bool result2 = dht.IsLastReadSuccessful;
                    if (!result1 && !result2)
                    {
                        Console.Write(".");
                        lastResult = false;
                    }
                    else {
                        if (!lastResult)
                            Console.WriteLine("");
                        lastResult = true;
                        if (!(temp is double.NaN))
                            Console.Write($"Temperature: {temp.ToString("0.0")} °C ");
                        if (!(humid is double.NaN))
                            Console.Write($"Humidity: { dht.Humidity.ToString("0.0")} % ");
                        Console.WriteLine("");
                    }
                    //if (dht.IsLastReadSuccessful)
                    //{
                    //    Console.WriteLine(
                    //        $"Temperature: {dht.Temperature.Celsius.ToString("0.0")} °C, Humidity: {dht.Humidity.ToString("0.0")} %");
                    //}
                    Thread.Sleep(2500);
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
        var pinFwd = 17; // <- Pin 11            If hi and pinBack is lo motor goes fwd
        var pinRev = 4;  //  <- Actual Pin 7      if hi and pinFwd is lo motor goes back (reverse)
        var pinEn = 27;  // <- Pin 13            Overall enable/disable  hi/lo

        //Nb: if pinFwd=pinRev hi or lo then its brake

        Console.WriteLine($"Let's blink an LED!");
        using (GpioController controller = new GpioController())
        {

            controller.OpenPin(pinFwd, PinMode.Output);
            Console.WriteLine($"GPIO pin enabled for use: {pinFwd}");
            controller.OpenPin(pinRev, PinMode.Output);
            Console.WriteLine($"GPIO pin enabled for use: {pinRev}");
            controller.OpenPin(pinRev, PinMode.Output);
            Console.WriteLine($"GPIO pin enabled for use: {pinEn}");

            controller.Write(pinEn, PinValue.Low);
            controller.Write(pinFwd, PinValue.Low);
            controller.Write(pinRev, PinValue.Low);
            
            Console.WriteLine("Motor Commands:");
                Console.WriteLine("===============");
            Console.WriteLine("E: Enable");
            Console.WriteLine("D: Disable");
            Console.WriteLine("F: Forwards");
            Console.WriteLine("R: Reverse");
            Console.WriteLine("B: Break");
            Console.WriteLine("Fwd, Rev and Brake don't apply  until enabled.");
            Console.WriteLine("");

                while (true)
            {
                var chrk = Console.ReadKey();
                bool fwdState = (bool)controller.Read(pinFwd);
                bool revState = (bool)controller.Read(pinRev);
                char ch = chrk.KeyChar;
                switch (ch)
                {
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
                }

            }

        }
    }


    }
}

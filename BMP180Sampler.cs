// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Ref: https://github.com/dotnet/iot/blob/master/src/devices/Bmp180/samples/Bmp180.Sample.cs


using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Bmp180;
using Iot.Units;

namespace DotNetCoreCoreGPIO
{
    /// <summary>
    /// Test program main class
    /// </summary>
    public static class BMP180Sampler
    {
        /// <summary>
        /// Entry point for example program
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Run()
        {
            Console.WriteLine("Using BMP180!");

            try
            {
                // bus id on the raspberry pi 3
                const int busId = 1;

                var i2cSettings = new I2cConnectionSettings(busId, Bmp180.DefaultI2cAddress);
                var i2cDevice = I2cDevice.Create(i2cSettings);
                var i2cBmp280 = new Bmp180(i2cDevice);

                if (i2cBmp280 != null)
                {
                    using (i2cBmp280)
                    {
                        // set samplings
                        i2cBmp280.SetSampling(Sampling.Standard);

                        // read values
                        Temperature tempValue = i2cBmp280.ReadTemperature();
                        Console.WriteLine($"Temperature {tempValue.Celsius} \u00B0C");
                        var preValue = i2cBmp280.ReadPressure();
                        Console.WriteLine($"Pressure {preValue.Hectopascal} hPa");
                        double altValue = i2cBmp280.ReadAltitude();
                        Console.WriteLine($"Altitude {altValue:0.##} m");
                        Thread.Sleep(1000);

                        // set higher sampling
                        i2cBmp280.SetSampling(Sampling.UltraLowPower);

                        // read values
                        tempValue = i2cBmp280.ReadTemperature();
                        Console.WriteLine($"Temperature {tempValue.Celsius} \u00B0C");
                        preValue = i2cBmp280.ReadPressure();
                        Console.WriteLine($"Pressure {preValue.Hectopascal} hPa");
                        altValue = i2cBmp280.ReadAltitude();
                        Console.WriteLine($"Altitude {altValue:0.##} m");
                    }
                } else {
                    Console.WriteLine($"Failed: No DMP180");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed: Probably no hw.");
            }

        }

    }

}
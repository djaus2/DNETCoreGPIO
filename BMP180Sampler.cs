// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Ref: https://github.com/dotnet/iot/blob/master/src/devices/Bmp180/samples/Bmp180.Sample.cs


using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Common;
using UnitsNet;
//using Iot.Units;

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
            Console.WriteLine("Using BME280!");

            try
            {
                // set this to the current sea level pressure in the area for correct altitude readings
                Pressure defaultSeaLevelPressure = WeatherHelper.MeanSeaLevel;
                // bus id on the raspberry pi 3
                const int busId = 1;
                I2cConnectionSettings i2cSettings = new(busId, Bme280.DefaultI2cAddress);
                using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
                using Bme280 bme80 = new Bme280(i2cDevice)
                {
                    // set higher sampling
                    TemperatureSampling = Sampling.LowPower,
                    PressureSampling = Sampling.UltraHighResolution,
                    HumiditySampling = Sampling.Standard,

                };

                // Perform a synchronous measurement
                var readResult = bme80.Read();

                // Note that if you already have the pressure value and the temperature, you could also calculate altitude by using
                // var altValue = WeatherHelper.CalculateAltitude(preValue, defaultSeaLevelPressure, tempValue) which would be more performant.
                bme80.TryReadAltitude(defaultSeaLevelPressure, out var altValue);

                Console.WriteLine($"Temperature: {readResult.Temperature?.DegreesCelsius:0.#}\u00B0C");
                Console.WriteLine($"Pressure: {readResult.Pressure?.Hectopascals:0.##}hPa");
                Console.WriteLine($"Altitude: {altValue.Meters:0.##}m");
                Console.WriteLine($"Relative humidity: {readResult.Humidity?.Percent:0.#}%");

                /*if (i2cBmp280 != null)
                {
                    using (i2cBmp280)
                    {
                        // set samplings
                        i2cBmp280.SetSampling(Sampling.Standard);

                        // read values
                        UnitsNet.Temperature tempValue = i2cBmp280.ReadTemperature();
                        Console.WriteLine($"Temperature {tempValue.DegreesCelsius} \u00B0C");
                        var preValue = i2cBmp280.ReadPressure();
                        Console.WriteLine($"Pressure {preValue.Hectopascals} hPa");
                        double altValue = i2cBmp280.ReadAltitude().Meters;
                        Console.WriteLine($"Altitude {altValue:0.##} m");
                        Thread.Sleep(1000);

                        // set higher sampling
                        i2cBmp280.SetSampling(Sampling.UltraLowPower);

                        // read values
                        tempValue = i2cBmp280.ReadTemperature();
                        Console.WriteLine($"Temperature {tempValue.DegreesCelsius} \u00B0C");
                        preValue = i2cBmp280.ReadPressure();
                        Console.WriteLine($"Pressure {preValue.Hectopascals} hPa");
                        altValue = i2cBmp280.ReadAltitude().Meters;
                        Console.WriteLine($"Altitude {altValue:0.##} m");
                    }
                } else {
                    Console.WriteLine($"Failed: No DMP180");
                }*/
            }
            catch (Exception)
            {
                Console.WriteLine("Failed: Probably no hw.");
            }

        }

    }

}
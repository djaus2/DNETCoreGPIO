# DNETCoreGPIO

A .NET Core app to run on the RPi. Works with both Raspian and Win10 IoT-Core unchanged.
Exemplifies System.Devices.GPIO and Iot.Device.Bindings Nuget packages. Provided as Visual Studio 2019 project.

_**This is based upon the GitHUb Repository: [dotnet/iot](https://github.com/dotnet/iot)<br>
Whereas the samples therein (from which this is taken, and extended) use library sources in that repository, this uses the associated Nuget packages.**_

**Code includes:**
1. Led and Button press  <-- Works
2. Temperature with BME280 sensor  ..2Do
3. Temperature and Humidity with DHTxx sensor  ..2Do
4. Temperature and Humidity with DHT22 sensor using 1-Wire <-- Works on Raspian not IOT-Core
5. LED driven by Software PWM.  <-- Works
6. H-Bridge Motor using L293D <-- Works on Raspian (Pins have changed) should work on IoT-Core

To run ```./DNETCoreGPIO  n``` on Raspian or ```.\DNETCoreGPIO  n``` in a PowerShell windows on IoT-Core,  where **n is 1,2 ... 6** as above.

# Run
## Running Led and Button
Connect LED to GPIO17 and button to GPIO4 and Ground (to the one in between. ie Pins 11 7 and 9 resectively.
## SW driven LED
As above
## DHT22
See circuit diagram under Circuits  .<br> Active pin is is GPIO26. I'm using a 10K pullup to that. <br>Some conversions fail, which output as dots. Doesn't work on IOT-Core.
## Motor
See circuit diagram in Circuits folder, left part. Pins: Enable (E1) GPIO22 Reverse GPIO 22 (I1) Forward GPIO 17 (I2). (L293D pins in brackets)<br>
I also attached LEDs to each output.

## Publishing
You need to Publish the app from Visual Studio for linux-arm or win-arm. You can publish to a share of the Pi, or build to a folder on the desktop and then copy or send it across.

- You can Publish as Framework Independent. See Raspian publish profile (It publishes to a share).Correction .. currently publishes to desktop folder
- Or Build as Framework dependent. The IoT-Core publish profile publishes to a folder under \bin. You copy iot from there.

NB: You could though build and deploy from VS Code.

More on [My Blog](http://www.sportronics.com.au)

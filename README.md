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
6. H-Bridge Motor using L293D <-- Works on Raspian (Pins have changed). Works on IoT-Core (see below though).

To run ```./DNETCoreGPIO  n``` on Raspian or ```.\DNETCoreGPIO  n``` in a PowerShell windows on IoT-Core,  where **n is 1,2 ... 6** as above.

# Run
## Running Led and Button
Pins:
- LED Anode to GPIO17
- Button1 to GPIO4
- Ground (LED Cathode and Button2) (to the one in between. <br>ie Pins 11 7 and 9 resectively.
- Reminder that the LED requires a current limiting resistor.
## SW driven LED
As above
## DHT22
See circuit diagram under Circuits  .<br> Active pin is is GPIO26. I'm using a 10K pullup to that. <br>Some conversions fail, which output as dots. Doesn't work on IOT-Core.
## Motor
See circuit diagram in Circuits folder, left part. <br>Pins (L293D pins in brackets):
- Enable  GPIO22 (E1)
- Reverse GPIO 22 (I1) 
- Forward GPIO 17 (I2)
<p>
<b>On IoT-Core DO NOT RUN IN POWERSHELL.  Can't get user input for Console app in Remote PowerShell.<br> Run an SSH session.</b><br>
I also attached a LED to each output.

## Publishing
You need to Publish the app from Visual Studio for linux-arm or win-arm. You can publish to a share of the Pi, or build to a folder on the desktop and then copy or send it across.

- You can Publish as Framework Independent. See Raspian publish profile (It publishes to a share).Correction .. currently publishes to desktop folder
- Or Build as Framework dependent. The IoT-Core publish profile publishes to a folder under \bin. You copy iot from there.

NB: You could though build and deploy from VS Code.

More on [My Blog](http://www.sportronics.com.au)

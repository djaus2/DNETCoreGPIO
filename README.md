# DNETCoreGPIO

A .NET Core app to run on the RPi. Works with both Raspian and Win10 IoT-Core unchanged.
Exemplifies System.Devices.GPIO and Iot.Device.Bindings Nuget packages. Provided as Visual Studio 2019 project.

_**This is based upon the GitHUbRepository: [dotnet/iot](https://github.com/dotnet/iot)<br>
Whereas the samples therein (from which this is taken, and extended) use library sources in that repository, this uses the associated Nuget packages.**_

**Code includes:**
1. Led and Button press  <-- Works
2. Temperature with BMP180 sensor
3. Temperature and Humidity with DHTxx sensor
4. Temperature and Humidity with DHT22 sensor using 1-Wire <-- Works
5. LED driven by Software PWM.  <-- Works
6. H-Bridge Motor using L293D

To run ```./DNETCoreGPIO  n``` on Raspian or ```.\DNETCoreGPIO  n``` in a PowerShell windows on IoT-Core,  where **n is 1,2 ... 6** as above<br>
Only 1 has been tested.

## Running Led and Button
Connect LED to GPIO17 and button to GPIO4 and Ground (to the one in between. ie Pins 11 7 and 9 resectively.

## Publishing
You need to Publish the app from Visual Studio.
- You can Publish as Framework Independent. See Raspian publish profile (It publishes to a share).
- Or Build as Framework depedent. The IoT-Core publsih profile publises to a folder under \bin. You copy iot from there.

NB: You could though build and deploy from VS Code.

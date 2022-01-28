# DNETCoreGPIO

<hr/>

**TRIGGETcmd** Now implemented DHT22 1 wire and Motor commands using Hey Google, ... Nb Speech is on RPi though.  
**Update** Can now send speech to the Google Nest! Separate repository coming

**Currently** .Net 6.0 and 2.0.0-prerelease.22069 for GPIO and Device Bindings.

<hr/>

A .NET Core app to run on the RPi. Works with both Raspian and Win10 IoT-Core unchanged.
Exemplifies System.Devices.GPIO and Iot.Device.Bindings Nuget packages. Provided as Visual Studio 2022 project.

_**This is based upon the GitHUb Repository: [dotnet/iot](https://github.com/dotnet/iot)<br>
Whereas the samples therein (from which this is taken, and extended) use library sources in that repository, this uses the associated Nuget packages.**_

**Code includes:_(i.e. Run the app with the following parameters)_**

1. Led and Button press  <-- Works
2. Temperature with BME280 sensor  ..2Do
3. Temperature and Humidity with DHTxx sensor  ..2Do
4. Temperature and Humidity with DHT22 sensor using 1-Wire <-- Works on Raspian not IOT-Core
5. LED driven by Software PWM.  <-- Works
6. H-Bridge Motor using L293D <-- Works on Raspian (Pins have changed). Works on IoT-Core (see below though).

Whilst the above run continously or until stopped the following only do a single pass. These are used by TRIGGERcmd

14. Temperature and Humidity with DHT22 sensor using 1-Wire. Get single value and write to /tmp/temperature.txt. See TRIGGERcmd.GetTempDHTxx1Wire()  

The following control the motor as in 6 but as separate commands.

20. Motor Partial off: Set Fwd and Rev to off
21. Motor Forward
22. Motor Reverse
23. Motor Enable
24. Motor Disable
25. Motor Off (Fwd=Rev=En=off)


**To run**  
- If publishing from desktop to RPI, from within the deployed folder: ```./DNETCoreGPIO  n``` on Raspian or ```.\DNETCoreGPIO  n``` in a PowerShell windows on IoT-Core,  where **n is 1,2 ... 6** as above.
- Assuming the .Net Framework is installed _(Net 6 for latest)_ on the RPi etc:
  - If cloned repository to RPI, ```dotnet build``` in the root of the repository clone then ```dotnet run n``` there where n is 1..6 as above.
    - Or ```DNETCoreGPIO n``` if built folder, eg ~/GPIO/DNETCoreGPIO/bin/net6.0 is in the PATH
  - In this latest iteration I installed VS Code on the RPi.

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
- Enable  GPIO 22 (E1)
- Reverse GPIO 27 (I1) 
- Forward GPIO 17 (I2)
<p>
<b>On IoT-Core DO NOT RUN IN POWERSHELL.  Can't get user input for Console app in Remote PowerShell.<br> Run an SSH session.</b>Or directlly on the device.<br>
I also attached a LED to each output.

## Publishing
You need to Publish the app from Visual Studio for linux-arm or win-arm. You can publish to a share of the Pi, or build to a folder on the desktop and then copy or send it across.

- You can Publish as Framework Independent. See Raspian publish profile (It publishes to a share).Correction .. currently publishes to desktop folder
- Or Build as Framework dependent. The IoT-Core publish profile publishes to a folder under \bin. You copy iot from there.

NB: You could though build and deploy from VS Code.

More on [My Blog](https://davidjones.sportronics.com.au)

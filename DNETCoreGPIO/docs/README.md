# DNETCoreGPIO

A .NET library then can be used directly with a .NET console app app to run on the RPi. Works with both Raspian (Debian).  
Uses System.Devices.GPIO and Iot.Device.Bindings Nuget packages.

## Usage

Create a .NET Console app:  
Run ```dotnet new console```  

Add the library as a Nuget package  
Run ``` dotnet add package DNETCoreGPIO --version 1.y.x```  
NB: Use the current version, in place of 1.y.x,check at https://www.nuget.org/packages/DNETCoreGPIO/  
 _Can use \* to get latest._ 

Change the 7th line in Program.cs as below:

```csharp

namespace ConsoleGPIOApp
{
	class Program
	{
		static void Main(string[] args)
		{
			DotNetCoreCoreGPIO.Program.Main(args);
		}
	}
}
```  

NB: if using .NET6.0 Console template you only need a one line file for Program.cs:  
```DotNetCoreCoreGPIO.Program.Main(args);```

Build and run the app with the required first parameter._(See below)_
  
***Based upon samples in the GitHUb Repository: [dotnet/iot](https://github.com/dotnet/iot)  

**New Feature:** Can change GPIO mappings as second parameter. eg "17,4,26,22,27,17,19", which is the current settings.  
_Which map to **led,button,dht22,motoren, motorfwd,motorrev,relay** respectively._


## Parameters

**Run the app with one of the following as the first parameter** _Note possible 2nd parameter for GPIO mappings as above._  

1. Led and Button press
2. Temperature with BME280 sensor  ..2Do
3. Temperature and Humidity with DHTxx sensor  ..2Do
4. Temperature and Humidity with DHT22 sensor using 1-Wire <
5. LED driven by Software PWM.
6. H-Bridge Motor using L293D

Whilst the above run continously or until stopped the following only do a single pass.  
***These are used by TRIGGERcmd***  

11/12. Relay On/Off

14. Temperature and Humidity with DHT22 sensor using 1-Wire.  
Get single value and write to /tmp/temperature.txt on RPi  
See TRIGGERcmd.GetTempDHTxx1Wire()  
15. Temperature Pressure and Humidity with BME280.  
Get single values and write to /tmp/temperature.txt, on RPi


16. Set LED as per 1.  
17. Clear LED ditto  
18. Toggle LED ditto


The following control the motor as in 6 but as separate commands.

20. Motor Partial off: Set Fwd and Rev to off
21. Motor Forward
22. Motor Reverse
23. Motor Enable
24. Motor Disable
25. Motor Off (Fwd=Rev=En=off)

The following run continously:  
30. (2Do) Start sending DHT22 1-Wire Telemetry to Azure IoT Hub. Requires a period (default 10 sec) between readings and DeviceConnectionString.  
31. Start sending BME280 Telemetry to Azure IoT Hub. Requires a period (default 10 sec) between readings and DeviceConnectionString.

PS: Can toggle a soleniod with the LED.


# Run

> NB: Look at circuit diagrams [here on GitHub](https://github.com/djaus2/DNETCoreGPIO/tree/master/DNETCoreGPIO/Circuits)

## Running Led and Button
Pins:
- LED Anode to GPIO17
- Button1 to GPIO4
- Ground (LED Cathode and Button2) (to the one in between. <br>ie Pins 11 7 and 9 resectively.
- Reminder that the LED requires a current limiting resistor.
## SW driven LED
As above
## DHT22
See circuit diagram under Circuits.  
 Active pin is is GPIO26. I'm using a 10K pullup to that.  
Some conversions fail, which output as dots. Doesn't work on IOT-Core.  
## Motor
See circuit diagram in Circuits folder, left part. <br>Pins (L293D pins in brackets):
- Enable  GPIO 22 (E1) (L293D Enble 1)
- Reverse GPIO 27 (I1) 
- Forward GPIO 17 (I2)
## Relay
See circuit diagram in Circuits folder, right part. <br>Pins (L293D pins in brackets):
- On/Off  GPIO 19 (E2)  _(L233D Enable 2)_
- Lo (I4)               
- Hi (I3)
- _I4 and I3 only need to be in opposite state_
- Probably want Motor disabled if is low voltage and relay is 12V coil voltage.
## BME280
- See circuit diagram **rpi-bmp280_i2c.png** for 4 pin connections.
- If unit is more than 4 pins see **BME280Sampler.Get()** for extra pins.
- Also see there wrt ***enabling I2C on RPi*** and for checking.
## Azure IOT Hub
- Can periodicly send sensor readings to an Azure IoT Hub
- Option 30. DHT22 1-Wire (2Do)
- Option 31. BME280
- Both connected as per DHT22 and BME280 above.
- Need 2 additional parameters:
  - Period: Time in seconds beteween readings, eg 10
  - DeviceConnectionString
- Nb: Need to provide 4 parameters in these cases:
  - [30|31] gpioString period deviceconnectionstring
	- gpioString and period can both be a dot
    - In these cases the defaults are used:
      - gpioString default is "17,4,26,22,27,17,19"
        - _Hint:_ You can replace individual gpios in the string with a dot and the default is used for that.
      - period: default is 10
  

  
More on [My Blog](https://davidjones.sportronics.com.au)

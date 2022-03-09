# DNETCoreGPIO

A .NET Core app to run on the RPi. Works with both Raspian and Win10 IoT-Core unchanged.
Exemplifies System.Devices.GPIO and Iot.Device.Bindings Nuget packages. Provided as Visual Studio 2022 project.

## Usage

Create a .NET Console app:  
Run ```dotnet new console```  

Add the library as a Nuget package  
Run ``` dotnet add package DNETCoreGPIO --version 1.0.x```  
NB: Use the current version, in place of 1.0.x,check at https://www.nuget.org/packages/DNETCoreGPIO/  
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

Build and run the app with the required parameter._(See below)_
  
***This is based upon the GitHUb Repository: [dotnet/iot](https://github.com/dotnet/iot)<br>
Whereas the samples therein (from which this is taken, and extended) use library sources in that repository, this uses the associated Nuget packages.***

**New Feature:** Can change GPIO mappings as second parameter. eg "17,4,26,22,27,17,19", which is the current settings.  
_Which map to **led,button,dht22,motoren, motorfwd,motorrev,relay** respectively._


## Parameters

**Run the app with the following parameters**

1. Led and Button press
2. Temperature with BME280 sensor  ..2Do
3. Temperature and Humidity with DHTxx sensor  ..2Do
4. Temperature and Humidity with DHT22 sensor using 1-Wire <
5. LED driven by Software PWM.
6. H-Bridge Motor using L293D

Whilst the above run continously or until stopped the following only do a single pass.  
***These are used by TRIGGERcmd***  
11/12. Relay On/Off  
14. Temperature and Humidity with DHT22 sensor using 1-Wire. Get single value and write to /tmp/temperature.txt. See TRIGGERcmd.GetTempDHTxx1Wire()  
15. Temperature Pressure and Humidity with BME280. Get single values and write to /tmp/temperature.txt  

The following control the motor as in 6 but as separate commands.

20. Motor Partial off: Set Fwd and Rev to off
21. Motor Forward
22. Motor Reverse
23. Motor Enable
24. Motor Disable
25. Motor Off (Fwd=Rev=En=off)


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

  
More on [My Blog](https://davidjones.sportronics.com.au)

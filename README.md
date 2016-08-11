[![MAG3110](MAG3110_I2CS.png)](https://www.controleverything.com/content/Compass?sku=MAG3110_I2CS)
# MAG3110
MAG3110 3-Axis Digital Magnetometer Electronic Compass  

The MAG3110 is a digital 3-axis magnetometer.

This Device is available from ControlEverything.com [SKU: MAG3110_I2CS]

https://www.controleverything.com/content/Compass?sku=MAG3110_I2CS

This Sample code can be used with Raspberry pi, Arduino and Beaglebone Black.

## Java 
Download and install pi4j library on Raspberry pi. Steps to install pi4j are provided at:

http://pi4j.com/install.html

Download (or git pull) the code in pi.

Compile the java program.
```cpp
$> pi4j MAG3110.java
```

Run the java program as.
```cpp
$> pi4j MAG3110
```

## Python 
Download and install smbus library on Raspberry pi. Steps to install smbus are provided at:

https://pypi.python.org/pypi/smbus-cffi/0.5.1

Download (or git pull) the code in pi. Run the program

```cpp
$> python MAG3110.py
```
 
## Arduino
Download and install Arduino Software (IDE) on your machine. Steps to install Arduino are provided at:
 
https://www.arduino.cc/en/Main/Software
 
Download (or git pull) the code and double click the file to run the program.
 
Compile and upload the code on Arduino IDE and see the output on Serial Monitor.


## C

Setup your BeagleBone Black according to steps provided at:

https://beagleboard.org/getting-started

Download (or git pull) the code in Beaglebone Black.

Compile the c program.
```cpp
$>gcc MAG3110.c -o MAG3110
```
Run the c program.
```cpp
$>./MAG3110
 ```
 
 ## Onion Omega

Get Started and setting up the Onion Omega according to steps provided at :

https://wiki.onion.io/Get-Started

To install the Python module, run the following commands:
```cpp
opkg update
```
```cpp
opkg install python-light pyOnionI2C
```

Download (or git pull) the code in Onion Omega. Run the program.

```cpp
$> python MAG3110.py
```
#####The code output is raw value of magnetic field in X, Y and Z axis

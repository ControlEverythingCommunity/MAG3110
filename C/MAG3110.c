// Distributed with a free-will license.
// Use it any way you want, profit or free, provided it fits in the licenses of its associated works.
// MAG3110
// This code is designed to work with the MAG3110_I2CS I2C Mini Module available from ControlEverything.com.
// https://www.controleverything.com/content/Compass?sku=MAG3110_I2CS#tabs-0-product_tabset-2

#include <stdio.h>
#include <stdlib.h>
#include <linux/i2c-dev.h>
#include <sys/ioctl.h>
#include <fcntl.h>

void main() 
{
	// Create I2C bus
	int file;
	char *bus = "/dev/i2c-1";
	if((file = open(bus, O_RDWR)) < 0)
	{
		printf("Failed to open the bus. \n");
		exit(1);
	}
	// Get I2C device, MAG110 I2C address is 0x0E(14)
	ioctl(file, I2C_SLAVE, 0x0E);

	// Select control register1(0x10)
	// Send Start Command , Active mode
	char config[2] = {0};
	config[0] = 0x10;
	config[1] = 0x01;
	write(file, config, 2);
	sleep(1);

	// Read 6 bytes of data
	// msb first
	// Read xMag msb data from register(0x01)
	char reg[1] = {0x01};
	write(file, reg, 1);
	char data[1] = {0};
	if(read(file, data, 1) != 1)
	{
		printf("Erorr : Input/output Erorr \n");
		exit(1);
	}
	char data_0 = data[0];

	// Read xMag lsb data from register(0x02)
	reg[0] = 0x02;
	write(file, reg, 1);
	read(file, data, 1);
	char data_1 = data[0];

	// Read yMag msb data from register(0x03)
	reg[0] = 0x03;
	write(file, reg, 1);
	read(file, data, 1);
	char data_2 = data[0];

	// Read yMag lsb data from register(0x04)
	reg[0] = 0x04;
	write(file, reg, 1);
	read(file, data, 1);
	char data_3 = data[0];

	// Read zMag msb data from register(0x05)
	reg[0] = 0x05;
	write(file, reg, 1);
	read(file, data, 1);
	char data_4 = data[0];

	// Read zMag lsb data from register(0x06)
	reg[0] = 0x06;
	write(file, reg, 1);
	read(file, data, 1);
	char data_5 = data[0];

	// Convert the data
	int xMag = (data_0 * 256 + data_1);
	if(xMag > 32767)
	{
		xMag -= 65536;
	}

	int yMag = (data_2 * 256 + data_3);
	if(yMag > 32767)
	{
		yMag -= 65536;
	}

	int zMag = (data_4 * 256 + data_5);
	if(zMag > 32767)
	{
	zMag -= 65536;
	}

	// Output data to screen
	printf("Magnetic field in X-Axis : %d \n", xMag);
	printf("Magnetic field in Y-Axis : %d \n", yMag);
	printf("Magnetic field in Z-Axis : %d \n", zMag);
}
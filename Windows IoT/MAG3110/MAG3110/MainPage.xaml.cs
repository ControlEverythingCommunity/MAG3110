// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace MAG3110
{
	struct Compass
	{
		public double X;
		public double Y;
		public double Z;
	};

	//	App that reads data over I2C from a MAG3110 compass
	public sealed partial class MainPage : Page
	{
		private const byte COMPASS_I2C_ADDR = 0x0E;	// I2C address of the MAG3110
		private const byte COMPASS_REG_CTRL_1 = 0x10;	 // Control Register 1 register
		private const byte COMPASS_REG_X = 0x01;	// X Axis data register
		private const byte COMPASS_REG_Y = 0x03;	// Y Axis data register
		private const byte COMPASS_REG_Z = 0x05;	// Z Axis data register

		private I2cDevice I2Ccompass;
		private Timer periodicTimer;

		public MainPage()
		{
			this.InitializeComponent();

			// Register for the unloaded event so we can clean up upon exit
			Unloaded += MainPage_Unloaded;

			// Initialize the I2C bus, compass, and timer
			InitI2Ccompass();
		}

		private async void InitI2Ccompass()
		{
			string aqs = I2cDevice.GetDeviceSelector();		// Get a selector string that will return all I2C controllers on the system
			var dis = await DeviceInformation.FindAllAsync(aqs);	// Find the I2C bus controller device with our selector string
			if (dis.Count == 0)
			{
				Text_Status.Text = "No I2C controllers were found on the system";
				return;
			}

			var settings = new I2cConnectionSettings(COMPASS_I2C_ADDR);
			settings.BusSpeed = I2cBusSpeed.FastMode;
			I2Ccompass = await I2cDevice.FromIdAsync(dis[0].Id, settings);	// Create an I2C Device with our selected bus controller and I2C settings
			if (I2Ccompass == null)
			{
				Text_Status.Text = string.Format(
					"Slave address {0} on I2C Controller {1} is currently in use by " +
					"another application. Please ensure that no other applications are using I2C.",
					settings.SlaveAddress,
					dis[0].Id);
				return;
			}

			/*
				Initialize the compass:
				For this device, we create 2-byte write buffers:
				The first byte is the register address we want to write to.
				The second byte is the contents that we want to write to the register.
			*/
			byte[] WriteBuf_Ctrl1 = new byte[] { COMPASS_REG_CTRL_1, 0x01 };	// 0x01 sets data output rate = 80.00 Hz, Over sample ratio = 16, 16-bit value read, normal operation, Active mode

			// Write the register settings
			try
			{
                		I2Ccompass.Write(WriteBuf_Ctrl1);
			}
			// If the write fails display the error and stop running
			catch (Exception ex)
			{
				Text_Status.Text = "Failed to communicate with device: " + ex.Message;
				return;
			}

			// Create a timer to read data every 300ms
			periodicTimer = new Timer(this.TimerCallback, null, 0, 300);
		}

		private void MainPage_Unloaded(object sender, object args)
		{
			// Cleanup
			I2Ccompass.Dispose();
		}

		private void TimerCallback(object state)
		{
			string xText, yText, zText;
			string addressText, statusText;

			// Read and format compass data
			try
			{
				Compass compass = ReadI2Ccompass();
				addressText = "I2C Address of the compass MAG3110: 0x0E";
				xText = String.Format("X Axis: {0:F0}", compass.X);
				yText = String.Format("Y Axis: {0:F0}", compass.Y);
				zText = String.Format("Z Axis: {0:F0}", compass.Z);
				statusText = "Status: Running";
			}
			catch (Exception ex)
			{
				xText = "X Axis: Error";
				yText = "Y Axis: Error";
				zText = "Z Axis: Error";
				statusText = "Failed to read from compass: " + ex.Message;
			}

			// UI updates must be invoked on the UI thread
			var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				Text_X_Axis.Text = xText;
				Text_Y_Axis.Text = yText;
				Text_Z_Axis.Text = zText;
				Text_Status.Text = statusText;
			});
		}

		private Compass ReadI2Ccompass()
		{
			byte[] RegAddrBuf = new byte[] { COMPASS_REG_X };	// Read data from the register address
			byte[] ReadBuf = new byte[6];				// We read 6 bytes sequentially to get all 3 two-byte axes registers in one read

			/*
				Read from the compass 
				We call WriteRead() so we first write the address of the X-Axis I2C register, then read all 3 axes
			*/
			I2Ccompass.WriteRead(RegAddrBuf, ReadBuf);

			// Check the endianness of the system and flip the bytes if necessary
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(ReadBuf, 0, 2);
				Array.Reverse(ReadBuf, 2, 2);
				Array.Reverse(ReadBuf, 4, 2);
			}

			/*
				In order to get the raw 16-bit data values, we need to concatenate two 8-bit bytes from the I2C read for each axis.
				We accomplish this by using the BitConverter class.
			*/
			short compassRawX = BitConverter.ToInt16(ReadBuf, 0);
			short compassRawY = BitConverter.ToInt16(ReadBuf, 2);
			short compassRawZ = BitConverter.ToInt16(ReadBuf, 4);

			Compass compass;
			compass.X = compassRawX;
			compass.Y = compassRawY;
			compass.Z = compassRawZ;

			return compass;
		}
	}
}

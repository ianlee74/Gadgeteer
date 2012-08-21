/*using System;
using System.Threading;
using System.Collections;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using CobraController.Objects;

namespace CobraController
{

    class OWSensors
    {
        #region Enums

        const string DS18S20 = "DS18S20";
        const string DS18B20 = "DS18B20";
        const string DS1822 = "DS1822";
        const string DS2438 = "DS2438";

        const string Temp = "Temp";
        const string Light = "Light";
        const string Hum = "Hum";

        enum Model
        {
            DS18S20 = 0x10,
            DS18B20 = 0x28,
            DS1822 = 0x22,
            DS2438 = 0x26
        }

        enum Command
        {
            SearchROM = 0xF0,
            ReadROM = 0x33,
            MatchROM = 0x55,
            SkipROM = 0xCC,
            AlarmSearch = 0xEC,
            StartTemperatureConversion = 0x44,
            StartVoltageConversion = 0xB4,
            ReadScratchPad = 0xBE,
            WriteScratchPad = 0x4E,
            CopySratchPad = 0x48,
            RecallEEPROM = 0xB8,
            ReadPowerSupply = 0xB4,
            ScratchPadPage0 = 0x00,
            ScratchPadPage3 = 0x03
        }

        enum ScratchPadDS2438
        {
            CONFIGURATION = 0,
            TEMP_LSB = 1,
            TEMP_MSB = 2,
            VOLTAGE_LSB = 3,
            VOLTAGE_MSB = 4,
            CURRENT_LSB = 5,
            CURRENT_MSB = 6,
            THRESHOLD = 7,
            SCRATCHPAD_CRC = 8
        }

        enum SPDS2438
        {
            CONFIGURATION,
            TEMP_LSB,
            TEMP_MSB,
            VOLTAGE_LSB,
            VOLTAGE_MSB,
            CURRENT_LSB,
            CURRENT_MSB,
            THRESHOLD,
            SCRATCHPAD_CRC
        }

        #endregion

        static OneWire ow;
        static double temptemp;

        public OWSensors(FEZ_Pin.Digital pin)
        {
            ow = new OneWire((Cpu.Pin)pin);
        }

        public void GetSensorValues()
        {
            lock (SensorCollection.GetInstance)
            {
                SensorCollection.GetInstance.Collection.Clear();

                int OW_number = 0;
                byte[] OW_address = new byte[8];
                string SensAddress;
                string NewSensAddress;
                string SensModel;
                string SensType;

                if (ow.Reset())
                {
                    while (ow.Search_GetNextDevice(OW_address))
                    {
                        OW_number++;

                        SensAddress = GetSensorAddress(OW_address);
                        SensModel = GetSensorModel(OW_address);
                        SensType = GetSensorType(OW_address);

                        if (SensModel == DS2438)
                        {
                            SensorData sd = new SensorData();
                            NewSensAddress = SensAddress + ":T";
                            sd.sensorAddress = SensAddress + ":T";
                            sd.sensorModel = SensModel;
                            sd.sensorType = Temp;
                            sd.sensorValue = GetTemperatureC(OW_address);
                            temptemp = sd.sensorValue;
                            SensorCollection.GetInstance.Collection.Add(sd);

                            if (MultiSensor(OW_address))
                            {
                                sd = new SensorData();
                                sd.sensorModel = SensModel;
                                sd.sensorType = SensType;
                                if (sd.sensorType == Hum) sd.sensorValue = GetHumidity(OW_address, temptemp);
                                if (sd.sensorType == Light) sd.sensorValue = GetLightLevel(OW_address);
                                //NewSensAddress = SensAddress;
                                if (sd.sensorType == Hum)
                                {
                                    NewSensAddress = SensAddress + ":H";
                                    sd.sensorAddress = SensAddress + ":H";
                                }
                                if (sd.sensorType == Light)
                                {
                                    NewSensAddress = SensAddress + ":L";
                                    sd.sensorAddress = SensAddress + ":L";
                                }
                                SensorCollection.GetInstance.Collection.Add(sd);
                            }
                        }
                        else
                        {
                            SensorData sd = new SensorData();
                            NewSensAddress = SensAddress + ":T";
                            sd.sensorAddress = SensAddress + ":T";
                            sd.sensorModel = SensModel;
                            sd.sensorType = Temp;
                            sd.sensorValue = GetTemperatureC(OW_address);
                            SensorCollection.GetInstance.Collection.Add(sd);
                        }
                        //Debug.Print("Model:" + SensModel + " Type:" + SensType + " Address:" + NewSensAddress );
                    }
                }
                //Debug.Print("Sensors found: " + SensorCollection.GetInstance.Collection.Count);
            }
        }

        private string GetSensorAddress(byte[] ow_address)
        {
            int i;
            string address = string.Empty;
            for (i = 0; i < 8; i++)
            {
                address += Helpers.FromByteToHex(ow_address[i]);
                if (i < 7)
                {
                    address += ":";
                }
            }
            return address;
        }

        private string GetSensorModel(byte[] ow_address)
        {
            switch ((ow_address[0]))
            {
                case (byte)Model.DS18B20:
                    return DS18B20;
                case (byte)Model.DS2438:
                    return DS2438;
                case (byte)Model.DS18S20:
                    return DS18S20;
                case (byte)Model.DS1822:
                    return DS1822;
                default:
                    return "unknown";
            }
        }

        private bool MultiSensor(byte[] ow_address)
        {
            byte[] model = new byte[1];
            ow.Reset();
            ow.WriteByte((byte)Command.MatchROM);
            ow.Write(ow_address, 0, ow_address.Length);
            ow.WriteByte((byte)Command.ReadScratchPad);
            ow.WriteByte((byte)Command.ScratchPadPage3);
            ow.Read(model, 0, 1);
            return model[0] > 0 ? true : false;
        }

        private string GetSensorType(byte[] ow_address)
        {
            byte[] model = new byte[1];
            ow.Reset();
            ow.WriteByte((byte)Command.MatchROM);
            ow.Write(ow_address, 0, ow_address.Length);
            ow.WriteByte((byte)Command.ReadScratchPad);
            ow.WriteByte((byte)Command.ScratchPadPage3);
            ow.Read(model, 0, 1);

            switch (model[0])
            {
                case 25:
                    return Hum;
                case 27:
                    return Light;
                default:
                    return "na";
            }
        }

        private float GetTemperatureC(byte[] ow_address)
		{
			float TemperatureC = 0;
			byte[] scratchpad = new byte[9];
			ushort tempLow = 0;
			ushort tempHigh = 0;
			byte countremain = 0;
			byte countc = 0;
			ushort temp = 0x0;
			float result = 0f;
			bool negative = false;
			int temptemp;
			short t = 0;
 
			switch (ow_address[0])
			{
				case (byte)Model.DS18S20:
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.StartTemperatureConversion);
					while (ow.ReadByte() == 0) { }
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.ReadScratchPad);
					ow.Read(scratchpad, 0, 9);
					tempLow = scratchpad[0];
					tempHigh = scratchpad[1];
					countremain = scratchpad[6];
					countc = scratchpad[7];
					Int16 rawTemperature = (Int16)((tempHigh << 8) | tempLow);
					TemperatureC = (float)(rawTemperature >> 1) - 0.25f + ((float)(countc - countremain) / (float)countc);
					break;
				case (byte)Model.DS18B20:
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.StartTemperatureConversion);
					while (ow.ReadByte() == 0) { }
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.ReadScratchPad);
					ow.Read(scratchpad, 0, 9);
					tempLow = scratchpad[0];
					tempHigh = scratchpad[1];
					byte config = scratchpad[4];
					bool crc = OneWire.CalculateCRC(scratchpad, 0, 8) == scratchpad[8];
					temp = tempLow;
					temp |= (ushort)(tempHigh << 8);
					temptemp = (((int)(tempHigh) << 8) | tempLow);
					TemperatureC = temptemp * 0.0625f;
					break;
				case (byte)Model.DS2438:
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.StartTemperatureConversion);
					while (ow.ReadByte() == 0) { }
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.RecallEEPROM);
					ow.WriteByte((byte)Command.ScratchPadPage0);
					ow.Reset();
					ow.WriteByte((byte)Command.MatchROM);
					ow.Write(ow_address, 0, ow_address.Length);
					ow.WriteByte((byte)Command.ReadScratchPad);
					ow.WriteByte((byte)Command.ScratchPadPage0);
					ow.Read(scratchpad, 0, 9);
					tempLow = scratchpad[1];
					tempHigh = scratchpad[2];
 
 
					// temperature is negative if sign bit set
					negative = ((tempHigh & 0x80) == 0) ? false : true;
 
					// construct complete temp value
					t = (short)((tempHigh << 8) + tempLow);
 
					// if negative, take two's complement to get absolute value
					if (negative)
						t = (short)-t;
 
					// get integer part if any
					result = (float)(t >> 8);
 
					// remove zero bits and add fractional part if any
					result += ((t >> 3) & 0x1f) * 0.03125f;
 
					// return value with correct sign
					TemperatureC = (negative) ? -result : result;
 
 
					break;
				default:
					break;
			}
			return TemperatureC;
		}

        private float GetHumidity(byte[] ow_address, double temp)
		{
			byte[] scratchpad = new byte[9];
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.WriteScratchPad);
			ow.WriteByte((byte)Command.ScratchPadPage0);
			ow.WriteByte((byte)0x0);
			//while (ow.ReadByte() == 0) { }
			Thread.Sleep(10);
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.StartTemperatureConversion);
			while (ow.ReadByte() == 0) { }
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.StartVoltageConversion);
			//Thread.Sleep(10);
			while (ow.ReadByte() == 0) { }
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.ReadScratchPad);
			ow.WriteByte((byte)Command.ScratchPadPage0);
			ow.Read(scratchpad, 0, 9);
 
			short t;
			float result;
 
			t = (short)((scratchpad[4] << 8) | scratchpad[3]);
 
			result = (float)(((t * 0.01f) - 0.958) / 0.0307f);
			result = (float)result / (1.0546f - (0.00216f * (float)temp));
			return result;
		}

        private float GetLightLevel(byte[] ow_address)
		{
			byte[] scratchpad = new byte[9];
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.WriteScratchPad);
			ow.WriteByte((byte)Command.ScratchPadPage0);
			ow.WriteByte((byte)0x0);
			//while (ow.ReadByte() == 0) { }
			Thread.Sleep(10);
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.StartVoltageConversion);
			//Thread.Sleep(10);
			while (ow.ReadByte() == 0) { }
 
			ow.Reset();
			ow.WriteByte((byte)Command.MatchROM);
			ow.Write(ow_address, 0, ow_address.Length);
			ow.WriteByte((byte)Command.ReadScratchPad);
			ow.WriteByte((byte)Command.ScratchPadPage0);
			ow.Read(scratchpad, 0, 9);
 
			short t;
			float result;
 
			t = (short)((scratchpad[4] << 8) | scratchpad[3]);
 
			result = (float)((t / 100f) / 5.0f) * 100.0f;
			return result;
		}

    }
}
*/
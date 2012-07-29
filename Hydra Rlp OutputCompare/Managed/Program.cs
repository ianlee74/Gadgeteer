using System;
using System.Threading;
using GHIElectronics.OSH.NETMF.Hardware.LowLevel;
using GHIElectronics.OSH.NETMF.Native;
using HydraOutputCompare;
using Microsoft.SPOT;

namespace SignalGen
{
    public class Program
    {
        static RLPLite.Procedure mf_SignalGen;

        public static void Main()
        {
            LoadELF();

            FEZ_Pin.Digital[] e_Pins = new FEZ_Pin.Digital[] { FEZ_Pin.Digital.Di4,  // Bit 0 = u8_Data[x] & 1
                                                               FEZ_Pin.Digital.Di5,  // Bit 1 = u8_Data[x] & 2
                                                               FEZ_Pin.Digital.Di6,  // Bit 2 = u8_Data[x] & 4
                                                               FEZ_Pin.Digital.LED}; // Bit 3 = u8_Data[x] & 8
            ResetSignalGenerator(e_Pins, 0x00);

            // Outputs with 250 µs clock:  (use oscilloscope to check!)
            // Pin 4: ...___X_X_X_X_X_X_X_X_X_X_X_X_X_X_X_X_X_X_X_X.... Pause 100 ms 
            // Pin 5: ...____XX__XX__XX__XX__XX__XX__XX__XX__XX__XX.... Pause 100 ms
            // Pin 6: ...______XXXX____XXXX____XXXX____XXXX____XXXX.... Pause 100 ms
            // LED  : ...___XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX.... Pause 100 ms
            // The LED is on all the time while data is written

            Byte[] u8_Data = new Byte[4096];
            for (int i=0; i<u8_Data.Length; i++)
            {
                u8_Data[i] = (Byte)((i%8) | 8);
            }

            while (true)
            {
                SignalGenerator(250, e_Pins, u8_Data);

                ResetSignalGenerator(e_Pins, 0x00);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Load the native code from the resources into the RAM
        /// </summary>
        protected static void LoadELF()
        {
            // Get ELF file into byte array from resources
            byte[] u8_ElfFile = Resources.GetBytes(Resources.BinaryResources.SignalGen);

            // This method loads the "Load Region" into RAM
            AddressSpace.Write(0xA0000000, u8_ElfFile, 0, 0x00100000);     //  TODO: Change last parameter to reflect actual size of elf file.

            mf_SignalGen = new RLPLite.Procedure(0xA00000c0);

            // We don't need this anymore
            u8_ElfFile = null;
            Debug.GC(true);
        }

        /// <summary>
        /// Switches all pins into Output Mode and sets all pins to their initial state
        /// </summary>
        protected static void ResetSignalGenerator(FEZ_Pin.Digital[] e_Pins, Byte u8_InitState)
        {
            SignalGenerator(0, e_Pins, new Byte[1] { u8_InitState });
        }

        /// <summary>
        /// This is a type-safe wrapper for the RLP function "SignalGenerator()"
        /// SignalGenerator is a blocking function that will not return before all data has been written!
        /// u32_Interval = interval in µs
        /// e_Pins[]     = 1 up to 8 digital output pins
        /// u8_Data[]    = The bytes to be written where Bit 0 is used for e_Pins[0], Bit 1 is used for e_Pins[1], ..
        /// </summary>
        protected static void SignalGenerator(UInt32 u32_Interval, FEZ_Pin.Digital[] e_Pins, Byte[] u8_Data)
        {
            Byte[] u8_Pins = new Byte[e_Pins.Length];
            for (int i=0; i<e_Pins.Length; i++)
            {
                u8_Pins[i] = (Byte)e_Pins[i];
            }

            int b_OK = mf_SignalGen.Invoke(u32_Interval, u8_Pins, u8_Data);
            if (b_OK == 0)
                throw new Exception("Error: Invalid parameters for RLP.SignalGenerator()");

            Register reg = new Register();

        }
    }
}

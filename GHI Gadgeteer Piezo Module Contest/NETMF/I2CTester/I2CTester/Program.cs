using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace I2CTester
{
    public partial class Program
    {
        private static SoftwareI2C i2c;

        // This method is run when the mainboard is powered up or reset.   
        private void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            var socket = GT.Socket.GetSocket(3, false, extender, null);
            // Set up software I2C port - SCL on Di6 and SDA on Di7.
            i2c = new SoftwareI2C( /*SCL*/ socket.CpuPins[6], /*SDA*/ socket.CpuPins[7], 100);

            (new Thread(I2C)).Start();
        }

        public void I2C()
        {
            byte i, dataOut = 0;

            try
            {
                //
                // Fill the EEPROM page beginning at memory location 0 ...
                i2c.Transmit(true, false, (byte) 0xA0); // Send control byte, ...
                i2c.Transmit(false, false, (byte) 0x00); //  high byte of address, and ...
                i2c.Transmit(false, false, (byte) 0x00); //  low byte of address.
                Debug.Print("\r\nWriting to EEPROM ...");
                for (i = 0; i < 63; i++)
                {
                    i2c.Transmit(false, false, i); // Transmit the data ...
                    //
                    Thread.Sleep(5); //  and wait for the write operation to complete.
                }
                i2c.Transmit(false, true, i); // Follow the 64th byte with stop condition.
                Thread.Sleep(5);
                //
                // Sequential read of EEPROM page ...
                i2c.Transmit(true, false, (byte) 0xA0);
                i2c.Transmit(false, false, (byte) 0x00);
                i2c.Transmit(false, false, (byte) 0x00);
                i2c.Transmit(true, false, (byte) 0xA1);
                for (i = 0; i < 63; i++)
                {
                    dataOut = i2c.Receive(true, false);
                    if (dataOut != i)
                    {
                        Debug.Print("Error at location " + i.ToString() + ", dataOut = " + dataOut.ToString());
                        while (true) ;
                    }
                    Debug.Print("Value read from eprom location " + i.ToString() + ": " + dataOut.ToString());
                }
                dataOut = i2c.Receive(false, true);
                if (dataOut != i)
                {
                    Debug.Print("Error at location " + i.ToString() + ", dataOut = " + dataOut.ToString());
                    while (true) ;
                }
                Debug.Print("Value read from eprom location " + i.ToString() + ": " + dataOut.ToString());
                Debug.Print("All done!");
            }
            catch (Exception e)
            {
                Debug.Print("Exception - " + e.Message);
            }
            while (true) ;
        }
    }
}


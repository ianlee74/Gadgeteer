using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Interfaces;
using Gadgeteer.Modules.Ledonet;
using Microsoft.SPOT.Hardware;

namespace IO9560ModulesTest
{
    public partial class Program
    {
        IO60P16 iO60P16;
        private byte cc;
        //static InterruptPort isr;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            
            iO60P16 = new IO60P16(5);
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Thread th = new Thread(new ThreadStart(Io60p_Thread));
            th.Start();
            Debug.Print("Program Started");
        }

        void Io60p_Thread()
        {
            const byte PORT = 7;

            iO60P16.SetPortMode(PORT, PORT_DRIVEMODE.RES_PULLDOWN);
            iO60P16.SetPortDirection(PORT, 0xff);
            iO60P16.SetPortOutput(PORT, 0x00);                          // This works.
            iO60P16.SetPortOutput(PORT, 0x02);                          // This works.
            Debug.Print(iO60P16.GetPortInput(PORT).ToString());         // This should print "2".  It prints "255" instead.        
            return;

            iO60P16.EnableInterruptPort(1, 0xFF); // interrupt disable Port 1
            iO60P16.StartInterruptHandler(int_pin_Interrupt);
            iO60P16.SetPortMode(3, PORT_DRIVEMODE.RES_PULLDOWN);
            iO60P16.SetPortMode(0, PORT_DRIVEMODE.RES_PULLDOWN);
            iO60P16.SetPortMode(1, PORT_DRIVEMODE.RES_PULLDOWN);
            //iO60P16.SetPortMode(7, PORT_DRIVEMODE.RES_PULLDOWN);
            iO60P16.SetPortOutput(1, 0x00);
            iO60P16.SetPortOutput(0, 0x00);
            iO60P16.SetPortOutput(7, 0x00);
            // PWM example (port 6 and 7)
            iO60P16.SetPortDirection(1, 0xFF); // pin 0..7 input
            //iO60P16.SetPortPWM(1, 0x80); // port 1 bit 7 = pwm
            //iO60P16.WriteRegister(0x28, 0); // Select PWM 0
            //iO60P16.WriteRegister(0x29, 0x4); // 32 khz
            //iO60P16.WriteRegister(0x2A, 64); // Period
            //iO60P16.WriteRegister(0x2B, 16);  // duty cycle
            //iO60P16.SetPortOutput(1, 0x80);
            iO60P16.GetStatusPort(1); // reset status of port 1
            iO60P16.EnableInterruptPort(1, 0xFB); // interrupt enable on bit 2 
            //
            byte rd_val = 0, rd_back = 0, status_port = 0;
            while (true)
            {
                if (cc > 255)
                    cc = 0;
                iO60P16.SetPortOutput(0, (byte)(cc & 0x0f));
                //iO60P16.SetPortOutput(7, (byte)(cc & 0x0f)); // send pattern on port 2 pins
                //Thread.Sleep(2);
                rd_val = iO60P16.GetPortInput(1);
                rd_back = iO60P16.GetPortInput(0);
                Debug.Print("Port 1:" + rd_val.ToString() + " - Read back Port 0:" + rd_back.ToString());
                //Debug.Print("Port 7:" + iO60P16.GetPortInput(7).ToString());
                iO60P16.SetPortOutput(3, rd_val);
                status_port = iO60P16.GetStatusPort(1);
                //Debug.Print("Status: " + status_port.ToString() );
                cc++;
                Thread.Sleep(250);
            }
        }

        void int_pin_Interrupt(InterruptInput sender, bool value)
        {
            PulseDebugLED();
            sender.Read();
            iO60P16.GetStatusPort(1);            
        }
    }
}

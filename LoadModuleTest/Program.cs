using System;
using System.Threading;
using Gadgeteer.Interfaces;
using Microsoft.SPOT;

namespace LoadModuleTest
{
    public partial class Program
    {
        private static DigitalOutput[] relays;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            relays = new DigitalOutput[7];           
            relays[0] = load.P1;
            relays[1] = load.P2;
            relays[2] = load.P3;
            relays[3] = load.P4;
            relays[4] = load.P5;
            relays[5] = load.P6;
            relays[6] = load.P7;

            var thread = new Thread(CycleRelays);
            thread.Start();
        }

        private void CycleRelays()
        {
            while (true)
            {
                //var state = !relays[0].Read();
                var state = !load.P5.Read();
                //Debug.Print(DateTime.Now + "     " + state);
                //for (var i = 0; i > 7; i++)
                //{
                    //relays[i].Write(state);         // This doesn't do anything.
                    //Thread.Sleep(200);

                    //load.P1.Write(state);     // This works.
                    //Thread.Sleep(500);
                    //load.P2.Write(state);     // This works.
                    //Thread.Sleep(500);
                    load.P3.Write(state);     // This works.
                    Thread.Sleep(100);
                    load.P4.Write(state);     // This works.
                    Thread.Sleep(100);
                    load.P5.Write(state);     // This works.
                    Thread.Sleep(100);
                    load.P6.Write(state);     // This works.
                    Thread.Sleep(100);
                    //load.P7.Write(state);     // This works.
                    //Thread.Sleep(500);
                //}
                //load.P5.Write(state);     // This works.
            }
        }
    }
}

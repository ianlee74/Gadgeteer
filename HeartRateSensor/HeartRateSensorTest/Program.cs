using System;
using System.Threading;
using Gadgeteer;
using Gadgeteer.Interfaces;
using Microsoft.SPOT;
using NETMFx.GroveHeartRateSensor;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace HeartRateSensorTest
{
    public partial class Program
    {
        private LedStripLPD8806 _ledStrip;
        private const int NUM_LEDS = 96;

        private GroveHeartRateSensor _heartRateSensor;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
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

            GT.Socket ledSocket = GT.Socket.GetSocket(ledExtender.ExtenderSocketNumber, true, ledExtender, null);
            _ledStrip = new LedStripLPD8806(ledSocket, NUM_LEDS);

            _heartRateSensor = new GroveHeartRateSensor(GT.Socket.GetSocket(heartRateExtender.ExtenderSocketNumber, true, heartRateExtender, null), 40);
            _heartRateSensor.HeartBeat += (sender, time) => 
                               {
                                   Debug.Print(@"  --\");
                                   Debug.Print(@"     >  [BPM: " + _heartRateSensor.BeatsPerMinute + "]");
                                   Debug.Print(@"  --/");
                                   BlinkAll();
                               };

            var timer1 = new GT.Timer(250);
            timer1.Tick += timer => Debug.Print(@"  |");
            timer1.Start();
            //oledDisplay.SimpleGraphics.BackgroundColor = GT.Color.Red;
            //oledDisplay.SimpleGraphics.DisplayTextInRectangle("Now running!", 2, 2, 120, 20, GT.Color.Black, Resources.GetFont(Resources.FontResources.NinaB));
        }

        private void BlinkAll()
        {
            _ledStrip.TurnOff();
            _ledStrip.BeginUpdate();
            for (var i = 0; i < NUM_LEDS; i++)
            {
                _ledStrip[i] = 0x00200000; // red
            }
            _ledStrip.EndUpdate();
            Thread.Sleep(200);
            _ledStrip.TurnOff();
        }

        private void WrapTest()
        {
            _ledStrip.TurnOff();

            _ledStrip.BeginUpdate();
            _ledStrip[0] = 0x00010000; // a tiny bit red
            _ledStrip[1] = 0x00050000; // sorta red
            _ledStrip[2] = 0x00200000; // red
            _ledStrip[3] = 0x00050000; // sorta red
            _ledStrip[4] = 0x00010000; // a tiny bit red
            _ledStrip.EndUpdate();

            for (int i = 0; i < NUM_LEDS * 5; i++)
            {
                _ledStrip.ShiftUp(true);
                Thread.Sleep(25);
            }
        }

    }
}

using System;
using System.Collections;
using System.Threading;
using Gadgeteer;
using Gadgeteer.Interfaces;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace CustomRelayTest
{
    public partial class Program
    {
        private DigitalOutput _relay;
        private bool _relayState;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            _relay = extender.SetupDigitalOutput(Socket.Pin.Three, _relayState);

            var timer = new GT.Timer(1000);
            timer.Tick += timer1 =>
                              {
                                  _relayState = !_relayState;
                                  _relay.Write(_relayState);
                              };
            timer.Start();
        }
    }
}

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerHydraCubeUI
{
    public partial class Program
    {
        void ProgramStarted()
        {
            /******************************************************************************************
            Access modules defined in the designer by typing their name:                            
            
            e.g.  button
                  camera1

            Initialize event handlers here.
            e.g. button.ButtonPressed += new GTM.MSR.Button.ButtonEventHandler(button_ButtonPressed);             
            ***************************************************************************************** */

            // Do one-time tasks here
            Debug.Print("Program Started");
            led.SwapGreenBlueValues();
            Thread.Sleep(1000);
            led.AddBlue();
            Thread.Sleep(1000);
            led.RemoveBlue();
            led.AddGreen();
            Thread.Sleep(1000);
            led.RemoveGreen();
            led.AddRed();
            Thread.Sleep(1000);
            led.RemoveRed();
            Thread.Sleep(1000);
        }
    }
}

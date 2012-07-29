using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace RgbLedTest
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
            led1.SwapGreenBlueValues();
            led2.SwapGreenBlueValues();
            led1.TurnRed();
            led2.TurnRed();
            Thread.Sleep(1000);
            led1.TurnGreen();
            led2.TurnGreen();
            Thread.Sleep(1000);
            led1.TurnBlue();
            led2.TurnBlue();
            Thread.Sleep(1000);
            led2.TurnOff();
/*
            Thread.Sleep(1000);
            led1.AddBlue();
            Thread.Sleep(1000);
            led1.RemoveBlue();
            led1.AddGreen();
            Thread.Sleep(1000);
            led1.RemoveGreen();
            led1.AddRed();
            Thread.Sleep(1000);
            led1.RemoveRed();
            Thread.Sleep(1000);
            led1.TurnOff();
            Thread.Sleep(1000);
            led1.TurnRed();
            Thread.Sleep(1000);
            led1.TurnGreen();
            Thread.Sleep(1000);
            led1.TurnBlue();
            Thread.Sleep(1000);
 */
            led1.TurnOff();
        }
    }
}

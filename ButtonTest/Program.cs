using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace ButtonTest
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
            button.ButtonPressed += delegate { Debug.Print("Pressed."); };
        }
    }
}

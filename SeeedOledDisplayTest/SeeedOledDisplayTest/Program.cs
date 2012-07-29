using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.Seeed;

namespace SeeedOledDisplayTest
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            oledDisplay.DebugPrintEnabled = true; 
            oledDisplay.SimpleGraphics.BackgroundColor = GT.Color.Red;
            //oledDisplay.SimpleGraphics.DisplayTextInRectangle("Now running!", 2, 2, 120, 20, GT.Color.Black, Resources.GetFont(Resources.FontResources.NinaB));
        }
    }
}

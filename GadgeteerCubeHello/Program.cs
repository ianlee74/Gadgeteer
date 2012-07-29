using System;
using System.Collections;
using System.Threading;
using Gadgeteer;
using Gadgeteer.Interfaces;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerCubeHello
{
    public partial class Program
    {
        private DigitalOutput[] _layers;
        private DigitalOutput[] _leds;

        private void ProgramStarted()
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
            button.ButtonPressed += OnButtonPressed;
            
            _layers = new DigitalOutput[]
                          {
                              extender7.SetupDigitalOutput(Socket.Pin.Six, false),
                              extender7.SetupDigitalOutput(Socket.Pin.Five, false),
                              extender7.SetupDigitalOutput(Socket.Pin.Four, false)
                          };

            _leds = new DigitalOutput[]
                        {
                            extender4.SetupDigitalOutput(Socket.Pin.Eight, false),
                            extender4.SetupDigitalOutput(Socket.Pin.Seven, false),
                            extender4.SetupDigitalOutput(Socket.Pin.Six, false),
                            extender4.SetupDigitalOutput(Socket.Pin.Five, false),
                            extender4.SetupDigitalOutput(Socket.Pin.Four, false),
                            extender4.SetupDigitalOutput(Socket.Pin.Three, false),
                            extender7.SetupDigitalOutput(Socket.Pin.Nine, false),
                            extender7.SetupDigitalOutput(Socket.Pin.Eight, false),
                            extender7.SetupDigitalOutput(Socket.Pin.Seven, false),
                        };
        }

        private void OnButtonPressed(Button sender, Button.ButtonState state)
        {
            Debug.Print("Pressed.");
            for (var layer = 0; layer < _layers.Length; layer++)
            {
                _layers[layer].Write(true);
                for (var led = 0; led < _leds.Length; led++)
                {
                    _leds[led].Write(true);
                    Thread.Sleep(250);
                    _leds[led].Write(false);
                }
                _layers[layer].Write(false);
            }
        }
    }
}

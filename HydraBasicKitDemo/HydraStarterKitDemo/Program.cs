using System.Threading;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Timer = Gadgeteer.Timer;
using Gadgeteer.Modules.GHIElectronics;

namespace HydraBasicKitDemo
{
    public partial class Program
    {
        void ProgramStarted()
        {
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            // Cycle through the lights to show we're alive.
            led.Animate(100, true, true, false);
            led.TurnLightOff(6);                // a bug causes the last light to stay on...
            
            // Light meter 
            var timer1 = new GT.Timer(100);
            timer1.Tick += OnLightMeterTick;
            timer1.Start();

            // Monitor joystick to update red LED brightness.
            var timer2 = new GT.Timer(10);
            timer2.Tick += OnRedBrightnessTick;
            timer2.Start();

            joystick.JoystickPressed += OnJoystickPressed;
        }

        void OnJoystickPressed(Joystick sender, Joystick.JoystickState state)
        {
            led.Animate(2, false, true, false);
        }

        void OnRedBrightnessTick(Timer timer)
        {
            led.TurnLightOn(7);
            Thread.Sleep((int)(joystick.GetJoystickPostion().Y*100)/20);
            led.TurnLightOff(7);
        }

        void TestLed()
        {
            var socket = GT.Socket.GetSocket(10, true, null, null);
            var led = socket.CpuPins[3];
            
        }

        private void OnLightMeterTick(Timer timer)
        {
            var lightLevel = (int) lightSensor.ReadLightSensorPercentage();
            //var pos = joystick.GetJoystickPostion();
            //Debug.Print("Joystick: (" + (int) (pos.X*100) + ", " + (int) (pos.Y*100) + ")   Light:  " + lightLevel);

            for (var ndx = 1; ndx < 7; ndx++)
            {
                if (ndx <= lightLevel/100.0*7)
                {
                    led.TurnLightOn(ndx);
                }
                else
                {
                    led.TurnLightOff(ndx);   
                }                
            }
        }
    }
}

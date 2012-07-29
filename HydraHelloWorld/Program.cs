using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using HydraNative;

namespace HydraHelloWorld
{
    public class Program
    {
        public static void Main()
        {
            Debug.Print("Hello World!");
            var button = new InterruptPort((Cpu.Pin)HydraNativePins.S12P03, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);       
            var button2 = new InterruptPort((Cpu.Pin)HydraNativePins.S03P03, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);

            var p4 = new OutputPort((Cpu.Pin)HydraNativePins.S07P04, false);
            var p6 = new OutputPort((Cpu.Pin)HydraNativePins.S07P06, false);
            var p8 = new OutputPort((Cpu.Pin)HydraNativePins.S07P08, false);
            var p9 = new OutputPort((Cpu.Pin)HydraNativePins.S07P09, false);                                                                          

            var level = new OutputPort((Cpu.Pin)HydraNativePins.S07P05, true);                                                                          
            var led = new OutputPort((Cpu.Pin)HydraNativePins.S07P07, false);                                                                           
            button.OnInterrupt += OnButtonPressed;
            button2.OnInterrupt += OnButtonPressed;

            while (true)
            {
                Thread.Sleep(500);
                led.Write(!led.Read());
            }
        }

        private static void OnButtonPressed(uint data1, uint data2, DateTime time)
        {
            Debug.Print("Pressed.");
        }
    }
}

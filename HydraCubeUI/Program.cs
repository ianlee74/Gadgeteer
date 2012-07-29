using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NETMFx.LedCube;
using NETMFx.LedCube.Effects;
using HydraNative;

namespace HydraCubeUI
{
    public class Program
    {

        public static void Main()
        {
            Debug.Print(HydraNative.HydraNative.HydraPin("PD14").ToString());

            var cube = new LedCube3(3,
                // Levels
                                        new[] { (Cpu.Pin)HydraNativePins.S07P06,
                                                (Cpu.Pin)HydraNativePins.S07P05,
                                                (Cpu.Pin)HydraNativePins.S07P04
                                        },
                // LEDs
                                        new[] { (Cpu.Pin)HydraNativePins.S04P08,
                                                 (Cpu.Pin)HydraNativePins.S04P07,
                                                 (Cpu.Pin)HydraNativePins.S04P06,
                                                 (Cpu.Pin)HydraNativePins.S04P05,
                                                 (Cpu.Pin)HydraNativePins.S04P04,
                                                 (Cpu.Pin)HydraNativePins.S04P03,
                                                 (Cpu.Pin)HydraNativePins.S07P09,
                                                 (Cpu.Pin)HydraNativePins.S07P08,
                                                 (Cpu.Pin)HydraNativePins.S07P07 });
            var effects = new CubeEffect[] {new AsciiChar(cube, "HELLO WORLD"), 
                                            new TallyUp(cube, 100),
                                            new TallyDown(cube, 50),
                                            new SirenClockwise(cube, 100),
                                            new SirenCounterclockwise(cube, 100),
                                            new AsciiChar(cube, 100, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"), 
                                            new SpiralUp(cube, 100),
                                            new SpiralDown(cube, 50),
                                            new Randomizer(cube, 20)
            };
            while (true)
            {
                foreach (var effect in effects)
                {
                    effect.Start();
                }
            }
        }
    }
}

using System.Threading;

namespace NETMFx.LedCube.Effects
{
    public class SpiralDown : CubeEffect 
    {
        public SpiralDown(LedCube cube) : base(cube) {}
        public SpiralDown(LedCube cube, int duration) : base(cube, duration) {}

        public override void Start(int duration)
        {
            //if (Center) LED[4].Write(true);
            int[] seq = { 3, 6, 7, 8, 5, 2, 1, 0 };
            for (var j = Cube.Size-1; j >= 0; j--)
            {
                Cube.Levels[j].Write(true);
                for (var i = 0; i < 8; i++)
                {
                    Cube.Leds[seq[i]].Write(true);
                    Thread.Sleep(duration);
                    Cube.Leds[seq[i]].Write(false);
                }
                Cube.Levels[j].Write(false);
            }
        }
    }
}

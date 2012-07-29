using System;
using System.Threading;
using Microsoft.SPOT;

namespace NETMFx.LedCube.Effects
{
    public class SirenCounterclockwise : CubeEffect
    {
        public SirenCounterclockwise(LedCube cube) : base(cube) {}
        public SirenCounterclockwise(LedCube cube, int duration) : base(cube, duration) {}

        public override void Start(int duration)
        {
            int[] seq = { 0, 1, 2, 5, 8, 7, 6, 3 };
            Cube.Levels[0].Write(true);
            Cube.Levels[1].Write(true);
            Cube.Levels[2].Write(true);
            for (var i = 0; i < 8; i++)
            {
                Cube.Leds[seq[i]].Write(true);
                Thread.Sleep(duration);
                Cube.Leds[seq[i]].Write(false);
            }
        }
    }
}

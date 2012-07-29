using System;
using Microsoft.SPOT.Hardware;

namespace NETMFx.LedCube.Effects
{
    public class Randomizer : CubeEffect
    {
        public Randomizer(LedCube cube) : base(cube) {}
        public Randomizer(LedCube cube, int duration) : base(cube, duration) {}

        private readonly Random _rand = new Random((int)(Utility.GetMachineTime().Ticks & 0xffffffff));

        public override void Start(int duration)
        {
            while (true)
            {
                var x = (byte)(_rand.Next()%3);
                var y = (byte) (_rand.Next()%3);
                var z = (byte) (_rand.Next()%3);
                Cube.Illuminate(x, y, z, duration);
            }
        }
    }
}

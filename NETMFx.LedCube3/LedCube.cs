using System;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace NETMFx.LedCube
{
    public abstract class LedCube
    {
        public readonly byte Size;
        public readonly OutputPort[] Levels;
        public readonly OutputPort[] Leds;
        public CubeOrientations Orientation = CubeOrientations.ZPos;

        protected LedCube(byte size, Cpu.Pin[] levelPins, Cpu.Pin[] ledPins, CubeOrientations orientation = CubeOrientations.ZPos )
        {            
            Size = size;
            Orientation = orientation;
            Levels = new OutputPort[Size];
            Leds = new OutputPort[Size*Size];

            // Make sure we have the correct number of level pins.
            if(levelPins.Length != size)
            {
                throw new ArgumentOutOfRangeException("levelPins", "You must define " + Size + " level pins.");
            }

            // Make sure we have the correct number of LED pins.
            if (ledPins.Length != size * size)
            {
                throw new ArgumentOutOfRangeException("ledPins", "You must define " + Size*Size + " led pins.");
            }

            // Create level ports.
            for (byte lvl = 0; lvl < Size; lvl++ )
            {
                Levels[lvl] = new OutputPort(levelPins[lvl], false);
            }

            // Create LED ports.
            for (byte led = 0; led < Size*Size; led++)
            {
                Leds[led] = new OutputPort(ledPins[led], false);
            }
        }

        public void Illuminate(Led led, int duration)
        {
            Leds[led.Port].Write(true);
            Levels[led.Level].Write(true);
            Thread.Sleep(duration);
            Leds[led.Port].Write(false);
            Levels[led.Level].Write(false);
        }

        public void Illuminate(byte x, byte y, byte z, int duration)
        {
            var led = CoordinateToLed(x, y, z);
            Illuminate(led, duration);
        }

        private Led CoordinateToLed(byte x, byte y, byte z)
        {
            var led = new Led();
            if (Orientation == CubeOrientations.ZPos)
            {
                // "Normal" orientation, no 3D translation necessary.
                led.Port = (byte)(x + 3 * z);
                led.Level = y;
            }
            else
            {
                // TODO: Translate the coordinate.
            }
            return led;
        }


        /// <summary>
        /// Turn all the LEDs off.
        /// </summary>
        public void Off()
        {
            for (var i = 0; i < Size; i++)
            {
                Leds[i].Write(false);
            }
            for (var i = 0; i < Size*Size; i++)
            {
                Leds[i].Write(false);
            }

        }
    }

    // I decided to give orientation based upon if you were looking at the side where the 1, 2, & 3 pins are
    // and placing the origin in the center of the cube.
    public enum CubeOrientations
    {
        ZPos,       // Default front
        ZNeg,       // Back
        XPos,       // Right
        XNeg,       // Left
        YPos,       // Top
        YNeg        // Bottom
    }
}

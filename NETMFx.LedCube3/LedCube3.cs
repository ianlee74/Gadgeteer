using Microsoft.SPOT.Hardware;

namespace NETMFx.LedCube
{
    /// <summary>
    /// A 3x3x3 LED Cube driver.
    /// </summary>
    public class LedCube3 : LedCube
    {
        public LedCube3( byte size, Cpu.Pin[] levelPins, Cpu.Pin[] ledPins, CubeOrientations orientation = CubeOrientations.ZPos) 
            : base(size, levelPins, ledPins, orientation)
        {
        }
    }
}

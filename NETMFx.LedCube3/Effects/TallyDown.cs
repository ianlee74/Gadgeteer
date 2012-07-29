namespace NETMFx.LedCube.Effects
{
    public class TallyDown : CubeEffect
    {
        public TallyDown(LedCube cube) : base(cube) {}
        public TallyDown(LedCube cube, int duration) : base(cube, duration) {}

        public override void Start(int duration)
        {
            for (var level = (byte)(Cube.Levels.Length - 1); level < (Cube.Size); level--)
            {
                for (var led = (byte)(Cube.Leds.Length - 1); led < (Cube.Size*Cube.Size); led--)
                {
                    //Cube.Illuminate(new Led(led, level),duration);
                    Cube.Illuminate((byte)(led % Cube.Size), level, (byte)(led / Cube.Size), duration);
                }
            }
        }
    }
}

namespace NETMFx.LedCube.Effects
{
    public abstract class CubeEffect
    {
        public readonly LedCube Cube;
        protected int Duration = 200;

        protected CubeEffect(LedCube cube)
        {
            Cube = cube;
        }

        protected CubeEffect(LedCube cube, int duration)
        {
            Cube = cube;
            Duration = duration;
        }

        public void Start()
        {
            Start(Duration);
        }

        public abstract void Start(int duration);
    }
}

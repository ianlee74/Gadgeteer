namespace NETMFx.LedCube
{
    public class Led
    {
        public Led() { }

        public Led(byte port, byte level)
        {
            Port = port;
            Level = level;
        }

        public byte Port;
        public byte Level;
    }
}

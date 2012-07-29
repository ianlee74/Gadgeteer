// Type: Gadgeteer.Interfaces.SoftwareI2C
// Assembly: Gadgeteer.DaisyLink, Version=2.42.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Microsoft .NET Gadgeteer\Core\Assemblies\.NET Micro Framework 4.2\Gadgeteer.DaisyLink.dll

using Gadgeteer;
using Gadgeteer.Modules;

namespace Gadgeteer.Interfaces
{
    public class SoftwareI2C
    {
        #region LengthErrorBehavior enum

        public enum LengthErrorBehavior
        {
            ThrowException,
            SuppressException,
        }

        #endregion

        public SoftwareI2C(Socket socket, Socket.Pin sdaPin, Socket.Pin sclPin, Module module);
        public static bool ForceManagedSoftwareI2CImplementation { get; set; }
        public static bool ForceManagedPullUps { get; set; }
        public void WriteRead(byte address, byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead, SoftwareI2C.LengthErrorBehavior errorBehavior = SoftwareI2C.LengthErrorBehavior.ThrowException);
        public int WriteRead(byte address, byte[] writeBuffer, byte[] readBuffer, SoftwareI2C.LengthErrorBehavior errorBehavior = SoftwareI2C.LengthErrorBehavior.ThrowException);
        public int Write(byte address, byte[] writeBuffer, SoftwareI2C.LengthErrorBehavior errorBehavior = SoftwareI2C.LengthErrorBehavior.ThrowException);
        public byte ReadRegister(byte address, byte register, SoftwareI2C.LengthErrorBehavior errorBehavior = SoftwareI2C.LengthErrorBehavior.ThrowException);
        public int Read(byte address, byte[] readBuffer, SoftwareI2C.LengthErrorBehavior errorBehavior = SoftwareI2C.LengthErrorBehavior.ThrowException);
    }
}

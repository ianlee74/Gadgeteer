// Type: Microsoft.SPOT.Hardware.NativeEventDispatcher
// Assembly: Microsoft.SPOT.Hardware, Version=4.1.2821.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Microsoft .NET Micro Framework\v4.1\Assemblies\le\Microsoft.SPOT.Hardware.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.SPOT.Hardware
{
    public class NativeEventDispatcher : IDisposable
    {
        protected NativeEventHandler m_callbacks;
        protected bool m_disposed;
        protected NativeEventHandler m_threadSpawn;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public NativeEventDispatcher(string strDriverName, ulong drvData);

        #region IDisposable Members

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Dispose();

        #endregion

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual void EnableInterrupt();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual void DisableInterrupt();

        [MethodImpl(MethodImplOptions.InternalCall)]
        protected virtual void Dispose(bool disposing);

        ~NativeEventDispatcher();

        public event NativeEventHandler OnInterrupt;
    }
}

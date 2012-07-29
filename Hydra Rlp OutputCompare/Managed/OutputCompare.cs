using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HydraOutputCompare
{
    public class OutputCompare
    {
        public OutputCompare(Cpu.Pin pin, bool initialSize, int MAX_TIMINGS_BUFFER_SIZE)
        {
            
        }

        public bool IsActive;

        public void Set(bool initialValue, uint[] timingsBuffer_us, int bufferOffset, int bufferCount, bool bufferRepeating)
        {
            
        }

        public void Set(bool pinState)
        {

        }

        public void SetBlocking(bool initialValue, uint[] timingsBuffer_us, int bufferOffset, int bufferCount, uint lastBitHoldTime_us, bool disableInterrupts)
        {
            
        }
    
        public void SetBlocking(bool initialValue, uint[] timingsBuffer_us, int bufferOffset, int bufferCount, uint lastBitHoldTime_us, bool disableInterrupts, uint carrierFrequency_hz)
        {

        }

        public void Dispose()
        {
            
        }
    }
}

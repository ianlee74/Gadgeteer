#include "RLP.h"

#define BYTE unsigned char
#define UINT unsigned int

// u32_Interval = 250 -> 250 µs
//
// pu8_Pins[0] = (Byte)FEZ_Pin.Digital.Di5 -> Pin 5
// pu8_Pins[1] = (Byte)FEZ_Pin.Digital.Di6 -> Pin 6
//
// pu8_Data[0] = 0x00 -> Pin 5 = Low,  Pin 6 = Low
// pu8_Data[1] = 0x01 -> Pin 5 = High, Pin 6 = Low
// pu8_Data[2] = 0x02 -> Pin 5 = Low,  Pin 6 = High
// pu8_Data[3] = 0x03 -> Pin 5 = High, Pin 6 = High
// etc..
//int WritePins(UINT u32_Interval, BYTE pin, BYTE* pu8_Data, UINT u32_DataLen)
int RLP_SetOC(void* par0, int* par1, unsigned char* par2)
{
	// the passed arguments ///////////////
// bool initialValue, uint[] timingsBuffer_us, int bufferOffset, int bufferCount, bool bufferRepeating
	unsigned int* timingsBuffer = par0;
	unsigned int initialValue = par1[0];
	unsigned int bufferOffset = par1[1];
	unsigned int bufferCount = par1[2];
	unsigned int bufferRepeating = par1[3];
	///////////////////////////////////////

	if (bufferCount < 1)
		return RLP_FALSE;
	
	UINT P,D;
	for (P=0; P<u32_PinCount; P++)
	{
		BYTE u8_Bit = 1 << P;
		
		UINT u32_State = ((pu8_Data[0] & u8_Bit) != 0) ? RLP_TRUE : RLP_FALSE;
		
		RLPext->GPIO.EnableOutputMode(pu8_Pins[P], u32_State);
	}
	
	for (D=0; D<u32_DataLen; D++)
	{
		for (P=0; P<u32_PinCount; P++) // This loop takes 2 µs per cycle on FEZ domino
		{
			BYTE u8_Bit = 1 << P;
			
			UINT u32_State = ((pu8_Data[D] & u8_Bit) != 0) ? RLP_TRUE : RLP_FALSE;
			
			RLPext->GPIO.WritePin(pu8_Pins[P], u32_State);
		}
	
		RLPext->Delay(u32_Interval);
	}
	
	return RLP_TRUE;
}

// ==========================================================================================================
//                                          .NET INTERFACE 
// ==========================================================================================================

// Arg[0] = Interval  (UInt32)     // in µs
// Arg[1] = used Pins (Byte[1..8]) // FEZ_Pin.Digital
// Arg[2] = Data      (Byte[n])    // Bit 0 for Pins[0], Bit 1 for Pins[1] etc...

// returns 1 on success
// returns 0 if invalid parameter have been passed
int SignalGenerator(UINT *generalArray, void **args, UINT argsCount, UINT *argSize)
{
	if (argsCount != 3)
		return RLP_FALSE;
		
	if (argSize[0] != sizeof(UINT))
		return RLP_FALSE; // Invalid type for interval
		
	return WritePins(*((UINT*)args[0]), (BYTE*)args[1], argSize[1], (BYTE*)args[2], argSize[2]);
}


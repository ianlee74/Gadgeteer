// DaisyLink

#include <Arduino.h>
#include <string.h>
#include <Wire.h>
#include "DaisyLink.h"

// Default values for PIN_UPSTREAM and PIN_DOWNSTREAM pins are:
// P1[3] = PIN_UPSTREAM
// P1[5] = PIN_DOWNSTREAM
// Upstream is defined as "up" toward the mainboard; Downstream is defined as
// "down" and away from the mainboard toward the end of the DaisyLink chain.
//
// IMPORTANT: It is possible to change these, but if alternative
// pins are not in P1, it will be necessary to edit Setup_As_Input, Setup_As_Output,
// and Read() to target the correct registers.

#define PIN_UPSTREAM		5
#define PIN_DOWNSTREAM		4

#define PIN_SDA_ENABLE		2
#define PIN_SCL_ENABLE		3

#define DEFAULT_I2C_ID		127		/* Default I2C address responded to only during the SETUP state */

// Possible internal states
#define STATE_RESET          0		/* Module has just reset either due to power-on or signal from upstream */
#define STATE_SETUP          1		/* Module is waiting for its I2C address to be set.  Responds to address 127 (0x7F) */
#define STATE_STANDBY        2		/* Module is waiting to be enabled by having its pullup resistor state set.  Responds to its final I2C address */
#define STATE_ACTIVE         3		/* Module's main function is active */

// Internal node state
char State;
char InterruptPulse;				// Non-zero if this module is currently driving an interrupt pulse
char DrivingNeighborBus;			// Non-zero if the upstream neighbor bus pin is being driven by this module

void Delay_ms(char delay);

//#define Setup_As_Input(pin)         { PRT1DM0 &= ~pin; PRT1DM1 |=  pin; PRT1DM2 &= ~pin; }
//#define Setup_As_Output(pin)        { PRT1DM0 |=  pin; PRT1DM1 &= ~pin; PRT1DM2 &= ~pin; }
//#define DisableDownstreamInterrupt  ( PRT1IE = 0 )
//#define EnableDownstreamInterrupt   ( PRT1IE = PIN_DOWNSTREAM )

#ifdef LegacyNeighborBus
#define IsActive(pin)    ( PRT1DR & pin )
#define IsNotActive(pin) ( 0 == (PRT1DR & pin) )
#else
#define IsActive(pin)    ( 0 == (PRT1DR & pin) )
#define IsNotActive(pin) ( PRT1DR & pin )
#endif

#define Enable_I2C_Pullups   ( PRT0DR &= ~0x80 )
#define Disable_I2C_Pullups  ( PRT0DR |=  0x80 )

#define Refresh(I2C_ram) memcpy(I2C_ram, &DL_Reg, sizeof(struct DL_Regs))

void Setup_As_Input(int pin)
{
	pinMode(pin, INPUT);
}

void Setup_As_Output(int pin)
{
	pinMode(pin, OUTPUT);
}

void SetToTristateMode(int pin)
{
	pinMode(pin, OUTPUT);
	digitalWrite(pin, LOW);			// Ensures internal pull-up is disabled.
	pinMode(pin, INPUT);			// Set to tri-state mode.
}

void DisableDownstreamInterrupt()
{
	SetToTristateMode(PIN_DOWNSTREAM);
}

void EnableDownstreamInterrupt()
{
	Setup_As_Output(PIN_DOWNSTREAM);
	pinMode(PIN_DOWNSTREAM, HIGH);
}

//==================================================================================
unsigned long ticks;	// counts in 100us up to 2^32 ticks ~= 4 billion ~= 400000s ~= 119 hours
unsigned long intStart;			// Value of ticks when this module began its interrupt pulse


// IMPORTANT: Manually added _VC3_ISR entry to boot.tpl file
// This interrupt gets called every 100uS if SysClk is 24Mhz
// and VC3 Source is VC1 and VC1 Divider is 16 and VC3 Divider is 150
// It is only accurate to +/- 2.5% when using internal oscillator
// 
//    org   18h                      ;VC3 Interrupt Vector
//    ljmp _VC3_ISR
//    reti
#pragma interrupt_handler VC3_ISR
void VC3_ISR(void)
{	
	ticks++;
}

// IMPORTANT: Manually added _PRT1_ISR entry to boot.tpl file
// This interrupt gets called whenever the state of the downstream
// neighbor bus changes from its last read value.  This interrupt
// should only be enabled while DaisyLink is in the Active state.
// This interrupt does little except transfer the downstream neighbor
// bus state to the upstream neighbor bus pin.
// 
//    org   1Ch                      ;GPIO Interrupt Vector
//    ljmp _PRT1_ISR
//    reti
#pragma interrupt_handler PRT1_ISR
void PRT1_ISR(void)
{
	if( IsActive(PIN_DOWNSTREAM) )
	{
		DrivingNeighborBus = 1;
		Setup_As_Output(PIN_UPSTREAM);
	}
	else if( !InterruptPulse )		// Only set the upstream pin inactive if we are not generating an interrupt ourselves
	{
		Setup_As_Input(PIN_UPSTREAM);
		DrivingNeighborBus = 0;
	}
}

// The tick count is used by both DaisyLink and the standard function
// so it should never be written to except on RESET.
unsigned long GetTicks(void)
{
	return ticks;
}

// This structure is a mirror of the starting 8 bytes of the I2C mapped RAM.
// We only use 6 bytes for now but we reserve the rest of the space.
// Although the I2C mapped memory can be changed via I2C, this information
// remains unchanged and is used to keep the I2C memory mapped info intact.
struct DL_Regs {
	BYTE ID;           // 0 This node's DaisyLink ID
	BYTE Config;       // 1 Configuration and status bits
	BYTE DLVersion;    // 2 Version number of DaisyLink protocol
	BYTE ModuleType;   // 3 Type of DaisyLink node    
	BYTE ModuleVersion;// 4 Version number of the module
	BYTE Manufacturer; // 5 Manufacturer of the module
	//BYTE Reserved4;  // 6 Reserved for future use		
	//BYTE Reserved5;  // 7 Reserved for future use		
} DL_Reg;

// These are the bits currently defined in the Config register of the 
// DL_Regs structure above
#define DL_REGS_CONFIG_PULLUPS	    0x01	/* Set high if I2C pullups are active for this module */
#define DL_REGS_CONFIG_DISABLE      0x02	/* Must be cleared in the STANDBY state for normal module operation */
#define DL_REGS_CONFIG_INTERRUPT	0x80	/* Bit is high if this module is in an interrupt state - must be cleared by writing 0 to Config register */


void ResetToInitState(void)
{
	State              = STATE_RESET;
	InterruptPulse     = 0;
	DrivingNeighborBus = 0;
	ticks              = 0;

	DisableDownstreamInterrupt;				// Disable all GPIO interrupts on port 1
	Setup_As_Input(PIN_UPSTREAM);			
	Setup_As_Output(PIN_DOWNSTREAM);		// Reset all downstream modules

	PRT1DR = 0;										// Neighbor bus drives low

	Disable_I2C_Pullups;
	
	DL_Reg.ID     = 0;
	DL_Reg.Config = DL_REGS_CONFIG_DISABLE;

	EzI2Cs_SetAddr(0);
}


void InitDaisyLink(BYTE *I2CRAM, BYTE manufacturer, BYTE type, BYTE moduleVersion, int ramsize)
{

	// DaisyLink
	DL_Reg.ModuleType    = type;
	DL_Reg.ModuleVersion = moduleVersion;
	DL_Reg.DLVersion     = 4;
	DL_Reg.Manufacturer  = manufacturer;
		
	// Initialize I2C module
	EzI2Cs_SetRamBuffer(ramsize, ramsize,(BYTE *) I2CRAM);
	EzI2Cs_Start();
	
	// enable interrupts
	PRT1IC0 = PIN_DOWNSTREAM;
	PRT1IC1 = PIN_DOWNSTREAM;						// Set downstream pin interrupt type to "change from last read"
	M8C_EnableIntMask( INT_MSK0, INT_MSK0_GPIO );	// Allow GPIO interrupts (but leave interrupt for actual pin disabled until Active state)
	M8C_EnableIntMask( INT_MSK0, INT_MSK0_VC3);		// Enable timer interrupts
	M8C_EnableGInt;									// Enable global interrupts
		
	ResetToInitState();
}


// Returns 1 if DaisyLink initialization is complete and Module
// function should be active.  Returns 0 if Module function should
// ignore I2C activity.  For some reason this compiler does not
// allow a function to return a bool.
char DaisyLink(BYTE *I2CRAM)
{
	struct DL_Regs *vDL_Regs = (struct DL_Regs *)I2CRAM;		// The copy of DL_Reg "seen" by the EzI2C
	char active = 0;

	switch( State )
	{
	
	// In this state, a reset condition exists.  The I2C should be unresponsive
	// and the DaisyLink state should wait until the upstream line goes high
	// before proceeding to the SETUP state.
	case STATE_RESET:
		if( IsNotActive(PIN_UPSTREAM) )				// If the mainboard has released the RESET condition
		{
			State = STATE_SETUP;					// Move to the Setup state
			DL_Reg.ID = DEFAULT_I2C_ID;
			Refresh(I2CRAM);						// Update I2C registers to show default I2C ID
			Enable_I2C_Pullups;
			EzI2Cs_SetAddr(DEFAULT_I2C_ID);			// Make this module respond to the I2C default ID
		}
		break;
		
	// In this state, the Module will respond to DaisyLink registers at the I2C default address.
	// This state is maintained until either the upstream line goes low again, or the I2C
	// address is changed.
	case STATE_SETUP:
		if( IsActive(PIN_UPSTREAM) )				// If the module is RESET again
		{
			ResetToInitState();
			Refresh(I2CRAM);
		}
		else if( DEFAULT_I2C_ID != vDL_Regs->ID )	// If the I2C ID has been set, move to the next state
		{
			DL_Reg.ID = vDL_Regs->ID;				// Fetch the newly set I2C ID
			State = STATE_STANDBY;					// Move to the Standby state
			EzI2Cs_SetAddr(DL_Reg.ID);				// Make this module respond to the newly set I2C ID
			Setup_As_Input(PIN_DOWNSTREAM);			// Allow the downstream module to leave the RESET state
		}
		break;
		
	// In this state, the module will respond to DaisyLink registers at the assigned I2C address.
	// This state is maintained until either the upstream line goes low again, or the DL_REGS_CONFIG_DISABLE
	// bit is set low by the mainboard (and with it, the state of the DL_REGS_CONFIG_PULLUPS bit).
	case STATE_STANDBY:
		if( IsActive(PIN_UPSTREAM) )				// If the module is RESET again
		{
			ResetToInitState();
			Refresh(I2CRAM);
		}
		else if( 0 == (vDL_Regs->Config & DL_REGS_CONFIG_DISABLE))			// If the mainboard has finished setting up this module
		{
			DL_Reg.Config = (vDL_Regs->Config & DL_REGS_CONFIG_PULLUPS);	// Keep the new resistor setting
			if( 0 == (DL_Reg.Config & DL_REGS_CONFIG_PULLUPS) )				// If pullups are disabled (this is not the last module in the chain)
			{
				Disable_I2C_Pullups;
			}
			Refresh(I2CRAM);
			active = 1;								// Enable the functionality of this module
			State = STATE_ACTIVE;					// Move to the Active state
		}
		break;
		
	// In this state, the module will respond to DaisyLink Registers and all module function registers
	// at the assigned I2C address.  It will remain in this state until interrupted by the module's
	// main function, a module upstream, or the mainboard (a reset).
	case STATE_ACTIVE:
		// If we are in an interrupt state
		if( DL_Reg.Config & DL_REGS_CONFIG_INTERRUPT )
		{
			// If the config register has been cleared
			if( !(vDL_Regs->Config & DL_REGS_CONFIG_INTERRUPT) )
			{
				DL_Reg.Config &= ~DL_REGS_CONFIG_INTERRUPT;		// Clear this module's interrupt state
				vDL_Regs->Config = DL_Reg.Config;
				InterruptPulse = 0;								// Indicate that we can stop pulling on the upstream neighbor bus
			}
			vDL_Regs->ID = DL_Reg.ID;							// Refresh the ID register (read from I2C bus)
			// Refresh all DaisyLink registers except for the ID and Config registers
			memcpy(&(vDL_Regs->DLVersion), &(DL_Reg.DLVersion), sizeof(struct DL_Regs)-2);
		}
		else
		{
			Refresh(I2CRAM);				// Refresh the DaisyLink I2C registers
		}
		// If this module's interrupt pulse has reached its maximum length
		if( InterruptPulse && (((WORD)ticks - intStart) > 10) )
		{
			InterruptPulse = 0;				// Indicate that we can stop pulling on the upstream neighbor bus
		}
		// NOTE: Only sample the downstream neighbor bus once while interrupts are off and act on what you find
		//       before restoring interrupts. The interrupt routine will then handle any changes that might have
		//       occurred while interrupts were off once interrupts are enabled again.
		DisableDownstreamInterrupt;			// Disable interrupts from the downstream neighbor bus
		if( IsActive(PIN_DOWNSTREAM) )
		{
			// If we should be driving the upstream bus, but aren't
			if( !DrivingNeighborBus )
			{
				Setup_As_Output(PIN_UPSTREAM);
				DrivingNeighborBus = 1;				// Drive the upstream neighbor bus line
			}
		}
		else
		{
			// If we shouldn't be driving the upstream bus, but are
			if( DrivingNeighborBus && !InterruptPulse )
			{
				Setup_As_Input(PIN_UPSTREAM);
				DrivingNeighborBus = 0;				// Release the upstream neighbor bus line
			}
		}
		// If upstream neighbor bus is being driven and it isn't by us
		if( !DrivingNeighborBus && IsActive(PIN_UPSTREAM) )
		{
			// Reset the DaisyLink
			State = STATE_RESET;
			ResetToInitState();
			Refresh(I2CRAM);
			break;
		}
		EnableDownstreamInterrupt;				// Enable the downstream neighbor bus interrupt
		active = 1;
		break;
	default:
		ResetToInitState();
		Refresh(I2CRAM);
		break;
	}

	return active;
}

void DaisyLinkInterrupt(BYTE *I2CRAM)
{
	struct DL_Regs *vDL_Regs = (struct DL_Regs *)I2CRAM;		// The copy of DL_Reg "seen" by the I2C

	DL_Reg.Config |= DL_REGS_CONFIG_INTERRUPT;		// Set the interrupt bit for this module
	Refresh(vDL_Regs);
	DisableDownstreamInterrupt;				// Disable the downstream neighbor bus interrupt
	Setup_As_Output(PIN_UPSTREAM);			// Alert the mainboard to the interrupt
	DrivingNeighborBus = 1;					// Remember that we are driving the neighbor bus now
	intStart = (WORD)ticks;					// Remember when the interrupt was started
	InterruptPulse = 1;						// Remember that the interrupt pulse was started
	EnableDownstreamInterrupt;				// Re-enable the downstream neighbor bus interrupt
}


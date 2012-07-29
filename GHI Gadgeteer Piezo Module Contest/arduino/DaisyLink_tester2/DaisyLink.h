//-------------------------------------------------
// Prototypes of the DaisyLink API.
//-------------------------------------------------

// Gets the number of 100 microsecond intervals since the last module reset
unsigned long GetTicks(void);

void InitDaisyLink(BYTE *I2CRAM, BYTE manufacturer, BYTE type, BYTE moduleVersion, int ramsize);
char DaisyLink(BYTE *I2CRAM);
void DaisyLinkInterrupt(BYTE *I2CRAM);

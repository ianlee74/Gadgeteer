/*
This class implements everything needed to make a .NET Gadgeteer DaisyLink slave.
*/

#include <Arduino.h>
#include "DaisyLink.h"

const int DL_STATE_RESET = 1;
const int DL_STATE_SETUP = 2;
const int DL_STATE_STANDBY = 3;
const int DL_STATE_ACTIVE = 4;

int _sdaActivatorPin;
int _sclActivatorPin;
int _neighborhoodBusActivatorPin;

DaisyLink::DaisyLink(int sdaActivatorPin, int sclActivatorPin, int neighborhoodBusActivatorPin)
{
  int _sdaActivatorPin = sdaActivatorPin;
  int _sclActivatorPin = sclActivatorPin;
  int _neighborhoodBusActivatorPin = neighborhoodBusActivatorPin;  
}

void DaisyLink::SetState(int state)
{
  switch(state)
  {
    case DL_STATE_RESET:
      break;
  };
}

void DaisyLink::SetToTriState(int pin)
{
  pinMode(pin, OUTPUT);
  digitalWrite( pin, LOW );  // disables the internal pull-up resistor
  pinMode( pin, INPUT );     // now we're tri-stated
}


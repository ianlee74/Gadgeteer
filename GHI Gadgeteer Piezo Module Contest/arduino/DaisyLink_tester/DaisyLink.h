/*
This class implements everything needed to make a .NET Gadgeteer DaisyLink slave.
*/

#ifndef DaisyLink_h
#define DaisyLink_h



class DaisyLink
{
  public:
    DaisyLink(int sdaActivatorPin, int sclActivatorPin, int neighborhoodBusActivatorPin);
    void SetState(int state);
    
  private:
    void SetToTriState(int pin);
};

#endif

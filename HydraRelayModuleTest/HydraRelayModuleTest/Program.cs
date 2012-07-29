using System.Threading;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace HydraRelayModuleTest
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            joystick.JoystickPressed += new Joystick.JoystickEventHandler(joystick_JoystickPressed);
        }

        void joystick_JoystickPressed(Joystick sender, Joystick.JoystickState state)
        {
            var relayState = !relays.Relay1;
            relays.Relay1 = relayState;
            Thread.Sleep(1000);
            relays.Relay2 = relayState;
            Thread.Sleep(1000);
            relays.Relay3 = relayState;
            Thread.Sleep(1000);
            relays.Relay4 = relayState;
            Thread.Sleep(1000);
        }
    }
}

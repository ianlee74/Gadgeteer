using Microsoft.SPOT;
using ThreelnDotOrg.NETMF.Hardware;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace DS18B20_Thermometer_Test
{
    public partial class Program
    {
        private DS18B20 thermometer;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
           Debug.Print("Program Started");

           GT.Socket thermoSocket = GT.Socket.GetSocket(thermoExtender.ExtenderSocketNumber, true, thermoExtender, null);
           thermometer = new DS18B20(thermoSocket.CpuPins[4]);

           var thermoTimer = new GT.Timer(1000);
           thermoTimer.Tick += timer => Debug.Print(thermometer.ReadTemperature().ToString());
           thermoTimer.Start();
        }
    }
}

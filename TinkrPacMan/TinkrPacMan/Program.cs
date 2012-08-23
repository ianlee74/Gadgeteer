using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;
using Skewworks.Tinkr.GadgeteerHelpers;
using Skewworks.Tinkr.Modals;

using Skewworks.Standards.NETMF.Applications;

namespace TinkrPacMan
{
    public partial class Program
    {

        private static Form frmMain;
        private static Appbar frmMainAppbar;
        private static bool _running;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Graphics.Initialize(TouchCollectionMode.UserHandledOrCP7);

            CP7TouchHandler CP7 = new CP7TouchHandler(display_CP7);

            // Subscribe to application events
            Graphics.Host.ApplicationLaunched += Host_ApplicationLaunched;
            Graphics.Host.ApplicationClosing += Host_ApplicationClosing;
            
            new Thread(Home).Start();
        }

        void Home()
        {
            frmMain = new Form("frmMain");
            frmMain.BackgroundImage = Resources.GetBitmap(Resources.BitmapResources.CP7Background);

            // Appbar
            frmMainAppbar = new Appbar("ab1", Fonts.Calibri9, Fonts.Calibri24);
            frmMainAppbar.AddMenuItems(new string[] { "Launch Pacman" });
            frmMainAppbar.AppMenuSelected += new OnAppMenuSelected((object sender, int id, string value) => new Thread(AppbarItemSelected).Start());
            frmMain.AddControl(frmMainAppbar);

            // Activate
            Graphics.ActiveContainer = frmMain;
        }

        void Host_ApplicationClosing(object sender, IApplication app)
        {
            if (_running)
            {
                frmMainAppbar.ClearMenuItems();
                frmMainAppbar.AddMenuItem("Launch Pacman");
                _running = false;
            }

            Graphics.ActiveContainer = frmMain;
        }

        void Host_ApplicationLaunched(object sender, IApplication app)
        {
            app.SendMessage(frmMainAppbar, "Appbar");
            app.SendMessage(joystick, "joystick");
            app.SendMessage(frmMain.BackgroundImage, "background");
        }

        void AppbarItemSelected()
        {
            Waiter w;
            int i = frmMainAppbar.SelectedIndex;

            frmMainAppbar.RemoveMenuItemAt(i);
            if (_running)
            {
                _running = !_running;
                w = new Waiter("Terminating Pacman", Fonts.Calibri14);
                w.Start();
                Graphics.Host.TerminateApplication(Graphics.Host.RunningApplications[0]);
                w.Stop();
                frmMainAppbar.AddMenuItem("Launch Pacman");
            }
            else
            {
                if (!sdCard.IsCardMounted)
                {
                    if (!sdCard.IsCardInserted)
                    {
                        Prompt.Show("Insert SD", "Cannot run without SD card and files", Fonts.Calibri18Bold, Fonts.Calibri14, PromptType.OK);
                        return;
                    }

                    sdCard.MountSDCard();
                }

                _running = !_running;
                w = new Waiter("Launching Pacman", Fonts.Calibri14);
                w.Start();
                Graphics.Host.LaunchApplication("\\SD\\Pacman\\TinkrPacManApp.pe");
                w.Stop();
                frmMainAppbar.AddMenuItem("Terminate Pacman");
            }
        }

    }
}

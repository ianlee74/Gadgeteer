using System;
using System.Threading;

using Microsoft.SPOT;

using dotnetwarrior.NetMF.Diagnostics;
using Pacman;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;
using Skewworks.Tinkr.Modals;

using Skewworks.Standards.NETMF.Applications;
using Skewworks.Standards.NETMF.Graphics;

namespace TinkrPacManApp
{
    [Serializable]
    public class PacMan : MarshalByRefObject, IApplication 
    {

        PacmanGame game;
        GhiJoystickInputProvider joystick;

        public ProductDetails ProductDetails
        {
            get { return new ProductDetails("Pacman", "Chris Taylor's Pacman", "Skewworks", "2012 Chris Taylor", "1.2"); }
        }

        public void Main(string ApplicationPath, string[] Args)
        {
            Form frmPacman = new Form("frmPacman");
            frmPacman.ButtonPressed += frmPacman_ButtonPressed;

            Bitmap bmpGame;
            Picturebox pbGame;

            bmpGame = new Bitmap(320, 240);

            if (Prompt.Show("Resolution Adjust", "Would you like to play at 640x480?", Fonts.Calibri18Bold, Fonts.Calibri14, PromptType.YesNo) == PromptResult.Yes)
            {
                pbGame = new Picturebox("pbGame", bmpGame, frmPacman.Width / 2 - 320, frmPacman.Height / 2 - 240, 640, 480, BorderStyle.BorderNone);
                pbGame.ScaleMode = ScaleMode.Stretch;
            }
            else
                pbGame = new Picturebox("pbGame", bmpGame, frmPacman.Width / 2 - 160, frmPacman.Height / 2 - 140, BorderStyle.BorderNone);

            pbGame.Background = Colors.Black;
            game = new PacmanGame(bmpGame, pbGame);
            frmPacman.AddControl(pbGame);

            Graphics.ActiveContainer = frmPacman;

            Thread.Sleep(100);
            if (joystick != null)
            {
                game.InputManager.AddInputProvider(joystick);
                game.Initialize();
            }

        }

        public string SendMessage(object sender, string message, object args = null)
        {
            if (message == "joystick")
            {
                joystick = new GhiJoystickInputProvider((Gadgeteer.Modules.GHIElectronics.Joystick)sender);
                if (game != null)
                {
                    game.InputManager.AddInputProvider(joystick);
                    game.Initialize();
                }
                return "Using";
            }

            return string.Empty;
        }

        public Image32 SizedApplicationIcon(IconSize RequestedSize)
        {
            return null;
        }

        public void Terminate()
        {
            game.Terminate();
        }

        private void frmPacman_ButtonPressed(object sender, int ButtonID)
        {
            Debug.Print("Button: " + ButtonID + " v " + (int)Graphics.ButtonIDs.Home);
            if ((Skewworks.Tinkr.Graphics.ButtonIDs)ButtonID == Graphics.ButtonIDs.Home)
                Graphics.Host.TerminateApplication(this);
        }

    }
}

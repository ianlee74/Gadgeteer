using System;
using System.Threading;
using Gadgeteer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using ThreelnDotOrg.NETMF.Hardware;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Timer = System.Threading.Timer;

namespace DigitalLedStrip
{
	/// <summary>
	/// test program for LedStripLPD8806 LED Strip
	/// </summary>
	public partial class Program
	{
		LedStripLPD8806 mLedStrip;
		private Random mRandom = new Random();
		private const int NUM_LEDS = 96;
		private int mFadeTestNumber;
		private Timer mTimer;

		void ProgramStarted()
		{
			GT.Socket ledSocket = GT.Socket.GetSocket(ledExtender.ExtenderSocketNumber, true, ledExtender, null);            
			mLedStrip = new LedStripLPD8806(ledSocket, NUM_LEDS);

		    var ledTimer = new GT.Timer(20000);
		    ledTimer.Tick += timer => FlashTest();
            ledTimer.Start();
		}

		private int RandomColor()
		{
			int hi = 31;
			int lo = 15;

			// (todo: yoiks.. this is probably reeeally slow and definitely ugly)
			int r = mRandom.Next(3) == 2 ? hi : mRandom.Next(2) == 1 ? lo : 0;
			int g = mRandom.Next(3) == 2 ? hi : mRandom.Next(2) == 1 ? lo : 0;
			int b = mRandom.Next(3) == 2 ? hi : mRandom.Next(2) == 1 ? lo : 0;

			return r << 16 | g << 8 | b << 0;
		}

		private void FlashTest()
		{
			for (int test = 0; test < 3; test++)
			{
				int r = test == 0 ? 32 : 0;
				int g = test == 1 ? 32 : 0;
				int b = test == 2 ? 32 : 0;

				mLedStrip.Set(r, g, b);

				Thread.Sleep(250);
			}

			mLedStrip.TurnOff();
			ScalesTest();
		}

		private void ScalesTest()
		{
			for (int test = 0; test < 3; test++)
			{
				int r = test == 0 ? 32 : 0;
				int g = test == 1 ? 32 : 0;
				int b = test == 2 ? 32 : 0;

				for (int i = 0; i < NUM_LEDS; i++)
				{
					mLedStrip.Set(i, r, g, b);
					Thread.Sleep(5);
				}

				for (int i = NUM_LEDS - 1; i >= 0; i--)
				{
					mLedStrip.TurnOff(i);
					Thread.Sleep(10);
				}
			}

			WrapTest();
		}

		private void WrapTest()
		{
			mLedStrip.TurnOff();

			mLedStrip.BeginUpdate();
			mLedStrip[0] = 0x00010000; // a tiny bit red
			mLedStrip[1] = 0x00050000; // sorta red
			mLedStrip[2] = 0x00200000; // red
			mLedStrip[3] = 0x00050000; // sorta red
			mLedStrip[4] = 0x00010000; // a tiny bit red
			mLedStrip.EndUpdate();

			for (int i = 0; i < NUM_LEDS * 5; i++)
			{
				mLedStrip.ShiftUp(true);
				Thread.Sleep(25);
			}

			CylonTest();
		}

		private void CylonTest()
		{
			mLedStrip.TurnOff();

			mLedStrip.BeginUpdate();
			mLedStrip[0] = 0x00010000; // a tiny bit red
			mLedStrip[1] = 0x00080000; // sorta red
			mLedStrip[2] = 0x00200000; // red
			mLedStrip[3] = 0x00080000; // sorta red
			mLedStrip[4] = 0x00010000; // a tiny bit red
			mLedStrip.EndUpdate();

			for (int i = 0; i < 3; i++)
			{
				// 32 leds, 5 in use.. shift up 32-5 times then shift down 32-5 times
				for (int j = 0; j < 27; j++)
				{
					mLedStrip.ShiftUp(false);
					Thread.Sleep(25);
				}

				for (int j = 0; j < 27; j++)
				{
					mLedStrip.ShiftDown(false);
					Thread.Sleep(25);
				}
			}

			RandomTest();
		}

		private void RandomTest()
		{
			for (int i = 0; i < 100; i++)
			{
				mLedStrip.BeginUpdate();

				for (int led = 0; led < NUM_LEDS; led++)
				{
					mLedStrip[led] = RandomColor();
				}

				mLedStrip.EndUpdate();

				Thread.Sleep(10);
			}

			StartFadeTest();
		}

		private void StartFadeTest()
		{
			mFadeTestNumber = 0;
			mLedStrip.OnFadeComplete += NextFadeTest;
			NextFadeTest(null);
		}

		private void NextFadeTest(object ignored)
		{
			if (mFadeTestNumber < 3)
			{
				// first few times through, fade to solid r, g or b
				int r = mFadeTestNumber == 0 ? 63 : 0;
				int g = mFadeTestNumber == 1 ? 63 : 0;
				int b = mFadeTestNumber == 2 ? 63 : 0;

				mLedStrip.InitFade(2500);
				mLedStrip.FadeTo(r, g, b);
				mLedStrip.StartFade();
			}
			else if (mFadeTestNumber < 20)
			{
				// fade to random colors; one second fade with 40 steps (25ms) each
				mLedStrip.InitFade(1000, 40);
				for (int i = 0; i < NUM_LEDS; i++)
				{
					mLedStrip.FadeTo(i, RandomColor());
				}

				mLedStrip.StartFade();
			}
			else
			{
				// all done - turn the strip off
				mLedStrip.TurnOff();
			}

			mFadeTestNumber++;
		}
	}
}
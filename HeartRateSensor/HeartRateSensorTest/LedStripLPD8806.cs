// LedStripLPD8806 by Dave Durant is licensed under the (CC BY-NC-SA 3.0) Attribution -
// Share Alike - Creative Commons license. Exception to non-commercial clause: allowed
// for use as an online code example.
// 
// See http://creativecommons.org/licenses/by-nc-sa/3.0/ for license details
// 

// uncomment the next line if you're not using the fade stuff and want
// to trim a few bytes off your code space
//#define NO_FADE

using System.Threading;
using Gadgeteer.Interfaces;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GT = Gadgeteer;

/// <summary>
/// class to manage controlling a LPD8806 "digital LED strip" based somewhat on the
/// code sample at http://www.ladyada.net/products/digitalrgbledstrip/index.html.
/// 
/// Note that the LPD8806 does the 3 color bytes for a LED as Green/Red/Blue and 
/// supports 7-bits (0-127) for each color. For methods that take an int for all
/// 3 colors, you should pass 00RRGGBB, not 00GGRRBB; the driver will unpack values
/// in the right order.
/// 
/// Fading is supported - you tell the code which colors you want to fade to, the
/// time it should take (in ms) and how many steps in the fade. See method comments
/// for details and NBs.
/// 
/// There's no locking in here - if you're playing with multiple threads, you
/// might want to rework parts of this code to be safer. There's also some cleanup
/// that could be done, if you really want to save a little CPU at the cost of a
/// little bit of flash
/// </summary>
public class LedStripLPD8806
{
	/// <summary>this LPD8806 wants every byte to have 0x80 on, for some reason</summary>
	private const byte MASK = 0x80;

	/// <summary>AND'ed in to get the real color component out</summary>
	private const byte UNMASK = 0x7F;

	/// <summary>number of LEDs in our strip</summary>
	private readonly int mNumLeds;

	/// <summary>number of bytes used for the strip (mNumLeds * 3)</summary>
	private readonly int mLedByteCount;

	/// <summary>LED data plus unlatch bytes</summary>
	private byte[] mData;

	/// <summary>true if the user doesn't want to push data to the strip yet</summary>
	private bool mUpdating;

	/// <summary>the SPI object for the strip itself</summary>
	private SPI mLedStrip;

	/// <summary>
	/// c'tor
	/// </summary>
	/// <param name="socket">the Gadgeteer socket that the strip is on</param>
	/// <param name="numLeds">the number of LEDs in the strip</param>
	public LedStripLPD8806(GT.Socket socket, int numLeds)
	{
	    var spiConfig = new SPI.Configuration(Cpu.Pin.GPIO_NONE,
	                                                        false, // chip select active state
	                                                        0, // chip select setup time
	                                                        0, // chip select hold time
	                                                        false, // clock idle state
	                                                        true, // clock edge (true = rising)
	                                                        2000,   // 2mhz
                                                            SPI.SPI_module.SPI1
                                                            );

		// the protocol seems to be that we need to write 1 + (1 per 64 LEDs) bytes
		// at the end of each update (I've only tested this on a 32-LED strip)
		int latchBytes = ((numLeds + 63) / 64) * 3;
		mLedByteCount = numLeds * 3;

		mData = new byte[mLedByteCount + latchBytes];
		mNumLeds = numLeds;
//        mLedStrip = new SPI(socket, spiConfig, SPI.Sharing.Exclusive, null);
        mLedStrip = new SPI(spiConfig);

		// start with all the LEDs off
		for (int i = 0; i < mLedByteCount; i++)
		{
			mData[i] = MASK;
		}

		// give the strip an inital poke of the latch bytes (no idea
		// why this is needed)
		mLedStrip.Write(new byte[latchBytes]);

		// push the initial values (all off) to the strip
		SendUpdate();
	}

	/// <summary>
	/// pushes the colors to the strip, if we're not doing an update
	/// </summary>
	public void SendUpdate()
	{
		if (!mUpdating)
			mLedStrip.Write(mData);
	}

	/// <summary>
	/// Pauses pushing colors to the strip, which is useful if you want to
	/// change more than one LED at a time. Call EndUpdate to resume pushing
	/// to the strip
	/// </summary>
	public void BeginUpdate()
	{
		mUpdating = true;
	}

	/// <summary>
	/// Re-enables pushing colors to the strip then pushes the current color
	/// set out
	/// </summary>
	public void EndUpdate()
	{
		mUpdating = false;
		SendUpdate();
	}

	/// <summary>
	/// shifts all LED values up the strip, away from LED 0
	/// </summary>
	/// <param name="wrap">true to copy the last LED in the strip back into
	/// the first LED in the strip; false to set LED 0 to off</param>
	public void ShiftUp(bool wrap)
	{
		byte lastG = wrap ? mData[mLedByteCount - 3] : MASK;
		byte lastR = wrap ? mData[mLedByteCount - 2] : MASK;
		byte lastB = wrap ? mData[mLedByteCount - 1] : MASK;

		for (int i = mLedByteCount - 1; i > 2; i--)
		{
			mData[i] = mData[i - 3];
		}

		mData[0] = lastG;
		mData[1] = lastR;
		mData[2] = lastB;

		SendUpdate();
	}

	/// <summary>
	/// shifts all LED values down the strip, towards LED 0
	/// </summary>
	/// <param name="wrap">true to copy the first LED in the strip back
	/// into the last LED in the strip; false to set the last LED to off</param>
	public void ShiftDown(bool wrap)
	{
		byte lastG = wrap ? mData[0] : MASK;
		byte lastR = wrap ? mData[1] : MASK;
		byte lastB = wrap ? mData[2] : MASK;

		for (int i = 0; i < mLedByteCount - 3; i++)
		{
			mData[i] = mData[i + 3];
		}

		mData[mLedByteCount - 0] = lastG;
		mData[mLedByteCount - 1] = lastR;
		mData[mLedByteCount - 2] = lastB;

		SendUpdate();
	}

	/// <summary>
	/// turns off a particular LED
	/// </summary>
	/// <param name="index">which LED (0-based) to turn off</param>
	public void TurnOff(int index)
	{
		int i = index * 3;

		mData[i + 0] = MASK;
		mData[i + 1] = MASK;
		mData[i + 2] = MASK;

		SendUpdate();
	}

	/// <summary>
	/// Turns off the entire LED strip
	/// </summary>
	public void TurnOff()
	{
		for (int i = 0; i < mLedByteCount; i += 3)
		{
			mData[i + 0] = MASK;
			mData[i + 1] = MASK;
			mData[i + 2] = MASK;
		}

		SendUpdate();
	}

	/// <summary>
	/// gets or sets a particular LED RGB color value
	/// </summary>
	/// <param name="index">the LED in question</param>
	/// <returns>the RGB (not BRG) colors in the lower 24 bits</returns>
	public int this[int index]
	{
		get
		{
			int i = index * 3;

			int g = (mData[i + 0] & UNMASK);
			int r = (mData[i + 1] & UNMASK);
			int b = (mData[i + 2] & UNMASK);

			// NB: returning RGB, not BRG
			int color = r << 16 | g << 8 | b << 0;

			return color;
		}

		set
		{
			Set(index, value >> 16, value >> 8, value);
		}
	}

	/// <summary>
	/// sets a particular LED to a color (0-127 for each color)
	/// </summary>
	/// <param name="index">the LED to set</param>
	/// <param name="red">the red value for the LED</param>
	/// <param name="green">the green value for the LED</param>
	/// <param name="blue">the blue value for the LED</param>
	public void Set(int index, int red, int green, int blue)
	{
		int i = index * 3;

		mData[i + 0] = (byte)(MASK | green);
		mData[i + 1] = (byte)(MASK | red);
		mData[i + 2] = (byte)(MASK | blue);

		SendUpdate();
	}

	/// <summary>
	/// sets all LEDs in the strip to a particular color
	/// </summary>
	/// <param name="red">the red value for the LED</param>
	/// <param name="green">the green value for the LED</param>
	/// <param name="blue">the blue value for the LED</param>
	public void Set(int red, int green, int blue)
	{
		for (int i = 0; i < mLedByteCount; i += 3)
		{
			mData[i + 0] = (byte)(MASK | green);
			mData[i + 1] = (byte)(MASK | red);
			mData[i + 2] = (byte)(MASK | blue);
		}

		SendUpdate();
	}


#if !NO_FADE
	#region fade-related stuff

	/// <summary>timer that fires once per fade step</summary>
	private Timer mFadeTimer;

	/// <summary>colors that the caller asked for</summary>
	private byte[] mFadeTo;

	/// <summary>amount of change for each LED on each step</summary>
	private double[] mFadeChange;

	/// <summary>number of steps in the fade sequence</summary>
	private int mFadeNumSteps;

	/// <summary>which step of the sequence we're currently on</summary>
	private int mFadeStep;

	/// <summary>how long the fade should last, in ms</summary>
	private int mFadeDuration;

	/// <summary>
	/// callback for when a fade is complete
	/// </summary>
	public event TimerCallback OnFadeComplete;


	/// <summary>
	/// initializes a new fade sequence with 50ms steps
	/// </summary>
	/// <param name="durationMs">how long the fade should take, in ms</param>
	public void InitFade(int durationMs)
	{
		InitFade(durationMs, durationMs / 50);
	}

	/// <summary>
	/// initializes a new fade sequence
	/// </summary>
	/// <param name="durationMs">how long the fade should take, in ms</param>
	/// <param name="steps">how many steps to do through the fade. I found that
	/// making this and duration work out >= 20ms works best</param>
	public void InitFade(int durationMs, int steps)
	{
		Debug.Assert(steps < durationMs, "InitFade: number of steps must be less than duration");

		if (mFadeTo == null)
		{
			mFadeTo = new byte[mLedByteCount];
			mFadeChange = new double[mLedByteCount];
		}

		// start with the colors we already have so LEDs don't change if the
		// caller doesn't do a FadeTo on them
		for (int i = 0; i < mLedByteCount; i++)
		{
			mFadeTo[i] = (byte)(mData[i] & UNMASK);
			mFadeChange[i] = 0.0;
		}

		mFadeStep = 0;
		mFadeDuration = durationMs;
		mFadeNumSteps = steps;
	}

	/// <summary>
	/// starts the fade sequence going
	/// </summary>
	public void StartFade()
	{
		// todo: this isn't exactly right - should take into account the time it
		// takes to push data to the strip and subtract that from the interval time
		int tick = mFadeDuration / mFadeNumSteps;

		mFadeTimer = new Timer(FadeOnTick, null, 1, tick);
	}

	/// <summary>
	/// specifies the color (00RRGGBB) to fade to for a partiular LED
	/// </summary>
	/// <param name="index">which LED we're talking about</param>
	/// <param name="color">the color to fade to</param>
	public void FadeTo(int index, int color)
	{
		FadeTo(index, color >> 16, color >> 8, color >> 0);
	}

	/// <summary>
	/// specifies the color to fade to for a particular LED
	/// </summary>
	/// <param name="index">which LED to fade</param>
	/// <param name="r">the red component of the color to fade to</param>
	/// <param name="g">the green component of the color to fade to</param>
	/// <param name="b">the blue component of the color to fade to</param>
	public void FadeTo(int index, int r, int g, int b)
	{
		int i = index * 3;

		mFadeTo[i + 0] = (byte)g;
		mFadeTo[i + 1] = (byte)r;
		mFadeTo[i + 2] = (byte)b;

		mFadeChange[i + 0] = (double)(mFadeTo[i + 0] - (mData[i + 0] & UNMASK)) / mFadeNumSteps;
		mFadeChange[i + 1] = (double)(mFadeTo[i + 1] - (mData[i + 1] & UNMASK)) / mFadeNumSteps;
		mFadeChange[i + 2] = (double)(mFadeTo[i + 2] - (mData[i + 2] & UNMASK)) / mFadeNumSteps;
	}

	/// <summary>
	/// fades the entire LED strip to a particular color
	/// </summary>
	/// <param name="r">the red component of the color to fade to</param>
	/// <param name="g">the green component of the color to fade to</param>
	/// <param name="b">the blue component of the color to fade to</param>
	public void FadeTo(int r, int g, int b)
	{
		for (int i = 0; i < mNumLeds; i++)
		{
			FadeTo(i, r, g, b);
		}
	}

	/// <summary>
	/// timer callback for each step of the fade
	/// </summary>
	/// <param name="ignored"></param>
	private void FadeOnTick(object ignored)
	{
		if (mFadeStep >= mFadeNumSteps)
		{
			// last step!

			// cleanup the timer first, just in case...
            if (mFadeTimer != null)
            {
                mFadeTimer.Dispose();
                mFadeTimer = null;
            }

		    // send the final data as what was originally ask for, to avoid 
			// FP errors that might have happened along the way
			for (int i = 0; i < mLedByteCount; i++)
			{
				mData[i] = (byte)(MASK | mFadeTo[i]);
			}

			SendUpdate();

			// give the user a poke, if they asked for it
			if (OnFadeComplete != null)
				OnFadeComplete(null);
		}
		else
		{
			// still not done.. increment each LED and push out the changes
			int step = mFadeStep++;
			int offset = mFadeNumSteps - step;

			for (int i = 0; i < mLedByteCount; i++)
			{
				mData[i] = (byte)(MASK | (byte)(mFadeTo[i] - (mFadeChange[i] * offset)));
			}

			SendUpdate();
		}
	}
	#endregion
#endif
}
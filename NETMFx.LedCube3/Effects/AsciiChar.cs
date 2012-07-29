using System;
using System.Threading;

namespace NETMFx.LedCube.Effects
{
    public class AsciiChar : CubeEffect
    {
        private const int DEFAULT_DURATION = 200;

        public string Phrase = "HELLO WORLD";

        public AsciiChar(LedCube cube, string phrase) : base(cube, DEFAULT_DURATION)
        {
            Phrase = phrase;
        }

        public AsciiChar(LedCube cube, int duration, string phrase) : base(cube, duration)
        {
            Phrase = phrase;
        }

        public override void Start(int duration)
        {
            Duration = duration;
            ShowWord(Phrase);
        }

        private void ShowWord(string word)
        {
            foreach (var c in word.ToCharArray())
            {
                ShowChar(c, Duration);
            }
        }

        private void ShowChar(char character, int levelDuration)
        {
            if (character == ' ')
            {
                Thread.Sleep(levelDuration);
                return;
            }
            for (var z = 0; z < Cube.Levels.Length; z++)
            {
                var t = DateTime.Now;                
                while (true)
                {
                    var elapsed = DateTime.Now.Subtract(t);
                    if(elapsed.Seconds * 1000 + elapsed.Milliseconds > levelDuration) break;
                    for (var y = 0; y < Cube.Levels.Length; y++)
                    {
                        for (var x = 0; x < Cube.Levels.Length; x++)
                        {
                            if ((GridMask[x + Cube.Levels.Length*y] & AsciiMask[character - 65]) <= 0) continue;
                            Cube.Illuminate((byte) x, (byte) y, (byte) z, 2);
                        }
                    }
                }
            }
        }

        // Masks used to decipher AsciiMask bits.
        private static readonly int[] GridMask = new[] {1, 2, 4, 8, 16, 32, 64, 128, 256};

        // This is a bitmask creating by looking at the cube from the front and counting identifying them like this:
        //  
        //      7   8   9       Ex. "O" =       O   O   O       = 111 101 111 = 0x1ef
        //      4   5   6                       O   *   O
        //      1   2   3                       O   O   O
        //
        private static readonly int[] AsciiMask = new[]{
                                                            0x1fd, // A = 1 1111 1101 
                                                            0x05b, // B = 0 0101 1011
                                                            0x1cf, // C = 1 1100 1111
                                                            0x13f, // D = 1 0011 1111
                                                            0x1df, // E = 1 1101 1111
                                                            0x1d9, // F = 1 1101 1001
                                                            0x1b7, // G = 1 1011 0111
                                                            0x17d, // H = 1 0111 1101
                                                            0x1d7, // I = 1 1101 0111 
                                                            0x173, // J = 1 1101 0011
                                                            0x15d, // K = 1 0101 1101
                                                            0x04f, // L = 0 0100 1111
                                                            0x1fd, // M = 1 1111 1101
                                                            0x17d, // N = 1 0111 1101
                                                            0x1ef, // O = 1 1110 1111
                                                            0x1b4, // P = 1 1011 0100
                                                            0x0de, // Q = 0 1101 1110
                                                            0x0dd, // R = 0 1101 1101
                                                            0x193, // S = 1 1001 0011
                                                            0x1d2, // T = 1 1101 0010
                                                            0x16f, // U =  1 0110 1111
                                                            0x16a, // V = 1 0110 1010
                                                            0x17f, // W = 1 0111 1111
                                                            0x155, // X = 1 0101 0101
                                                            0x16a, // Y = 1 0110 1010
                                                            0x1d7  // Z = 1 1101 0111
                                                        };
    }

}

using System;

namespace HydraNative
{
    public enum HydraNativePins
    {
        S03P03 = 40,
        S03P04 = 41,
        S03P05 = 44,
        S03P06 = 45,
        S03P07 = 26,
        S03P08 = 25,
        S03P09 = 27,

        S04P03 = 34,
        S04P04 = 11,
        S04P05 = 12,
        S04P06 = 46,
        S04P07 = 26,
        S04P08 = 25,
        S04P09 = 22,

        S05P03 = 9,
        S05P04 = 22,
        S05P05 = 21,
        S05P06 = 10,
        S05P08 = 23,
        S05P09 = 24,

        S06P03 = 113,
        S06P04 = 13,
        S06P05 = 14,
        S06P06 = 29,
        S06P07 = 30,
        S06P08 = 23,
        S06P09 = 24,

        S07P03 = 115,
        S07P04 = 6,
        S07P05 = 7,
        S07P06 = 116,
        S07P07 = 110,
        S07P08 = 111,
        S07P09 = 112,

        S12P03 = 73,
        S12P04 = 74,
        S12P05 = 75,
        S12P06 = 76,
        S12P07 = 77,
        S12P08 = 71,
        S12P09 = 70
    }

    public static class HydraNative
    {
        public static int HydraPin(string pinName)
        {
            pinName = pinName.ToUpper();
            if (pinName.Substring(0, 1) != "P") return -1;
            return (((int)(pinName.ToCharArray(1, 1)[0]) - 65) * 32 + Int32.Parse(pinName.Substring(2, pinName.Length - 2)));
        }

    }
}

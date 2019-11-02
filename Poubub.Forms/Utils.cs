using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Forms
{
    public class Utils
    {
        private static Random _rnd = new Random(DateTime.Now.Millisecond);
        public static Color GetRainbowColor()
        {
            string[] pallete = new string[] { "B15C48", "B25476", "955D8C", "7C5C8E", "555D91", "59749F", "6FB2A0", "70B265" };
            int argb = Int32.Parse("FF" + pallete[_rnd.Next(pallete.Length)], System.Globalization.NumberStyles.HexNumber);
            int swing = 10;
            Color c = Color.FromArgb(argb);
            return Color.FromArgb(255,
                  Math.Max(Byte.MinValue, Math.Min(Byte.MaxValue, _rnd.Next(c.R - swing, c.R + swing))),
                  Math.Max(Byte.MinValue, Math.Min(Byte.MaxValue, _rnd.Next(c.G - swing, c.G + swing))),
                  Math.Max(Byte.MinValue, Math.Min(Byte.MaxValue, _rnd.Next(c.B - swing, c.B + swing)))
                  );
        }
        public static Color GetDarkColor()
        {
            string[] pallete = new string[] { "272727", "262626", "2E2E2E", "2F2F2F" };
            int argb = Int32.Parse("FF" + pallete[_rnd.Next(pallete.Length)], System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }
        public static Color GetLightColor()
        {
            string[] pallete = new string[] { "F5F6EE", "F4F5ED", "F1F0EB", "F5F6F0", "F2F1EC", "F3F4ED" };
            int argb = Int32.Parse("FF" + pallete[_rnd.Next(pallete.Length)], System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }
    }
}

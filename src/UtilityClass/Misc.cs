using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UtilityClass
{
    public class Misc
    {
        private static Random random = new Random();

        public static int getRandom(int aMin, int aMax)
        {
            return random.Next(aMin, aMax);
        }

        public static string getWorkingDirectory()
        {
            return (Directory.GetCurrentDirectory());
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] dBytes)
        {
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetString(dBytes);
        }
    }
}

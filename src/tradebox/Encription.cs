using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using UtilityClass;

namespace tradebox
{
    [Serializable]
    public class Encription
    {
        private static string encodeFile = Misc.getWorkingDirectory() + @"\taskcmd.enc";
        private static string nonEncodeFile = Misc.getWorkingDirectory() + @"\taskcmd.cfg";

        public static bool isEncFileExist()
        {
            return (File.Exists(encodeFile));
        }

        public static bool isFileExist()
        {
            return (File.Exists(nonEncodeFile));
        }

        public static string readEncFile()
        {            
            DESCryptoServiceProvider AES256Bit = new DESCryptoServiceProvider();
            byte[] lvar = { 0x51,0xEE,0x55,0x95,0x2B,0x38,0x14,0x4C};
            byte[] lmodel = { 0xC7, 0xCE, 0xC7, 0xF9, 0x2D, 0x13, 0xEB, 0x18 };
            AES256Bit.Mode = CipherMode.CBC;
            AES256Bit.Key = lvar;
            AES256Bit.IV = lmodel;

            //Create a file stream to read the encrypted file back.
            FileStream fsread = new FileStream(encodeFile,
                                           FileMode.Open,
                                           FileAccess.Read);
            //Create a DES decryptor from the DES instance.
            ICryptoTransform desdecrypt = AES256Bit.CreateDecryptor();
            //Create crypto stream set to read and do a 
            //DES decryption transform on incoming bytes.
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,
                                                         desdecrypt,
                                                         CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            
            string lsr = new StreamReader(cryptostreamDecr).ReadToEnd();
            return (lsr);           
        }


        public static string readFile()
        {


            //Create a file stream to read the encrypted file back.
            FileStream fsread = new FileStream(nonEncodeFile,
                                           FileMode.Open,
                                           FileAccess.Read);
         
            string lsr = new StreamReader(fsread).ReadToEnd();
            return (lsr);
        }


		
    }
}

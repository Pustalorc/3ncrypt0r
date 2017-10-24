using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _3ncrypt0r
{
    class Program
    {
        private readonly static char[] CharacterDictionary = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþ".ToCharArray();

        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Write the text to encrypt.");
                var ToEncrypt = Console.ReadLine();
                char[] LettersInEncrypt = ToEncrypt.ToCharArray();
                byte[] Keys = new byte[6];
                for (var i = 0; i < 6; i++)
                    Keys[i] = (byte)Between(0, CharacterDictionary.Length);

                byte[] ByteValues = new byte[LettersInEncrypt.Length + (LettersInEncrypt.Length % 2)];
                for (var i = 0; i < ByteValues.Length; i++)
                    ByteValues[i] = (byte)CharacterDictionary.ToList().IndexOf(i >= LettersInEncrypt.Length ? ' ' : LettersInEncrypt[i]);

                string Encrypted = "";
                for (var i = 0; i < ByteValues.Length; i += 2)
                {
                    var top = (((ByteValues[i] * Keys[0]) + (ByteValues[i + 1] * Keys[1])) + Keys[4]) % CharacterDictionary.Length;
                    Encrypted += CharacterDictionary[top];
                    var bot = (((ByteValues[i] * Keys[2]) + (ByteValues[i + 1] * Keys[3])) + Keys[5]) % CharacterDictionary.Length;
                    Encrypted += CharacterDictionary[bot];
                }
                Console.WriteLine("Encrypted = " + Encrypted + "\nKey = " + string.Join("", Keys.ToList().ConvertAll(k => CharacterDictionary[k].ToString()).ToArray()));
                Clipboard.SetText(Encrypted + "\nKey: " + string.Join("", Keys.ToList().ConvertAll(k => CharacterDictionary[k].ToString()).ToArray()));
            }
        }

        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public static int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }
    }
}

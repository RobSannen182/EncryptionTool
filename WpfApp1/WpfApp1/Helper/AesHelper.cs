using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Collections.ObjectModel;

namespace WpfApp1.Helper
{
    class AesHelper
    {
        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        public static string GetPath => Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("WpfApp1")) + @"WpfApp1\WpfApp1\Keys\AES\AesKeys.txt";
        public static ObservableCollection<AesModel> GetAesKeys()
        {
            string path = GetPath;
            ObservableCollection<AesModel> AesModels = new ObservableCollection<AesModel>();

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string[] lines;
                    while (!sr.EndOfStream)
                    {
                        lines = sr.ReadLine().Split(',');
                        AesModels.Add(new AesModel()
                        {
                            KeyName = lines[0],
                            Key = Convert.FromBase64String(lines[1]),
                            IV = Convert.FromBase64String(lines[2])
                        });
                    }
                }
            }
            return AesModels;
        }
        public static void AddKeyToMemory(AesModel aesModel)
        {
            using (FileStream aFile = new FileStream(GetPath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(aFile))
                {
                    sw.WriteLine($"{aesModel.KeyName},{Convert.ToBase64String(aesModel.Key)},{Convert.ToBase64String(aesModel.IV)}");
                }
            }
        }
    }
}

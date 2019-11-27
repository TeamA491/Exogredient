using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TeamA.Exogredient.Services
{
    public class SecurityService
    {
        private readonly static RSACryptoServiceProvider FieldRSA = new RSACryptoServiceProvider();
        private readonly static RSAParameters _publicKey = FieldRSA.ExportParameters(false);
        private readonly static RSAParameters _privateKey = FieldRSA.ExportParameters(true);

        public static RSAParameters GetRSAPublicKey()
        {
            return _publicKey;
        }

        public static RSAParameters GetRSAPrivateKey()
        {
            return _privateKey;
        }

        //Reference:
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=netstandard-2.0
        public byte[] EncryptAES(string plainData, byte[] key, byte[] IV)
        {
            try
            {
                byte[] encryptedData;

                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    ICryptoTransform encryptor = aes.CreateEncryptor(key, IV);
                    MemoryStream ms = new MemoryStream();

                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainData);
                        }
                        encryptedData = ms.ToArray();
                    }

                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Reference:
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=netstandard-2.0
        public string DecryptAES(byte[] encryptedData, byte[] key, byte[] IV)
        {
            try
            {
                string decryptedData;
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    //Create AES decryptor using the given key and IV
                    ICryptoTransform decryptor = aes.CreateDecryptor(key, IV);

                    MemoryStream ms = new MemoryStream(encryptedData);
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            decryptedData = sr.ReadToEnd();
                        }
                    }

                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Reference:
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netstandard-2.0
        public byte[] EncryptRSA(byte[] plainData, RSAParameters publicKey)
        {
            try
            {
                byte[] decryptedData;
                using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    //Take the public key for decryption
                    rsa.ImportParameters(publicKey);
                    decryptedData = rsa.Encrypt(plainData, false);
                }
                return decryptedData;
            }
            catch(CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Reference:
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netstandard-2.0
        public byte[] DecryptRSA(byte[] encryptedData, RSAParameters privateKey)
        {
            try
            {
                byte[] decryptedData;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    //Take the private key for decryption
                    rsa.ImportParameters(privateKey);

                    decryptedData = rsa.Decrypt(encryptedData, false);
                }

                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Reference:
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netstandard-2.0

        public string HashPassword(string password, byte[] salt, int iterations, int hashLength)
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hashBytes = rfc.GetBytes(hashLength);
            string hashString = BytesToHexString(hashBytes);
            return hashString;
        }


        public string HashWithSHA1(string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            string hashedPassword = BytesToHexString(sha1.ComputeHash(Encoding.ASCII.GetBytes(password)));
            return hashedPassword;
        }

        /*
        public string HashSHA1WithHexString(string password, Encoding encoding)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            string hashedPassword = BytesToHexString(sha1.ComputeHash(encoding.GetBytes(ToHexString(password))));
            return hashedPassword;
        }
        */

        public string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public byte[] HexStringToBytes(string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            char[] charArray = hexString.ToCharArray();
            
            for(int i = 0; i < charArray.Length/2; i++)
            {
                string temp = "" + charArray[i*2] + charArray[i*2+1];
                bytes[i] = Convert.ToByte(temp,16);
            }

            return bytes;
        }

        public string ToHexString(string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            return BytesToHexString(bytes);
        }

        /*
        public Tuple<RSAParameters,RSAParameters> GenerateRSAKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            return new Tuple<RSAParameters, RSAParameters>(rsa.ExportParameters(false), rsa.ExportParameters(true));
        }
        */

        public byte[] GenerateSalt(int saltLength)
        {
            byte[] salt = new byte[saltLength];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public byte[] GenerateAESKey()
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            return aes.Key;
        }

        public byte[] GenerateAESIV()
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            return aes.IV;
        }

    }
}

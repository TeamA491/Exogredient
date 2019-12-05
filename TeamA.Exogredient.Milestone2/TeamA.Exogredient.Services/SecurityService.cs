using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace TeamA.Exogredient.Services
{
    public static class SecurityService
    {
        // Reference:
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=netstandard-2.0
        /// <summary>
        /// Encrypt a string with AES.
        /// </summary>
        /// <param name="plainData"> string to be encrypted </param>
        /// <param name="key"> key to be used for encryption </param>
        /// <param name="IV"> Initialization vectore to be used for encryption </param>
        /// <returns> byte array of the encrypted data </returns>
        public static byte[] EncryptAES(string plainData, byte[] key, byte[] IV)
        {
            try
            {
                byte[] encryptedData;

                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    // Create AES encryptor using the given key and IV.
                    ICryptoTransform encryptor = aes.CreateEncryptor(key, IV);
                    MemoryStream ms = new MemoryStream();

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            // Write data to be encrypted to the crypto stream.
                            sw.Write(plainData);
                        }
                        // Get the encrypted data from the memory stream in an array.
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

        // Reference:
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=netstandard-2.0
        /// <summary>
        /// Decrypt data encrypted with AES.
        /// </summary>
        /// <param name="encryptedData"> data encrypted with AES </param>
        /// <param name="key"> key used for AES encryption </param>
        /// <param name="IV"> IV used for AES encryption </param>
        /// <returns> the string of decrypted data </returns>
        public static string DecryptAES(byte[] encryptedData, byte[] key, byte[] IV)
        {
            try
            {
                string decryptedData;
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    // Create AES decryptor using the given key and IV.
                    ICryptoTransform decryptor = aes.CreateDecryptor(key, IV);

                    // Pass the encrypted data to the memory stream.
                    MemoryStream ms = new MemoryStream(encryptedData);
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            // Read the decrypted data as string from the stream reader.
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

        // Reference:
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netstandard-2.0
        /// <summary>
        /// Encrypt a byte array with RSA.
        /// </summary>
        /// <param name="plainData"> a byte array to be encrypted </param>
        /// <param name="publicKey"> public key used for encryption </param>
        /// <returns> a decrypted byte array </returns>
        public static byte[] EncryptRSA(byte[] plainData, byte[] publicKey)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // Pass the public key for encryption.
                    rsa.ImportCspBlob(publicKey);
                    // Encrypt the plain data.
                    decryptedData = rsa.Encrypt(plainData, false);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        // Reference:
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netstandard-2.0
        /// <summary>
        /// Decrypt an encrypted byte array with RSA.
        /// </summary>
        /// <param name="encryptedData"> a byte array encrypted with RSA </param>
        /// <param name="privateKey"> private key that is paired with public key used for encryption </param>
        /// <returns> a decrypted byte array </returns>
        public static byte[] DecryptRSA(byte[] encryptedData, byte[] privateKey)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // Pass the private key for decryption.
                    rsa.ImportCspBlob(privateKey);
                    // Decrypt the encrypted data.
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

        /// <summary>
        /// Hash a string data with HMAC SHA256.
        /// </summary>
        /// <param name="data"> a string to be hashed </param>
        /// <returns> a string of hash code </returns>
        public static string HashWithHMACSHA256(string data)
        {
            byte[] dataBytes = UtilityService.HexStringToBytes(data);
            using (HMACSHA256 hmac = new HMACSHA256())
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return UtilityService.BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// Hash a password with SHA3-256 KDF. 
        /// </summary>
        /// <param name="password"> password to be hashed</param>
        /// <param name="salt"> salt used for hashing </param>
        /// <param name="iterations"> number of iterations </param>
        /// <param name="hashLength"> the length of the output hashcode </param>
        /// <returns> a string of derived key </returns>
        public static string HashWithKDF(string password, byte[] salt, int iterations = 10000, int hashLength = 32)
        {

            byte[] passwordBytes = UtilityService.HexStringToBytes(password);
            Pkcs5S2ParametersGenerator pbkdf = new Pkcs5S2ParametersGenerator(new Sha3Digest());
            pbkdf.Init(passwordBytes, salt, iterations);
            KeyParameter derivedKey = (KeyParameter)pbkdf.GenerateDerivedMacParameters(hashLength * 8);

            return UtilityService.BytesToHexString(derivedKey.GetKey());
        }

        /// <summary>
        /// Hash password with SHA1.
        /// This function is used to match the output hashcode with the hashcodes of the most common 4000 passwords.
        /// Never use the hashcodes generated by this function for storing password.
        /// </summary>
        /// <param name="password"> password to be hashed </param>
        /// <returns> hex string of the hashed password </returns>
        public static string HashWithSHA1(string password)
        {
            using(SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                // Convert the password to ASCII byte array ->
                // Compute the hashcode in byte array with the ASCII byte array ->
                // Convert the hashcode byte array to a hex string
                return UtilityService.BytesToHexString(sha1.ComputeHash(Encoding.ASCII.GetBytes(password)));
            }
        }
        
        /// <summary>
        /// Generate a salt.
        /// </summary>
        /// <param name="saltLength"> length of the salt in bytes </param>
        /// <returns> byte array of salt </returns>
        public static byte[] GenerateSalt(int saltLength = 8)
        {
            // Create a byte array for salt with the given salt length
            byte[] salt = new byte[saltLength];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Fill the byte array for salt with random bytes 
                rng.GetBytes(salt);
            }
            return salt;
        }

        /* FOR TESTING PURPOSE, NEED TO BE IMPLEMENTED ON THE FRONTEND */
        /// <summary>
        /// Generate a key for AES encryption and decryption
        /// </summary>
        /// <returns> size 32 byte array of AES key </returns>
        public static byte[] GenerateAESKey()
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                return aes.Key;
            }
            
        }

        /// <summary>
        /// Generate a IV for AES encryption and decryption
        /// </summary>
        /// <returns> size 16 byte array of IV </returns>
        public static byte[] GenerateAESIV()
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                return aes.IV;
            }
        }

    }
}

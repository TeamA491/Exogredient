using System;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Managers
{
    public static class RegistrationManager
    {
        // Encrypted password, encrypted AES key, and AES IV are all in hex string format.
        public static async Task RegisterAsync(bool scopeAnswer, string firstName, string lastName,
                                               string email,string phoneNumber, string aesKey,
                                               string encryptedPassword, string aesIV)
        {
            Console.WriteLine("Here in the manager.");
        }
    }
}

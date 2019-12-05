using System;
using System.Threading.Tasks;

namespace TeamA.Exogredient.Managers
{
    public static class RegistrationManager
    {
        public static async Task RegisterAsync(bool scopeAnswer, string firstName, string lastName,
                                               string email,string phoneNumber, string aesKey,
                                               string aesCipher)
        {
            Console.WriteLine("Here in the manager.");
        }
    }
}

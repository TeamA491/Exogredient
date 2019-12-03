using System;
using System.Collections.Generic;


namespace TeamA.Exogredient.DAL
{
    public class UserRecord
    {
        IDictionary<string, string> data = new Dictionary<string, string>();

        public UserRecord(string userName, string firstName = null, string lastName = null, string email = null, 
            string phoneNumber = null, string password = null, string salt = null,
            string disabled = null, string userType = null)
        {
            data.Add("username", userName);
            data.Add("first_name", firstName);
            data.Add("last_name", lastName);
            data.Add("email", email);
            data.Add("phone_number", phoneNumber);
            data.Add("password", password);
            data.Add("disabled", disabled);
            data.Add("user_type", userType);
            data.Add("salt", salt);
        }

        public IDictionary<string, string> GetData()
        {
            return data;
        }
    }
}

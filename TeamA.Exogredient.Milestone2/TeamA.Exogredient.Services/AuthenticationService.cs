using System;
using System.Collections.Generic;
using System.Text;
using TeamA.Exogredient.DAL;

namespace TeamA.Exogredient.Services
{
    public class AuthenticationService
    {
        UserDAO _userDao;
        public AuthenticationService()
        {
            _userDao = new UserDAO();
        }

        public void DisableUserName(string userName)
        {
            try
            {
                if (!_userDao.UserNameExists(userName))
                {
                    return;
                }
                UserRecord disabledUser = new UserRecord(userName, disabled: "true");
                _userDao.Update(disabledUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Authenticate(string userName, string password)
        {
            try
            {
                if (!_userDao.UserNameExists(userName))
                {
                    return false;
                }
                if (_userDao.IsUserNameDisabled(userName))
                {
                    // HACK make custom exception
                    throw new Exception("This username is locked! To enable, contact the admin");
                }
                if (_userDao.GetPassword(userName).Equals(password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
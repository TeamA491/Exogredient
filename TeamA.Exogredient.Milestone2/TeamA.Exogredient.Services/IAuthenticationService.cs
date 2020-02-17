using System;
namespace TeamA.Exogredient.Services
{
    public interface IAuthenticationService
    {
        bool Authenticate(object existing, object credentials);
    }
}

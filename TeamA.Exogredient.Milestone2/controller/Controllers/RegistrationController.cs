using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;


namespace controller.Controllers
{
    //public class student
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationManager _registrationManager;

        public RegistrationController(RegistrationManager registrationManager)
        {
            _registrationManager = registrationManager;
        }

        [HttpPost("register1")]
        public async Task<bool> Register1Async(RegisrationRequest req)
        {
            //RegistrationManager.
            return await _registrationManager.TesterAsync(true, new UserRecord(req.UserName, req.FirstName + "/" + req.LastName, req.Email, req.PhoneNumber, req.EncryptedPassword,1,"Customer","salty",0,"", 0,0,0,0,0)).ConfigureAwait(false);
        }


    }
}
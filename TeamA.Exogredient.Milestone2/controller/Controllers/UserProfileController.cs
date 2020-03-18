using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.DataHelpers;

namespace controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileManager _userProfileManager;
        public UserProfileController(UserProfileManager userProfileManager)
        {
            _userProfileManager = userProfileManager;
        }
    
        // api/UserProfile/username
        [HttpGet("GetProfileScore/{username}")]
        public async Task<List<ProfileScoreResult>> GetProfileScores(string username)
        {
            return await _userProfileManager.GetProfileScore(username).ConfigureAwait(false);
        }

        // api/UserProfile/username/1
        [HttpGet("GetRecentUploads/{username}/{pagination}")]
        public async Task<List<UploadResult>> GetRecentUploads(string username, int pagination)
        {
            return await _userProfileManager.GetRecentUploads(username, pagination).ConfigureAwait(false);
        }

        // api/UserProfile/username/3
        [HttpGet("GetInProgressUploads/{username}/{pagination}")]
        public async Task<List<UploadResult>> GetInProgressUploads(string username, int pagination)
        {
            return await _userProfileManager.GetInProgressUploads(username, pagination).ConfigureAwait(false);
        }
    }
}
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class VisionPost
    {
        public string Username { get; set; }
        public IFormFile File { get; set; }
        public string IPAddress { get; set; }
    }
}

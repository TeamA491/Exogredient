using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class ProfileScoreResult
    {
        public int UploadUpvote { get; }
        
        public int UploadDownvote { get; }

        public ProfileScoreResult(int uploadUpvote, int uploadDownvote)
        {
            UploadUpvote = uploadUpvote;
            UploadDownvote = uploadDownvote;
        }
    }
}

using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TicketController
{
    public class TicketSearchRequest
    {
        public Dictionary<Constants.TicketSearchFilter, object> filterParams { get; set; }
    }
}

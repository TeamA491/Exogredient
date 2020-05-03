using System;
namespace TicketController
{
    public class TicketUpdateRequest
    {
        public string fieldUpdate { get; set; }
        public uint ticketID { get; set; }
    }
}

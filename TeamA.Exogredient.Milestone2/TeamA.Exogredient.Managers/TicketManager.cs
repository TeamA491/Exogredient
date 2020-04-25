using System;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Managers
{
    public class TicketManager
    {
        private readonly TicketService ticketService;
        private readonly LoggingManager loggingManager;

        public TicketManager(TicketService ticketService, LoggingManager loggingManager)
        {
            this.ticketService = ticketService;
            this.loggingManager = loggingManager;
        }
    }
}

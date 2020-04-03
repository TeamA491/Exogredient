using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public class TicketingService
    {
        public TicketingService()
        {
        }

        public async Task<TicketRecord[]> GetTicketsByFilterAsync(Dictionary<Constants.TicketSearchFilter, string> filterParams)
        {
            // TODO CONSTRUCT SEARCH QUERY BASED ON 

            foreach (KeyValuePair<Constants.TicketSearchFilter, string> filter in filterParams)
            {
                if (filter.Key == Constants.TicketSearchFilter.Category)
                {
                    // CONVERT filter.Value ENUM WITH ENUM.TRYPARSE, THROW ERROR IF INCORRECT, CATCH IN MANAGER
                    // TELL UI THAT SEARCH COULD NOT BE DONE
                }
                else if (filter.Key == Constants.TicketSearchFilter.DateFrom)
                {

                }
                else if (filter.Key == Constants.TicketSearchFilter.DateTo)
                {

                }
                else if (filter.Key == Constants.TicketSearchFilter.FlagColor)
                {

                }
                else if (filter.Key == Constants.TicketSearchFilter.ReadStatus)
                {

                }
                else if (filter.Key == Constants.TicketSearchFilter.Status)
                {

                }
            }

            // TODO AUTHORIZE WITH JWT
            TicketRecord[] tickets = {new TicketRecord(1,"","","","")};
            return tickets;
        }

        public async Task<bool> UpdateTicketStatusAsync(uint ticketID, string newStatus)
        {
            // TODO AUTHORIZE WITH JWT
            return true;
        }

        public async Task<bool> UpdateTicketCategoryAsync(uint ticketID, string newCategory)
        {
            // TODO AUTHORIZE WITH JWT
            return true;
        }

        public async Task<bool> UpdateTicketReadStatusAsync(uint ticketID, string newReadStatus)
        {
            // TODO AUTHORIZE WITH JWT
            return true;
        }

        public async Task<bool> UpdateTicketFlagColorAsync(uint ticketID< string newFlagColor)
        {
            // TODO AUTHORIZE WITH JWT
            return true;
        }

        public async Task<bool> SubmitTicketAsync(string category, string description)
        {
            return true;
        }

        private static T TryConvertEnum<T>(string value) where T : Enum
        {
            bool success = Enum.TryParse(value, out T result);
            if (!success)
                throw new ArgumentException("");

            return result;
        }
    }
}

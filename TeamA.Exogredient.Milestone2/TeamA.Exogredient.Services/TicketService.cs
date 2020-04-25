using System;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public class TicketService
    {
        private readonly TicketDAO ticketDAO;

        public TicketService(TicketDAO ticketDAO)
        {
            this.ticketDAO = ticketDAO;
        }

        /// <summary>
        /// Will return all tickets that meet the search criteria
        /// </summary>
        /// <param name="filterParams">A dictionary containing search criteria</param>
        /// <returns>Array of TicketRecords that meet the search criteria</returns>
        public async Task<List<TicketRecord>> GetTicketsByFilterAsync(Dictionary<Constants.TicketSearchFilter, string> filterParams)
        {
            // TODO AUTHORIZE WITH JWT

            // Go through all the search params
            foreach (KeyValuePair<Constants.TicketSearchFilter, string> filter in filterParams)
            {
                if (filter.Key == Constants.TicketSearchFilter.Category)
                {
                    // Make sure we are using a correct Enum value
                    Constants.TicketCategories category;
                    TryConvertEnum(filter.Value, out category);

                    // CONVERT filter.Value ENUM WITH ENUM.TRYPARSE, THROW ERROR IF INCORRECT, CATCH IN MANAGER
                    // TELL UI THAT SEARCH COULD NOT BE DONE
                }
                else if (filter.Key == Constants.TicketSearchFilter.DateFrom)
                {
                    // Make sure we are using a uint
                    uint ticketID;
                    TryConvertUInt(filter.Value, out ticketID);
                }
                else if (filter.Key == Constants.TicketSearchFilter.DateTo)
                {
                    // Make sure we are using a uint
                    uint ticketID;
                    TryConvertUInt(filter.Value, out ticketID);
                }
                else if (filter.Key == Constants.TicketSearchFilter.FlagColor)
                {
                    // Make sure we are using a correct Enum value
                    Constants.TicketFlagColors flagColor;
                    TryConvertEnum(filter.Value, out flagColor);
                }
                else if (filter.Key == Constants.TicketSearchFilter.ReadStatus)
                {
                    // Make sure we are using a correct Enum value
                    Constants.TicketReadStatuses ticketReadStatus;
                    TryConvertEnum(filter.Value, out ticketReadStatus);
                }
                else if (filter.Key == Constants.TicketSearchFilter.Status)
                {
                    // Make sure we are using a correct Enum value
                    Constants.TicketStatuses ticketStatus;
                    TryConvertEnum(filter.Value, out ticketStatus);
                }
            }

            // Temp
            List<TicketRecord> tickets = new List<TicketRecord>();
            return tickets;
        }

        /// <summary>
        /// Updates a ticket's status
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newStatus">The new status that we want it to have</param>
        /// <returns>Whether we successfully changed the status or not</returns>
        public async Task<bool> UpdateTicketStatusAsync(uint ticketID, Constants.TicketStatuses newStatus)
        {
            // TODO AUTHORIZE WITH JWT

            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException("");

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, newStatus.ToString(), null, null);
            await ticketDAO.UpdateAsync(ticketRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates a ticket's category
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newCategory">The new ticket category to use</param>
        /// <returns>Whether the ticket category succesfully changed or not</returns>
        public async Task<bool> UpdateTicketCategoryAsync(uint ticketID, Constants.TicketCategories newCategory)
        {
            // TODO AUTHORIZE WITH JWT
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException("");

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, newCategory.ToString(), null, null, null);
            await ticketDAO.UpdateAsync(ticketRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates a ticket's read status
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newReadStatus">The new read status to use</param>
        /// <returns>Whether the read status succesfully changed or not</returns>
        public async Task<bool> UpdateTicketReadStatusAsync(uint ticketID, bool newReadStatus)
        {
            // TODO AUTHORIZE WITH JWT
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException("");

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, null, null, null, newReadStatus);
            await ticketDAO.UpdateAsync(ticketRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates a ticket's flag color
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newFlagColor">The new flag color to use</param>
        /// <returns>Whether the flag color succesfully changed or not</returns>
        public async Task<bool> UpdateTicketFlagColorAsync(uint ticketID, Constants.TicketFlagColors newFlagColor)
        {
            // TODO AUTHORIZE WITH JWT
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException("");

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, null, newFlagColor.ToString(), null);
            await ticketDAO.UpdateAsync(ticketRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Submits a ticket to the system
        /// </summary>
        /// <param name="category">The category of the ticket</param>
        /// <param name="description">The description inside the ticket</param>
        /// <returns>Whether the ticket was saved or not</returns>
        public async Task<bool> SubmitTicketAsync(Constants.TicketCategories category, string description)
        {
            TicketRecord ticketRecord = new TicketRecord((uint)TimeUtilityService.CurrentUnixTime(),
                                                        category.ToString(),
                                                        Constants.TicketStatuses.Unresolved.ToString(),
                                                        Constants.TicketFlagColors.None.ToString(),
                                                        description);
            // TODO CHECK IF DESCRIPTION MEETS MINIMUM OR EXCEEDS MAXIMUM
            await ticketDAO.CreateAsync(ticketRecord).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Tries to convert a string to an enum
        /// </summary>
        /// <typeparam name="T">A generic type that allows any enum to be put in</typeparam>
        /// <param name="value">The value to convert to an enum</param>
        /// <param name="result">Where the converted string will be saved</param>
        private static void TryConvertEnum<T>(string value, out T result) where T : struct, Enum
        {
            bool success = Enum.TryParse(value, out result);
            if (!success)
                throw new ArgumentException("");
        }

        /// <summary>
        /// Tries to convert a string to a uint
        /// </summary>
        /// <param name="value">The value to convert to an enum</param>
        /// <param name="result">Where the converted string will be saved</param>
        private static void TryConvertUInt(string value, out uint result)
        {
            bool success = uint.TryParse(value, out result);
            if (!success)
                throw new ArgumentException("");
        }
    }
}

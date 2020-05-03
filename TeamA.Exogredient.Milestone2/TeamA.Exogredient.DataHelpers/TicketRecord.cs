using System;
using System.Collections.Generic;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class TicketRecord : ISQLRecord
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public TicketRecord(long submitTimestamp, string category, string status,
                            string flagColor, string description, bool isRead = false)
        {
            _data.Add(Constants.TicketDAOSubmitTimestampColumn, submitTimestamp);
            _data.Add(Constants.TicketDAOCategoryColumn, category);
            _data.Add(Constants.TicketDAOStatusColumn, status);
            _data.Add(Constants.TicketDAOFlagColorColumn, flagColor);
            _data.Add(Constants.TicketDAODescriptionColumn, description);
            _data.Add(Constants.TicketDAOIsReadColumn, isRead);
        }

        public IDictionary<string, object> GetData()
        {
            return _data;
        }
    }
}

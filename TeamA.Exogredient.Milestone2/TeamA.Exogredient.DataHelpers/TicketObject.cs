namespace TeamA.Exogredient.DataHelpers
{
    public class TicketObject : IDataObject
    {
        public long SubmitTimestamp { get; }
        public string Category { get; }
        public string Status { get; }
        public string FlagColor { get; }
        public string Description { get; }
        public bool IsRead { get; }

        public TicketObject(long submitTimestamp, string category, string status,
                            string flagColor, string description, bool isRead = false)
        {
            SubmitTimestamp = submitTimestamp;
            Category = category;
            Status = status;
            FlagColor = flagColor;
            Description = description;
            IsRead = isRead;
        }
    }
}

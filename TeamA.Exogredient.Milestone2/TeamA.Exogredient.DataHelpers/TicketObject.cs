namespace TeamA.Exogredient.DataHelpers
{
    public class TicketObject : IDataObject
    {
        public uint SubmitTimestamp { get; }
        public string Category { get; }
        public string Status { get; }
        public string FlagColor { get; }
        public string Description { get; }
        public bool IsRead { get; }

        public TicketObject(uint submitTimestamp, string category, string status,
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

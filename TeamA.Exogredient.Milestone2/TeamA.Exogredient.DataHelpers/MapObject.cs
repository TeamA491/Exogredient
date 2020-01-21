namespace TeamA.Exogredient.DataHelpers
{
    /// <summary>
    /// This object represents an item stored in the Map table.
    /// </summary>
    public class MapObject : IDataObject
    {
        public string Hash { get; }
        public string Actual { get; }
        public int Occurrences { get; }

        /// <summary>
        /// Constructs a MapObject by initializing its public fields.
        /// </summary>
        /// <param name="hash">The hash stored in the table (string)</param>
        /// <param name="actual">The actual value of the hash stored in the table (string)</param>
        /// <param name="occurrences">The occurrences of the hash in other tables (int)</param>
        public MapObject(string hash, string actual, int occurrences)
        {
            Hash = hash;
            Actual = actual;
            Occurrences = occurrences;       
        }
    }
}

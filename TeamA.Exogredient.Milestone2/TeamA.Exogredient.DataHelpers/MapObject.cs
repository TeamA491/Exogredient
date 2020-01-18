namespace TeamA.Exogredient.DataHelpers
{
    public class MapObject : IDataObject
    {
        public string Hash { get; }
        public string Actual { get; }
        public int Occurrences { get; }

        public MapObject(string hash, string actual, int occurrences)
        {
            Hash = hash;
            Actual = actual;
            Occurrences = occurrences;       
        }
    }
}

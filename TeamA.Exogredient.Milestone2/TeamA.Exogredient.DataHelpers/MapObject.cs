namespace TeamA.Exogredient.DataHelpers
{
    public class MapObject : IDataObject
    {
        public string Hash { get; }
        public string Actual { get; }

        public MapObject(string hash, string actual)
        {
            Hash = hash;
            Actual = actual;
        }
    }
}

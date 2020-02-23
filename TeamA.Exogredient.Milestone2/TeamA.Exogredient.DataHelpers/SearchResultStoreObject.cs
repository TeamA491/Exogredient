using System;
namespace TeamA.Exogredient.DataHelpers
{
    public class SearchResultStoreObject: IDataObject, IEquatable<SearchResultStoreObject>
    {
        public int StoreId { get; }
        public string StoreName { get; }
        public int UploadNum { get; }
        public double distance { get; }

        public SearchResultStoreObject(int storeId, string storeName, int uploadNum)
        {
            StoreId = storeId;
            StoreName = storeName;
            UploadNum = uploadNum;
        }

        public bool Equals(SearchResultStoreObject other)
        {
            if(other == null)
                return false;
            

            if(this.StoreId == other.StoreId)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            SearchResultStoreObject storeObj = obj as SearchResultStoreObject;
            if (storeObj == null)
                return false;
            else
                return Equals(storeObj);
        }

        public static bool operator ==(SearchResultStoreObject store1, SearchResultStoreObject store2)
        {
            if (((object)store1) == null || ((object)store2) == null)
                return Object.Equals(store1, store2);

            return store1.Equals(store2);
        }

        public static bool operator !=(SearchResultStoreObject store1, SearchResultStoreObject store2)
        {
            if (((object)store1) == null || ((object)store2) == null)
                return !Object.Equals(store1, store2);

            return !(store1.Equals(store2));
        }


    }
}

using System;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.DataHelpers
{
    public class StoreResult
    {
        public int StoreId { get; }
        public string StoreName { get; }
        public int IngredientNum { get; }
        public double Distance { get; set; }

        public StoreResult(int storeId, string storeName, int ingredientNum, double distance)
        {
            StoreId = storeId;
            StoreName = storeName;
            IngredientNum = ingredientNum;
            Distance = Math.Round(distance, Constants.FractionalDigits, MidpointRounding.AwayFromZero);
        }

        //public bool Equals(StoreResult other)
        //{
        //    if(other == null)
        //        return false;
            

        //    if(this.StoreId == other.StoreId)
        //        return true;
        //    else
        //        return false;
        //}

        //public override bool Equals(Object obj)
        //{
        //    if (obj == null)
        //        return false;

        //    StoreResult storeObj = obj as StoreResult;
        //    if (storeObj == null)
        //        return false;
        //    else
        //        return Equals(storeObj);
        //}

        //public static bool operator ==(StoreResult store1, StoreResult store2)
        //{
        //    if (((object)store1) == null || ((object)store2) == null)
        //        return Object.Equals(store1, store2);

        //    return store1.Equals(store2);
        //}

        //public static bool operator !=(StoreResult store1, StoreResult store2)
        //{
        //    if (((object)store1) == null || ((object)store2) == null)
        //        return !Object.Equals(store1, store2);

        //    return !(store1.Equals(store2));
        //}


    }
}

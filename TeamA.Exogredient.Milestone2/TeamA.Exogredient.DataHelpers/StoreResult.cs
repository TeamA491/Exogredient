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
            Distance = distance;
        }
    }
}

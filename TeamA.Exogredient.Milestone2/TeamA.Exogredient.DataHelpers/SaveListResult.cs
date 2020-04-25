using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class SaveListResult
    {
        public string Username { get; }

        public string IngredientName { get; }

        public int StoreId { get; }

        public SaveListResult(string username, string ingredientName, int storeId)
        {
            Username = username;
            IngredientName = ingredientName;
            StoreId = storeId;
        }
    }
}

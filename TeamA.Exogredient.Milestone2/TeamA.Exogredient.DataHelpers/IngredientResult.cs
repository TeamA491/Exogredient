namespace TeamA.Exogredient.DataHelpers
{
    public class IngredientResult
    {
        public string IngredientName { get; }
        public double AveragePrice { get; }
        public int UploadNum { get; }

        public IngredientResult(string ingredientName, double averagePrice, int uploadNum)
        {
            IngredientName = ingredientName;
            AveragePrice = averagePrice;
            UploadNum = uploadNum;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TeamA.Exogredient.DataHelpers
{
    public class SnapShotResult
    {
        public string _month { get; }
        public string operations { get; }
        public string amount_of_customers_and_storeowners { get; }
        public string top_cities_that_uses_application { get; }
        public string top_users_that_upload { get; }
        public string top_most_uploaded_ingredients { get; }
        public string top_most_uploaded_stores { get; }
        public string top_most_searched_ingredients { get; }
        public string top_most_searched_stores { get; }
        public string top_most_upvoted_users { get; }
        public string top_most_downvoted_users { get; }

        public SnapShotResult(string month, string operationsDict, string usersDict, string topCitiesDict, string topUploadedUsersDict, string topUploadedIngredientDict, string topUploadedStoreDict,
                            string topSearchedIngredientDict, string topSearchedStoreDict, string topUpvotedUsersDict, string topDownvotedUsersDict)
        {
            _month = month;
            operations = operationsDict;
            amount_of_customers_and_storeowners = usersDict;
            top_cities_that_uses_application = topCitiesDict;
            top_users_that_upload = topUploadedUsersDict;
            top_most_uploaded_ingredients = topUploadedIngredientDict;
            top_most_uploaded_stores = topUploadedStoreDict;
            top_most_searched_ingredients = topSearchedIngredientDict;
            top_most_searched_stores = topSearchedStoreDict;
            top_most_upvoted_users = topUpvotedUsersDict;
            top_most_downvoted_users = topDownvotedUsersDict;
        }
    }
}

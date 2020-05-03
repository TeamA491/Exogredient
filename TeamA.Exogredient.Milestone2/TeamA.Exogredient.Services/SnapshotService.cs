using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using System.Linq;
using IpData;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;

namespace TeamA.Exogredient.Services
{

    //public class IpInfo
    //{
    //    public string Ip { get; set; }
    //    public string City { get; set; }
    //    public string Region { get; set; }
    //    public string Country { get; set; }
    //}

    public class SnapshotService
    {
        LogDAO _logDAO;
        UserDAO _userDAO;
        UploadDAO _uploadDAO;
        SnapshotDAO _snapshotDAO;

        public SnapshotService(LogDAO logDAO, UserDAO userDAO, UploadDAO uploadDAO, SnapshotDAO snapshotDAO)
        {
            _logDAO = logDAO;
            _userDAO = userDAO;
            _uploadDAO = uploadDAO;
            _snapshotDAO = snapshotDAO;
        }

        /// <summary>
        /// Method to format the lists of snapshots into one huge string.
        /// It is done in a way that is closest to the json format.
        /// Basically designed to be json objects in a list that is formatted like a json.
        /// </summary>
        /// <param name="snapshotList">The list of snapshots.</param>
        /// <returns>A string that has all the data from the snapshots.</returns>
        public string FormatSnapShotListIntoJson(List<SnapShotResult> snapshotList)
        {
            string formattedString = "{\"snapshots\": [";

            foreach (var snapshot in snapshotList)
            {
                formattedString +=
                 $@"{{""{Constants.SnapshotMonth}"": ""{snapshot._month}"", " +
                 $@"""{Constants.SnapshotOperationsDict}"": ""{snapshot.operations}"", " +
                 $@"""{Constants.SnapshotUsersDict}"": ""{snapshot.count_of_registered_users}"", " +
                 $@"""{Constants.SnapshotTopCityDict}"": ""{snapshot.top_cities_that_uses_application}"", " +
                 $@"""{Constants.SnapshotTopUserUploadedDict}"": ""{snapshot.top_users_that_upload}"", " +
                 $@"""{Constants.SnapshotTopUploadedIngredientDict}"": ""{snapshot.top_most_uploaded_ingredients}"", " +
                 $@"""{Constants.SnapshotTopUploadedstoreDict}"": ""{snapshot.top_most_uploaded_stores}"", " +
                 $@"""{Constants.SnapshotTopSearchedIngredientDict}"": ""{snapshot.top_most_searched_ingredients}"", " +
                 $@"""{Constants.SnapshotTopSearchedStoreDict}"": ""{snapshot.top_most_searched_stores}"", " +
                 $@"""{Constants.SnapshotTopUpvotedUserDict}"": ""{snapshot.top_most_upvoted_users}"", " +
                 $@"""{Constants.SnapshotTopDownvotedUserDict}"": ""{snapshot.top_most_downvoted_users}""}},";
            }
            // Delete the last ",".
            formattedString = formattedString.Substring(0, formattedString.Length - 1);
            formattedString += "] }";
            return formattedString;
        }

        /// <summary>
        /// Method to format the dictonary of operations into json format.
        /// </summary>
        /// <param name="operationsDict">A dictionary with operations that the used performed.</param>
        /// <returns>A sting that contains the information from the dictionary.</returns>
        public string FormatOperationsDict(Dictionary<string, List<List<int>>> operationsDict)
        {
            string operationsString = "";
            foreach (var operation in operationsDict)
            {
                operationsString += "{'name':" + "'" + operation.Key + "','value':[";
                foreach (var day in operation.Value)
                {
                    operationsString += (day[0] + "," + day[1] + "," + day[2] + ",");
                }
                operationsString = operationsString.Substring(0, operationsString.Length - 1);
                operationsString += "]},";
            }
            if (operationsString.Length > 0)
            {
                operationsString = operationsString.Substring(0, operationsString.Length - 1);
            }
            operationsString = "[" + operationsString + "]";
            return operationsString;
        }

        /// <summary>
        /// Method to format the most common dictionary in the service (Dictionary<string, int>) into json format.
        /// This is needed for most of the dictionaries that I will be using.
        /// </summary>
        /// <param name="dict">A string, int dictionary.</param>
        /// <returns>A string result that holds the data from the dictionary.</returns>
        public string FormatStringIntDict(Dictionary<string, int> dict)
        {
            string formattedString = "";
            foreach (var things in dict)
            {
                formattedString += "{'name':" + "'" + things.Key + "','value':" + things.Value + "},";
            }
            if (formattedString.Length > 0)
            {
                formattedString = formattedString.Substring(0, formattedString.Length - 1);
            }
            formattedString = "[" + formattedString + "]";
            return formattedString;
        }

        /// <summary>
        /// Method to format year or month into a string.
        /// It first just convert the integer into a string.
        /// It then compares the length, and if it is less than 2, then it adds a "0" in front.
        /// This process is for formatting the month, because a year can't be less than 4 in length.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>A string that represents month or year depending on parameter.</returns>
        public string FormatIntMonthYearToString(int number)
        {
            string numberString = Convert.ToString(number);
            if (numberString.Length < 2)
            {
                numberString = numberString.Insert(0, "0");
            }
            return numberString;
        }

        /// <summary>
        /// Method to drop all the values in a string, int dictionary until there is only 10 pair left.
        /// This method is necessary for the top 10 feature.
        /// </summary>
        /// <param name="dict">A dictionary that will be used for the top 10 feature.</param>
        /// <returns>A dictionary that only has 10 pair at most.</returns>
        public Dictionary<string, int> DropTillTen(Dictionary<string, int> dict)
        {
            while(dict.Count > 10)
            {
                dict.Remove(dict.Keys.Last());
            }
            return dict;
        }

        /// <summary>
        /// Method used to sort a dictionary by the value (integer).
        /// It will create a new dictionary object and then append the pair values.
        /// The pair is sorted using linq in descending order to get the top 10.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>A value sorted diciontary (descending order).</returns>
        public Dictionary<string, int> SortDictionaryByIntValues(Dictionary<string, int> dict)
        {
            Dictionary<string, int> sortedDictionary = new Dictionary<string, int>();

            var sortedValues = from pair in dict orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in sortedValues)
            {
                sortedDictionary.Add(pair.Key, pair.Value);
            }
            return sortedDictionary;
        }

        /// <summary>
        /// Method to get the total amount of days in specific month.
        /// </summary>
        /// <param name="year">Year needed to get specific month.</param>
        /// <param name="month">Month used to find amount of days.</param>
        /// <returns></returns>
        public int GetDaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        /// <summary>
        /// An async method used to get all the logs in a specific month.
        /// </summary>
        /// <param name="year">The year to get specific month/.</param>
        /// <param name="month">The month to get the logs.</param>
        /// <param name="amountOfDays">The amount of days in the specfic mongth.</param>
        /// <returns>A list of list of the LogResult object.</returns>
        public async Task<List<List<LogResult>>> GetLogsInMonth(int year, int month, int amountOfDays)
        {
            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);
          
            //logresult
            //basically a list of days and inside that are list of logs
            //logs = timestamp, operation, identifier, ipaddress, errortype
            //month = {[logs, logs, logs], [logs, logs, logs], [logs, logs],..}
            //day = [logs, logs, logs, logs]
            
            var logResults = await _logDAO.ReadSpecificMonthAsync(yearString, monthString, amountOfDays).ConfigureAwait(false);
            return logResults;
        }

        /// <summary>
        /// Method to get a dictionary of string key and a list of list or int for value.
        /// This is a dictionary that will hold the name of the operation as the key.
        /// It will hold the list of days and then its values in each day.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <param name="amountOfDays">The amount of days in specific month.</param>
        /// <returns></returns>
        public Dictionary<string, List<List<int>>> GetOperationDict(List<List<LogResult>> logResults, int amountOfDays)
        {
            //Dictionary for every operation performed.
            //First type string will be the name of the operation, and second type List are the days and the amount of time the operation is performed each day.

            var operationsDict = new Dictionary<string, List<List<int>>>();

            // This is for graphs, operations and amount of that specific operations done per day
            int date = 0;
            foreach (var list in logResults) // for each days in a month
            {
                foreach (var logs in list) // for each logs in a day
                {
                    // Only get the first part of the operationInfo because it is the name of the operation.
                    string[] operationInfo = logs.Operation.Split('/');
                    string operation = operationInfo[0];
                    string errorType = logs.ErrorType;
                    if (!operationsDict.ContainsKey(operation))
                    {
                        var days = new List<List<int>>();
                        for (int i = 0; i < amountOfDays; i++)
                        {
                            var day = new List<int>(new int[3]);
                            days.Add(day);
                        }
                        operationsDict.Add(operation, days);
                    }
                    operationsDict[operation][date][2] = operationsDict[operation][date][2] + 1;
                    if (errorType == "null") // success operation
                    {
                        operationsDict[operation][date][0] = operationsDict[operation][date][0] + 1;
                    }
                    else // fail operation
                    {
                        operationsDict[operation][date][1] = operationsDict[operation][date][1] + 1;
                    }
                }
                // Increment date so the data can be broken up into dates.
                date++;
            }
            return operationsDict;
        }

        /// <summary>
        /// Method to get the amount of users there is based on the user type.
        /// </summary>
        /// <returns>A dictionary of users with their type and the amount.</returns>
        public async Task<Dictionary<string, int>> GetUsersDict()
        {     
            //Dictionary for registered user information
            //First type string will be the type of users, and second type will be the amount.
            var usersDict = new Dictionary<string, int>();

            int numberOfCustomers = await _userDAO.CountUsersTypeAsync(Constants.CustomerUserType).ConfigureAwait(false);

            usersDict.Add(Constants.CustomerUserType, numberOfCustomers);
            return usersDict;
        }

        /// <summary>
        /// Method used to make a list of specific cities using ipdata.
        /// ipdata is a ip lookup tool that can retrieve specific data relating to given ips.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>A dictionary with the name of the city and the amount use relating to that city.</returns>
        public async Task<Dictionary<string, int>> GetCityDictAsync(List<List<LogResult>> logResults)
        {
            /**
             * Top area that uses the application the most.
             * First type string will be the contatenated address, and second type int will be the amount.
             */
            var cityDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    // Code for using ipInfo, but the server was down so I switched back to ipdata.
                    //IpInfo ipInfo = new IpInfo();

                    ////Online resource on how to get the data from ipinfo (stackoverflow Offir Pe'er). 
                    //try
                    //{
                    //    //Download the jsonformat of information relating to specific ip from ipinfo.io with the help of WebClient.
                    //    string info = new WebClient().DownloadString("http://ipinfo.io/" + logs.IpAddress);
                    //    //Convert the Json formatted string into specific object.
                    //    ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);

                    //    //Get the english name of the country of abbreviations.
                    //    RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                    //    ipInfo.Country = myRI1.EnglishName;
                    //}
                    //catch (Exception)
                    //{
                    //    ipInfo.Country = null;
                    //}

                    //if (ipInfo.Country != null && ipInfo.Region != null && ipInfo.City != null)
                    //{
                    //    if (!cityDict.ContainsKey(ipInfo.Country + " " + ipInfo.Region + ", " + ipInfo.City))
                    //    {
                    //        cityDict.Add(ipInfo.Country + " " + ipInfo.Region + ", " + ipInfo.City, 0);
                    //    }
                    //    cityDict[ipInfo.Country + " " + ipInfo.Region + ", " + ipInfo.City] = cityDict[ipInfo.Country + " " + ipInfo.Region + ", " + ipInfo.City] + 1;
                    //}

                    //Key needed to use the IpDataClient.
                    var client = new IpDataClient("230afeb118ec79bd5bd63d52fcf9de1d4ad038e92bca6d4e9851fbac");
                    //Method to lookup the ip address.
                    var ipInfo = await client.Lookup(logs.IpAddress).ConfigureAwait(false);

                    //If the region field is not empty, then I will format the data and add it to my dictionary.
                    if (ipInfo.Region != null)
                    {
                        if (!cityDict.ContainsKey(ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City))
                        {
                            cityDict.Add(ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City, 0);
                        }
                        cityDict[ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City] = cityDict[ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City] + 1;
                    }
                }
            }
            return cityDict;
        }

        /// <summary>
        /// Method used to get all the users that uploaded this month.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>The list of users and the amount of time they uploaded.</returns>
        public Dictionary<string, int> GetUserUploadedDict(List<List<LogResult>> logResults)
        {
            //Top users that upload the most.
            //First type string will be the username, and second type int will be the amount. 
             
            // Constants.CreateUploadOperation/storename/ingredient
            var userUploadedDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        if (!userUploadedDict.ContainsKey(logs.Identifier))
                        {
                            userUploadedDict.Add(logs.Identifier, 0);
                        }
                        userUploadedDict[logs.Identifier] = userUploadedDict[logs.Identifier] + 1;
                    }
                }
            }
            return userUploadedDict;
        }

        /// <summary>
        /// Method to get the ingredients being uploaded and the amount of time they have been uploaded.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>A dictionary with the name of the ingredient and the amount.</returns>
        public Dictionary<string, int> GetUploadedIngredientDict(List<List<LogResult>> logResults)
        {
            /**
             * Top ingredients that uploaded the most.
             * First type string will be the name of the ingredient, and second type int will be the amount. 
             */
            var uploadedIngredientDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        //Has to be split because there are extra information.
                        string[] uploadInfo = logs.Operation.Split('/');
                        string ingredientName = uploadInfo[2];
                        if (!uploadedIngredientDict.ContainsKey(ingredientName))
                        {
                            uploadedIngredientDict.Add(ingredientName, 0);
                        }
                        uploadedIngredientDict[ingredientName] = uploadedIngredientDict[ingredientName] + 1;
                    }
                }
            }
            return uploadedIngredientDict;
        }

        /// <summary>
        /// Method to get the store that has been uploaded to the most and the amount.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>The dictionary with the name of the stores and the amount of times they were uploaded to.</returns>
        public Dictionary<string, int> GetUploadedStoreDict(List<List<LogResult>> logResults)
        {
            //Top stores that have been uploaded to the most.
            //First type string will be the name of the store, and second type int will be the amount.
            var uploadedStoreDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        //Has to be split because there are extra information.
                        string[] uploadInfo = logs.Operation.Split('/');
                        string storeName = uploadInfo[1];
                        if (!uploadedStoreDict.ContainsKey(storeName))
                        {
                            uploadedStoreDict.Add(storeName, 0);
                        }
                        uploadedStoreDict[storeName] = uploadedStoreDict[storeName] + 1;
                    }
                }
            }
            return uploadedStoreDict;
        }

        /// <summary>
        /// Method to get the name of the most searched ingredient and the amount.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log reults.</param>
        /// <returns>A dictionary with the name of the ingredient and the amount of time it has been searched.</returns>
        public Dictionary<string, int> GetSearchedIngredientDict(List<List<LogResult>> logResults)
        {
            
            //Top ingredient that is searched up the most.
            //First type string will be the name of the ingredient, and second type int will be the amount.
            
            var searchedIngredientDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
                        //Has to be split because there are extra information.
                        string[] searchInfo = logs.Operation.Split('/');
                        string searchType = searchInfo[1];
                        string searchName = searchInfo[2];
                        if (searchType == "ingredient")
                        {
                            if (!searchedIngredientDict.ContainsKey(searchName))
                            {
                                searchedIngredientDict.Add(searchName, 0);
                            }
                            searchedIngredientDict[searchName] = searchedIngredientDict[searchName] + 1;
                        }
                    }
                }
            }
            return searchedIngredientDict;
        }

        /// <summary>
        /// Method to get the name of searched store and the amount of times they have been searched.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>A dictionary with the name of the store and the amount of time it has been searched.</returns>
        public Dictionary<string, int> GetSearchedStoreDict(List<List<LogResult>> logResults)
        {
            /**
             * Top store that is searched up the most.
             * First type string will be the name of the store, and second tpe int will be the amount.
             */
            var searchedStoreDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
                        //Has to be split because there are extra information.
                        string[] searchInfo = logs.Operation.Split('/');
                        string searchType = searchInfo[1];
                        string searchName = searchInfo[2];
                        if (searchType == "store")
                        {
                            if (!searchedStoreDict.ContainsKey(searchName))
                            {
                                searchedStoreDict.Add(searchName, 0);
                            }
                            searchedStoreDict[searchName] = searchedStoreDict[searchName] + 1;
                        }
                    }
                }
            }
            return searchedStoreDict;
        }

        /// <summary>
        /// Method to first get all the uploads that has been upvoted.
        /// It will be a dictionary of the id of the uploads and the amount of time it has been upvoted.
        /// The second step is to get the users that is pertaining to those uploads.
        /// If multiple uploads belong to that user, increment their score.
        /// Users and score are stored in a stirng, int dictionary.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>A dictionary of users and the amount of upvotes they have.</returns>
        public async Task<Dictionary<string, int>> GetUpvotedUserDict(List<List<LogResult>> logResults)
        {
            
            //Top most upvoted users.
            //First type string will be the username, and second type will be the amount.
            
            // List of uploads that has been affected by upvotes. Constants.upvoteOperation/uploadID
            var upvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.UpvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!upvotedUploadsDict.ContainsKey(uploadID))
                        {
                            upvotedUploadsDict.Add(uploadID, 0);
                        }
                        upvotedUploadsDict[uploadID] = upvotedUploadsDict[uploadID] + 1;
                    }
                    // If an upload was undoed, then make sure to subtract the score.
                    else if (logs.Operation.Contains(Constants.UndoUpvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!upvotedUploadsDict.ContainsKey(uploadID))
                        {
                            upvotedUploadsDict.Add(uploadID, 0);
                        }
                        upvotedUploadsDict[uploadID] = upvotedUploadsDict[uploadID] - 1;
                    }
                }
            }
            // List of UploadID and usernames.
            var usersDict = await _uploadDAO.GetUsersWithUploadsAsync(upvotedUploadsDict).ConfigureAwait(false);

            // List of users that uploaded those uploads with amount of upvotes.
            var upvotedUsersDict = new Dictionary<string, int>();
            //Interate through the dictionary, add new users to the usersDict and give them their score.
            //If a user come up again, just increment their score with the new value.
            foreach(var user in usersDict)
            {
                if (!upvotedUsersDict.ContainsKey(user.Value))
                {
                    upvotedUsersDict.Add(user.Value, upvotedUploadsDict[user.Key]);
                }
                else
                {
                    upvotedUsersDict[user.Value] = upvotedUsersDict[user.Value] + upvotedUploadsDict[user.Key];
                }
            }
            return upvotedUsersDict;
        }

        /// <summary>
        /// Method to first get all the uploads that has been downvoted.
        /// It will be a dictionary of the id of the uploads and the amount of time it has been downvoted.
        /// The second step is to get the users that is pertaining to those uploads.
        /// If multiple uploads belong to that user, increment their score.
        /// Users and score are stored in a stirng, int dictionary.
        /// </summary>
        /// <param name="logResults">The list pertaining to the log results.</param>
        /// <returns>A dictionary of users and the amount of downvotes they have.</returns>
        public async Task<Dictionary<string, int>> GetDownvotedUserDict(List<List<LogResult>> logResults)
        {
            
            //Top most downvoted users.
            //First type string will be the username, and second type will be the amount.
            
            // List of uploads that has been affected by upvotes. Constants.upvoteOperation/uploadID
            var downvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.DownvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information.
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!downvotedUploadsDict.ContainsKey(uploadID))
                        {
                            downvotedUploadsDict.Add(uploadID, 0);
                        }
                        downvotedUploadsDict[uploadID] = downvotedUploadsDict[uploadID] + 1;
                    }
                    else if (logs.Operation.Contains(Constants.UndoDownvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information.
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!downvotedUploadsDict.ContainsKey(uploadID))
                        {
                            downvotedUploadsDict.Add(uploadID, 0);
                        }
                        downvotedUploadsDict[uploadID] = downvotedUploadsDict[uploadID] - 1;
                    }
                }
            }
            // List of UploadID and usernames.
            var usersDict = await _uploadDAO.GetUsersWithUploadsAsync(downvotedUploadsDict).ConfigureAwait(false);

            // List of users that uploaded those uploads with amount of downvotes.
            var downvotedUsersDict = new Dictionary<string, int>();
            //Interate through the dictionary, add new users to the usersDict and give them their score.
            //If a user come up again, just increment their score with the new value.
            foreach (var user in usersDict)
            {
                if (!downvotedUsersDict.ContainsKey(user.Value))
                {
                    downvotedUsersDict.Add(user.Value, downvotedUploadsDict[user.Key]);
                }
                else
                {
                    downvotedUsersDict[user.Value] = downvotedUsersDict[user.Value] + downvotedUploadsDict[user.Key];
                }
            }
            return downvotedUsersDict;
        }

        /// <summary>
        /// Method to finalize a string,int dictionary.
        /// It sort the dictionaries by the integer value, then drops until there are only 10 pair left.
        /// It then format the information into a json format.
        /// </summary>
        /// <param name="dict">A string,int dictionary </param>
        /// <returns>A finalized string that is in json format.</returns>
        public string FinalizeStringIntDictForSnap(Dictionary<string, int> dict)
        {
            return FormatStringIntDict(DropTillTen(SortDictionaryByIntValues(dict)));
        }

        /// <summary>
        /// Method that creates a snapshot based on the list of data.
        /// It will name and store the snapshot based on the given year and month.
        /// </summary>
        /// <param name="year">Year needed for naming and storing purposes.</param>
        /// <param name="month">Month needed for naming and storing purposes.</param>
        /// <param name="snapshot">The list of strings that has the snapshot data.</param>
        /// <returns>A bool result depending on whether it failed or not.</returns>
        public async Task<bool> CreateSnapShotAsync(int year, int month, List<string> snapshot)
        {
            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);
            bool status = false;
            try
            {
                status = await _snapshotDAO.StoreSnapshotAsync(yearString, monthString, snapshot).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new ArgumentException(Constants.FailCreateSnapShot);
            }
            return status;
        }

        /// <summary>
        /// Method to read a specific snapshot.
        /// </summary>
        /// <param name="year">Year needed to get specific snapshot.</param>
        /// <param name="month">Month needed to get specific snapshot.</param>
        /// <returns></returns>
        public async Task<SnapShotResult> ReadOneSnapshotAsync(int year, int month)
        {
            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);

            var snapshot = await _snapshotDAO.ReadMonthlySnapshotAsync(yearString, monthString);
            return snapshot;
        }

        /// <summary>
        /// Method to read all the snapshot relating to the year.
        /// </summary>
        /// <param name="year">Year needed to get all the snapshots pertaining to it. </param>
        /// <returns>A formatted string with all the data in multiple snapshots.</returns>
        public async Task<String> ReadMultiSnapshotAsync(int year)
        {
            string yearString = FormatIntMonthYearToString(year);

            var snapshotList = await _snapshotDAO.ReadYearlySnapshotAsync(yearString);

            // Format the data into a string format.
            var formattedSnapshotList = FormatSnapShotListIntoJson(snapshotList);

            return formattedSnapshotList;
        }

    }
}

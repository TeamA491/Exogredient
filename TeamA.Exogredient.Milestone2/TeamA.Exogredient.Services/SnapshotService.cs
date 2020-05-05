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
        /// Method to get all the years and months pertaining to that year that has snapshots.
        /// </summary>
        /// <returns>A json formatted string with the data.</returns>
        public async Task<String> GetYearAndMonthAsync()
        {
            var yearMonthDict = new Dictionary<string, List<string>>();
            var yearMonthList = await _snapshotDAO.GetYearAndMonthAsync().ConfigureAwait(false);

            foreach (var yearMonth in yearMonthList)
            {
                // e.g. 202011
                var year = yearMonth.Substring(0, 4);
                var month = yearMonth.Substring(4, 2);

                // Remove the 0 in front of months because it messes with the json format.
                if (month.Substring(0,1) == "0")
                {
                    month = month.Substring(1);
                }

                // If the year (key) does not already exists in the dictionary, then make a list for the month and then add the year and the month list.
                if (!yearMonthDict.ContainsKey(year))
                {
                    var tempDict = new List<string>();
                    tempDict.Add(month);
                    yearMonthDict.Add(year, tempDict);
                }
                // If the year does exist, then get the list of month, add the new mongth, and then update the monthlist of the year.
                else
                {
                    var dictHolder = yearMonthDict[year];
                    dictHolder.Add(month);
                    yearMonthDict[year] = dictHolder;
                }
            }

            var formattedYearMonth = FormatYearMonthDict(yearMonthDict);
            return formattedYearMonth;
        }

        /// <summary>
        /// Method to format the YearMonthDict into json format.
        /// </summary>
        /// <param name="yearMonthDict">The dictionary pertaining to the year and month with snapshots.</param>
        /// <returns>The json formatted string.</returns>
        public string FormatYearMonthDict(Dictionary<string, List<string>> yearMonthDict)
        {
            string formattedString = "";
            foreach (var year in yearMonthDict)
            {
                formattedString += "{'name':" + "'" + year.Key + "','value':[";
                foreach (var month in year.Value)
                {
                    formattedString += month + ",";
                }
                if (formattedString.Length > 0)
                {
                    formattedString = formattedString.Substring(0, formattedString.Length - 1);
                }
                formattedString += "]},";
            }
            if (formattedString.Length > 0)
            {
                formattedString = formattedString.Substring(0, formattedString.Length - 1);
            }

            formattedString = "{\"YearMonth\":\"[" + formattedString + "]\"}";

            return formattedString;
        }

        /// <summary>
        /// Method to get the data from the operation list from multiple snapshots.
        /// The data will be calculated and combined to return as a single dictionary.
        /// </summary>
        /// <param name="yearString">The year that the user requested for reading the multiple snapshot.</param>
        /// <param name="snapshotList">The list of snapshots.</param>
        /// <returns>A dictionary with operation data for each months.</returns>
        public Dictionary<string, List<int>> SortOperationDataFromultipleSnapshot(string yearString, List<SnapShotResult> snapshotList)
        {
            // Get a list of months that we have the snapshots for.
            var availableMonths = new List<string>();
            foreach (var snapshot in snapshotList)
            {
                availableMonths.Add(snapshot._month);
            }

            var operationDict = new Dictionary<string, List<int>>();
            var index = 0; // To keep track of which month we are on.
            // Iterate 12 times, once for each month.
            for (int i = 1; i < 13; i++) {
                // Get the string value pertaining to a month.
                var monthString = FormatIntMonthYearToString(i);
                // If we have a snapshot available for that month.
                if (availableMonths.Contains(yearString + monthString))
                {
                    // Get the operations property from a snapshot for that month. 
                    var stringOperations = snapshotList[index].operations.Substring(2, snapshotList[index].operations.Length - 4);
                    // Split up all the operations and loop through them.
                    var splittedOperations = new List<string>(stringOperations.Split(new string[] { "},{" }, StringSplitOptions.None));
                    foreach (var pair in splittedOperations)
                    {
                        var formattedString = pair.Replace("'", "");
                        formattedString = formattedString.Substring(5, formattedString.Length - 5);

                        // Split up the operation name and the values.
                        var operationNameAndValue = new List<string>(formattedString.Split(new string[] { ",value:" }, StringSplitOptions.None));
                        operationNameAndValue[1] = operationNameAndValue[1].Substring(1, operationNameAndValue[1].Length - 2);

                        // Split up the numbers in the values section.
                        // eg. [1,2,5,23,5,2]
                        var splitData = new List<string>(operationNameAndValue[1].Split(new string[] { "," }, StringSplitOptions.None));

                        var numberList = new List<int>();
                        // Loop through these splitted data now and check if they are actually numbers. If they are, add them to the numberList.
                        foreach (var num in splitData)
                        {
                            bool number = Int32.TryParse(num, out int x);
                            if (number)
                            {
                                numberList.Add(x);
                            }
                        }

                        var success = 0;
                        var fail = 0;
                        var total = 0;
                        // Iterate through the numberList and sort them by their specific meaning.
                        // The order this these values are, success, fail, total, in this pattern throughout the entire list.
                        // eg. 1,2,3. Where 1 is the success, 2 is the fail, and 3 is the total.
                        for (int j = 0; j < numberList.Count; j++)
                        {
                            if (j % 3 == 0)
                            {
                                success += numberList[j];
                            }
                            else if ((j - 1) % 3 == 0)
                            {
                                fail += numberList[j];
                            }
                            else
                            {
                                total += numberList[j];
                            }
                        }
                        // If the operation(key) does not already exist in the operation dictionary.
                        if (!operationDict.ContainsKey(operationNameAndValue[0]))
                        {
                            // Make a list and check what month we are in. (i is keeping track of months)
                            // Since this is first popping up, I have to fill out all the values before it as 0's because of the months with no data.
                            // It is adding 3 zeros because of the success,fail,total format.
                            var tempDict = new List<int>();
                            for (int k = 1; k < i; k++)
                            {
                                tempDict.Add(0);
                                tempDict.Add(0);
                                tempDict.Add(0);
                            }
                            // Add the current data (success, fail, total) to it now.
                            tempDict.Add(success);
                            tempDict.Add(fail);
                            tempDict.Add(total);
                            // Add the list as the value relating the operation key.
                            operationDict.Add(operationNameAndValue[0], tempDict);
                        }
                        // If the key already exists.
                        else
                        {
                            // Get the data list pertaining to the operation, add the success, fail, total value and reassign it as the new value.
                            var dictHolder = operationDict[operationNameAndValue[0]];
                            dictHolder.Add(success);
                            dictHolder.Add(fail);
                            dictHolder.Add(total);
                            operationDict[operationNameAndValue[0]] = dictHolder;
                        }
                    }
                    index++;
                }
                // If we do not have a snapshot for that specific month.
                else
                {
                    // Loop through the operations in the operation dictionary.
                    for (int l = 0; l < operationDict.Count; l ++)
                    {
                        // Get the list (data) for that operation and then add 0's to the data to represent success, fail, total.
                        // Assign that new updated list as the value for that operation.
                        var operation = operationDict.Keys.ElementAt(l);
                        var dictHolder = operationDict[operation];
                        dictHolder.Add(0);
                        dictHolder.Add(0);
                        dictHolder.Add(0);
                        operationDict[operation] = dictHolder;
                    }
                }
            }
            return operationDict;
        }

        /// <summary>
        /// Method specific to reading multiple snapshots.
        /// This is used for the user count dictionary.
        /// Each pair is a usertype and the amount of users of that type.
        /// That means the latest snapshot should have the information for that year.
        /// In this method I just get the last snapshot property pertaining to the registered user count and return it like it was.
        /// </summary>
        /// <param name="snapshotList">A list of snapshots.</param>
        /// <returns>A string value pertaining to the user count property from the last existing snapshot of that year.</returns>
        public string GetUsersResultFromSnapshotList(List<SnapShotResult>snapshotList)
        {
            // Get the count and to get the last snapshot, get the index with the size - 1.
            var snapshotListSize = snapshotList.Count;
            var registeredUser = snapshotList[snapshotListSize - 1].count_of_registered_users;
            return registeredUser;
        }

        /// <summary>
        /// Method used to combine the data from the list of snapshots.
        /// There has to be some string manipulation to extract the data from the snapshots.
        /// Combine the data into a dictionary with a string key and a int value and return it.
        /// </summary>
        /// <param name="snapshotList">A list with multiple snapshots.</param>
        /// <param name="property">A property relating to the property of a snapshot. Needed to know what dictionary to create.</param>
        /// <returns>A dictionary with all the combined data from the snapshots.</returns>
        public Dictionary<string, int> CombineSnapshotsDataForTopTen(List<SnapShotResult> snapshotList, string property)
        {
            var dictWithCombinedData = new Dictionary<string, int>();
            // Loop through each snapshot and get the snapshot property pertaining to the parameter (property).
            foreach (var snapshot in snapshotList)
            {
                // Get the property of the snapshot with a string.
                var data = snapshot.GetType().GetProperty(property).GetValue(snapshot, null);
                // The data was in system.string object format and the Substring method could not be used so it needed to be turned into a String.
                var dataString = (string)data;

                var formattedData = dataString.Substring(2, dataString.Length - 4);
                var splitData = new List<string>(formattedData.Split(new string[] { "},{" }, StringSplitOptions.None));

                // There are multiple pairs in the data, so it neededs to be split up.
                // eg. [{Name: david, Value: 1},{Name: jason, Value: 2}] 
                foreach (var pair in splitData)
                {
                    var formattedPair = pair.Replace("'", "");
                    formattedPair = formattedPair.Substring(5, formattedPair.Length - 5);
                    // Split up the name and the value.
                    var NameAndValue = new List<string>(formattedPair.Split(new string[] { ",value:" }, StringSplitOptions.None));

                    // Convert the value from a string into an int.
                    bool number = Int32.TryParse(NameAndValue[1], out int x);
                    if (number)
                    {
                        // If the dictionary does not contain the key, add the key and assign the value x to it. Where x is the converted int.
                        if (!dictWithCombinedData.ContainsKey(NameAndValue[0]))
                        {
                            dictWithCombinedData.Add(NameAndValue[0], x);
                        }
                        // If the key already exist then just increment the value of the key with the value x. 
                        else
                        {
                            dictWithCombinedData[NameAndValue[0]] = dictWithCombinedData[NameAndValue[0]] + x;
                        }
                    }
                }
            }
            return dictWithCombinedData;
        }

        /// <summary>
        /// Method to format the dictonary of operations into json format when given a dictionary of string and a list of int.
        /// This method is specially used for the read multiple snapshot operation.
        /// </summary>
        /// <param name="formattedOperationsDict">A dictionary with operations that the used performed and a list of int values.</param>
        /// <returns>A string that contains the information from the dictionary.</returns>
        public string FinalizeOperationsDict(Dictionary<string, List<int>> formattedOperationsDict)
        {
            string operationsString = "";
            foreach (var operation in formattedOperationsDict)
            {
                operationsString += "{'name':" + "'" + operation.Key + "','value':[";
                foreach (var number in operation.Value)
                {
                    operationsString += number + ",";
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
        /// Method to format the dictonary of operations into json format when the data is more messy.
        /// This method is specially used for when creating the snapshot.
        /// </summary>
        /// <param name="operationsDict">A dictionary with operations that the used performed and a list of lists of int values.</param>
        /// <returns>A string that contains the information from the dictionary.</returns>
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
            // A year will never be less than 2, so this check is pertaining to months.
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
            // If the dictionary has more than 10 pair, drop them.
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

            // Sort the Dictionary by the value in descneding order.
            var sortedValues = from pair in dict orderby pair.Value descending select pair;
            foreach (KeyValuePair<string, int> pair in sortedValues)
            {
                // Add the value into a new dictionary by this order.
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
        public async Task<List<List<LogResult>>> GetLogsInMonthAsync(int year, int month, int amountOfDays)
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
            // Dictionary for every operation performed.
            // First type string will be the name of the operation, and second type List are the days and the amount of time the operation is performed each day.

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
                    // Each unique operation is a key.
                    if (!operationsDict.ContainsKey(operation))
                    {
                        // A list that will hold data for each day in a month.
                        var days = new List<List<int>>();
                        for (int i = 0; i < amountOfDays; i++)
                        {
                            // A list that will hold the data for just that day.
                            // This will be size 3 for each day.
                            // Amount of success, fail, and total for each operation for that specific day.
                            var day = new List<int>(new int[3]);
                            days.Add(day);
                        }
                        operationsDict.Add(operation, days);
                    }
                    operationsDict[operation][date][2] = operationsDict[operation][date][2] + 1; // total
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
        public async Task<Dictionary<string, int>> GetUsersDictAsync()
        {     
            // Dictionary for registered user information
            // First type string will be the type of users, and second type will be the amount.
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

                    // Key needed to use the IpDataClient.
                    var client = new IpDataClient("230afeb118ec79bd5bd63d52fcf9de1d4ad038e92bca6d4e9851fbac");
                    // Method to lookup the ip address.
                    var ipInfo = await client.Lookup(logs.IpAddress).ConfigureAwait(false);

                    // If the region field is not empty, then I will format the data and add it to my dictionary.
                    if (ipInfo.Region != null)
                    {
                        // Check if the specific city already exist in the dictionary, if it doesn't add it.
                        if (!cityDict.ContainsKey(ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City))
                        {
                            cityDict.Add(ipInfo.CountryName + " " + ipInfo.Region + ", " + ipInfo.City, 0);
                        }
                        // Increment the amount specific to the city by one.
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
            //First type string will be the username, and second type int will be the amount. 
             
            // Constants.CreateUploadOperation/storename/ingredient
            var userUploadedDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    // Check if it was a create upload operation.
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        // Get the username and then add it to the dictionary.
                        if (!userUploadedDict.ContainsKey(logs.Identifier))
                        {
                            userUploadedDict.Add(logs.Identifier, 0);
                        }
                        // Increment the amount specific the username by one.
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
                        // UploadOperation/storename/ingredientname
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
            // Top stores that have been uploaded to the most.
            // First type string will be the name of the store, and second type int will be the amount.
            var uploadedStoreDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because there are extra information.
                        // UploadOperation/storename/ingredientname
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

            //First type string will be the name of the ingredient, and second type int will be the amount.
            
            var searchedIngredientDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because there are extra information.
                        // Searchoperation/ingredient/"apple"
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
            // First type string will be the name of the store, and second tpe int will be the amount.
            var searchedStoreDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because there are extra information.
                        // Searchoperation/store/"walmart"
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
        public async Task<Dictionary<string, int>> GetUpvotedUserDictAsync(List<List<LogResult>> logResults)
        {
            
            //Top most upvoted users.
            //First type string will be the username, and second type will be the amount.
            
            // List of uploads that has been affected by upvotes.
            var upvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    // If the log is of an upvote operation and if that operation was successful.
                    if (logs.Operation.Contains(Constants.UpvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information
                        // UpvoteOperation/uploadID
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!upvotedUploadsDict.ContainsKey(uploadID))
                        {
                            upvotedUploadsDict.Add(uploadID, 0);
                        }
                        upvotedUploadsDict[uploadID] = upvotedUploadsDict[uploadID] + 1;
                    }
                    // If the log is of an undo upvote operation and if that operation was successful.
                    // If an upload was undoed, then make sure to subtract the score.
                    else if (logs.Operation.Contains(Constants.UndoUpvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information
                        // UndoUpvoteOperation/uploadID
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
        public async Task<Dictionary<string, int>> GetDownvotedUserDictAsync(List<List<LogResult>> logResults)
        {            
            //First type string will be the username, and second type will be the amount.
            
            // List of uploads that has been affected by upvotes.
            var downvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    // If the log is of an downvote operation and if that operation was successful.
                    if (logs.Operation.Contains(Constants.DownvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information.
                        // DownvoteOperation/uploadID
                        string[] entireOperation = logs.Operation.Split('/');
                        string uploadID = entireOperation[1];
                        if (!downvotedUploadsDict.ContainsKey(uploadID))
                        {
                            downvotedUploadsDict.Add(uploadID, 0);
                        }
                        downvotedUploadsDict[uploadID] = downvotedUploadsDict[uploadID] + 1;
                    }
                    // If the log is of an undo downvote operation and if that operation was successful.
                    // If an upload was undoed, then make sure to subtract the score.
                    else if (logs.Operation.Contains(Constants.UndoDownvoteOperation) && logs.ErrorType == "null")
                    {
                        // Has to be split because of extra information.
                        // UndoDownvoteOperation/uploadID
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
            // Interate through the dictionary, add new users to the usersDict and give them their score.
            // If a user come up again, just increment their score with the new value.
            foreach (var user in usersDict)
            {
                // If the key does not exist then add the key and the value pertaining to it.
                if (!downvotedUsersDict.ContainsKey(user.Value))
                {
                    downvotedUsersDict.Add(user.Value, downvotedUploadsDict[user.Key]);
                }
                // If the key does exists then increment the value with the new value.
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
        /// <param name="dict">A dictionary that needs to be finalized into json format. </param>
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
        /// <returns>A snapshot with the all the data in that month.</returns>
        public async Task<SnapShotResult> ReadOneSnapshotAsync(int year, int month)
        {
            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);

            var snapshot = await _snapshotDAO.ReadMonthlySnapshotAsync(yearString, monthString).ConfigureAwait(false);
            return snapshot;
        }

        /// <summary>
        /// Method to read all the snapshot relating to the year.
        /// Format the snapshot objects and get their data and combine it.
        /// Sort through the data and return the finalized data to be stored into a single snapshot object to be sent up.
        /// </summary>
        /// <param name="year">Year needed to get all the snapshots pertaining to it. </param>
        /// <returns>A snapshot with the data with all the data in that year.</returns>
        public async Task<SnapShotResult> ReadMultiSnapshotAsync(int year)
        {
            string yearString = FormatIntMonthYearToString(year);

            // Get all the snapshots pertaining the the specified year.
            // The return value is a list of snapshots.
            var snapshotList = await _snapshotDAO.ReadYearlySnapshotAsync(yearString).ConfigureAwait(false);

            // Format and combine the data for all the snapshots.
            
            // The return value is a Dictionary<string, List<int>>.
            // The string value is the operation name, the list of int is the data seperated by months.
            var formattedOperationsDict = SortOperationDataFromultipleSnapshot(yearString, snapshotList);
            
            // The return value is a Dictionary<string, int>.
            // The string is for the name of the thing e.g. username, store name, ingredient name. 
            // The int value is the amount.
            var formattedCityDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopCityDict);
            var formattedUserUploadedDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopUserUploadedDict);
            var formattedUploadedIngredientDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopUploadedIngredientDict);
            var formattedUploadedStoreDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopUploadedStoreDict);
            var formattedSearchedIngredientDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopSearchedIngredientDict);
            var formattedSearchedStoreDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopSearchedStoreDict);
            var formattedUpvotedUserDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopUpvotedUserDict);
            var formattedDownvotedUserDict = CombineSnapshotsDataForTopTen(snapshotList, Constants.SnapshotTopDownvotedUserDict);

            // Finalize the data and turn it into a string, json format.
            var finalizedOperationsDict = FinalizeOperationsDict(formattedOperationsDict);
            var finalizedUsersDict = GetUsersResultFromSnapshotList(snapshotList);
            var finalizedCityDict = FinalizeStringIntDictForSnap(formattedCityDict);
            var finalizedUserUploadedDict = FinalizeStringIntDictForSnap(formattedUserUploadedDict);
            var finalizedUploadedIngredientDict = FinalizeStringIntDictForSnap(formattedUploadedIngredientDict);
            var finalizedUploadedStoreDict = FinalizeStringIntDictForSnap(formattedUploadedStoreDict);
            var finalizedSearchedIngredientDict = FinalizeStringIntDictForSnap(formattedSearchedIngredientDict);
            var finalizedSearchedStoreDict = FinalizeStringIntDictForSnap(formattedSearchedStoreDict);
            var finalizedUpvotedUserDict = FinalizeStringIntDictForSnap(formattedUpvotedUserDict);
            var finalizedDownvotedUserDict = FinalizeStringIntDictForSnap(formattedDownvotedUserDict);

            // Create a snapshot object and store the strings in the correct properties.
            var snapshot = new SnapShotResult(yearString, finalizedOperationsDict, finalizedUsersDict, finalizedCityDict, finalizedUserUploadedDict, finalizedUploadedIngredientDict,
                finalizedUploadedStoreDict, finalizedSearchedIngredientDict, finalizedSearchedStoreDict, finalizedUpvotedUserDict, finalizedDownvotedUserDict);

            return snapshot;
        }

    }
}

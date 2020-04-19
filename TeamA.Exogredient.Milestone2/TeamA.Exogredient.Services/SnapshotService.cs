using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamA.Exogredient.AppConstants;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.DataHelpers;
using IpData;

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

        public string FormatOperationsDict(Dictionary<string, List<List<int>>> operationsDict)
        {
            string operationsString = "{";
            foreach (var operation in operationsDict)
            {
                operationsString += "'" + operation.Key + "'" + ":[";
                foreach (var day in operation.Value)
                {
                    operationsString += (day[0] + "," + day[1] + "," + day[2] + ",");
                }
                operationsString = operationsString.Substring(0, operationsString.Length - 1);
                operationsString += "], ";
            }
            if (operationsString.Length > 0)
            {
                operationsString = operationsString.Substring(0, operationsString.Length - 2);
            }
            operationsString += "}";
            return operationsString;
        }

        public string FormatStringIntDict(Dictionary<string, int> dict)
        {
            string formattedString = "";
            foreach (var things in dict)
            {
                formattedString += things.Key + ":" + things.Value + ",";
            }
            if (formattedString.Length > 0)
            {
                formattedString = formattedString.Substring(0, formattedString.Length - 1);
            }
            return formattedString;
        }

        public string FormatIntMonthYearToString(int number)
        {
            string numberString = Convert.ToString(number);
            if (numberString.Length < 2)
            {
                numberString = numberString.Insert(0, "0");
            }
            return numberString;
        }

        public async Task<bool> CreateSnapShotAsync(int year, int month, int amountOfDays)
        {

            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);

            /**
             * logresult
             * basically a list of days and inside that are list of logs
             * logs = timestamp, operation, identifier, ipaddress, errortype
             * month = {[logs, logs, logs], [logs, logs, logs], [logs, logs],..}
             * day = [logs, logs, logs, logs]
             */
            List<List<LogResult>> logResults =  await _logDAO.ReadSpecificMonthAsync(yearString, monthString, amountOfDays).ConfigureAwait(false);


            /**
             * Dictionary for every operation performed.
             * First type string will be the name of the operation, and second type List are the days and the amount of time the operation is performed each day.
             */
            Dictionary<string, List<List<int>>> operationsDict = new Dictionary<string, List<List<int>>>();


            // This is for graphs, operations and amount of that specific operations done per day
            int date = 0;
            foreach (var list in logResults) // for each days in a month
            {
                foreach (var logs in list) // for each logs in a day
                {
                    string[] operationInfo = logs.Operation.Split('/');
                    string operation = operationInfo[0];
                    string errorType = logs.ErrorType;
                    if (!operationsDict.ContainsKey(operation))
                    {
                        var days  = new List<List<int>>();
                        for (int i = 0; i < amountOfDays; i++)
                        {
                            var day = new List<int>(new int[3]);
                            days.Add(day);
                        }
                        operationsDict.Add(operation, days);
                    }
                    operationsDict[operation][date][2] = operationsDict[operation][date][2] + 1;
                    if (errorType == "null") //success
                    {
                        operationsDict[operation][date][0] = operationsDict[operation][date][0] + 1;
                    }
                    else // fail
                    {
                        operationsDict[operation][date][1] = operationsDict[operation][date][1] + 1;
                    }
                }
                date++;
            }

            Console.WriteLine("All the logs.");
            foreach (var days in logResults)
            {
                foreach (var logs in days) // for each logs in a day
                {
                    Console.WriteLine(logs.Operation + ", " + logs.Identifier);
                }
            }

            Console.WriteLine("------------------------------------------------------------------------");




            Console.WriteLine("All the operations.");
            foreach (var value in operationsDict)
            {
                Console.WriteLine(value.Key);
                foreach (var shit in value.Value)
                {
                    foreach (var shit2 in shit)
                    {
                        Console.Write(shit2 + ", ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            Console.WriteLine("------------------------------------------------------------------------");


            /**
             * Dictionary for registered user information
             * First type string will be the type of users, and second type will be the amount.
             */
            Dictionary<string, int> usersDict = new Dictionary<string, int>();
            int numberOfCustomers = await _userDAO.CountUsersTypeAsync(Constants.CustomerUserType).ConfigureAwait(false);
            int numberOfStoreOwners = await _userDAO.CountUsersTypeAsync(Constants.StoreOwnerUserType).ConfigureAwait(false);

            usersDict.Add(Constants.CustomerUserType, numberOfCustomers);
            usersDict.Add(Constants.StoreOwnerUserType, numberOfStoreOwners);


            /**
            * Top area that uses the application the most.
            * First type string will be the contatenated address, and second type int will be the amount.
            */
            Dictionary<string, int> cityDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    var client = new IpDataClient("230afeb118ec79bd5bd63d52fcf9de1d4ad038e92bca6d4e9851fbac");

                    var ipInfo = await client.Lookup(logs.IpAddress).ConfigureAwait(false);

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


            /**
             * Top users that upload the most.
             * First type string will be the username, and second type int will be the amount. 
             */
            // Constants.CreateUploadOperation/storename/ingredient
            Dictionary<string, int> userUploadedDict = new Dictionary<string, int>();
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


            /**
             * Top ingredients that uploaded the most.
             * First type string will be the name of the ingredient, and second type int will be the amount. 
             */
            Dictionary<string, int> uploadedIngrdientDict = new Dictionary<string, int>();
            // Eli to add on the logs. What store being uploaded to and what ingredient.
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
                        string[] uploadInfo = logs.Operation.Split('/');
                        string ingredientName = uploadInfo[2];
                        if (!uploadedIngrdientDict.ContainsKey(ingredientName))
                        {
                            uploadedIngrdientDict.Add(ingredientName, 0);
                        }
                        uploadedIngrdientDict[ingredientName] = uploadedIngrdientDict[ingredientName] + 1;
                    }
                }
            }


            /**
             * Top stores that have been uploaded to the most.
             * First type string will be the name of the store, and second type int will be the amount.
             */
            Dictionary<string, int> uploadedStoreDict = new Dictionary<string, int>();
            // Eli to add on the logs. What store being uploaded (id and name) to and what ingredient.
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.CreateUploadOperation) && logs.ErrorType == "null")
                    {
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

            /**
             * Top ingredient that is searched up the most.
             * First type string will be the name of the ingredient, and second type int will be the amount.
             */
            Dictionary<string, int> searchedIngredientDict = new Dictionary<string, int>();
            // Charlie to add on the logs. searched term and name of searched thing.
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
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


            /**
             * Top store that is searched up the most.
             * First type string will be the name of the store, and second tpe int will be the amount.
             */
            Dictionary<string, int> searchedStoreDict = new Dictionary<string, int>();
            // Charlie to add on the logs. searched term and name of searched thing.
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.SearchOperation) && logs.ErrorType == "null")
                    {
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


            /**
             * Top most upvoted users.
             * First type string will be the username, and second type will be the amount.
             */
            // List of uploads that has been affected by upvotes. Constants.upvoteOperation/uploadID
            Dictionary<string, int> upvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.UpvoteOperation) && logs.ErrorType == "null")
                    {
                        string[] upvoteWithUpload = logs.Operation.Split('/');
                        string uploadID = upvoteWithUpload[1];
                        if (!upvotedUploadsDict.ContainsKey(uploadID))
                        {
                            upvotedUploadsDict.Add(uploadID, 0);
                        }
                        upvotedUploadsDict[uploadID] = upvotedUploadsDict[uploadID] + 1;
                    }
                }
            }
            // List of users that uploaded those uploads with amount of upvotes.
            Dictionary<string, int> upvotedUserDict = await _uploadDAO.GetUsersWithUploadsAsync(upvotedUploadsDict).ConfigureAwait(false);


            /**
            * Top most downvoted users.
            * First type string will be the username, and second type will be the amount.
            */
            // List of uploads that has been affected by upvotes. Constants.upvoteOperation/uploadID
            Dictionary<string, int> downvotedUploadsDict = new Dictionary<string, int>();
            foreach (var days in logResults) // for each days in a month
            {
                foreach (var logs in days) // for each logs in a day
                {
                    if (logs.Operation.Contains(Constants.DownvoteOperation) && logs.ErrorType == "null")
                    {
                        string[] downvoteWithUpload = logs.Operation.Split('/');
                        string uploadID = downvoteWithUpload[1];
                        if (!downvotedUploadsDict.ContainsKey(uploadID))
                        {
                            downvotedUploadsDict.Add(uploadID, 0);
                        }
                        downvotedUploadsDict[uploadID] = downvotedUploadsDict[uploadID] + 1;
                    }
                }
            }
            // List of users that uploaded those uploads with amount of upvotes.
            Dictionary<string, int> downvotedUserDict = await _uploadDAO.GetUsersWithUploadsAsync(downvotedUploadsDict).ConfigureAwait(false);


            List<string> snapshot = new List<string>();
            snapshot.Add(FormatOperationsDict(operationsDict));
            snapshot.Add(FormatStringIntDict(usersDict));
            snapshot.Add(FormatStringIntDict(cityDict));
            snapshot.Add(FormatStringIntDict(userUploadedDict));
            snapshot.Add(FormatStringIntDict(uploadedIngrdientDict));
            snapshot.Add(FormatStringIntDict(uploadedStoreDict));
            snapshot.Add(FormatStringIntDict(searchedIngredientDict));
            snapshot.Add(FormatStringIntDict(searchedStoreDict));
            snapshot.Add(FormatStringIntDict(upvotedUserDict));
            snapshot.Add(FormatStringIntDict(downvotedUserDict));

            bool something = await _snapshotDAO.StoreSnapshotAsync(snapshot, yearString, monthString).ConfigureAwait(false);

            return true;
        }

        public async Task<SnapShotResult> ReadOneSnapshotAsync(int year, int month)
        {
            string yearString = FormatIntMonthYearToString(year);
            string monthString = FormatIntMonthYearToString(month);

            var snapshot = await _snapshotDAO.ReadMonthlySnapshotAsync(yearString, monthString);
            return snapshot;
        }

        public async Task<List<SnapShotResult>> ReadMultiSnapshotAsync(int year)
        {
            string yearString = FormatIntMonthYearToString(year);

            var snapshotList = await _snapshotDAO.ReadYearlySnapshotAsync(yearString);
            return snapshotList;
        }

    }
}

using System;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.SQLCreateConsoleApp
{
    /// <summary>
    /// Class containg the script to create the sql tables in the *already existent mysql schema*.
    /// </summary>
    public class SQLCreateScript
    {
        private static readonly string _connection = Constants.SQLConnection;

        private const string _exogredientSchema = Constants.ExogredientSQLSchemaName;
        private const string _mapSchema = Constants.MapSQLSchemaName;

        /// <summary>
        /// Function to execute the necessary create table sql statements.
        /// </summary>
        /// <returns>Task</returns>
        public static async Task Main()
        {
            // Directions: Uncomment the specific create function that you do not want to execute.

            //await CreateUserTable().ConfigureAwait(false);
            await CreateIPTable().ConfigureAwait(false);
            //await CreateMapTable().ConfigureAwait(false);
            //await CreateTicketTables().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the User Table in the mysql schema.
        /// </summary>
        /// <returns>Task (bool)</returns>
        private static async Task<bool> CreateUserTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);
            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_exogredientSchema}`.`{Constants.UserDAOtableName}` (" +
                               $@"`{Constants.UserDAOusernameColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOusernameColumn] ? Constants.DefaultHashCharacterLength : Constants.MaximumUsernameCharacters)}) NOT NULL," +
                               $@"`{Constants.UserDAOnameColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOnameColumn] ? Constants.DefaultHashCharacterLength : (Constants.MaximumFirstNameCharacters + Constants.MaximumLastNameCharacters + 1))}) NOT NULL," +
                               $@"`{Constants.UserDAOemailColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOemailColumn] ? Constants.DefaultHashCharacterLength : Constants.MaximumEmailCharacters)}) NOT NULL," +
                               $@"`{Constants.UserDAOphoneNumberColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOphoneNumberColumn] ? Constants.DefaultHashCharacterLength : Constants.PhoneNumberCharacterLength)}) NOT NULL," +
                               $@"`{Constants.UserDAOpasswordColumn}` VARCHAR({Constants.DefaultHashCharacterLength}) NOT NULL," +
                               $@"`{Constants.UserDAOdisabledColumn}` TINYINT(1) NOT NULL," +
                               $@"`{Constants.UserDAOuserTypeColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOuserTypeColumn] ? Constants.DefaultHashCharacterLength : Constants.MaximumUserTypeLength)}) NOT NULL," +
                               $@"`{Constants.UserDAOsaltColumn}` VARCHAR({Constants.DefaultSaltLength}) NOT NULL," +
                               $@"`{Constants.UserDAOtempTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeColumn}` VARCHAR({(Constants.UserDAOIsColumnMasked[Constants.UserDAOemailCodeColumn] ? Constants.DefaultHashCharacterLength : Constants.EmailCodeLength)}) NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOloginFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.UserDAOlastLoginFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.UserDAOphoneCodeFailuresColumn}` INT NOT NULL," +
                               $@"PRIMARY KEY (`{Constants.UserDAOusernameColumn}`)," +
                               $@"UNIQUE INDEX `{Constants.UserDAOemailColumn}_UNIQUE` (`{Constants.UserDAOemailColumn}` ASC) VISIBLE," +
                               $@"UNIQUE INDEX `{Constants.UserDAOphoneNumberColumn}_UNIQUE` (`{Constants.UserDAOphoneNumberColumn}` ASC) VISIBLE);";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            connection.Close();
            return true;
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the IP Address table in the mysql schema.
        /// </summary>
        /// <returns>Task (bool)</returns>
        private static async Task<bool> CreateIPTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);
            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_exogredientSchema}`.`{Constants.IPAddressDAOtableName}` (" +
                               $@"`{Constants.IPAddressDAOIPColumn}` VARCHAR({(Constants.IPAddressDAOIsColumnMasked[Constants.IPAddressDAOIPColumn] ? Constants.DefaultHashCharacterLength : Constants.IPAddressLength)}) NOT NULL," +
                               $@"`{Constants.IPAddressDAOtimestampLockedColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.IPAddressDAOregistrationFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.IPAddressDAOlastRegFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.IPAddressDAOIPColumn}`));";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            connection.Close();
            return true;
        }

        private static async Task<bool> CreateMapTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);
            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_mapSchema}`.`{Constants.MapDAOTableName}` (" +
                               $@"`{Constants.MapDAOHashColumn}` VARCHAR({Constants.DefaultHashCharacterLength}) NOT NULL," +
                               $@"`{Constants.MapDAOActualColumn}` LONGTEXT NOT NULL," +
                               $@"`{Constants.MapDAOoccurrencesColumn}` INT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.MapDAOHashColumn}`));" +
                               $@"INSERT INTO `{_mapSchema}`.`{Constants.MapDAOTableName}` (`{Constants.MapDAOHashColumn}`, `{Constants.MapDAOActualColumn}`, `{Constants.MapDAOoccurrencesColumn}`) VALUES('0', '1', '1');" +
                               $@"INSERT INTO `{_mapSchema}`.`{Constants.MapDAOTableName}` (`{Constants.MapDAOHashColumn}`, `{Constants.MapDAOActualColumn}`, `{Constants.MapDAOoccurrencesColumn}`) VALUES('1', '0', '1');";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            Environment.SetEnvironmentVariable("COUNT", (2).ToString(), EnvironmentVariableTarget.User);

            connection.Close();
            return true;
        }

        private static async Task<bool> CreateTicketTables()
        {
            MySqlConnection connection = new MySqlConnection(_connection);
            connection.Open();

            string insertCategoriesQuery = BuildMultiSingleValueInsertStatement(Constants.TicketCategoryDAOTableName, Constants.TicketCategories);
            string insertStatusesQuery = BuildMultiSingleValueInsertStatement(Constants.TicketStatusDAOTableName, Constants.TicketStatuses);
            string insertFlagColorsQuery = BuildMultiSingleValueInsertStatement(Constants.TicketFlagColorDAOTableName, Constants.TicketFlagColors);

            StringBuilder sqlString = new StringBuilder($@"
                -- Enumeration tables
                CREATE TABLE IF NOT EXISTS `{Constants.TicketCategoryDAOTableName}` (
                    `{Constants.TicketDAOCategoryColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    PRIMARY KEY (`{Constants.TicketDAOCategoryColumn}`)
                );
                CREATE TABLE IF NOT EXISTS `{Constants.TicketStatusDAOTableName}` (
                    `{Constants.TicketDAOStatusColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    PRIMARY KEY (`{Constants.TicketDAOStatusColumn}`)
                );
                CREATE TABLE IF NOT EXISTS `{Constants.TicketFlagColorDAOTableName}` (
                    `{Constants.TicketDAOFlagColorColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    PRIMARY KEY (`{Constants.TicketDAOFlagColorColumn}`)
                );

                CREATE TABLE IF NOT EXISTS `{Constants.TicketDAOTableName}` (
                    `{Constants.TicketDAOTicketIDColumn}` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	                `{Constants.TicketDAOSubmitTimestampColumn}` INT UNSIGNED NOT NULL,
                    `{Constants.TicketDAOCategoryColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    `{Constants.TicketDAOStatusColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    `{Constants.TicketDAOFlagColorColumn}` VARCHAR({Constants.DefaultVarCharLength}) NOT NULL,
                    `{Constants.TicketDAODescriptionColumn}` LONGTEXT NOT NULL,
                    `{Constants.TicketDAOIsReadColumn}` BOOLEAN NOT NULL DEFAULT 0,

                    PRIMARY KEY (`{Constants.TicketDAOTicketIDColumn}`),
                    FOREIGN KEY (`{Constants.TicketDAOCategoryColumn}`)
                        REFERENCES `{Constants.TicketCategoryDAOTableName}`(`{Constants.TicketDAOCategoryColumn}`) ON DELETE CASCADE,
                    FOREIGN KEY (`{Constants.TicketDAOStatusColumn}`)
                      REFERENCES `{Constants.TicketStatusDAOTableName}`(`{Constants.TicketDAOStatusColumn}`) ON DELETE CASCADE,
                    FOREIGN KEY (`{Constants.TicketDAOFlagColorColumn}`)
                      REFERENCES `{Constants.TicketFlagColorDAOTableName}`(`{Constants.TicketDAOFlagColorColumn}`) ON DELETE CASCADE
                );
                
                {insertCategoriesQuery}
                {insertStatusesQuery}
                {insertFlagColorsQuery}
            ");

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            connection.Close();
            return true;
        }

        // TODO: MOVE TO STRINGUTILITY
        private static string BuildMultiSingleValueInsertStatement(string tableName, string[] values)
        {
            StringBuilder insertSQLString = new StringBuilder($"INSERT INTO {tableName} VALUES ");
            foreach (string s in values)
            {
                insertSQLString.Append($"(`{s}`),");
            }

            // Remove the last character ","
            insertSQLString.Length--;
            insertSQLString.Append(";");
            return insertSQLString.ToString();
        }
    }
}

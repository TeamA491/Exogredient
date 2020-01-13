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
            // Directions: Comment out the specific create function that you do not want to execute.

            await CreateUserTable().ConfigureAwait(false);
            //await CreateIPTable().ConfigureAwait(false);
            //await CreateMapTable().ConfigureAwait(false);
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
                               $@"`{Constants.UserDAOusernameColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOusernameColumn] ? Constants.DefaultHashLength : Constants.MaximumUsernameCharacters)}) NOT NULL," +
                               $@"`{Constants.UserDAOnameColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOnameColumn] ? Constants.DefaultHashLength : (Constants.MaximumFirstNameCharacters + Constants.MaximumLastNameCharacters + 1))}) NOT NULL," +
                               $@"`{Constants.UserDAOemailColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOemailColumn] ? Constants.DefaultHashLength : Constants.MaximumEmailCharacters)}) NOT NULL," +
                               $@"`{Constants.UserDAOphoneNumberColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOphoneNumberColumn] ? Constants.DefaultHashLength : Constants.PhoneNumberCharacterLength)}) NOT NULL," +
                               $@"`{Constants.UserDAOpasswordColumn}` VARCHAR({Constants.DefaultHashLength}) NOT NULL," +
                               $@"`{Constants.UserDAOdisabledColumn}` TINYINT(1) NOT NULL," +
                               $@"`{Constants.UserDAOuserTypeColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOuserTypeColumn] ? Constants.DefaultHashLength : Constants.MaximumUserTypeLength)}) NOT NULL," +
                               $@"`{Constants.UserDAOsaltColumn}` VARCHAR({Constants.DefaultSaltLength}) NOT NULL," +
                               $@"`{Constants.UserDAOtempTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeColumn}` VARCHAR({(Constants.IsColumnMasked[Constants.UserDAOemailCodeColumn] ? Constants.DefaultHashLength : Constants.EmailCodeLength)}) NOT NULL," +
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
                               $@"`{Constants.IPAddressDAOIPColumn}` VARCHAR({Constants.IPAddressLength}) NOT NULL," +
                               $@"`{Constants.IPAddressDAOtimestampLockedColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.IPAddressDAOregistrationFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.IPAddressDAOlastRegFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.IPAddressDAOIPColumn}`));";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            return true;
        }

        private static async Task<bool> CreateMapTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_mapSchema}`.`{Constants.MapDAOTableName}` (" +
                               $@"`{Constants.MapDAOHashColumn}` VARCHAR({Constants.DefaultHashLength}) NOT NULL," +
                               $@"`{Constants.MapDAOActualColumn}` LONGTEXT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.MapDAOHashColumn}`));";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            return true;
        }
    }
}

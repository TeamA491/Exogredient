using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.SQLCreateConsoleApp
{
    public class SQLCreateScript
    {
        private static readonly string _connection = Constants.SQLConnection;

        private const string _schema = Constants.SQLSchemaName;

        // USER TABLE
        private const string _userTableName = Constants.UserDAOtableName;

        private const string _username = Constants.UserDAOusernameColumn;
        private const string _firstName = Constants.UserDAOfirstNameColumn;
        private const string _lastName = Constants.UserDAOlastNameColumn;
        private const string _email = Constants.UserDAOemailColumn;
        private const string _phoneNumber = Constants.UserDAOphoneNumberColumn;
        private const string _password = Constants.UserDAOpasswordColumn;
        private const string _disabled = Constants.UserDAOdisabledColumn;
        private const string _userType = Constants.UserDAOuserTypeColumn;
        private const string _salt = Constants.UserDAOsaltColumn;
        private const string _tempTimestamp = Constants.UserDAOtempTimestampColumn;
        private const string _emailCode = Constants.UserDAOemailCodeColumn;
        private const string _emailCodeTimestamp = Constants.UserDAOemailCodeTimestampColumn;
        private const string _loginFailures = Constants.UserDAOloginFailuresColumn;
        private const string _lastLoginFailTimestamp = Constants.UserDAOlastLoginFailTimestampColumn;
        private const string _emailCodeFailures = Constants.UserDAOemailCodeFailuresColumn;
        private const string _phoneCodeFailures = Constants.UserDAOphoneCodeFailuresColumn;

        // IP TABLE
        private const string _ipTableName = Constants.IPAddressDAOtableName;

        private const string _ip = Constants.IPAddressDAOIPColumn;
        private const string _timestampLocked = Constants.IPAddressDAOtimestampLockedColumn;
        private const string _registrationFailures = Constants.IPAddressDAOregistrationFailuresColumn;
        private const string _lastRegFailTimestamp = Constants.IPAddressDAOlastRegFailTimestampColumn;

        public static async Task Main(string[] args)
        {
            // Directions: Uncomment the line which corresponds to the table you want to create.

            await CreateUserTable();
            //await CreateIPTable();
        }

        private static async Task CreateUserTable()
        {

            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();
            string sqlString = @$"CREATE TABLE `{_schema}`.`{_userTableName}` (" +
                               $@"`{_username}` VARCHAR(200) NOT NULL," +
                               $@"`{_firstName}` VARCHAR(200) NOT NULL," +
                               $@"`{_lastName}` VARCHAR(200) NOT NULL," +
                               $@"`{_email}` VARCHAR(200) NOT NULL," +
                               $@"`{_phoneNumber}` VARCHAR(10) NOT NULL," +
                               $@"`{_password}` VARCHAR(2000) NOT NULL," +
                               $@"`{_disabled}` TINYINT(1) NOT NULL," +
                               $@"`{_userType}` VARCHAR(11) NOT NULL," +
                               $@"`{_salt}` VARCHAR(200) NOT NULL," +
                               $@"`{_tempTimestamp}` BIGINT NOT NULL," +
                               $@"`{_emailCode}` VARCHAR(6) NOT NULL," +
                               $@"`{_emailCodeTimestamp}` BIGINT NOT NULL," +
                               $@"`{_loginFailures}` INT NOT NULL," +
                               $@"`{_lastLoginFailTimestamp}` BIGINT NOT NULL," +
                               $@"`{_emailCodeFailures}` INT NOT NULL," +
                               $@"`{_phoneCodeFailures}` INT NOT NULL," +
                               $@"PRIMARY KEY (`{_username}`)," +
                               $@"UNIQUE INDEX `{_email}_UNIQUE` (`{_email}` ASC) VISIBLE," +
                               $@"UNIQUE INDEX `{_phoneNumber}_UNIQUE` (`{_phoneNumber}` ASC) VISIBLE);";

            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync();
            await command.DisposeAsync();
        }

        private static async Task CreateIPTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();
            string sqlString = @$"CREATE TABLE `{_schema}`.`{_ipTableName}` (" +
                               $@"`{_ip}` VARCHAR(15) NOT NULL," +
                               $@"`{_timestampLocked}` BIGINT NOT NULL," +
                               $@"`{_registrationFailures}` INT NOT NULL," +
                               $@"`{_lastRegFailTimestamp}` BIGINT NOT NULL," +
                               $@"PRIMARY KEY(`{_ip}`));";

            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync();
            await command.DisposeAsync();
        }
    }
}

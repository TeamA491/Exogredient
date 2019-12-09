using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.SQLCreateConsoleApp
{
    public class SQLCreateScript
    {
        private static readonly string _connection = Constants.SQLConnection;

        private const string _schema = Constants.SQLSchemaName;

        public static async Task Main(string[] args)
        {
            // Directions: Uncomment the line which corresponds to the table you want to create.

            await CreateUserTable().ConfigureAwait(false);
            //await CreateIPTable().ConfigureAwait(false);
        }

        private static async Task CreateUserTable()
        {

            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();
            string sqlString = @$"CREATE TABLE `{_schema}`.`{Constants.UserDAOtableName}` (" +
                               $@"`{Constants.UserDAOusernameColumn}` VARCHAR(200) NOT NULL," +
                               $@"`{Constants.UserDAOfirstNameColumn}` VARCHAR(200) NOT NULL," +
                               $@"`{Constants.UserDAOlastNameColumn}` VARCHAR(200) NOT NULL," +
                               $@"`{Constants.UserDAOemailColumn}` VARCHAR(200) NOT NULL," +
                               $@"`{Constants.UserDAOphoneNumberColumn}` VARCHAR(10) NOT NULL," +
                               $@"`{Constants.UserDAOpasswordColumn}` VARCHAR(2000) NOT NULL," +
                               $@"`{Constants.UserDAOdisabledColumn}` TINYINT(1) NOT NULL," +
                               $@"`{Constants.UserDAOuserTypeColumn}` VARCHAR(11) NOT NULL," +
                               $@"`{Constants.UserDAOsaltColumn}` VARCHAR(200) NOT NULL," +
                               $@"`{Constants.UserDAOtempTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeColumn}` VARCHAR(6) NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOloginFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.UserDAOlastLoginFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.UserDAOemailCodeFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.UserDAOphoneCodeFailuresColumn}` INT NOT NULL," +
                               $@"PRIMARY KEY (`{Constants.UserDAOusernameColumn}`)," +
                               $@"UNIQUE INDEX `{Constants.UserDAOemailColumn}_UNIQUE` (`{Constants.UserDAOemailColumn}` ASC) VISIBLE," +
                               $@"UNIQUE INDEX `{Constants.UserDAOphoneNumberColumn}_UNIQUE` (`{Constants.UserDAOphoneNumberColumn}` ASC) VISIBLE);";

            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);
        }

        private static async Task CreateIPTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();
            string sqlString = @$"CREATE TABLE `{_schema}`.`{Constants.IPAddressDAOtableName}` (" +
                               $@"`{Constants.IPAddressDAOIPColumn}` VARCHAR(15) NOT NULL," +
                               $@"`{Constants.IPAddressDAOtimestampLockedColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.IPAddressDAOregistrationFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.IPAddressDAOlastRegFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.IPAddressDAOIPColumn}`));";

            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);
        }
    }
}

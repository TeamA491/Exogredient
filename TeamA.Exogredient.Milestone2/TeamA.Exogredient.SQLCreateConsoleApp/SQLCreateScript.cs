using System;
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
            //await CreateAnonymousUserTable().ConfigureAwait(false);
            //await CreateStoreTable().ConfigureAwait(false);
            await CreateUploadTable().ConfigureAwait(false);

            //await CreateMapTable().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the user table in the mysql schema.
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

            return true;
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the anonymous_user table in the mysql schema.
        /// </summary>
        /// <returns>Task (bool)</returns>
        private static async Task<bool> CreateAnonymousUserTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_exogredientSchema}`.`{Constants.AnonymousUserDAOTableName}` (" +
                               $@"`{Constants.AnonymousUserDAOIPColumn}` VARCHAR({(Constants.AnonymousUserDAOIsColumnMasked[Constants.AnonymousUserDAOIPColumn] ? Constants.DefaultHashCharacterLength : Constants.IPAddressLength)}) NOT NULL," +
                               $@"`{Constants.AnonymousUserDAOtimestampLockedColumn}` BIGINT NOT NULL," +
                               $@"`{Constants.AnonymousUserDAOregistrationFailuresColumn}` INT NOT NULL," +
                               $@"`{Constants.AnonymousUserDAOlastRegFailTimestampColumn}` BIGINT NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.AnonymousUserDAOIPColumn}`));";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the upload table in the mysql schema.
        /// </summary>
        /// <returns>Task (bool)</returns>
        private static async Task<bool> CreateUploadTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_exogredientSchema}`.`{Constants.UploadDAOTableName}` (" +
                               $@"`{Constants.UploadDAOUploadIdColumn}` INT NOT NULL AUTO_INCREMENT," +
                               $@"`{Constants.UploadDAOPostTimeDateColumn}` TIMESTAMP NOT NULL," +
                               $@"`{Constants.UploadDAOUploaderColumn}` VARCHAR({Constants.MaximumUsernameCharacters}) NOT NULL," +
                               $@"`{Constants.UploadDAOStoreIdColumn}` INT NOT NULL," +
                               $@"`{Constants.UploadDAODescriptionColumn}` VARCHAR({Constants.MaximumUploadDescriptionCharacters}) NOT NULL," +
                               $@"`{Constants.UploadDAORatingColumn}` VARCHAR({Constants.MaxRatingDigits}) NOT NULL," +
                               $@"`{Constants.UploadDAOPhotoColumn}` VARCHAR({Constants.MaximumPhotoCharacters}) NOT NULL," +
                               $@"`{Constants.UploadDAOPriceColumn}` DOUBLE({Constants.MaximumPriceDigits},{Constants.PriceAccuracyDigits}) NOT NULL," +
                               $@"`{Constants.UploadDAOPriceUnitColumn}` VARCHAR({Constants.PriceUnitMaxCharacters}) NOT NULL," +
                               $@"`{Constants.UploadDAOIngredientNameColumn}` VARCHAR({Constants.MaximumIngredientNameCharacters}) NOT NULL," +
                               $@"`{Constants.UploadDAOUpvoteColumn}` INT NOT NULL," +
                               $@"`{Constants.UploadDAODownvoteColumn}` INT NOT NULL," +
                               $@"`{Constants.UploadDAOInProgressColumn}` TINYINT(1) NOT NULL," +
                               $@"`{Constants.UploadDAOCategoryColumn}` VARCHAR({Constants.MaximumCategoryCharacters}) NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.UploadDAOUploadIdColumn}`)," +
                               $@"INDEX `{Constants.UploadDAOStoreIdColumn}_idx` (`{Constants.UploadDAOStoreIdColumn}` ASC) INVISIBLE," +
                               $@"CONSTRAINT `{Constants.UploadDAOUploaderColumn}`" +
                               $@"  FOREIGN KEY (`{Constants.UploadDAOUploaderColumn}`)" +
                               $@"  REFERENCES `{Constants.ExogredientSQLSchemaName}`.`{Constants.UserDAOtableName}` (`{Constants.UserDAOusernameColumn}`)" +
                               $@"  ON DELETE NO ACTION" +
                               $@"  ON UPDATE NO ACTION," +
                               $@"CONSTRAINT `{Constants.UploadDAOStoreIdColumn}`" +
                               $@"  FOREIGN KEY (`{Constants.UploadDAOStoreIdColumn}`)" +
                               $@"  REFERENCES `{Constants.ExogredientSQLSchemaName}`.`{Constants.StoreDAOTableName}` (`{Constants.StoreDAOStoreIdColumn}`)" +
                               $@"  ON DELETE NO ACTION" +
                               $@"  ON UPDATE NO ACTION);";

            // Create the commmand object and execute/dispose it asynchronously.
            MySqlCommand command = new MySqlCommand(sqlString, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            await command.DisposeAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Executes the CREATE TABLE statement for the store table in the mysql schema.
        /// </summary>
        /// <returns>Task (bool)</returns>
        private static async Task<bool> CreateStoreTable()
        {
            MySqlConnection connection = new MySqlConnection(_connection);

            connection.Open();

            // Construct the sql string based on the constants for table name, column names, and variable length values.
            string sqlString = @$"CREATE TABLE `{_exogredientSchema}`.`{Constants.StoreDAOTableName}` (" +
                               $@"`{Constants.StoreDAOStoreIdColumn}` INT NOT NULL AUTO_INCREMENT," +
                               $@"`{Constants.StoreDAOStoreNameColumn}` VARCHAR({Constants.StoreNameMaxCharacters}) NOT NULL," +
                               $@"`{Constants.StoreDAOLatitudeColumn}` DOUBLE NOT NULL," +
                               $@"`{Constants.StoreDAOLongitudeColumn}` DOUBLE NOT NULL," +
                               $@"`{Constants.StoreDAOPlaceIdColumn}` VARCHAR({Constants.PlaceIDCharacters}) NOT NULL," +
                               $@"`{Constants.StoreDAOStoreDescriptionColumn}` VARCHAR({Constants.StoreDescriptionMaxCharacters}) NOT NULL," +
                               $@"PRIMARY KEY(`{Constants.StoreDAOStoreIdColumn}`)," +
                               $@"UNIQUE INDEX geocode ({Constants.StoreDAOLatitudeColumn}, {Constants.StoreDAOLongitudeColumn}));";

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

            return true;
        }
    }
}

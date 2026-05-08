using Microsoft.Data.SqlClient;

namespace ManageClientSql.Repository;

public class DatabaseHelper
{
    private static string _dbName =>
        Environment.GetEnvironmentVariable("DB_NAME") ?? "ManageClient";

    // Connection to master — used only to create the database if needed
    private static string MasterConnectionString =>
        $"Server=localhost,1433;Database=master;User Id=sa;Password=Sa@123456;TrustServerCertificate=True;";

    public static string ConnectionString =>
        $"Server=localhost,1433;Database={_dbName};User Id=sa;Password=Sa@123456;TrustServerCertificate=True;";

    public static void InitializeDatabase()
    {
        // Step 1: create the database if it doesn't exist (connect to master)
        using (var masterConnection = new SqlConnection(MasterConnectionString))
        {
            masterConnection.Open();
            var createDb = masterConnection.CreateCommand();
            createDb.CommandText = $@"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{_dbName}')
                CREATE DATABASE [{_dbName}]";
            createDb.ExecuteNonQuery();
        }

        // Step 2: create the table if it doesn't exist
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            IF NOT EXISTS (
                SELECT * FROM sysobjects
                WHERE name='Client' AND xtype='U'
            )
            CREATE TABLE Client (
                Id          INT PRIMARY KEY IDENTITY(1,1),
                FullName    NVARCHAR(100) NOT NULL,
                PhoneNumber NVARCHAR(20)  NOT NULL,
                Email       NVARCHAR(100) NOT NULL
            );";
        command.ExecuteNonQuery();
    }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
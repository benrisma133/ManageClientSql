using Microsoft.Data.SqlClient;
using ManageClientSql.Repository.Models;

namespace ManageClientSql.Repository;

public class ClientRepository
{
    // Returns new ID after insert
    public static int Add(Client client)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Client (FullName, PhoneNumber, Email)
            VALUES (@FullName, @PhoneNumber, @Email);
            SELECT SCOPE_IDENTITY();";

        command.Parameters.AddWithValue("@FullName", client.FullName);
        command.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
        command.Parameters.AddWithValue("@Email", client.Email);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static bool Update(Client client)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Client
            SET FullName    = @FullName,
                PhoneNumber = @PhoneNumber,
                Email       = @Email
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", client.Id);
        command.Parameters.AddWithValue("@FullName", client.FullName);
        command.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
        command.Parameters.AddWithValue("@Email", client.Email);

        return command.ExecuteNonQuery() > 0;
    }

    public static bool Delete(int id)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Client WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public static Client? GetClientById(int id)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Client WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Client
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                PhoneNumber = reader.GetString(2),
                Email = reader.GetString(3)
            };
        }

        return null;
    }

    public static List<Client> GetAll()
    {
        var clients = new List<Client>();

        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Client";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            clients.Add(new Client
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                PhoneNumber = reader.GetString(2),
                Email = reader.GetString(3)
            });
        }

        return clients;
    }

    public static bool IsClientExistByPhone(string phone, int excludeId = 0)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Client WHERE PhoneNumber = @Phone AND Id != @ExcludeId";
        command.Parameters.AddWithValue("@Phone", phone);
        command.Parameters.AddWithValue("@ExcludeId", excludeId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public static bool IsClientExistByEmail(string email, int excludeId = 0)
    {
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Client WHERE Email = @Email AND Id != @ExcludeId";
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@ExcludeId", excludeId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }
}
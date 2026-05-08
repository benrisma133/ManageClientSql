using ManageClientSql.Repository;
using ManageClientSql.Repository.Models;
using ManageClientSql.Service;

namespace ManageClientSql.Tests;

public class ClientServiceTests : IDisposable
{
    public ClientServiceTests()
    {
        // Use a separate test database — never touches your real data
        Environment.SetEnvironmentVariable("DB_NAME", "ManageClientSqlTest");
        DatabaseHelper.InitializeDatabase();
    }

    public void Dispose()
    {
        // Clean up all data after each test
        using var connection = DatabaseHelper.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Client";
        command.ExecuteNonQuery();
    }

    // ─── Add ────────────────────────────────────────────────────────────────

    [Fact]
    public void Save_AddNew_ReturnsTrue_AndSwitchesToUpdateMode()
    {
        var service = new ClientService
        {
            Client = new Client
            {
                FullName = "Ismail Ben",
                PhoneNumber = "0601000001",
                Email = "ismail@test.com"
            }
        };

        bool result = service.Save();

        Assert.True(result);
        Assert.True(service.Client.Id > 0);
        Assert.Equal(ClientService.enMode.Update, service.Mode);
    }

    [Fact]
    public void Save_AddNew_AssignsNewId()
    {
        var service = new ClientService
        {
            Client = new Client
            {
                FullName = "Test User",
                PhoneNumber = "0601000002",
                Email = "test@test.com"
            }
        };

        service.Save();

        Assert.True(service.Client.Id > 0);
    }

    // ─── Duplicate checks ───────────────────────────────────────────────────

    [Fact]
    public void Save_AddNew_FailsIfPhoneAlreadyExists()
    {
        var service1 = new ClientService
        {
            Client = new Client
            {
                FullName = "Client One",
                PhoneNumber = "0601000003",
                Email = "one@test.com"
            }
        };
        service1.Save();

        var service2 = new ClientService
        {
            Client = new Client
            {
                FullName = "Client Two",
                PhoneNumber = "0601000003", // same phone
                Email = "two@test.com"
            }
        };

        bool result = service2.Save();

        Assert.False(result);
    }

    [Fact]
    public void Save_AddNew_FailsIfEmailAlreadyExists()
    {
        var service1 = new ClientService
        {
            Client = new Client
            {
                FullName = "Client One",
                PhoneNumber = "0601000004",
                Email = "same@test.com"
            }
        };
        service1.Save();

        var service2 = new ClientService
        {
            Client = new Client
            {
                FullName = "Client Two",
                PhoneNumber = "0601000005",
                Email = "same@test.com" // same email
            }
        };

        bool result = service2.Save();

        Assert.False(result);
    }

    // ─── Update ─────────────────────────────────────────────────────────────

    [Fact]
    public void Save_Update_ReturnsTrueAfterSuccessfulAdd()
    {
        var service = new ClientService
        {
            Client = new Client
            {
                FullName = "Before Update",
                PhoneNumber = "0601000006",
                Email = "before@test.com"
            }
        };
        service.Save(); // AddNew → switches to Update mode

        service.Client.FullName = "After Update";
        bool result = service.Save(); // Update

        Assert.True(result);
    }

    // ─── Delete ─────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_ExistingClient_ReturnsTrue()
    {
        var service = new ClientService
        {
            Client = new Client
            {
                FullName = "To Delete",
                PhoneNumber = "0601000007",
                Email = "delete@test.com"
            }
        };
        service.Save();

        bool result = service.Delete();

        Assert.True(result);
    }

    // ─── GetAll ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetAll_ReturnsAllAddedClients()
    {
        var s1 = new ClientService { Client = new Client { FullName = "A", PhoneNumber = "0601000008", Email = "a@test.com" } };
        var s2 = new ClientService { Client = new Client { FullName = "B", PhoneNumber = "0601000009", Email = "b@test.com" } };
        s1.Save();
        s2.Save();

        var list = new ClientService().GetAll();

        Assert.Equal(2, list.Count);
    }

    // ─── GetById ────────────────────────────────────────────────────────────

    [Fact]
    public void GetClientById_ReturnsCorrectClient()
    {
        var service = new ClientService
        {
            Client = new Client
            {
                FullName = "Find Me",
                PhoneNumber = "0601000010",
                Email = "findme@test.com"
            }
        };
        service.Save();

        var found = service.GetClientById(service.Client.Id);

        Assert.NotNull(found);
        Assert.Equal("Find Me", found.FullName);
    }
}
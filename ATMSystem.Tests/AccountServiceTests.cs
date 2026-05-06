using ATMSystem.Models;
using ATMSystem.Repositories;
using ATMSystem.Services;
using Moq;

namespace ATMSystem.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _service = new AccountService(_mockRepo.Object);
    }

    // login tests

    [Fact]
    public void Login_ValidCredentials_ReturnsAccount()
    {
        var account = new Account { Login = "john", Pin = "12345", Role = "customer" };
        _mockRepo.Setup(r => r.GetByLoginAndPin("john", "12345")).Returns(account);

        var result = _service.Login("john", "12345");

        Assert.NotNull(result);
        Assert.Equal("john", result.Login);
    }

    [Fact]
    public void Login_InvalidCredentials_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByLoginAndPin("john", "wrong")).Returns((Account?)null);

        var result = _service.Login("john", "wrong");

        Assert.Null(result);
    }

    [Fact]
    public void Login_EmptyLogin_ReturnsNull()
    {
        var result = _service.Login("", "12345");
        Assert.Null(result);
    }

    [Fact]
    public void Login_EmptyPin_ReturnsNull()
    {
        var result = _service.Login("john", "");
        Assert.Null(result);
    }

    [Fact]
    public void Login_WhitespaceLogin_ReturnsNull()
    {
        var result = _service.Login("   ", "12345");
        Assert.Null(result);
    }

    // ─── deposit tests

    [Fact]
    public void Deposit_ValidAmount_ReturnsNewBalance()
    {
        _mockRepo.Setup(r => r.GetByLogin("testuser"))
            .Returns(new Account { Login = "testuser", Balance = 100 });

        decimal result = _service.Deposit("testuser", 50);

        Assert.Equal(150, result);
        _mockRepo.Verify(r => r.UpdateBalance("testuser", 150), Times.Once);
    }

    [Fact]
    public void Deposit_ZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Deposit("testuser", 0));
    }

    [Fact]
    public void Deposit_NegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Deposit("testuser", -50));
    }

    [Fact]
    public void Deposit_AccountNotFound_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetByLogin("ghost")).Returns((Account?)null);

        Assert.Throws<InvalidOperationException>(() => _service.Deposit("ghost", 50));
    }

    // ─── withdraw tests

    [Fact]
    public void Withdraw_ValidAmount_ReturnsNewBalance()
    {
        _mockRepo.Setup(r => r.GetByLogin("testuser"))
            .Returns(new Account { Login = "testuser", Balance = 200 });

        decimal result = _service.Withdraw("testuser", 50);

        Assert.Equal(150, result);
        _mockRepo.Verify(r => r.UpdateBalance("testuser", 150), Times.Once);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetByLogin("testuser"))
            .Returns(new Account { Login = "testuser", Balance = 20 });

        Assert.Throws<InvalidOperationException>(() => _service.Withdraw("testuser", 100));
    }

    [Fact]
    public void Withdraw_ZeroAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Withdraw("testuser", 0));
    }

    [Fact]
    public void Withdraw_NegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Withdraw("testuser", -10));
    }

    [Fact]
    public void Withdraw_AccountNotFound_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetByLogin("ghost")).Returns((Account?)null);

        Assert.Throws<InvalidOperationException>(() => _service.Withdraw("ghost", 50));
    }

    [Fact]
    public void Withdraw_ExactBalance_Succeeds()
    {
        _mockRepo.Setup(r => r.GetByLogin("testuser"))
            .Returns(new Account { Login = "testuser", Balance = 100 });

        decimal result = _service.Withdraw("testuser", 100);

        Assert.Equal(0, result);
    }

    // ─── get balance tests

    [Fact]
    public void GetBalance_ValidLogin_ReturnsAccount()
    {
        var account = new Account { Login = "john", Balance = 500 };
        _mockRepo.Setup(r => r.GetByLogin("john")).Returns(account);

        var result = _service.GetBalance("john");

        Assert.Equal(500, result.Balance);
    }

    [Fact]
    public void GetBalance_AccountNotFound_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetByLogin("ghost")).Returns((Account?)null);

        Assert.Throws<InvalidOperationException>(() => _service.GetBalance("ghost"));
    }

    // ─── create account tests

    [Fact]
    public void CreateAccount_ValidAccount_CallsRepoCreate()
    {
        var account = new Account
        {
            Login = "newuser",
            Pin = "11111",
            HolderName = "New User",
            Balance = 100,
        };

        _service.CreateAccount(account);

        _mockRepo.Verify(r => r.Create(account), Times.Once);
    }

    [Fact]
    public void CreateAccount_EmptyLogin_ThrowsArgumentException()
    {
        var account = new Account { Login = "", Pin = "12345", Balance = 0 };

        Assert.Throws<ArgumentException>(() => _service.CreateAccount(account));
    }

    [Fact]
    public void CreateAccount_ShortPin_ThrowsArgumentException()
    {
        var account = new Account { Login = "john", Pin = "123", Balance = 0 };

        Assert.Throws<ArgumentException>(() => _service.CreateAccount(account));
    }

    [Fact]
    public void CreateAccount_NonNumericPin_ThrowsArgumentException()
    {
        var account = new Account { Login = "john", Pin = "abc12", Balance = 0 };

        Assert.Throws<ArgumentException>(() => _service.CreateAccount(account));
    }

    [Fact]
    public void CreateAccount_NegativeBalance_ThrowsArgumentException()
    {
        var account = new Account { Login = "john", Pin = "12345", Balance = -50 };

        Assert.Throws<ArgumentException>(() => _service.CreateAccount(account));
    }

    [Fact]
    public void CreateAccount_SetsDefaultStatusAndRole()
    {
        var account = new Account
        {
            Login = "john",
            Pin = "12345",
            HolderName = "John",
            Balance = 0,
            Status = "",
            Role = "",
        };

        _service.CreateAccount(account);

        Assert.Equal("Active", account.Status);
        Assert.Equal("customer", account.Role);
    }

    // ─── delete account tests

    [Fact]
    public void DeleteAccount_ValidId_ReturnsHolderName()
    {
        var account = new Account { AccountId = 1, HolderName = "John Doe" };
        _mockRepo.Setup(r => r.GetById(1)).Returns(account);

        var result = _service.DeleteAccount(1);

        Assert.Equal("John Doe", result);
        _mockRepo.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteAccount_AccountNotFound_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetById(99)).Returns((Account?)null);

        Assert.Throws<InvalidOperationException>(() => _service.DeleteAccount(99));
    }

 // ─── update account tests

    [Fact]
    public void UpdateAccount_ValidData_CallsRepoUpdate()
    {
        var existing = new Account { AccountId = 1, HolderName = "Old Name" };
        _mockRepo.Setup(r => r.GetById(1)).Returns(existing);

        var updated = new Account
        {
            AccountId = 1,
            HolderName = "New Name",
            Pin = "99999",
            Status = "Active",
        };

        _service.UpdateAccount(updated);

        _mockRepo.Verify(r => r.Update(updated), Times.Once);
    }

    [Fact]
    public void UpdateAccount_AccountNotFound_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetById(99)).Returns((Account?)null);

        var account = new Account { AccountId = 99, Pin = "12345", Status = "Active" };

        Assert.Throws<InvalidOperationException>(() => _service.UpdateAccount(account));
    }

    [Fact]
    public void UpdateAccount_InvalidPin_ThrowsArgumentException()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Account { AccountId = 1 });

        var account = new Account { AccountId = 1, Pin = "abc", Status = "Active" };

        Assert.Throws<ArgumentException>(() => _service.UpdateAccount(account));
    }

    [Fact]
    public void UpdateAccount_InvalidStatus_ThrowsArgumentException()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Account { AccountId = 1 });

        var account = new Account { AccountId = 1, Pin = "12345", Status = "Banana" };

        Assert.Throws<ArgumentException>(() => _service.UpdateAccount(account));
    }

    [Fact]
    public void UpdateAccount_DisabledStatus_Succeeds()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Account { AccountId = 1 });

        var account = new Account { AccountId = 1, Pin = "12345", Status = "Disabled" };

        _service.UpdateAccount(account);

        _mockRepo.Verify(r => r.Update(account), Times.Once);
    }

    // ─── search account tests

    [Fact]
    public void SearchAccount_ValidId_ReturnsAccount()
    {
        var account = new Account { AccountId = 1, HolderName = "John" };
        _mockRepo.Setup(r => r.GetById(1)).Returns(account);

        var result = _service.SearchAccount(1);

        Assert.NotNull(result);
        Assert.Equal("John", result.HolderName);
    }

    [Fact]
    public void SearchAccount_NotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetById(99)).Returns((Account?)null);

        var result = _service.SearchAccount(99);

        Assert.Null(result);
    }
}
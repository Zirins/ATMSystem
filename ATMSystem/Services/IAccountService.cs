using ATMSystem.Models;

namespace ATMSystem.Services
{
    /// <summary>
    /// Defines cash logic operations for the ATM system.
    /// </summary>
    public interface IAccountService
    {
        Account? Login(string login, string pin);

        decimal Withdraw(string login, decimal amount);

        decimal Deposit(string login, decimal amount);

        Account GetBalance(string login);

        void CreateAccount(Account account);

        string DeleteAccount(int id);

        void UpdateAccount(Account account);

        Account? SearchAccount(int id);
    }
}
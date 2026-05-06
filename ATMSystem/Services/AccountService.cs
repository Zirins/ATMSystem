using ATMSystem.Models;
using ATMSystem.Repositories;

namespace ATMSystem.Services
{
    /// <summary>
    /// Implements ATM business logic using an account repository.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository repo;

        public AccountService(IAccountRepository repo)
        {
            this.repo = repo;
        }

        public Account? Login(string login, string pin)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pin))
            {
                return null;
            }

            return this.repo.GetByLoginAndPin(login, pin);
        }

        public decimal Withdraw(string login, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be greater than zero.");
            }

            var account = this.repo.GetByLogin(login)
                ?? throw new InvalidOperationException("Account not found.");

            if (amount > account.Balance)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            decimal newBalance = account.Balance - amount;
            this.repo.UpdateBalance(login, newBalance);
            return newBalance;
        }

        public decimal Deposit(string login, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero.");
            }

            var account = this.repo.GetByLogin(login)
                ?? throw new InvalidOperationException("Account not found.");

            decimal newBalance = account.Balance + amount;
            this.repo.UpdateBalance(login, newBalance);
            return newBalance;
        }

        public Account GetBalance(string login)
        {
            return this.repo.GetByLogin(login)
                ?? throw new InvalidOperationException("Account not found.");
        }

        public void CreateAccount(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Login))
            {
                throw new ArgumentException("Login cannot be empty.");
            }

            if (account.Pin.Length != 5 || !account.Pin.All(char.IsDigit))
            {
                throw new ArgumentException("PIN must be exactly 5 digits.");
            }

            if (account.Balance < 0)
            {
                throw new ArgumentException("Starting balance cannot be negative.");
            }

            account.Status = string.IsNullOrWhiteSpace(account.Status) ? "Active" : account.Status;
            account.Role = string.IsNullOrWhiteSpace(account.Role) ? "customer" : account.Role;

            this.repo.Create(account);
        }

        public string DeleteAccount(int id)
        {
            var account = this.repo.GetById(id)
                ?? throw new InvalidOperationException("Account not found.");

            this.repo.Delete(id);
            return account.HolderName;
        }

        public void UpdateAccount(Account account)
        {
            _ = this.repo.GetById(account.AccountId)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.Pin.Length != 5 || !account.Pin.All(char.IsDigit))
            {
                throw new ArgumentException("PIN must be exactly 5 digits.");
            }

            if (account.Status != "Active" && account.Status != "Disabled")
            {
                throw new ArgumentException("Status must be Active or Disabled.");
            }

            this.repo.Update(account);
        }

        public Account? SearchAccount(int id)
        {
            return this.repo.GetById(id);
        }
    }
}
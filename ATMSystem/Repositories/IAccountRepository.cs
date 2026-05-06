using ATMSystem.Models;

namespace ATMSystem.Repositories
{
    /// <summary>
    /// Defines data access operations for the accounts.
    /// </summary>
    public interface IAccountRepository
    {
        Account? GetByLoginAndPin(string login, string pin);
        Account? GetById(int id);
        Account? GetByLogin(string login);

        void Create(Account account);
        void Delete(int id);
        void Update(Account account);
        void UpdateBalance(string login, decimal newBalance);
    }
}
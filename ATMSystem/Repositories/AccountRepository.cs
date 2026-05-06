using ATMSystem.Models;
using MySql.Data.MySqlClient;

namespace ATMSystem.Repositories
{
    /// <summary>
    /// MySQL implementation of account data access.
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly string connString;

        public AccountRepository(string connString)
        {
            this.connString = connString;
        }

        public Account? GetByLoginAndPin(string login, string pin)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = "SELECT * FROM accounts WHERE login=@login AND pin=@pin";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pin", pin);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapAccount(reader) : null;
        }

        public Account? GetById(int id)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = "SELECT * FROM accounts WHERE account_id=@id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapAccount(reader) : null;
        }

        public Account? GetByLogin(string login)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = "SELECT * FROM accounts WHERE login=@login";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapAccount(reader) : null;
        }

        public void Create(Account account)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = @"INSERT INTO accounts (login, pin, holder_name, balance, status, role)
                                   VALUES (@login, @pin, @holder, @balance, @status, @role)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", account.Login);
            cmd.Parameters.AddWithValue("@pin", account.Pin);
            cmd.Parameters.AddWithValue("@holder", account.HolderName);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@status", account.Status);
            cmd.Parameters.AddWithValue("@role", account.Role);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = "DELETE FROM accounts WHERE account_id=@id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public void Update(Account account)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = @"UPDATE accounts
                                   SET holder_name=@holder, pin=@pin, status=@status
                                   WHERE account_id=@id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@holder", account.HolderName);
            cmd.Parameters.AddWithValue("@pin", account.Pin);
            cmd.Parameters.AddWithValue("@status", account.Status);
            cmd.Parameters.AddWithValue("@id", account.AccountId);

            cmd.ExecuteNonQuery();
        }

        public void UpdateBalance(string login, decimal newBalance)
        {
            using var conn = new MySqlConnection(this.connString);
            conn.Open();

            const string query = "UPDATE accounts SET balance=@balance WHERE login=@login";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@balance", newBalance);
            cmd.Parameters.AddWithValue("@login", login);

            cmd.ExecuteNonQuery();
        }

        private static Account MapAccount(MySqlDataReader reader)
        {
            return new Account
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                Login = reader["login"].ToString() ?? string.Empty,
                Pin = reader["pin"].ToString() ?? string.Empty,
                HolderName = reader["holder_name"].ToString() ?? string.Empty,
                Balance = Convert.ToDecimal(reader["balance"]),
                Status = reader["status"].ToString() ?? string.Empty,
                Role = reader["role"].ToString() ?? string.Empty,
            };
        }
    }
}
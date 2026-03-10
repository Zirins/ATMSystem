using MySql.Data.MySqlClient;

class Program
{

    static string connString = "server=localhost;database=atm_system;uid=root;pwd=74327huHUFDdsf&&98767";

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("ATM SYSTEM LOGIN SCREEN");

            Console.Write("Enter login info: ");
            string login = Console.ReadLine();

            Console.Write("Enter PIN: ");
            string pin = Console.ReadLine();

            Login(login, pin);
        }

    }

    static void Login(string login, string pin)
    {

        try
        {
            using MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();

            string query = "SELECT role, holder_name FROM accounts WHERE login=@login and pin=@pin";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pin", pin);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {

                string role = reader.GetString("role");
                string name = reader.GetString("holder_name");

                Console.WriteLine($"Welcome {name}");

                if (role == "admin")
                {
                    Console.WriteLine("Logged in as ADMIN");
                    AdminMenu();
                }
                else
                {
                    Console.WriteLine("Logged in as CUSTOMER");
                    CustomerMenu(login);
                }

            }
            else
            {
                Console.WriteLine("Invalid login or PIN");
            }

        }


        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        Console.ReadLine();
    }

    static void CustomerMenu(string login)
    {
        while (true)
        {
            Console.WriteLine("CUSTOMER MENU (CHOOSE ONE OF THE 4 HERE)");
            Console.WriteLine("1 - Withdraw Cash");
            Console.WriteLine("2 - Deposit Cash");
            Console.WriteLine("3 - Display Blanace");
            Console.WriteLine("4 - Exit");

            Console.WriteLine("Choose option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    WithdrawCash(login);
                    break;
                case "2":
                    DepositCash(login);
                    break;
                case "3":
                    DisplayBalance(login);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;

            }
        }
    }

    static void DisplayBalance(string login)
    {
        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string query = "SELECT balance, account_id FROM accounts WHERE login=@login";

        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@login", login);

        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            Console.WriteLine("Account #" + reader["account_id"]);
            Console.WriteLine("Balance: " + reader["balance"]);
        }    
    
    }

    static void WithdrawCash(string login)
    {
        Console.Write("Enter withdrawal amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string checkQuery = "SELECT balance FROM accounts WHERE login=@login";
        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("@login", login);

        decimal balance = Convert.ToDecimal(checkCmd.ExecuteScalar());

        if (amount > balance)
        {
            Console.WriteLine("Insufficient funds.");
            return;
        }

        decimal newBalance = balance - amount;

        string updateQuery = "UPDATE accounts SET balance=@balance WHERE login=@login";
        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);

        updateCmd.Parameters.AddWithValue("@balance", newBalance);
        updateCmd.Parameters.AddWithValue("@login", login);

        updateCmd.ExecuteNonQuery();

        Console.WriteLine("Cash Successfully Withdrawn");
        Console.WriteLine("Withdrawn: " + amount);
        Console.WriteLine("New Balance: " + newBalance);
    }

    static void DepositCash(string login)
    {
        Console.Write("Enter deposit amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string checkQuery = "SELECT balance FROM accounts WHERE login=@login";
        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("@login", login);

        decimal balance = Convert.ToDecimal(checkCmd.ExecuteScalar());

        decimal newBalance = balance + amount;

        string updateQuery = "UPDATE accounts SET balance=@balance WHERE login=@login";
        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);

        updateCmd.Parameters.AddWithValue("@balance", newBalance);
        updateCmd.Parameters.AddWithValue("@login", login);

        updateCmd.ExecuteNonQuery();

        Console.WriteLine("Cash Deposited Successfully");
        Console.WriteLine("Deposited: " + amount);
        Console.WriteLine("New Balance: " + newBalance);
    }


    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine("\nADMIN MENU");
            Console.WriteLine("1 - Create New Account");
            Console.WriteLine("2 - Delete Account");
            Console.WriteLine("3 - Update Account");
            Console.WriteLine("4 - Search Account");
            Console.WriteLine("5 - Exit");

            Console.Write("Choose option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateAccount();
                    break;

                case "2":
                    DeleteAccount();
                    break;

                case "3":
                    UpdateAccount();
                    break;

                case "4":
                    SearchAccount();
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Invalid option!! Enter a real option");
                    break;
            }
        }
    }

    static void CreateAccount()
    {
        Console.Write("Login: ");
        string login = Console.ReadLine();

        Console.Write("Pin Code (5 digits): ");
        string pinInput = Console.ReadLine();

        if (pinInput.Length != 5 || !int.TryParse(pinInput, out int pin))
        {
            Console.WriteLine("PIN must be exactly 5 digits.");
            return;
        }

        Console.Write("Holder Name: ");
        string holder = Console.ReadLine();

        Console.Write("Starting Balance: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
        {
            Console.WriteLine("Invalid balance amount.");
            return;
        }

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string query = "INSERT INTO accounts (login,pin,holder_name,balance,status,role) VALUES (@login,@pin,@holder,@balance,'Active','customer')";

        MySqlCommand cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@login", login);
        cmd.Parameters.AddWithValue("@pin", pin);
        cmd.Parameters.AddWithValue("@holder", holder);
        cmd.Parameters.AddWithValue("@balance", balance);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Account Successfully Created");
    }

    static void DeleteAccount()
    {
        Console.Write("Enter account number to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid account number.");
            return;
        }

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string checkQuery = "SELECT holder_name FROM accounts WHERE account_id=@id";

        MySqlCommand cmd = new MySqlCommand(checkQuery, conn);
        cmd.Parameters.AddWithValue("@id", id);

        object result = cmd.ExecuteScalar();

        if (result == null)
        {
            Console.WriteLine("Account not found!!!");
            return;
        }

        Console.WriteLine($"You want to delete the account held by {result}");
        Console.Write("Enter the account number you wish to delete again to confirm: ");

        if (!int.TryParse(Console.ReadLine(), out int confirmId) || confirmId != id)
        {
            Console.WriteLine("Confirmation failed.");
            return;
        }
        string deleteQuery = "DELETE FROM accounts WHERE account_id=@id";
        MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
        deleteCmd.Parameters.AddWithValue("@id", id);

        deleteCmd.ExecuteNonQuery();

        Console.WriteLine("Account Deleted Successfully!!");
    }

    static void SearchAccount()
    {
        Console.Write("Enter account number: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid account number.");
            return;
        }

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string query = "SELECT * FROM accounts WHERE account_id=@id";

        MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            Console.WriteLine("\nAccount #" + reader["account_id"]);
            Console.WriteLine("Holder: " + reader["holder_name"]);
            Console.WriteLine("Balance: " + reader["balance"]);
            Console.WriteLine("Status: " + reader["status"]);
            Console.WriteLine("Login: " + reader["login"]);
            Console.WriteLine("Pin: " + reader["pin"]);
        }
        else
        {
            Console.WriteLine("Account not found");
        }
    }
    static void UpdateAccount()
    {
        Console.Write("Enter account number: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid account number.");
            return;
        }

        Console.Write("New Holder Name: ");
        string holder = Console.ReadLine();

        Console.Write("New PIN: ");
        string pinInput = Console.ReadLine();

        if (pinInput.Length != 5 || !int.TryParse(pinInput, out int pin))
        {
            Console.WriteLine("PIN must be exactly 5 digits.");
            return;
        }

        Console.Write("New Status (Active/Disabled): ");
        string status = Console.ReadLine();

        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string query = "UPDATE accounts SET holder_name=@holder,pin=@pin,status=@status WHERE account_id=@id";

        MySqlCommand cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@holder", holder);
        cmd.Parameters.AddWithValue("@pin", pin);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@id", id);

        int rows = cmd.ExecuteNonQuery();

        if (rows > 0)
            Console.WriteLine("Account Updated Successfully!!");
        else
            Console.WriteLine("Account not found");
    }
}
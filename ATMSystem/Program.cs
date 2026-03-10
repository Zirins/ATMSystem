using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;

class Program
{

    static string connString = "server=localhost;database=atm_system;uid=root;pwd=74327huHUFDdsf&&98767";

    static void Main()
    {
        Console.WriteLine("ATM SYSTEM LOGIN SCREEN");

        Console.Write("Enter login info: ");
        string login = Console.ReadLine();

        Console.Write("Enter PIN: ");
        string pin = Console.ReadLine();

        Login(login, pin);

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
        decimal amount = decimal.Parse(Console.ReadLine());

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
        decimal amount = decimal.Parse(Console.ReadLine());

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
                    // DeleteAccount();
                    break;

                case "3":
                    // UpdateAccount();
                    break;

                case "4":
                    // SearchAccount();
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
        int pin = int.Parse(Console.ReadLine());

        Console.Write("Holder Name: ");
        string holder = Console.ReadLine();

        Console.Write("Starting Balance: ");
        decimal balance = decimal.Parse(Console.ReadLine());

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
}
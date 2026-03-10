using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using MySql.Data.MySqlClient;

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
                    Console.WriteLine("Deposit feature coming next...");
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
        Console.Write("Enter withdrawl amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());


        using MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();

        string checkQuery = "SELECT balance FROM accounts WHEREE login=@login";
        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("@login", login);


    }

}
using System;
using System.Diagnostics.Eventing.Reader;
using MySql.Data.MySqlClient;

class Program
{

    string connString = "server=localhost;database=atm_system;uid=root;pwd=74327huHUFDdsf&&98767";

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

            string query = "SELECT role, holder_name FROM accounts WHERE login=l@login and pin=@pin";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pin", pin);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {

                string role = reader.GetString("role");
                string name = reader.GetString("holder_name");


            }

        }




    }

}
using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connString = "server=localhost;database=atm_system;uid=root;pwd=74327huHUFDdsf&&98767";

        MySqlConnection conn = new MySqlConnection(connString);

        try
        {
            conn.Open();
            Console.WriteLine("Database Connected Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Connection Failed: " + ex.Message);
        }

        Console.ReadLine();
    }
}
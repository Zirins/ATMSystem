using ATMSystem.Models;
using ATMSystem.Services;
using System.Diagnostics.CodeAnalysis;


namespace ATMSystem.UI
{
    /// <summary>
    /// Handles all console input/output for the ATM system.
    /// </summary>
    ///     [ExcludeFromCodeCoverage]
    public class ConsoleUI
    {
        private readonly IAccountService _service;

        /// <summary>
        /// Initializes a new instance of <see cref="ConsoleUI"/>.
        /// </summary>
        /// <param name="service">The account service to use.</param>
        public ConsoleUI(IAccountService service)
        {
            _service = service;
        }

        /// <summary>Starts the ATM application loop.</summary>
        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\n=== ATM SYSTEM LOGIN ===");
                Console.Write("Enter login: ");
                string login = Console.ReadLine() ?? string.Empty;

                Console.Write("Enter PIN: ");
                string pin = Console.ReadLine() ?? string.Empty;

                try
                {
                    var account = _service.Login(login, pin);

                    if (account == null)
                    {
                        Console.WriteLine("Invalid login or PIN.");
                        continue;
                    }

                    Console.WriteLine($"Welcome, {account.HolderName}!");

                    if (account.Role == "admin")
                        AdminMenu();
                    else
                        CustomerMenu(login);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void CustomerMenu(string login)
        {
            while (true)
            {
                Console.WriteLine("\n=== CUSTOMER MENU ===");
                Console.WriteLine("1 - Withdraw Cash");
                Console.WriteLine("2 - Deposit Cash");
                Console.WriteLine("3 - Display Balance");
                Console.WriteLine("4 - Exit");
                Console.Write("Choose option: ");

                switch (Console.ReadLine())
                {
                    case "1": WithdrawCash(login);   break;
                    case "2": DepositCash(login);    break;
                    case "3": DisplayBalance(login); break;
                    case "4": return;
                    default:  Console.WriteLine("Invalid option."); break;
                }
            }
        }

        private void WithdrawCash(string login)
        {
            Console.Write("Enter withdrawal amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            try
            {
                decimal newBalance = _service.Withdraw(login, amount);
                Console.WriteLine("Cash Successfully Withdrawn.");
                Console.WriteLine($"Withdrawn: {amount:C}");
                Console.WriteLine($"New Balance: {newBalance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DepositCash(string login)
        {
            Console.Write("Enter deposit amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            try
            {
                decimal newBalance = _service.Deposit(login, amount);
                Console.WriteLine("Cash Deposited Successfully.");
                Console.WriteLine($"Deposited: {amount:C}");
                Console.WriteLine($"New Balance: {newBalance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DisplayBalance(string login)
        {
            try
            {
                var account = _service.GetBalance(login);
                Console.WriteLine($"\nAccount #{account.AccountId}");
                Console.WriteLine($"Date: {DateTime.Now:MM/dd/yyyy}");
                Console.WriteLine($"Balance: {account.Balance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== ADMIN MENU ===");
                Console.WriteLine("1 - Create New Account");
                Console.WriteLine("2 - Delete Account");
                Console.WriteLine("3 - Update Account");
                Console.WriteLine("4 - Search Account");
                Console.WriteLine("5 - Exit");
                Console.Write("Choose option: ");

                switch (Console.ReadLine())
                {
                    case "1": CreateAccount();  break;
                    case "2": DeleteAccount();  break;
                    case "3": UpdateAccount();  break;
                    case "4": SearchAccount();  break;
                    case "5": return;
                    default:  Console.WriteLine("Invalid option."); break;
                }
            }
        }

        private void CreateAccount()
        {
            Console.Write("Login: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("PIN (5 digits): ");
            string pin = Console.ReadLine() ?? string.Empty;

            Console.Write("Holder Name: ");
            string holder = Console.ReadLine() ?? string.Empty;

            Console.Write("Starting Balance: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
            {
                Console.WriteLine("Invalid balance.");
                return;
            }

            try
            {
                _service.CreateAccount(new Account
                {
                    Login      = login,
                    Pin        = pin,
                    HolderName = holder,
                    Balance    = balance,
                    Status     = "Active",
                    Role       = "customer",
                });
                Console.WriteLine("Account Successfully Created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteAccount()
        {
            Console.Write("Enter account number to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            try
            {
                var account = _service.SearchAccount(id);
                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    return;
                }

                Console.WriteLine($"You wish to delete the account held by {account.HolderName}.");
                Console.Write("Re-enter account number to confirm: ");

                if (!int.TryParse(Console.ReadLine(), out int confirmId) || confirmId != id)
                {
                    Console.WriteLine("Confirmation failed.");
                    return;
                }

                _service.DeleteAccount(id);
                Console.WriteLine("Account Deleted Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void UpdateAccount()
        {
            Console.Write("Enter account number: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            Console.Write("New Holder Name: ");
            string holder = Console.ReadLine() ?? string.Empty;

            Console.Write("New PIN (5 digits): ");
            string pin = Console.ReadLine() ?? string.Empty;

            Console.Write("New Status (Active/Disabled): ");
            string status = Console.ReadLine() ?? string.Empty;

            try
            {
                _service.UpdateAccount(new Account
                {
                    AccountId  = id,
                    HolderName = holder,
                    Pin        = pin,
                    Status     = status,
                });
                Console.WriteLine("Account Updated Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SearchAccount()
        {
            Console.Write("Enter account number: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            var account = _service.SearchAccount(id);

            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine($"\nAccount #{account.AccountId}");
            Console.WriteLine($"Holder: {account.HolderName}");
            Console.WriteLine($"Balance: {account.Balance:C}");
            Console.WriteLine($"Status: {account.Status}");
            Console.WriteLine($"Login: {account.Login}");
            Console.WriteLine($"PIN: {account.Pin}");
        }
    }
}
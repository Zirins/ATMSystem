using System.Diagnostics.CodeAnalysis;
using ATMSystem.Repositories;
using ATMSystem.Services;
using ATMSystem.UI;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main()
    {
        try
        {
            const string connString = "server=localhost;database=atm_system;uid=root;pwd=74327huHUFDdsf&&98767";

            IAccountRepository repository = new AccountRepository(connString);
            IAccountService service = new AccountService(repository);
            var ui = new ConsoleUI(service);

            ui.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
    }
}
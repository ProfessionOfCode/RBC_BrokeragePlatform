namespace RBC_BrokeragePlatform.Launcher
{
    internal class Program
    {
        private const string WPF_APP_NAME = "RBC.BrokeragePlatform.WPF.exe";
        private const string WEBAPI_APP_NAME = "RBC.BrokeragePlatform.WebAPI.exe";

        static void Main(string[] args)
        {
            Console.WriteLine("Launching RBC Brokerage Platform...");

            try
            {
                // Launch WebAPI application
                var webApiProcess = new System.Diagnostics.Process();
                webApiProcess.StartInfo.FileName = Path.Combine("webApi",WEBAPI_APP_NAME);
                webApiProcess.StartInfo.UseShellExecute = true;
                webApiProcess.Start();

                Console.WriteLine($"\nWeb API application `{WEBAPI_APP_NAME}` launched successfully.");

                // Launch the WPF application
                var wpfProcess = new System.Diagnostics.Process();
                wpfProcess.StartInfo.FileName = Path.Combine("wpf", WPF_APP_NAME);
                wpfProcess.StartInfo.UseShellExecute = true;
                wpfProcess.Start();

                Console.WriteLine($"\nWindows Presentation Foundation (WPF) application `{WPF_APP_NAME}` launched successfully.");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);

            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}

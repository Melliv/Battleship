using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApp
{
    public class Program
    {

        public static void Main(string[] args)
        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("et-EE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("et-EE");
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
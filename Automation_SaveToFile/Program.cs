using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class EquityModelAutomation
{

    public string symbol { set; get; }

    public string stockName { set; get; }

    public decimal buyATPrice { set; get; }

    public decimal? buyATChange { set; get; }

    public decimal sellATPrice { set; get; }

    public decimal currentPrice { set; get; }


    public decimal currentChange { set; get; }


    public string stockCode { set; get; }


    public int qty { set; get; }


    public bool IsBuy { set; get; }


    public bool IsSell { set; get; }


    public int bgcolor { set; get; }


    public string match { set; get; }



    public int bullishCount { set; get; }



    public int bearishCount { set; get; }


    public DateTime lttDateTime { set; get; }

    public string data { set; get; }

    public int bullishCount_100 { set; get; }

    public int bullishCount_95 { set; get; }

    public decimal volumeDifferecne { get; set; }

    public decimal? triggredPrice { set; get; }
    public DateTime triggredLtt { set; get; }

    public override string ToString()
    {

        return string.Format("{0} | {1} | {2} | {3} | {4} |{5} |{6} | {7} | {8} |{9} | {10} | {11} | {12} @||@",
            this.symbol ,  this.bullishCount_95,this.bullishCount_100, this.bullishCount, this.bearishCount, this.lttDateTime.ToString(), this.match?.ToString() ?? "", this.data?.ToString() ?? "", this.stockName?.ToString() ?? "", this.stockCode?.ToString() ?? "", this.volumeDifferecne, this.triggredPrice?.ToString() ?? "", !string.IsNullOrEmpty(this.triggredLtt.ToString()) ? this.triggredLtt.ToLongDateString() : "");
    }
}

class Program
{


    //https://localhost:5001
    static StringBuilder stringBuilder = new StringBuilder();

    static async Task Main(string[] args)
    {

        //var builder = new ConfigurationBuilder() // .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
        //      .SetBasePath(Directory.GetCurrentDirectory())
        //      .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true);

        //IConfiguration config = builder.Build();

        var HUbUrl = "";// config.GetSection("appSettings:url").Value;


        string arg = "0";
        if (args.Any())
            arg = args[0];

        Console.WriteLine(arg);


        switch (Convert.ToInt16(arg))
        {
            case 0:

                HUbUrl = "http://localhost:90/BreezeOperation";
                break;
            case 1:

                HUbUrl = "http://localhost:91/BreezeOperation";
                break;
            case 2:

                HUbUrl = "http://localhost:92/BreezeOperation";
                break;
            case 3:

                HUbUrl = "http://localhost:99/BreezeOperation";
                break;
            case 4:

                HUbUrl = "http://localhost:49/BreezeOperation";
                break;
        }


        // HUbUrl = "https://localhost:7189/breezeOperation";
       //  HUbUrl = "https://localhost:7189/breezeOperation";
        Console.WriteLine(HUbUrl);

        string filename = @"C:\Hosts\Files\" + DateTime.Now.Date.ToShortDateString() + "_Automation" + "_" + arg + ".txt";
        if (!System.IO.File.Exists(filename))
        {
            System.IO.File.WriteAllText(filename, Environment.NewLine);
        }

        await using var connection = new HubConnectionBuilder().WithUrl(HUbUrl).WithAutomaticReconnect().Build();
        connection.KeepAliveInterval = TimeSpan.FromSeconds(60);

        await connection.StartAsync();

        Console.WriteLine(connection.ConnectionId);

        connection.On<string>("ExportBuyStockAlterFromAPP_IND_Filesave", async param =>
        {

           // Console.WriteLine(param);
            RunFileSave(param, Convert.ToInt16(arg));
        });

        connection.Closed += async (exception) =>
        {
            Console.WriteLine(exception);
        };


        string s = "";
        while (s != "kill")
            s = Console.ReadLine();

    }

    private static void RunFileSave(string param, int arg)
    {
        try
        {
            //Initialize SDK



            // connection.ServerTimeout.Add(TimeSpan.FromMinutes(120));





            var livedata = System.Text.Json.JsonSerializer.Deserialize<List<EquityModelAutomation>>(param, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }).FirstOrDefault();

            string filename = @"C:\Hosts\Files\" + DateTime.Now.Date.ToShortDateString() + "_Automation" + "_" + arg + ".txt";

            //Console.WriteLine(filename);
            var count = Regex.Matches(stringBuilder.ToString(), Environment.NewLine).Count();
            stringBuilder.AppendLine((livedata.ToString()));

            TimeSpan end = TimeSpan.Parse("09:16");   // 2 AM
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (count > 1000 | now <= end)
            {
                System.IO.File.AppendAllLines(filename, new[] { stringBuilder.ToString() });
                stringBuilder = new StringBuilder();
                Console.Clear();
            }
            //Thread.Sleep(1000);
            //System.IO.File.WriteAllText(filename, "");

            //File.AppendAllLines(filename, new[] { livedata.ToString() });
            //dynamic livedata = JsonSerializer.Deserialize<dynamic>(param);


            // Callback to receive ticks.





        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }

}
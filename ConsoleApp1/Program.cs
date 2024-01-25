using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using ConsoleApp1;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace ConsoleApp;



public static class Program
{


    public static void GetCandlepattern()
    {

        var groupbysymbol = GetQuotesFromFeed().GroupBy(x => x.symbol).ToList();//.Where(x=>x.Key.ToString()== "1.1!500336");
        foreach (var item in groupbysymbol)
        {
            IEnumerable<Quote> resulst = item.Select(x => new Quote { Close = x.Close, Open = x.Open, Date = x.Date, High = x.High, Low = x.Low, Volume = x.Volume }).OrderByDescending(x=>x.Date).ToList().Take(10).ToList();
            IEnumerable<CandleResult> results111 =
resulst.GetMarubozu(100).Where(x => x.Match == Match.BullSignal);
            IEnumerable<VolatilityStopResult> results =
  resulst.GetVolatilityStop(5, 3);

            IEnumerable<SlopeResult> resultsss =
  resulst.GetSlope(10);

            IEnumerable<RsiResult> resultssss
  = resulst
    .GetObv()
    .GetRsi(5);

            IEnumerable<ElderRayResult> results3 =
                        resulst.GetElderRay(5);

            IEnumerable<SlopeResult> results4 =
           resulst.GetSlope(5);
            if (resultssss.Any(x => x.Rsi >= 100) &&  results4.Any(x=>x.RSquared >0.90) && results4.Any(x => x.Slope > 2))
            {
                Console.WriteLine(item.Key);

                IEnumerable<HtlResult> results11 =
          resulst.GetHtTrendline();

                ;
             

                //      Console.WriteLine(item.Key + " " + string.Join(',', (results111.Select(x => x.Price))));
                //      if (results111.Any())
                //      {
                //          IEnumerable<AtrStopResult> results1 = resulst.GetAtrStop(21, 3, EndType.HighLow);

                //          IEnumerable<ElderRayResult> results3 =
                //        resulst.GetElderRay(5);
                //          IEnumerable<SlopeResult> results4 =
                //        resulst.GetSlope(5).Condense();

                //          IEnumerable<CandleResult> resultsDOJI =
                //          resulst.GetDoji(0.3);

                //          IEnumerable<HtlResult> results1111 =
                //        resulst.GetHtTrendline();
                //          IEnumerable<CandleResult> results12211 =
                //    resulst.GetMarubozu(95);//.RemoveWarmupPeriods(1);

                //          IEnumerable<SuperTrendResult> resulstssss =
                //    resulst.GetSuperTrend(5, 4);

                //          IEnumerable<RsiResult> ressssults =
                //    resulst.Use(CandlePart.HL2).GetRsi(3);//.GetPmo(5, 5, 5);

                //          IEnumerable<VwmaResult> resultssssss =
                //          resulst.GetVwma(5);

                //          IEnumerable<StdDevResult> resulssssts =
                //    resulst.GetStdDev(5, 1);
                //          var resultsssssss = resulst
                //      .GetAtr()
                //      .GetSlope(5);

                string filename = @"C:\Hosts\Files\Test" + DateTime.Now.Date.ToShortDateString() + ".txt";

                System.IO.File.AppendAllText(filename, Environment.NewLine);
                System.IO.File.AppendAllText(filename, DateTime.Now.ToShortTimeString());
                System.IO.File.AppendAllText(filename, item.Key + " " + string.Join(',', (results111.Select(x => x.Price))));

            }

        }


    }











    public static void Main()
    {

        GetCandlepattern();
    }

    // Place function in here..




    // fetch historical quotes from data provider

    //      IEnumerable<Quote> quotes = GetQuotesFromFeed();
    //      // calculate 10-period SMA
    //      IEnumerable<SmaResult> results = quotes.GetSma(20);

    //      IEnumerable<AtrStopResult> results1 = quotes.GetAtrStop(21, 3, EndType.HighLow);


    //      IEnumerable<ElderRayResult> results3 =
    //quotes.GetElderRay(5);

    //      IEnumerable<EmaResult> results2 = quotes.GetEma(10);


    //      IEnumerable<BetaResult> results34 = quotes
    //.GetBeta(quotes, 5, BetaType.Standard);
    //      IEnumerable<SlopeResult> results4 =
    //quotes.GetSlope(5).Condense();

    //      IEnumerable<HtlResult> results11 =
    //quotes.GetHtTrendline();

    //      IEnumerable<CandleResult> results222 =
    //quotes.GetDoji(0.2);

    //      IEnumerable<CandleResult> results111 =
    //quotes.GetMarubozu(95);//.RemoveWarmupPeriods(1);


    //      var resultssss = quotes
    //  .GetBaseQuote(CandlePart.OHLC4)
    //  .GetRsi(5);

    //      IEnumerable<SuperTrendResult> resulstssss =
    //quotes.GetSuperTrend(5, 4);

    //      IEnumerable<VwmaResult> resultssssss =
    //quotes.GetVwma(5);

    //      IEnumerable<StdDevResult> resulssssts =
    //quotes.GetStdDev(5, 1);
    //      var resultsssssss = quotes
    //  .GetAtr()
    //  .GetSlope(5);

    //      IEnumerable<RsiResult> ressssults =
    //quotes.Use(CandlePart.HL2).GetRsi(5);//.GetPmo(5, 5, 5);

    //      //      IEnumerable<PivotsResult> resusssslts =
    //      //quotes.GetPivots(20, 20, 5, EndType.HighLow);

    //      List<StochRsiResult> resultsListssss =
    //quotes
    //.GetStochRsi(5, 5, 3, 1)
    //.ToList();

    //      // show results
    //      Console.WriteLine("SMA Results ---------------------------");

    //      foreach (SmaResult r in results.TakeLast(1000))
    //      {
    //          // only showing last 10 records for brevity
    //          Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}");
    //      }

    //      // optionally, you can lookup individual values by date
    //      DateTime lookupDate = DateTime
    //          .Parse("2021-08-12T17:08:17.9746795+02:00", CultureInfo.InvariantCulture);

    //      double? specificSma = results.Find(lookupDate).Sma;

    //      Console.WriteLine();
    //      Console.WriteLine("SMA on Specific Date ------------------");
    //      Console.WriteLine($"SMA on {lookupDate:u} was ${specificSma:N3}");

    //      // analyze results (compare to quote values)
    //      Console.WriteLine();
    //      Console.WriteLine("SMA Analysis --------------------------");

    //      /************************************************************
    //        Results are usually returned with the same number of
    //        elements as the provided quotes; see individual indicator
    //        docs for more information.

    //        As such, converting to List means they can be indexed
    //        with the same ordinal position.
    //       ************************************************************/

    //      List<Quote> quotesList = quotes
    //          .ToList();

    //      List<SmaResult> resultsList = results
    //          .ToList();

    //      for (int i = quotesList.Count - 25; i < quotesList.Count; i++)
    //      {
    //          // only showing ~25 records for brevity

    //          Quote q = quotesList[i];
    //          SmaResult r = resultsList[i];

    //          bool isBullish = (double)q.Close > r.Sma;

    //          Console.WriteLine($"SMA on {r.Date:u} was ${r.Sma:N3}"
    //                            + $" and Bullishness is {isBullish}");
    //      }


    public static DataTable CopyToSQL(string lines)
    {

        var dt = new DataTable();

        for (int index = 0; index < 25; index++)
            dt.Columns.Add(new DataColumn());

        foreach (var line in lines.Split(new[] { "\r\n" }, StringSplitOptions.None))
        {
            try
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var cols = line.Split(',');

                    DataRow dr = dt.NewRow();
                    for (int cIndex = 0; cIndex < 25; cIndex++)
                    {
                        dr[cIndex] = cols[cIndex];
                    }

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {

                // throw;
            }
        }



        return dt;

    }

    private static Collection<QuoteWithSymbol> GetQuotesFromFeed()
    {

        string line;


        using var fileStream = new FileStream(@"C:\Hosts\Files\02-01-2024.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var textReader = new StreamReader(fileStream);

        var text = textReader.ReadToEnd();
        textReader.Close();
        // Read the file and display it line by line.
        //System.IO.StreamReader file =
        //    new System.IO.StreamReader();

        string readallline = text;
        var dt = CopyToSQL(readallline);

        List<DataRow> rows = dt.Rows.Cast<DataRow>().ToList().ToList();

        var list = rows.ToList().Select(x => new QuoteWithSymbol
        {
            Open = Convert.ToDecimal(x.ItemArray[1]),
            Close = Convert.ToDecimal(x.ItemArray[2]),
            Low = Convert.ToDecimal(x.ItemArray[4]),
            High = Convert.ToDecimal(x.ItemArray[3]),
            Volume = Convert.ToDecimal(x.ItemArray[16]),
            Date = Convert.ToDateTime(x.ItemArray[20]),
            symbol = x.ItemArray[0].ToString(),


        });
        //Collection<Quote> quotes = (from DataRow row in dt.Rows

        //      select new Quote
        //      {
        //          //Close = row["FirstName"].ToString(),
        //          //_LastName = row["Last_Name"].ToString()

        //      }).ToList();

        /************************************************************

         We're mocking a data provider here by simply importing a
         JSON file, a similar format of many public APIs.

         This approach will vary widely depending on where you are
         getting your quote history.

         See https://github.com/DaveSkender/Stock.Indicators/discussions/579
         for free or inexpensive market data providers and examples.

         The return type of IEnumerable<Quote> can also be List<Quote>
         or ICollection<Quote> or other IEnumerable compatible types.

         ************************************************************/

        //string json = File.ReadAllText("quotes.data.json");

        //Collection<Quote> quotes = JsonConvert.DeserializeObject<IReadOnlyCollection<Quote>>(json)
        //    .ToSortedCollection();

        return list.ToSortedCollection();
    }
}
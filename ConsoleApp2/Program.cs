using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using ConsoleApp1;
using ConsoleApp2;
using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace Backtest;


public static class Program
{
    public static void Main()
    {
        string Date = "15-01-2024.txt";
        // See ConsoleApp first.  This is more advanced.

        /* This is a basic 20-year backtest-style analysis of
         * Stochastic RSI.  It will buy-to-open (BTO) one share
         * when the Stoch RSI (%K) is below 20 and crosses over the
         * Signal (%D). The reverse Sell-to-Close (STC) and
         * Sell-To-Open (STO) occurs when the Stoch RSI is above 80 and
         * crosses below the Signal.
         *
         * As a result, there will always be one open LONG or SHORT
         * position that is opened and closed at signal crossover
         * points in the overbought and oversold regions of the indicator.
         */

        // fetch historical quotes from data provider

        var groupbysymbol = SendAllStocksForLoad("1.1!541956").GroupBy(x => x.symbol).ToList(); // GetQuotesFromFeed().GroupBy(x => x.symbol).ToList();//.Where(x=>x.Key.ToString()== "1.1!500336");
        foreach (var item in groupbysymbol)
        {

            string filename = @"C:\Hosts\Files\back_Test" + DateTime.Now.Date.ToShortDateString() + ".txt";
            List<Quote> quotesList = item.Select(x => new Quote { Close = x.Close, Open = x.Open, Date = x.Date, High = x.High, Low = x.Low, Volume = x.Volume }).OrderBy(x => x.Date).ToList();
            IEnumerable<CandleResult> candleResult = quotesList.GetDoji(0.2);

            IEnumerable<MacdResult> macdresult = quotesList.GetMacd();

            IEnumerable<VolatilityStopResult> Volatilityresults = quotesList.GetVolatilityStop(5, 3);

            //IEnumerable<SlopeResult> resultsss = quotesList.GetSlope(10);

            IEnumerable<RsiResult> rsiResults = quotesList.GetObv().GetRsi(5);


            var results = (from x in candleResult
                           join y in macdresult on x.Date equals y.Date
                           join z in rsiResults on x.Date equals z.Date
                           join a in quotesList.ToList() on x.Date equals a.Date
                           select new
                           {
                               x.Date,
                               Match = string.IsNullOrEmpty(x.Match.ToString()) ? "______" : x.Match.ToString(),
                               x.Candle,
                               Price = x.Price.HasValue ? x.Price.Value.ToString("N2") : "______",
                               Macd = y.Macd.HasValue ? y.Macd.Value.ToString("N2") : "______",

                               FastEma = y.FastEma.HasValue ? y.FastEma.Value.ToString("N2") : "______",

                               SlowEma = y.SlowEma.HasValue ? y.SlowEma.Value.ToString("N2") : "______",
                               Signal = y.Signal.HasValue ? y.Signal.Value.ToString("N2") : "______",

                               Histogram = y.Histogram.HasValue ? y.Histogram.Value.ToString("N2") : "______",
                               Rsi = z.Rsi.HasValue ? z.Rsi.Value.ToString("N2") : "______",
                               Open = a.Open,
                               Change = a.Close,
                               Volume=a.Volume

                           }
                           );
            
            string filenamenew = @"C:\Hosts\Files\"+ Date + "_new" + DateTime.Now.Date.ToShortDateString() + ".txt";
            System.IO.File.AppendAllText(filenamenew, item.Key + " " + "Date" + " " +
                      $"Time" + " " +
                      $"Match" + " " +
                      $"Open" + " " +
                      $"Change" + " " +
                      $"Volume" + " " +
                      $"IsBearish" + " " +
                      $"IsBullish" + " " +
                      $"Price" + " " +
                      $"Macd" + " " +
                      $"FastEma" + " " +
                      $"SlowEma" + " " +
                      $"Signal" + " " +
                      $"Histogram" + " " +
                      $"Rsi");
            System.IO.File.AppendAllText(filenamenew, Environment.NewLine);
            foreach (var xyz in results)
            {


                System.IO.File.AppendAllText(filenamenew, 
                      item.Key + " " +
                      $"{xyz.Date} " +
                      $"{xyz.Match} " +
                      $"{xyz.Open} " +
                      $"{xyz.Change} " +
                       $"{xyz.Volume} " +
                      $"{xyz.Candle.IsBearish.ToString() ?? "-"} " +
                      $"{xyz.Candle.IsBullish.ToString() ?? "-"} " +
                      $"{xyz.Price.ToString() ?? "-"} " +
                      $"{xyz.Macd.ToString() ?? "-"} " +
                      $"{xyz.FastEma.ToString() ?? "-"} " +
                      $"{xyz.SlowEma.ToString() ?? "-"} " +
                      $"{xyz.Signal.ToString() ?? "-"} " +
                      $"{xyz.Histogram.ToString() ?? "-"} " +
                      $"{xyz.Rsi.ToString() ?? "-"} ");

                System.IO.File.AppendAllText(filenamenew, Environment.NewLine);
                //$"{xyz.StochRsi,7:N1}" +
                //$"{xyz.Signal,7:N1}" +
                //$"{cross,7}" +
                //$"{rlzGain + trdGain,13:c2}");
            }

            // IEnumerable<ElderRayResult> results3 = quotesList.GetElderRay(5);




            System.IO.File.AppendAllText(filename, candleResult.ToString());
            System.IO.File.AppendAllText(filename, Volatilityresults.ToString());
            System.IO.File.AppendAllText(filename, rsiResults.ToString());


            //  List<QuoteWithSymbol> quotesList = GetQuotesFromFeed().Where(x => x.symbol.ToString() == "1.1!500336").ToList();

            // calculate Stochastic RSI
            List<StochRsiResult> resultsList =
            quotesList
                .GetStochRsi(14, 14, 3)
                .ToList();

            // initialize
            decimal trdPrice = 0;
            decimal trdQty = 0;
            decimal rlzGain = 0;

            Console.WriteLine(item.Key.ToString());
            Console.WriteLine("   Date         Close  StRSI Signal  Cross    Net Gains");
            Console.WriteLine("______________________________________________________________________________");

            // roll through history
            for (int i = 1; i < quotesList.Count; i++)
            {

                Quote q = quotesList[i];
                StochRsiResult e = resultsList[i]; // evaluation period
                StochRsiResult l = resultsList[i - 1]; // last (prior) period
                string cross = string.Empty;

                // unrealized gain on open trade
                decimal trdGain = trdQty * (q.Close - trdPrice);

                // check for LONG event
                // condition: Stoch RSI was <= 20 and Stoch RSI crosses over Signal
                if (l.StochRsi <= 20
                 && l.StochRsi < l.Signal
                 && e.StochRsi >= e.Signal
                 && trdQty != 1)
                {
                    // emulates BTC + BTO  buy-to-open (BTO)
                    rlzGain += trdGain;
                    trdQty = 1;
                    trdPrice = q.Close;
                    cross = "LONG";
                }

                // check for SHORT event
                // condition: Stoch RSI was >= 80 and Stoch RSI crosses under Signal
                else if (l.StochRsi >= 80
                 && l.StochRsi > l.Signal
                 && e.StochRsi <= e.Signal
                 && trdQty != -1)
                {
                    // emulates STC + STO
                    rlzGain += trdGain;
                    trdQty = -1;
                    trdPrice = q.Close;
                    cross = "SHORT";
                }

                if (cross != string.Empty)
                {
                    Console.WriteLine(
                        $"{q.Date} " +
                        $"{q.Close,10:c2}" +
                        $"{e.StochRsi,7:N1}" +
                        $"{e.Signal,7:N1}" +
                        $"{cross,7}" +
                        $"{rlzGain + trdGain,13:c2}");



                    System.IO.File.AppendAllText(filename, Environment.NewLine);
                    System.IO.File.AppendAllText(filename, DateTime.Now.ToShortTimeString());
                    System.IO.File.AppendAllText(filename, item.Key + " " + $"{q.Date} " +
                        $"{q.Close,10:c2}" +
                        $"{e.StochRsi,7:N1}" +
                        $"{e.Signal,7:N1}" +
                        $"{cross,7}" +
                        $"{rlzGain + trdGain,13:c2}");
                }
            }
        }
    }

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


    public static IEnumerable<QuoteWithSymbol> SendAllStocksForLoad(string symbol)
    {

        try
        {



            //  var NewStock = db.Live_Stocks.FromSql("Execute dbo.SP_GET_LIVE_STOCKS_BY_STOCK {0}", stock.Symbol).ToList(); //.FirstOrDefault(x => x.symbol == stock.Symbol);
            List<QuoteWithSymbol> stocks = new List<QuoteWithSymbol>();
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection("Server=HAADVISRI\\AGS;Database=STOCK;User ID=sa;Password=240149;TrustServerCertificate=True;Trusted_Connection=true;MultipleActiveResultSets=true;"))
            {

                //using(SqlConnection conn = new SqlConnection("Server=103.21.58.192;Database=skyshwx7_;User ID=Honey;Password=K!cjn3376;TrustServerCertificate=false;Trusted_Connection=false;MultipleActiveResultSets=true;")) {
                SqlCommand sqlComm = new SqlCommand("Get_StockticksbySymbol", conn);
                sqlComm.Parameters.AddWithValue("@symbol", symbol);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;

                da.Fill(ds);
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                var NewStock = ds.Tables[0].Rows.Cast<DataRow>();

                foreach (var r in NewStock)
                {
                    try
                    {
                        var _stokc = new QuoteWithSymbol();
                        _stokc.symbol = r[0].ToString();
                        _stokc.Open = Convert.ToDecimal(r[1].ToString());
                        _stokc.Close = Convert.ToDecimal(r[2].ToString());
                        _stokc.High = Convert.ToDecimal(r[3].ToString());
                        _stokc.Low = Convert.ToDecimal(r[4].ToString());
                       // _stokc.change = Convert.ToDouble(r[5].ToString());
                       // _stokc.bPrice = Convert.ToDouble(r[6].ToString());
                        //_stokc.totalBuyQt = Convert.ToInt32(r[7]);
                       // _stokc.sPrice = Convert.ToDouble(r[8].ToString());
                        //_stokc.totalSellQ = Convert.ToInt32(r[9]);
                        //_stokc.avgPrice = Convert.ToDouble(r[11].ToString());
                        _stokc.Volume = Convert.ToDecimal(r[16].ToString());//.Replace("L", "100000").Replace("C", "1000000"));
                      //  _stokc.lowerCktLm = Convert.ToDouble(r[18].ToString());
                       // _stokc.upperCktLm = Convert.ToDouble(r[19].ToString());
                        _stokc.Date =Convert.ToDateTime(r[20].ToString());
                       // _stokc.close = Convert.ToDouble(r[21].ToString());
                        _stokc.stockName = r[23].ToString();
                        //_stokc.Week_min = !string.IsNullOrEmpty(r[25].ToString()) ? Convert.Todouble(r[25]) : default(double?);
                        //_stokc.Week_max = !string.IsNullOrEmpty(r[26].ToString()) ? Convert.Todouble(r[26]) : default(double?);
                        //_stokc.TwoWeeks_min = !string.IsNullOrEmpty(r[27].ToString()) ? Convert.Todouble(r[27]) : default(double?);
                        //_stokc.TwoWeeks_max = !string.IsNullOrEmpty(r[28].ToString()) ? Convert.Todouble(r[28]) : default(double?);
                        //_stokc.Month_min = !string.IsNullOrEmpty(r[29].ToString()) ? Convert.Todouble(r[29]) : default(double?);
                        //_stokc.Month_max = !string.IsNullOrEmpty(r[30].ToString()) ? Convert.Todouble(r[30]) : default(double?);
                        //_stokc.Three_month_min = !string.IsNullOrEmpty(r[31].ToString()) ? Convert.Todouble(r[31]) : default(double?);
                        //_stokc.Three_month_max = !string.IsNullOrEmpty(r[32].ToString()) ? Convert.Todouble(r[32]) : default(double?);
                        stocks.Add(_stokc);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

                return stocks;//.Where(X => X.Open <= 120);
            }


        }
        catch (Exception ex)
        {

            throw;

        }
        return Enumerable.Empty<QuoteWithSymbol>();

    }
    private static Collection<QuoteWithSymbol> GetQuotesFromFeed()
    {

        string line;
        string Date = "15-01-2024.txt";


        using var fileStream = new FileStream(@"C:\Hosts\Files\"+ Date, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
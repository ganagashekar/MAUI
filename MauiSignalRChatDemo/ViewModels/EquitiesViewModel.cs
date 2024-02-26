
using Breeze;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSignalRChatDemo.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Skender.Stock.Indicators;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Timers;

namespace MauiSignalRChatDemo.ViewModels
{

    public partial class BuyStockAlertModel : ObservableObject
    {
        [ObservableProperty]
        public string _symbol;
        [ObservableProperty]
        public string _stockName;
        [ObservableProperty]
        public decimal _buyATPrice;
        [ObservableProperty]
        public decimal? _buyATChange;
        [ObservableProperty]
        public decimal _sellATPrice;
        [ObservableProperty]
        public decimal _currentPrice;

        [ObservableProperty]
        public decimal _currentChange;

        [ObservableProperty]
        public string _stockCode;

        [ObservableProperty]
        public int _qty;

        [ObservableProperty]
        public bool _IsBuy;

        [ObservableProperty]
        public bool _IsSell;

        [ObservableProperty]
        public int _bgcolor;

        [ObservableProperty]
        public string _match;


        [ObservableProperty]
        public int _bullishCount;

        [ObservableProperty]
        public int _bearishCount;

        [ObservableProperty]
        public DateTime _lttDateTime;

        [ObservableProperty]
        public string _data;
    }



    public partial class EquitiesViewModel : ObservableObject
    {
        static System.Timers.Timer timer;
        private readonly HubConnection _hubConnection;

        [ObservableProperty]
        string _name;

        [ObservableProperty]
        string _message;

        [ObservableProperty]
        ObservableCollection<BuyStockAlertModel> _messages;

        //List<LiveStockData> listofTicks = new List<LiveStockData>();

        Dictionary<string, List<LiveStockData>> dictionary = new Dictionary<string, List<LiveStockData>>();

        [ObservableProperty]
        bool _isConnected;



        void schedule_Timer()
        {
            //Console.WriteLine("### Timer Started ###");

            //DateTime nowTime = DateTime.Now;
            //DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 15, 28, 0);
            ////Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            ////if (nowTime > scheduledTime)
            ////{
            ////    scheduledTime = scheduledTime.AddDays(1);
            ////}

            //double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            //timer = new System.Timers.Timer(tickTime);
            //timer.Elapsed += new ElapsedEventHandler(timer_ElapsedAsync);
            //timer.Start();
        }

        void timer_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("### Timer Stopped ### \n");
            timer.Stop();
            try
            {
                _hubConnection.InvokeAsync("ExportBuyStockAlterFromAPP", JsonSerializer.Serialize(this.Messages.ToList()));

                Message = string.Empty;
            }
            catch (Exception ex)
            {


            }

        }

        public async Task InitiICICAsync()
        {
            var text = System.IO.File.ReadAllText("C:\\Hosts\\ICICI_Key\\jobskeys.txt");

            string[] lines = text.Split(
    new string[] { Environment.NewLine },
    StringSplitOptions.None
            );


            Random r = new Random();

            // Console.WriteLine(connection.ConnectionId);
            // string HUbUrl = "http://localhost/StockSignalRServer/livefeedhub";
            try
            {
                string APIKEY = string.Empty;
                string APISecret = string.Empty;
                string token = string.Empty;
                //Initialize SDK
                string[] line;
                string arg = "4";



                switch (Convert.ToInt16(arg))
                {
                    case 0:
                        line = lines[0].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 1:
                        line = lines[1].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 2:
                        line = lines[2].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 3:
                        line = lines[3].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                    case 4:
                        line = lines[4].ToString().Split(',');
                        APIKEY = line[0];
                        APISecret = line[1];
                        token = line[2];
                        break;
                }
                // Generate Session
                Console.WriteLine(arg);
                BreezeConnect breeze = new BreezeConnect(APIKEY);
                //Console.WriteLine(args[1].ToString());
                breeze.generateSessionAsPerVersion(APISecret, token);

                // Connect to WebSocket
                var responseObject = breeze.wsConnectAsync();
                Console.WriteLine(JsonSerializer.Serialize(responseObject));

                breeze.subscribeFeedsAsync("4.1!SUZLON");

                breeze.ticker(async (data) =>
                {


                });
            }
            catch
            {

            }
        }


        public DateTime GetParseLTT(string ltt)
        {
            try
            {
                string[] test = ltt.Split(' ');
                string dateformat = string.Format("{0}-{1}-{2} {3}", test.Last(), test[1].ToString(), test[2].ToString(), test[3].ToString());
                var result = DateTime.TryParse(dateformat, out var dt);
                return dt;
            }
            catch (Exception ex)
            {

               return Convert.ToDateTime(ltt);
            }
        }
        public EquitiesViewModel()
        {
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl($"http://localhost:90/BreezeOperation").WithAutomaticReconnect()//WithKeepAliveInterval(TimeSpan.FromSeconds(30))
            //    //.WithUrl("https://localhost:7189/BreezeOperation")
            //    .Build();
            _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:45/breezeOperation").WithAutomaticReconnect().Build();
           // _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7189/BreezeOperation").WithAutomaticReconnect().Build();
            _hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(30);
            Connect();
            schedule_Timer();
            var bullsis = new List<string>() { Match.BullBasis.ToString(), Match.BullConfirmed.ToString(), Match.BullSignal.ToString() };
                        var barish = new List<string>() { Match.BearBasis.ToString(), Match.BearConfirmed.ToString(), Match.BearSignal.ToString() };


            _hubConnection.On<BuyStockAlertModel[]>("SendGetBuyStockTriggers", async result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in result)
                    {
                        _messages.Add(new BuyStockAlertModel()
                        {
                            _bgcolor = 1,
                            _buyATChange = item._buyATChange == Convert.ToDecimal(-9999.00) ? null : item._buyATChange,
                            _stockCode = item._stockCode,
                            _IsBuy = true,
                            _stockName = item._stockName,
                            _buyATPrice = item._buyATPrice,
                            _symbol = item._symbol,
                            _sellATPrice = item._sellATPrice,
                            _currentPrice = 0
                        }
                        );
                    }

                });


            });


            _hubConnection.On<string>("SendCaptureLiveDataForAutomation", async param =>
            {
                LiveStockData livedata = JsonSerializer.Deserialize<LiveStockData>(param);
                livedata.LTT_DATE = GetParseLTT(livedata.ltt); //Convert.ToDateTime(livedata.ltt); //
                var dictionaryValue = CollectionsMarshal.GetValueRefOrAddDefault(dictionary, livedata.symbol, out bool exists);
                if (dictionaryValue==null)
                {
                    dictionaryValue = new List<LiveStockData>();
                }
               
                dictionaryValue.Add(livedata);
                var findsymbol = _messages.FirstOrDefault(x => x.Symbol == livedata.symbol);
                if (_messages.Count > 0 && findsymbol != null)
                {
                    try
                    {
                        List<Quote> quotesList = dictionaryValue.Where(x => x.symbol == livedata.symbol).ToList().Select(x => new Quote
                        {
                            Close = Convert.ToDecimal(livedata.last),
                            Open = Convert.ToDecimal(livedata.open),
                            Date = Convert.ToDateTime(livedata.LTT_DATE),
                            High = Convert.ToDecimal(livedata.high),
                            Low = Convert.ToDecimal(livedata.low),
                            Volume = Convert.ToDecimal(livedata.ttv)
                        }).OrderBy(x => x.Date).ToList();
                        IEnumerable<MacdResult> macdresult = quotesList.GetMacd(12,26,9);
                       // var GetWma = quotesList.GetWma(lookbackPeriods);
                        IEnumerable<VolatilityStopResult> Volatilityresults = quotesList.GetVolatilityStop(5, 3);
                        IEnumerable<RsiResult> rsiResults = quotesList.GetObv().GetRsi(5);
                        var candleResult = quotesList.GetMarubozu(85).OrderByDescending(x => x.Date).FirstOrDefault();
                        dictionary[livedata.symbol] = dictionaryValue.OrderByDescending(x => Convert.ToDateTime(livedata.LTT_DATE)).Take(100).ToList();
                        _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Match = candleResult.Match.ToString();
                        var newresul = new
                        {

                            candleResult = candleResult,
                            macdresult = macdresult.LastOrDefault(),
                            rsiResults = rsiResults.LastOrDefault(),
                            Volatilityresults = Volatilityresults.LastOrDefault()
                        };
                        
                        if (candleResult != null && bullsis.Any(x=> x.ToString().Contains(candleResult.Match.ToString())))
                        {
                            
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).BullishCount = _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).BullishCount + 1;
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Data = JsonSerializer.Serialize(newresul);
                            _hubConnection.InvokeAsync("ExportBuyStockAlterFromAPP_IND", JsonSerializer.Serialize(_messages.Where(x => x.Symbol == livedata.symbol).ToList()));
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Data = "";
                           // _hubConnection.InvokeAsync("GetTopStockforBuyAutomation");
                        }

                        if (candleResult != null && barish.Any(x => x.ToString().Contains(candleResult.Match.ToString())))
                        {
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).BearishCount = _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).BearishCount + 1;
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Data = JsonSerializer.Serialize(newresul);
                            _hubConnection.InvokeAsync("ExportBuyStockAlterFromAPP_IND", JsonSerializer.Serialize(_messages.Where(x => x.Symbol == livedata.symbol).ToList()));
                            _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Data = "";
                           // _hubConnection.InvokeAsync("GetTopStockforBuyAutomation");
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).LttDateTime = livedata.LTT_DATE;
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentPrice = Convert.ToDecimal(livedata.last);
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentChange = Convert.ToDecimal(livedata.change);
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Bgcolor = Convert.ToDecimal(livedata.change.Value) > 0 ? 3 : 1;
                    if (Convert.ToDecimal(livedata.last) > 0 && (findsymbol.BuyATPrice >= Convert.ToDecimal(livedata.last) || findsymbol.BuyATChange >= Convert.ToDecimal(livedata.change)) && findsymbol.IsBuy == true)
                    {
                        //int qty = Convert.ToInt16(10000 / findsymbol.BuyATPrice);
                        //await _hubConnection.SendAsync("BuyOrSellEquity", livedata.symbol, qty, "NSE", "market", Convert.ToDecimal(livedata.last.Value).ToString(), Convert.ToDecimal(livedata.last.Value).ToString(), findsymbol.StockCode, "Buy");
                        //_messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsSell = true;
                        //_messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsBuy = false;
                    }
                }
            });

            //_hubConnection.On<object>("CaptureLiveDataForAutomation", async param =>
            //{
            //    //LiveStockData livedata = JsonSerializer.Deserialize<LiveStockData>(param);
            //    //var findsymbol = _messages.FirstOrDefault(x => x.Symbol == livedata.symbol);
            //    //if (_messages.Count > 0 && findsymbol != null)
            //    //{
            //    //    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentPrice = Convert.ToDecimal(livedata.last);
            //    //    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentChange = Convert.ToDecimal(livedata.change);

            //    //    if ((findsymbol.BuyATPrice >= Convert.ToDecimal(livedata.last) || findsymbol.BuyATChange >= Convert.ToDecimal(livedata.change)) && findsymbol.IsBuy == true)
            //    //    {
            //    //        int qty = Convert.ToInt16(10000 / findsymbol.BuyATPrice);
            //    //        await _hubConnection.SendAsync("BuyOrSellEquity", livedata.symbol, qty, "NSE", "market", Convert.ToDecimal(livedata.last.Value).ToString(), Convert.ToDecimal(livedata.last.Value).ToString(), findsymbol.StockCode, "Buy");
            //    //        _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsSell = true;
            //    //        _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsBuy = false;
            //    //    }
            //    //}
            //});


        }



        [RelayCommand]
        async Task Connect()
        {

            if (_hubConnection.State == HubConnectionState.Connecting ||
                _hubConnection.State == HubConnectionState.Connected) return;

            await _hubConnection.StartAsync();
            Messages ??= new ObservableCollection<BuyStockAlertModel>();
            _hubConnection.SendAsync("GetBuyStockTriggers");
            IsConnected = true;


        }

        [RelayCommand]
        async Task Disconnect()
        {
            _messages.FirstOrDefault().StockName = "ganga";

            if (_hubConnection.State == HubConnectionState.Disconnected) return;

            await _hubConnection.StopAsync();

            IsConnected = false;
        }

        [RelayCommand]
        async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Message)) return;

            await _hubConnection.InvokeAsync("SendMessage", Name, Message);

            Message = string.Empty;
        }

        [RelayCommand]
        async Task ExportJsonData()
        {

            try
            {
                await _hubConnection.InvokeAsync("ExportBuyStockAlterFromAPP", JsonSerializer.Serialize(this.Messages.ToList()));

                Message = string.Empty;
            }
            catch (Exception ex)
            {


            }
        }

        [RelayCommand]
        async Task PerformSearchCommand()
        {

            try
            {
                await _hubConnection.InvokeAsync("ExportBuyStockAlterFromAPP", JsonSerializer.Serialize(this.Messages.ToList()));

                Message = string.Empty;
            }
            catch (Exception ex)
            {


            }
        }
    }
}

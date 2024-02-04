using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text.Json;


using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Timers;

namespace MyOwnStockAutomation.ViewModels
{

    public partial class PredictedStocksAutomation : ObservableObject
    {

        [ObservableProperty]
        public int _bulishCount;

        [ObservableProperty]
        public int _bearishCount;

        [ObservableProperty]
        public DateTime _ltt;

        [ObservableProperty]
        public string _symbol;

        [ObservableProperty]
        public double? _candleResult_Price;

        [ObservableProperty]
        public double? _candleResult_Match;

        [ObservableProperty]
        public double? _candleResult_Size;

        [ObservableProperty]
        public double? _candleResult_Body;

        [ObservableProperty]
        public double? _candleResult_UpperWick;

        [ObservableProperty]
        public double? _candleResult_LowerWick;

        [ObservableProperty]
        public double? _candleResult_BodyPct;

        [ObservableProperty]
        public double? _candleResult_UpperWickPct;

        [ObservableProperty]
        public double? _candleResult_LowerWickPct;

        [ObservableProperty]
        public bool _candleResult_IsBullish;

        [ObservableProperty]
        public bool _candleResult_IsBearish;

        [ObservableProperty]
        public double? _candleResult_Volume;

        [ObservableProperty]
        public double? _macdresult_Macd;

        [ObservableProperty]
        public double? _macdresult_Signal;

        [ObservableProperty]
        public double? _macdresult_FastEma;

        [ObservableProperty]
        public double? _macdresult_SlowEma;

        [ObservableProperty]
        public double? _macdresult_Rsi;

        [ObservableProperty]
        public double? _Volatilityresults_Sar;

        [ObservableProperty]
        public double? _Volatilityresults_UpperBand;

        [ObservableProperty]
        public double? _Volatilityresults_LowerBand;

        [ObservableProperty]
        public string _Volatilityresults_IsStop;

    }


    public partial class StockPredictionViewModel : ObservableObject
    {
        //private static System.Timers.Timer aTimer;

       
        [ObservableProperty]
        public ObservableCollection<PredictedStocksAutomation> _pridictedStocks;


        private readonly HubConnection _hubConnection;

        [ObservableProperty]
        bool _isConnected;

        [RelayCommand]
        async Task Connect()
        {

            if (_hubConnection.State == HubConnectionState.Connecting ||
                _hubConnection.State == HubConnectionState.Connected) return;

            await _hubConnection.StartAsync();

            _hubConnection.SendAsync("GetTopStockforBuyAutomation");
            // _hubConnection.SendAsync("GetBuyStockTriggers");
            _isConnected = true;


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

        
        public StockPredictionViewModel()
        {
            //_pridictedStocks ??= new ObservableCollection<PredictedStocksAutomation>();
           PridictedStocks ??= new ObservableCollection<PredictedStocksAutomation>();
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl($"http://localhost:90/BreezeOperation").WithAutomaticReconnect()//WithKeepAlivedouble?erval(TimeSpan.FromSeconds(30))
            //    //.WithUrl("https://localhost:7189/BreezeOperation")
            //    .Build();
            _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:45/breezeOperation").WithAutomaticReconnect().Build();
            //_hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7189/BreezeOperation").WithAutomaticReconnect().Build();
            _hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(30);
            Connect();
            //SetTimer();





            _hubConnection.On<PredictedStocksAutomation[]>("SendExportBuyStockAlterFromAPP_IND", async param =>
            {
                try
                {
                    //  var livedata = JsonConvert.DeserializeObject<List<EquityModelAutomation>>("");//.des.<List<EquityModelAutomation>>(param, new JsonSerializerOptions
                    //{
                    //PropertyNameCaseInsensitive = true
                    //}).ToList();

                    _pridictedStocks.Clear();
                    //PridictedStocks = null;
                    //PridictedStocks = new ObservableCollection<PredictedStocksAutomation>();
                    foreach (var item in param)
                    {
                        //_pridictedStocks.Remove(item);
                        _pridictedStocks.Add(item);
                    }
                    //Thread.Sleep(1);
                }
                catch (Exception ex)
                {

                    throw;
                }
                finally {
                   // PridictedStocks = null;
                }



            });

            


        }
    }
}

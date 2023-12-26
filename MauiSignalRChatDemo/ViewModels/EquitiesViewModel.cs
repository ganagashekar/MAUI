
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiSignalRChatDemo.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Text.Json;

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
        public decimal _buyATChange;
        [ObservableProperty]
        public decimal _sellATPrice;
        [ObservableProperty]
        public decimal _currentPrice;

        [ObservableProperty]
        public decimal _currentChange;

        [ObservableProperty]
        public string _stockCode;

        [ObservableProperty]
        public int  _qty;

        [ObservableProperty]
        public bool _IsBuy;

        [ObservableProperty]
        public bool _IsSell;

        [ObservableProperty]
        public int _bgcolor;
    }



    public partial class EquitiesViewModel : ObservableObject
    {
        private readonly HubConnection _hubConnection;

        [ObservableProperty]
        string _name;

        [ObservableProperty]
        string _message;

        [ObservableProperty]
        ObservableCollection<BuyStockAlertModel> _messages;

        [ObservableProperty]
        bool _isConnected;

         

        public EquitiesViewModel()
        {
            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl($"http://localhost:90/BreezeOperation").WithAutomaticReconnect()//WithKeepAliveInterval(TimeSpan.FromSeconds(30))
            //    //.WithUrl("https://localhost:7189/BreezeOperation")
            //    .Build();
              _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:90/BreezeOperation").WithAutomaticReconnect().Build();
            _hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(30);
            Connect();

           
            
           
            _hubConnection.On<BuyStockAlertModel[]>("SendGetBuyStockTriggers", async result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in result)
                    {
                        _messages.Add(new BuyStockAlertModel() { _bgcolor= 1, _buyATChange = item._buyATChange, _stockCode=item._stockCode, _IsBuy=true, _stockName = item._stockName, _buyATPrice = item._buyATPrice, _symbol = item._symbol, _sellATPrice = item._sellATPrice, _currentPrice = 0 }
                        );
                    }

                });


            });
            _hubConnection.On<string>("SendCaptureLiveDataForAutomation", async  param =>
            {

                LiveStockData livedata = JsonSerializer.Deserialize<LiveStockData>(param);
                var findsymbol = _messages.FirstOrDefault(x => x.Symbol == livedata.symbol);
                if (_messages.Count > 0 && findsymbol != null)
                {
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentPrice = Convert.ToDecimal(livedata.last);
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentChange = Convert.ToDecimal(livedata.change);
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).Bgcolor =  Convert.ToDecimal(livedata.change.Value) > 0 ? 3:1;
                    if (Convert.ToDecimal(livedata.last) > 0 && (findsymbol.BuyATPrice >= Convert.ToDecimal(livedata.last) || findsymbol.BuyATChange >= Convert.ToDecimal(livedata.change)) && findsymbol.IsBuy == true)
                    {
                        int qty = Convert.ToInt16(10000/findsymbol.BuyATPrice);
                        await _hubConnection.SendAsync("BuyOrSellEquity", livedata.symbol, qty, "NSE","market", Convert.ToDecimal(livedata.last.Value).ToString(), Convert.ToDecimal(livedata.last.Value).ToString(), findsymbol.StockCode,"Buy");
                        _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsSell= true;
                        _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).IsBuy = false;
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
    }
}

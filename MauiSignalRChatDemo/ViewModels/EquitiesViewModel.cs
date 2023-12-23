
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
        public decimal _sellATPrice;
        [ObservableProperty]
        public decimal _currentPrice;

        [ObservableProperty]
        public decimal _currentChange;
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
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:90/BreezeOperation")
                .Build();

            Connect();

            _hubConnection.On<BuyStockAlertModel[]>("SendGetBuyStockTriggers", result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in result)
                    {
                        _messages.Add(new BuyStockAlertModel() { _stockName = item._stockName, _buyATPrice = item._buyATPrice, _symbol = item._symbol, _sellATPrice = item._sellATPrice, _currentPrice = 0 }
                        );
                    }

                });


            });
            _hubConnection.On<string>("SendLiveData", param =>
            {
                LiveStockData livedata = JsonSerializer.Deserialize<LiveStockData>(param);
                if (_messages.Count > 0 && _messages.FirstOrDefault(x => x._symbol == livedata.symbol) != null)
                {
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentPrice = Convert.ToDecimal(livedata.last);
                    _messages.FirstOrDefault(x => x.Symbol == livedata.symbol).CurrentChange = Convert.ToDecimal(livedata.change);
                }
            });


        }

        [RelayCommand]
        async Task Connect()
        {

            if (_hubConnection.State == HubConnectionState.Connecting ||
                _hubConnection.State == HubConnectionState.Connected) return;

            await _hubConnection.StartAsync();
            Messages ??= new ObservableCollection<BuyStockAlertModel>();
            await _hubConnection.SendAsync("GetBuyStockTriggers");
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

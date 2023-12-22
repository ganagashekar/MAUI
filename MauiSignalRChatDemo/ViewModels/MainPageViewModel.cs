using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace MauiSignalRChatDemo.ViewModels
{

    public  partial class BuyStockAlertModel :ObservableObject
    {
        [ObservableProperty]
        public string _symbol;
        [ObservableProperty]
        public string _stockName;
        [ObservableProperty]
        public decimal _price;
    }
    //public partial class EquitiesModel
    //{

    //    public string symbol { get; set; }


    //    public string stockname { get; set; }


    //    public double open { get; set; }


    //    public double close { get; set; }


    //    public double last { get; set; }


    //    public double change { get; set; }


    //    public decimal buyPrice { get; set; }


    //    public double sellPrice { get; set; }
    //}
    public partial class MainPageViewModel : ObservableObject
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

        public MainPageViewModel()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:7189/BreezeOperation")
                .Build();

            //_obsrvequities ??= new ObservableCollection<EquitiesViewModel>();

            _hubConnection.On<BuyStockAlertModel[]>("SendGetBuyStockTriggers", result =>
            {


                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in result)
                    {

                        _messages.Add(new BuyStockAlertModel() { _stockName = item._stockName }




                        );
                    }



                    //Messages.Add($"{user} says {message}");
                });
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

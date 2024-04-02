using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using StocksBuy.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StocksBuy.ViewModel
{
    public partial class EquitiesBuy : ObservableObject

    {
        [ObservableProperty]
        public string _symbol;
        [ObservableProperty]
        public string _stockName;
        [ObservableProperty]
        public decimal _buy_Percent;
        [ObservableProperty]
        public decimal _sell_Percent;
        [ObservableProperty]
        public string _ltt;
        [ObservableProperty]
        public string _securityId;
        [ObservableProperty]
        public bool _isExecute;
        [ObservableProperty]
        public string _orderId;


    }


    public partial class EquitiesBuyViewModel : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<EquitiesBuy> _equitiesBuys;

        Dictionary<string, List<LiveStockData>> dictionary = new Dictionary<string, List<LiveStockData>>();

        private readonly HubConnection _hubConnection;


        public EquitiesBuyViewModel()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:8080/BreezeOperation").WithAutomaticReconnect().Build();
            Connect(0);


            _hubConnection.On<Equities[]>("SendGetBuyForAutomation_Auto", async result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in result)
                    {
                        EquitiesBuys.Add(new EquitiesBuy()
                        {
                            StockName = item.stock_name,
                            Symbol = item.symbol,
                            SecurityId=item.SecurityId
                        }
                        ); ;

                    }


                });



            });

           

            _hubConnection.On<string>("SendCaptureLiveDataForAutomation_Auto", param =>
            {

                LiveStockData livedata = JsonSerializer.Deserialize<LiveStockData>(param);
                livedata.LTT_DATE = GetParseLTT(livedata.ltt); //Convert.ToDateTime(livedata.ltt); //
                int totalqty = (livedata.totalBuyQt ?? 0) + (livedata.totalSellQ ?? 0);
                decimal buyqty = Math.Round(Convert.ToDecimal(livedata.totalBuyQt.Value) / totalqty * 100,2);
                decimal sellqty = Math.Round(Convert.ToDecimal(livedata.totalSellQ.Value) / totalqty * 100, 2);
                var dictionaryValue = CollectionsMarshal.GetValueRefOrAddDefault(dictionary, livedata.symbol, out bool exists);
                if (dictionaryValue?.Count > 99)
                {
                    dictionaryValue.RemoveRange(0, 50);
                }
                else if (dictionaryValue == null)
                {
                    dictionaryValue = new List<LiveStockData>();
                }
                decimal volumedifference = Convert.ToDecimal(dictionaryValue.LastOrDefault()?.ttv);
                volumedifference = !string.IsNullOrEmpty(livedata.ttv) ? Convert.ToDecimal(livedata.ttv) - Convert.ToDecimal(volumedifference) : 0;
                dictionaryValue.Add(livedata);

                EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).Buy_Percent = buyqty;
                EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).Sell_Percent = sellqty;
                EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).Ltt = livedata.LTT_DATE.ToShortTimeString();
                var isorderproceesed = EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).IsExecute;
                var securityId = EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).SecurityId;
                if (buyqty < 20 && !isorderproceesed)
                {
                    EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).IsExecute = true;
                    var filteredtxt = ExecuteCommand(@"C:\Hosts\Breeze\Automation_buy.bat", securityId, "NSE","buy","LIMIT","(livedata.last-0.50).ToString()",(10).ToString(), (livedata.last - 1).ToString());
                    if (!string.IsNullOrEmpty(filteredtxt))
                    {
                        _hubConnection.SendAsync("SendPOPlaceOrder", "Executed", securityId + " " + "Order placed successfully");
                        EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).OrderId = filteredtxt;
                    }
                    else
                    {
                        EquitiesBuys.FirstOrDefault(x => x.Symbol == livedata.symbol).IsExecute = false;
                    }
                }

            });

        }

        static string ExecuteCommand(string filename, params string[] securityId)
        {
            string filteredtxt = string.Empty;
            try
            {
                int exitCode;
                ProcessStartInfo processInfo;
                Process process;

                processInfo = new ProcessStartInfo(filename, securityId);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                // *** Redirect the output ***
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;

                process = Process.Start(processInfo);
                process.WaitForExit();

                // *** Read the streams ***
                // Warning: This approach can lead to deadlocks, see Edit #2
                string output = process.StandardOutput.ReadToEnd();
                
                string error = process.StandardError.ReadToEnd();

               


                exitCode = process.ExitCode;

                Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
                if (output.Contains("Order placed successfully"))
                {
                   
                    return output.ToString().Split("\r\n").Where(x => x != "").LastOrDefault();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {

                throw;
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

        [RelayCommand]
        public async Task Connect(int arg)
        {
            try
            {

                //if (_hubConnection.State == HubConnectionState.Connecting ||
                //    _hubConnection.State == HubConnectionState.Connected) return;

                await _hubConnection.StartAsync();
                EquitiesBuys ??= new ObservableCollection<EquitiesBuy>();
                _hubConnection.SendAsync("GetBuyForAutomation_Auto");


            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

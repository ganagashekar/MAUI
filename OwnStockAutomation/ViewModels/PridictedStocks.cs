using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnStockAutomation.Models
{
    public class Ticker_Stocks_Histry_Extended_Mdl
    {



        public string symbol { get; set; }



        //public bool? IsBearish { get; set; }

        //public bool? IsBullish { get; set; }

        //public decimal? Macd { get; set; }

        //public decimal? FastEma { get; set; }

        //public decimal? SlowEma { get; set; }

        //public decimal? Signal { get; set; }

        //public decimal? Histogram { get; set; }

        //public decimal? Rsi { get; set; }

        // public DateTime Createdon { get; set; }

        public int BearishCount { get; set; }
        public int BulishCount { get; set; }
        public DateTime Ltt { get; set; }
        public string Match { get; set; }

        public string Data { get; set; }
    }
}

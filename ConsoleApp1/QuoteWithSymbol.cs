using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class QuoteWithSymbol : Quote
    {
        public string symbol { get; set; }
        public string stockName { get; set; }

        public int buyQTY { get; set; }
        public int sellQty { get; set; }

    }
}

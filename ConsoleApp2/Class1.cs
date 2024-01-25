using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Equities
    {
        public string? msn_secid { get; set; }

        public string symbol { get; set; }
        public double? open { get; set; }
        public double? last { get; set; }
        public double? high { get; set; }
        public double? low { get; set; }
        public double? change { get; set; }
        public double? bPrice { get; set; }
        public int bQty { get; set; }
        public double? sPrice { get; set; }
        public int sQty { get; set; }
        public int ltq { get; set; }
        public double? avgPrice { get; set; }
        public string quotes { get; set; }
        public int ttq { get; set; }
        public int totalBuyQt { get; set; }
        public int totalSellQ { get; set; }
        public string ttv { get; set; }
        public string trend { get; set; }
        public double? lowerCktLm { get; set; }
        public double? upperCktLm { get; set; }
        public string ltt { get; set; }
        public double? close { get; set; }
        public string exchange { get; set; }
        public string stock_name { get; set; }
        public string VolumeC { set; get; }
    }
}

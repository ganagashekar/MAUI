using Newtonsoft.Json;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnStockAutomation.Models
{
    public class LiveStockData
    {
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
        public DateTime LTT_DATE { set; get; }


        //public string lttDate
        //{
        //    get { return ltt; }
        //    // set { MyDate = DateTime.Parse(value); }
        //    set
        //    {
        //        string[] test = this.ltt.Split(' ');
        //        string dateformat = string.Format("{0}-{1}-{2} {3}", test.Last(), test[1].ToString(), test[2].ToString(), test[3].ToString());
        //        DateTime.TryParse(dateformat, out var dt);
        //        ltt = dt.ToShortDateString();
        //    }
        //}



        public double? close { get; set; }
        public string exchange { get; set; }
        public string stock_name { get; set; }

        public string volumeC { get; set; }

        public bool isNotified { get; set; }


        public override string ToString()
        {
            const DateTimeStyles style = DateTimeStyles.AllowWhiteSpaces;


            //, CultureInfo.InvariantCulture,
            //               style, out var dt) ? dt : null as DateTime?;
            string[] test = this.ltt.Split(' ');
            string dateformat = string.Format("{0}-{1}-{2} {3}", test.Last(), test[1].ToString(), test[2].ToString(), test[3].ToString());
            var result = DateTime.TryParse(dateformat, out var dt);
            return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24}", this.symbol, this.open, this.last, this.high, this.low, this.change, this.bPrice, this.bQty, this.sPrice, this.sQty, this.ltq, this.avgPrice, this.quotes, this.ttq, this.totalBuyQt, this.totalSellQ, this.ttv, this.trend, this.lowerCktLm, this.upperCktLm, dateformat, this.close, this.exchange, this.stock_name, this.volumeC);
        }
    }
}

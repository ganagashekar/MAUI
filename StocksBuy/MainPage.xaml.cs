using StocksBuy.ViewModel;

namespace StocksBuy
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(EquitiesBuyViewModel equitiesBuy)
        {
            InitializeComponent();
            BindingContext = equitiesBuy;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
        }
    }

}

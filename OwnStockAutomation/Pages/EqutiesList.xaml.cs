
using MyOwnStockAutomation.ViewModels;

namespace MyOwnStockAutomation.Pages;

public partial class EqutiesList : ContentPage
{
	public EqutiesList(EquitiesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }


}
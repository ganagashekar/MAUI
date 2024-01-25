
using MauiSignalRChatDemo.ViewModels;

namespace MauiSignalRChatDemo.Pages;

public partial class EqutiesList : ContentPage
{
	public EqutiesList(EquitiesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }


}
using WorkoutLogg.Models;
using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}
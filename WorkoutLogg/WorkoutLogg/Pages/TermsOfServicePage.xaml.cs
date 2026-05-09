namespace WorkoutLogg.Pages;

public partial class TermsOfServicePage : ContentPage
{
	public TermsOfServicePage()
	{
		InitializeComponent();
	}

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new CreateAccount();
    }
}
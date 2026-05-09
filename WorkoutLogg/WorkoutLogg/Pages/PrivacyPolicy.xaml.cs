namespace WorkoutLogg.Pages;

public partial class PrivacyPolicy : ContentPage
{
	public PrivacyPolicy()
	{
		InitializeComponent();
	}

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new CreateAccount();
    }
}
namespace WorkoutLogg.Pages;

public partial class PasswordSuccess : ContentPage
{
	public PasswordSuccess()
	{
		InitializeComponent();
	}

    private async void OnBackToSignInClicked(object sender, EventArgs e)
    {
        // Сбрасываем весь стек навигации и идём на Login
        Application.Current!.Windows[0].Page = new LoginPage();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}
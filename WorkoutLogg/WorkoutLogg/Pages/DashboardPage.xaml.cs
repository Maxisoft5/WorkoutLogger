namespace WorkoutLogg.Pages;
using WorkoutLogg.Pages.Controls;


public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        DateLabel.Text = DateTime.Now.ToString("dddd, dd MMM").ToUpper();
        HelloUserText.Text = "";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var currentUser = await CurrentUserStore.GetCurrentUser();
        if (currentUser != null)
        {
            HelloUserText.Text = $"Good morning, {currentUser.FullName}";
            AvatarLabel.Text = $"{currentUser.FullName?.ToUpper().First()}";
        }
    }

    private async void OnSeeAllWorkoutsTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Workouts");
    }
}
namespace WorkoutLogg.Pages;
using WorkoutLogg.Pages.Controls;


public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        DateLabel.Text = DateTime.Now.ToString("dddd, dd MMM").ToUpper();
    }

    private async void OnSeeAllWorkoutsTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Workouts");
    }
}
using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class LoggerPage : ContentPage
{
    private readonly LoggerPageModel _vm;

    public LoggerPage(LoggerPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
        DateLabel.Text = _vm.DateLabel;
        Calendar.MarkedDates = _vm.MarkedDates;
    }

    private async void OnAddLogTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"AddLog?date={_vm.SelectedDate:yyyy-MM-dd}");
    }

    private async void OnDateSelected(object sender, DateTime date)
    {
        _vm.SelectedDate = date;
        Calendar.SelectedDate = date;
        await _vm.LoadAsync();
        DateLabel.Text = _vm.DateLabel;
    }

    private async void OnSessionOptionsTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Guid sessionId) return;

        var edit   = Loc.Get("Common_Edit");
        var delete = Loc.Get("Common_Delete");
        var action = await DisplayActionSheet(null, Loc.Get("Common_Cancel"), null, edit, delete);

        if (action == edit)
            await Shell.Current.GoToAsync($"AddLog?sessionId={sessionId}");
        else if (action == delete)
        {
            bool ok = await DisplayAlert(
                Loc.Get("Logger_DeleteTitle"), Loc.Get("Logger_DeleteMsg"),
                Loc.Get("Common_Delete"), Loc.Get("Common_Cancel"));
            if (ok)
            {
                await _vm.DeleteSessionCommand.ExecuteAsync(sessionId);
                Calendar.MarkedDates = _vm.MarkedDates;
            }
        }
    }
}

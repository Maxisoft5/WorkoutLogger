using Grpc.Core;
using System.Collections.ObjectModel;
using WorkoutLogger.Grpc.Contracts;

namespace WorkoutLogg.Pages;

public partial class LoggerPage : ContentPage
{
    private IDispatcherTimer? _timer;
    private int _remainingSeconds = 90;
    private bool _isRunning = false;
    private readonly ExercisesGrpcClient _grpc;
    public ObservableCollection<ExerciseDto> Exercises { get; } = new();

    public LoggerPage(ExercisesGrpcClient grpc)
    {
        InitializeComponent();
        _grpc = grpc;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Exercises.Clear();

        try
        {
            await foreach (var ex in _grpc.StreamAsync())
            {
                Exercises.Add(ex); // появляются на UI по мере прихода
            }
        }
        catch (RpcException ex)
        {
            await DisplayAlert("Error", $"gRPC failed: {ex.Status.Detail}", "OK");
        }
    }

    // ── Rest Timer ────────────────────────────────
    private void OnTimerToggled(object sender, EventArgs e)
    {
        if (_isRunning)
            StopTimer();
        else
            StartTimer();
    }

    private void StartTimer()
    {
        _isRunning = true;
        TimerButton.Text = "⏸ Pause";

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) =>
        {
            if (_remainingSeconds <= 0)
            {
                StopTimer();
                _remainingSeconds = 90;
                UpdateTimerLabel();
                return;
            }
            _remainingSeconds--;
            UpdateTimerLabel();
        };
        _timer.Start();
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _isRunning = false;
        TimerButton.Text = "▶ Start";
    }

    private void UpdateTimerLabel()
    {
        var m = _remainingSeconds / 60;
        var s = _remainingSeconds % 60;
        TimerLabel.Text = $"{m:D2}:{s:D2}";
    }

    // ── Exercise actions ──────────────────────────
    private void OnAddSetTapped(object sender, TappedEventArgs e)
    {
        // TODO: добавить строку сета в соответствующее упражнение
        var exercise = e.Parameter?.ToString();
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        // TODO: открыть ExercisePickerPage
        await DisplayAlert("Add Exercise", "Exercise picker — coming soon", "OK");
    }

    private void OnDateSelected(object sender, DateTime date)
    {
        // date — выбранная дата, фильтруй список тренировок
    }
}
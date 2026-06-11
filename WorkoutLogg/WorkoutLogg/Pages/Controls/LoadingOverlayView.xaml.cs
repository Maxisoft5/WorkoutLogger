namespace WorkoutLogg.Pages.Controls;

public partial class LoadingOverlayView : ContentView
{
    private readonly EcgDrawable _ecg = new();
    private IDispatcherTimer? _timer;
    private float _elapsed = 0f;
    private const float CycleMs = 1800f;

    public LoadingOverlayView()
    {
        InitializeComponent();
        EcgView.Drawable = _ecg;
    }

    // Показывает оверлей сразу (без таймера) — вызывать из конструктора и OnDisappearing
    public void Preload()
    {
        _timer?.Stop();
        _timer = null;
        _ecg.DrawProgress = 0.35f;
        _ecg.Opacity = 0.7f;
        EcgView.Invalidate();
        Opacity = 1f;
        InputTransparent = false;
    }

    // Показывает с анимацией — вызывать в начале OnAppearing
    public void Show()
    {
        _timer?.Stop();
        _elapsed = 0f;
        Opacity = 1f;
        InputTransparent = false;
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(32);
        _timer.Tick += Tick;
        _timer.Start();
    }

    public void Hide()
    {
        _timer?.Stop();
        _timer = null;
        Opacity = 0f;
        InputTransparent = true;
    }

    private void Tick(object? sender, EventArgs e)
    {
        _elapsed += 16f;
        float t = (_elapsed % CycleMs) / CycleMs;

        if (t < 0.70f)
        {
            _ecg.DrawProgress = t / 0.70f;
            _ecg.Opacity = 1f;
        }
        else if (t < 0.85f)
        {
            _ecg.DrawProgress = 1f;
            _ecg.Opacity = 1f - (t - 0.70f) / 0.15f;
        }
        else
        {
            _ecg.DrawProgress = 0f;
            _ecg.Opacity = 0f;
        }

        EcgView.Invalidate();
    }
}

namespace WorkoutLogg.Pages.Controls;

public partial class LoadingOverlayView : ContentView
{
    private readonly EcgDrawable _ecg = new();
    private IDispatcherTimer? _timer;
    private readonly System.Diagnostics.Stopwatch _animationTime = new();
    private const float CycleMs = 1800f;

    public LoadingOverlayView()
    {
        InitializeComponent();
        EcgView.Drawable = _ecg;
        Unloaded += (_, _) => StopAnimation();
    }

    // Показывает оверлей сразу (без таймера) — вызывать из конструктора и OnDisappearing
    public void Preload()
    {
        StopAnimation();
        _ecg.DrawProgress = 0.35f;
        _ecg.Opacity = 0.7f;
        EcgView.Invalidate();
        Opacity = 1f;
        InputTransparent = false;
    }

    // Показывает с анимацией — вызывать в начале OnAppearing
    public void Show()
    {
        StopAnimation();
        _animationTime.Restart();
        _ecg.DrawProgress = 0.35f;
        _ecg.Opacity = 0.7f;
        Opacity = 1f;
        InputTransparent = false;
        EcgView.Invalidate();
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(32);
        _timer.Tick += Tick;
        _timer.Start();
    }

    public void Hide()
    {
        StopAnimation();
        Opacity = 0f;
        InputTransparent = true;
    }

    // На Android отдаём кадр отрисовке до создания следующей страницы.
    // Два callback vsync позволяют пройти одному циклу рисования между ними.
    public async Task ShowAndRenderAsync()
    {
        Show();
#if ANDROID
        if (Handler?.PlatformView is Android.Views.View view && view.IsAttachedToWindow)
        {
            var rendered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            using var secondFrame = new Java.Lang.Runnable(() => rendered.TrySetResult());
            using var firstFrame = new Java.Lang.Runnable(() => view.PostOnAnimation(secondFrame));
            view.PostOnAnimation(firstFrame);
            try
            {
                // Приложение могло уйти в фон или потерять окно между кадрами.
                await rendered.Task.WaitAsync(TimeSpan.FromMilliseconds(250));
            }
            catch (TimeoutException)
            {
                // Не задерживаем навигацию, если окно больше не рисуется.
            }
            finally
            {
                view.RemoveCallbacks(firstFrame);
                view.RemoveCallbacks(secondFrame);
            }
            return;
        }
#endif
        await Dispatcher.DispatchAsync(() => { });
    }

    private void StopAnimation()
    {
        _timer?.Stop();
        _timer = null;
        _animationTime.Stop();
    }

    private void Tick(object? sender, EventArgs e)
    {
        float t = (_animationTime.ElapsedMilliseconds % CycleMs) / CycleMs;

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

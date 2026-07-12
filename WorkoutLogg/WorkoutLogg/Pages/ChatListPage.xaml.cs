using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class ChatListPage : ContentPage
{
    private readonly ChatListPageModel _vm;

    public ChatListPage(ChatListPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        PageLoading.Preload();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        PageLoading.Show();
        try
        {
            await _vm.LoadAsync();
        }
        finally
        {
            PageLoading.Hide();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PageLoading.Preload();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnConversationTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not ConversationItem item) return;
        await Shell.Current.GoToAsync($"ChatThread?conversationId={item.Id}");
    }
}

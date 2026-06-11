using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;
using Modules.Users.Infrastructure.Authorization;

namespace WorkoutLogg.Pages;

public partial class OnboardingProfilePage : ContentPage
{
    private UserSex _selectedSex = UserSex.Male;
    private string? _profilePictureDataUrl;
    public IAuthApi AuthApi { get; set; }
    public OnboardingProfilePage()
    {
        InitializeComponent();
        BirthDatePicker.MaximumDate = DateTime.Today;
        AuthApi = Application.Current!.Handler.MauiContext!.Services
            .GetRequiredService<IAuthApi>();
    }

    private void OnFullNameChanged(object sender, TextChangedEventArgs e)
    {
        var name = e.NewTextValue?.Trim();
        AvatarLabel.Text = string.IsNullOrEmpty(name) ? "A" : name[0].ToString().ToUpper();
    }

    private async void OnChangeAvatarTapped(object sender, EventArgs e)
    {
        var photo = await MediaPicker.PickPhotoAsync();
        if (photo is null) return;

        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        _profilePictureDataUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";

        AvatarImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        AvatarInitialsBorder.IsVisible = false;
        AvatarPhotoBorder.IsVisible = true;
    }

    private void OnMaleSelected(object sender, EventArgs e) => SetSex("Male");
    private void OnFemaleSelected(object sender, EventArgs e) => SetSex("Female");
    private void OnOtherSelected(object sender, EventArgs e) => SetSex("Other");

    private void SetSex(string sex)
    {
        // Сброс всех
        MaleBorder.BackgroundColor = Colors.White; MaleBorder.Stroke = Color.FromArgb("#E5E7EB"); MaleBorder.StrokeThickness = 1.5; MaleButton.TextColor = Color.FromArgb("#374151");
        FemaleBorder.BackgroundColor = Colors.White; FemaleBorder.Stroke = Color.FromArgb("#E5E7EB"); FemaleBorder.StrokeThickness = 1.5; FemaleButton.TextColor = Color.FromArgb("#374151");

        // Активный
        switch (sex)
        {
            case "Male":
                _selectedSex = UserSex.Male;
                MaleBorder.BackgroundColor = Color.FromArgb("#7C3AED");
                MaleBorder.StrokeThickness = 0;
                MaleButton.TextColor = Colors.White;
                break;
            case "Female":
                _selectedSex = UserSex.Female;
                FemaleBorder.BackgroundColor = Color.FromArgb("#7C3AED");
                FemaleBorder.StrokeThickness = 0;
                FemaleButton.TextColor = Colors.White;
                break;
        }
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var user = new UserDto()
        {
            DateOfBirth = BirthDatePicker.Date,
            Identity = _selectedSex,
            UserRegistrationStep = Modules.Users.DTO.Users.UserRegistrationStep.Body,
            ProfilePicture = _profilePictureDataUrl
        };
        var currentToken = await LoginService.GetActiveToken();
        var updated = await AuthApi.UpdateAccount($"Bearer {currentToken}", user);
        if (updated.IsSuccessStatusCode)
        {
            Application.Current!.Windows[0].Page = new OnboardingBodyStatsPage();
        }
    }
}
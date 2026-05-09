using CommunityToolkit.Mvvm.Input;
using WorkoutLogg.Models;

namespace WorkoutLogg.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}
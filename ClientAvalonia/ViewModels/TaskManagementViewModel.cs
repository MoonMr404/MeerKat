using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class TaskManagementViewModel : ViewModelBase {
    public ObservableCollection<TaskListTemplate> taskListList { get; set; } = new();
    
    private UserService userService;
    private TeamService teamService;
    private TaskListService taskListService;
    private UserDto userDto;
    private MainWindowViewModel mainWindowViewModel;

    public TaskManagementViewModel(MainWindowViewModel mainWindowViewModel)
    {
        userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        teamService = Locator.Current.GetService<TeamService>() ?? throw new InvalidOperationException();
        taskListService = Locator.Current.GetService<TaskListService>() ?? throw new InvalidOperationException();
        this.mainWindowViewModel = mainWindowViewModel;
        LoadTasksAsync();
    }
    
    public async Task LoadTasksAsync()
    {
        var taskLists = await taskListService.GetTaskListsAsync(true);
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            taskListList.Clear();
            foreach (TaskListDto taskList in taskLists)
            {
                taskListList.Add(new TaskListTemplate(taskList));
            }
        });
    } 
}

public partial class TaskListTemplate : ObservableObject
{
    public TaskListDto taskLists { get; }
    public ObservableCollection<TaskTemplate> taskTemplates { get; set; } = new();
 
    public TaskListTemplate(TaskListDto taskLists)
    {
        this.taskLists = taskLists;
        if (taskLists.Tasks != null)
        {
            foreach (TaskDto task in taskLists.Tasks)
            {
                taskTemplates.Add(new TaskTemplate(task));
            }
        }
    }
    
    [RelayCommand]
    public void editTaskList()
    {
      
    }
    
}

public partial class TaskTemplate : ObservableObject
{
    public TaskDto task { get; }
    public TaskTemplate(TaskDto task)
    {
        this.task  = task;
    }
    
    [RelayCommand]
    public void editTask()
    {
      
    }
    
}
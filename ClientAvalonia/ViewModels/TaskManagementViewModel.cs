using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClientAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;
using Splat;

namespace ClientAvalonia.ViewModels;

public partial class TaskManagementViewModel : ViewModelBase {
    public ObservableCollection<TaskListTemplate> taskListList { get; set; } = new();
    public ObservableCollection<UserTemplate> membersList { get; set; } = new();
    
    private UserService _userService;
    private TaskService _taskService;
    private TaskListService _taskListService;
    private UserDto _userDto;
    [ObservableProperty] public MainWindowViewModel _mainWindowViewModel;
    
    [ObservableProperty] private string _taskListName;
    [ObservableProperty] private string _taskListDescription;
    [ObservableProperty] private bool _taskListPopupEnabled;
    [ObservableProperty] private string _taskName;
    [ObservableProperty] private string _taskDescription;
    [ObservableProperty] private DateOnly _taskDeadline;
    [ObservableProperty] public bool _taskPopupEnabled;
    [ObservableProperty] private bool _isLeader;
    
    public TaskListDto _selectedTaskListDto;

    public TaskManagementViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _userService = Locator.Current.GetService<UserService>() ?? throw new InvalidOperationException();
        _taskService = Locator.Current.GetService<TaskService>() ?? throw new InvalidOperationException();
        _taskListService = Locator.Current.GetService<TaskListService>() ?? throw new InvalidOperationException();
        this._mainWindowViewModel = mainWindowViewModel;
        LoadTasksAsync();
        LoadMembersAsync();
    }
    
    

    public async Task LoadUsersAsync()
    {
        _userService.GetUserSelfAsync(true);
        var user= await _userService.GetUserSelfAsync(true);
        _userDto = user;
    }
    
    public async Task LoadTasksAsync()
    {
        await LoadUsersAsync();
        var taskLists = await _taskListService.GetTaskListByTeamAsync(_mainWindowViewModel.SelectedTeam.Id, true);
        IsLeader = _mainWindowViewModel.SelectedTeam.ManagerId == _userDto.Id;
        
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            taskListList.Clear();
            foreach (TaskListDto taskList in taskLists)
            {
                taskListList.Add(new TaskListTemplate(taskList, this));
            }
        });
    } 
    
    public async Task LoadMembersAsync()
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            membersList.Clear();
            foreach (UserDto user in _mainWindowViewModel.SelectedTeam.Members)
            {
                membersList.Add(new UserTemplate(user));
            }
        });
    } 
    
    [RelayCommand]
    public async void createTaskList()
    {
        var taskListDto = new TaskListDto()
        {
            Id = Guid.Empty,
            Name = _taskListName,
            Description = _taskListDescription,
            TeamId = _mainWindowViewModel.SelectedTeam.Id
        };
        try
        {
            await _taskListService.CreateTaskListAsync(taskListDto);
            await LoadTasksAsync();
            TaskListPopupEnabled = false;
        }
        catch (Exception ex)
        {
            
        }
    }
    
    [RelayCommand]
    public async void createTask()
    {
        var task = new TaskDto()
        {
            Id = Guid.Empty,
            Name = TaskName,
            Description = TaskDescription,
            Deadline = TaskDeadline,
            Status = "Da completare",
            TaskListId = _selectedTaskListDto.Id
        };
        try
        {
            await _taskService.CreateTaskAsync(task);
            await LoadTasksAsync();
            TaskPopupEnabled = false;
        }
        catch (Exception ex)
        {
            
        }
    }
}

public partial class TaskListTemplate : ObservableObject
{
    public TaskListDto taskList { get; }
    public ObservableCollection<TaskTemplate> taskTemplates { get; set; } = new();
    private TaskListService _taskListService;
    private TaskManagementViewModel _taskManagementViewModel;
 
    public TaskListTemplate(TaskListDto taskList, TaskManagementViewModel taskManagementViewModel)
    {
        _taskListService = Locator.Current.GetService<TaskListService>() ?? throw new InvalidOperationException();
        _taskManagementViewModel = taskManagementViewModel;
        this.taskList = taskList;
        if (taskList.Tasks != null)
        {
            var tasksOrdered = taskList.Tasks.OrderBy(t => t.Status);
            foreach (TaskDto task in tasksOrdered)
            {
                taskTemplates.Add(new TaskTemplate(task,_taskManagementViewModel));
            }
        }
    }
    
    [RelayCommand]
    public void addTask()
    {
        _taskManagementViewModel._selectedTaskListDto = taskList;
        _taskManagementViewModel.TaskPopupEnabled = true;
    }
    
    [RelayCommand]
    public async void deleteTaskList()
    {
        await _taskListService.DeleteTaskListAsync(taskList.Id);
        await _taskManagementViewModel.LoadTasksAsync();
    }
}

public partial class TaskTemplate : ObservableObject
{
    public TaskDto task { get; }
    public String IsChecked { get; private set; }
    public bool IsEnabled { get; private set; }
    
    private TaskService _taskService;
    private TaskManagementViewModel _taskManagementViewModel;
    public TaskTemplate(TaskDto task, TaskManagementViewModel taskManagementViewModel)
    {
        this.task  = task;
        _taskService = Locator.Current.GetService<TaskService>() ?? throw new InvalidOperationException();
        _taskManagementViewModel = taskManagementViewModel;

        switch (task.Status)
        {
            case "Da completare":
                IsEnabled = true;
                IsChecked = "False";
                break;
            case "Consegnata":
                IsEnabled = false;
                IsChecked = "True";
                break;
            case "Consegnata in ritardo":
                IsEnabled = false;
                IsChecked = "{x:Null}";
                break;
        }
    }

    [RelayCommand]
    public async void completeTask()
    {
        await _taskService.CompleteTaskAsync(task.Id);
        await _taskManagementViewModel.LoadTasksAsync();
    }
    
    [RelayCommand]
    public void editTask()
    {
      
    }
    
}

public partial class UserTemplate : ObservableObject
{
    public UserDto user { get; }
    public UserTemplate(UserDto user)
    {
        this.user = user;
    }
    
    [RelayCommand]
    public void toggleDeleteMember()
    {
        
    }
    
}
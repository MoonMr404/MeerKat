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
    
    private UserService _userService;
    private TaskService _taskService;
    private TaskListService _taskListService;
    private UserDto _userDto;
    private MainWindowViewModel _mainWindowViewModel;
    
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
            Status = "Da consegnare",
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
            foreach (TaskDto task in taskList.Tasks)
            {
                taskTemplates.Add(new TaskTemplate(task));
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
    public TaskTemplate(TaskDto task)
    {
        this.task  = task;
    }
    
    [RelayCommand]
    public void editTask()
    {
      
    }
    
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;

namespace ClientAvalonia.ViewModels;

public partial class TaskManagementViewModel : ViewModelBase
{
    
 
    public ObservableCollection<TaskDto> taskList { get; set; } = new();
    public ObservableCollection<TaskListTemplate> taskListList { get; set; } = new();
    
    
    public static ObservableCollection<TaskDto> taskList2 { get; set; } = new()
    {
        new TaskDto(){ Id = new Guid(), Name = "Task 1", Description = "task d'esempio numero 1", TaskListId = new Guid(), Deadline = DateOnly.MaxValue},
        new TaskDto(){ Id = new Guid(), Name = "Task 2", Description = "task d'esempio numero 2", TaskListId = new Guid(), Deadline = DateOnly.MaxValue},
    };
    
    public ObservableCollection<TaskListTemplate> taskListList2 { get; set; } = new()
    {
        new TaskListTemplate(new TaskListDto()
            { Id = new Guid(), Name = "Tasklist 1", Description = "Lista di task numero 1", TeamId = Guid.NewGuid(), Tasks = taskList2}),
        new TaskListTemplate(new TaskListDto()
            { Id = new Guid(), Name = "Tasklist 2", Description = "Lista di task numero 2", TeamId = Guid.NewGuid(), Tasks = null}),
    };
    
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
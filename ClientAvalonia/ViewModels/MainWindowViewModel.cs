using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Dto;

namespace ClientAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
     [ObservableProperty]
     private bool _IsPaneOpen = true;

     [ObservableProperty] private ViewModelBase _currentPage;

     [ObservableProperty] private PageListTemplate? _selectedListItem =  new PageListTemplate(typeof(UserViewModel), "personregular");
     
     [ObservableProperty]
     private TeamDto? _selectedTeam;

     public MainWindowViewModel()
     {
          SelectedListItem = Pages.First();
     }
     
     public ObservableCollection<PageListTemplate> Pages { get; } = new()
     {
          new PageListTemplate(typeof(UserViewModel), "personregular"),
          new PageListTemplate(typeof(TeamsViewModel), "peoplecommunityregular"),
          new PageListTemplate(typeof(TaskManagementViewModel), "peoplecommunityregular", false)
     };
     
     partial void OnSelectedListItemChanged(PageListTemplate? value)
     {
          if (value is null) return;
          var instance= Activator.CreateInstance(value.ModelType, this);
          if (instance is null) return;
          if (value.ModelType == typeof(TaskManagementViewModel) && SelectedTeam is null) return;
          CurrentPage = (ViewModelBase)instance;
     }
     
     
     [RelayCommand]
     private void TriggerPane()
     {
          IsPaneOpen = !IsPaneOpen;
     }
}

public class PageListTemplate
{
     public PageListTemplate(Type type, String iconKey, bool isEnabled = true)
     {
          ModelType = type;
          Label= type.Name.Replace("ViewModel", "");
          IsEnabled = isEnabled;

          Application.Current!.TryFindResource(iconKey, out var res);
          ListItemIcon = (StreamGeometry)res!;
     }

     public String Label { get; }
     public Type ModelType { get; }
     public StreamGeometry ListItemIcon { get; }
     public bool IsEnabled { get; }
}
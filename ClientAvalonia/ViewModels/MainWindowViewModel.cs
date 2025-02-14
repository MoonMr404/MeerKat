using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
     [ObservableProperty]
     private bool _IsPaneOpen = true;

     [ObservableProperty] private ViewModelBase _currentPage;

     [ObservableProperty] private PageListTemplate? _selectedListItem =  new PageListTemplate(typeof(UserViewModel), "personregular");

     public MainWindowViewModel()
     {
          SelectedListItem = Pages.First();
     }
     
     public ObservableCollection<PageListTemplate> Pages { get; } = new()
     {
          new PageListTemplate(typeof(UserViewModel), "personregular"),
          new PageListTemplate(typeof(TeamsViewModel), "peoplecommunityregular")
     };
     
     partial void OnSelectedListItemChanged(PageListTemplate? value)
     {
          if (value is null) return;
          var instance= Activator.CreateInstance(value.ModelType);
          if (instance is null) return;
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
     public PageListTemplate(Type type, String iconKey)
     {
          ModelType =type;
          Label= type.Name.Replace("ViewModel", "");

          Application.Current!.TryFindResource(iconKey, out var res);
          ListItemIcon = (StreamGeometry)res!;
     }

     public String Label { get; }
     public Type ModelType { get; }
     public StreamGeometry ListItemIcon { get; }
}
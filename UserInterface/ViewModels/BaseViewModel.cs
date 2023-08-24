﻿namespace UserInterface.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

///<summary>
/// Automatically implements INotifyPropertyChanged and INotifyPropertyChanging.
/// Can also do source generation for our properties
/// </summary>
public partial class BaseViewModel : ObservableObject
{
  public BaseViewModel()
  {
  }

  // The public variations of these properties have been generated by source generators
  // To view this generated code: Depenendcies -> MVVM.SourceGenerators => ObservablePropertyGenerator
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(IsNotBusy))]
  bool isBusy;

  [ObservableProperty]
  string title;

  public bool IsNotBusy => !IsBusy;
}
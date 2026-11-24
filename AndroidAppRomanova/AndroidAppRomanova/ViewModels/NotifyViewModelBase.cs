using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidAppRomanova.ViewModels
{
    public abstract partial class NotifyViewModelBase : ViewModelBase
    {
        [ObservableProperty] private bool isNotificationVisible;
        [ObservableProperty] private string notificationTitle = "";
        [ObservableProperty] private string notificationMessage = "";

        protected void ShowNotification(string title, string message)
        {
            NotificationTitle = title;
            NotificationMessage = message;
            IsNotificationVisible = true;
        }

        [RelayCommand]
        private void CloseNotification() => IsNotificationVisible = false;
    }

}

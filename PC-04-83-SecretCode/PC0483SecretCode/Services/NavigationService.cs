using System;
using PC0483SecretCode.ViewModels;
using ReactiveUI;

namespace PC0483SecretCode.Services
{
    public class NavigationService : ReactiveObject
    {
        public const string ConverterName = "Converter";
    
        private ViewModelBase _currentPage;
        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage is IDisposable disposable)
                    disposable.Dispose();  // giải phóng cái cũ
    
                this.RaiseAndSetIfChanged(ref _currentPage, value);
                // RaisePropertyChanged nếu bạn dùng ReactiveUI hoặc INotifyPropertyChanged
            }
        }

        public NavigationService()
        {
            CurrentPage = new ConverterViewModel();
        }
    
        public void GoBack()
        {
        
        }

        public void NavigateTo(string pageName)
        {
            switch (pageName)
            {
                case ConverterName:
                    CurrentPage = new ConverterViewModel();
                    break;
            }
        }
    }
}
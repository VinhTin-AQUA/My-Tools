using PC0483SecretCode.Services;
using Splat;

namespace PC0483SecretCode.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public NavigationService NavigationService { get; set; }

        public MainWindowViewModel(NavigationService? navigationService = null)
        { 
            this.NavigationService = navigationService ?? Locator.Current.GetService<NavigationService>()!;
        }
        
        public override void Dispose()
        {
            // các lệnh giải phóng tài nguyên
            base.Dispose();
        }
    }
}
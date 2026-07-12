using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using PC0483SecretCode.Models;
using PC0483SecretCode.Services;
using Splat;

namespace PC0483SecretCode.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        public NavigationService NavigationService { get; set; }

        public ObservableCollection<SidebarItem> Items { get; set; } = [
        new()
        {
            Icon = "avares://PC0483SecretCode/Assets/Svg/convert.svg",
            Name = "Converter"
        }];

        public SidebarViewModel()
        {
            this.NavigationService = Locator.Current.GetService<NavigationService>()!;
        }
            
        public override void Dispose()
        {
            // các lệnh giải phóng tài nguyên
            base.Dispose();
        }
    }
}
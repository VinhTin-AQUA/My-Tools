using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PC0483SecretCode.Controls
{
    public partial class SidebarView : UserControl
    {
        private bool sidebarVisible = true;
    
        public SidebarView()
        {
            InitializeComponent();
        }

        private void Toggle_Sidebar_OnClick(object? sender, RoutedEventArgs e)
        {
            if (sidebarVisible)
            {
                SidebarColumn.Width = 50;
                sidebarVisible = false;
                SidebarLabel.IsVisible = false;
                return;
            }
        
            SidebarColumn.Width = 200;
            sidebarVisible = true;
            SidebarLabel.IsVisible = true;
        }
    }
}
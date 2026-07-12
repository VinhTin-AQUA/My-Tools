using ReactiveUI;

namespace PC0483SecretCode.Models
{
    public class SidebarItem : ReactiveObject
    {
        public string Name { get; set; } = "";
        public string Icon  { get; set; } = "";
    }
}
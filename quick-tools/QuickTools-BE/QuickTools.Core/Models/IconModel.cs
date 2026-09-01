using QuickTools.Core.Enums;

namespace QuickTools.Core.Models
{
    public class IconModel : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public EIconType IconType { get; set; }
    }
}
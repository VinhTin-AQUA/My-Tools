using Microsoft.AspNetCore.Components;

namespace QuickTools.MobileTemplate.Components.Shared
{
    public partial class UiButton
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string Type { get; set; } = "button";

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }
    }
}
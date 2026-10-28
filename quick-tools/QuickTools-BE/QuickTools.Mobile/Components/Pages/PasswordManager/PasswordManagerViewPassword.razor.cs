using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerViewPassword : ComponentBase
    {
        [Parameter]
        public GoogleSheetPasswordModel? Password { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        private bool _showPassword;

        private sealed record ViewPasswordField(string Label, string Value);

        private List<ViewPasswordField> Fields
        {
            get
            {
                if (Password is null)
                {
                    return [];
                }

                return
                [
                    new("ID", Password.Id.ToString()),
                    new("Account Name", Password.AccountName),
                    new("Username", Password.UserName),
                    new("Password", Password.Password),
                    new("Note", Password.Note),
                ];
            }
        }

        private string DisplayValue(ViewPasswordField field)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                return "—";
            }

            if (field.Label == "Password" && !_showPassword)
            {
                return "••••••••";
            }

            return field.Value;
        }

        private void TogglePasswordVisibility()
        {
            _showPassword = !_showPassword;
        }

        private async Task Close()
        {
            await VisibleChange.InvokeAsync(false);
        }

        private void CopyValue(ViewPasswordField field)
        {
            Console.WriteLine($"[PasswordManager] Copy {field.Label}: {field.Value}");
        }
    }
}

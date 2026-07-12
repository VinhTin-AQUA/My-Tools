using PC0483SecretCode.Services;
using Splat;

namespace PC0483SecretCode.Bootstraper
{
    public class AppBootstrapper
    {
        public AppBootstrapper()
        {
            Locator.CurrentMutable.RegisterConstant(new NavigationService(), typeof(NavigationService)); // 
        }
    }
}
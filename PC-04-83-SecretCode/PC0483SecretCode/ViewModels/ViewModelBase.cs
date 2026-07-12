using System;
using ReactiveUI;

namespace PC0483SecretCode.ViewModels
{
    public class ViewModelBase : ReactiveObject,  IDisposable
    { 
        public virtual void Dispose()
        {
            // override trong ViewModel con để giải phóng resource
        }
    }
}
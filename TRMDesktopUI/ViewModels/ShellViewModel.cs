using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {

        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private SimpleContainer _container;
        private ILoggedInUserModel _loggedInUser;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel loggedInUser)
        {
            _events = events;
            _salesVM = salesVM;
            _loggedInUser = loggedInUser;

            _events.Subscribe(this);

            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_loggedInUser.Token);
            }
        }

        public void Handle(LogOnEvent message)
        {
            // Close Login screen and show Sales view
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            // Clear credentials
            _loggedInUser.LogOffUser();

            // Close Sales View
            // Show login view
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}

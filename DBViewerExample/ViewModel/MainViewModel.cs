using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace DBViewerExample.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                Set(nameof(CurrentViewModel), ref _currentViewModel, value);
            }
        }

        private ViewModelBase _subViewModel;

        public ViewModelBase SubViewModel
        {
            get
            {
                return _subViewModel;
            }
            set
            {
                Set(nameof(SubViewModel), ref _subViewModel, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<HomeViewModel>();
        }
    }
}
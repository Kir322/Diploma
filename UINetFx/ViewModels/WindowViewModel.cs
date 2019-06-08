using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UINetFx.Commands;

namespace UINetFx.ViewModels
{
    public class WindowViewModel : BaseViewModel
    {
        private readonly Window window;
        private readonly DataImportViewModel dataVM = new DataImportViewModel();
        private readonly ClusteringViewModel clusteringVM = new ClusteringViewModel();
        private readonly RepresentationViewModel representationVM = new RepresentationViewModel();
        private object currentView;

        public WindowViewModel(Window window)
        {
            this.window = window;
            this.CurrentView = this.dataVM;

            this.Close = new RelayCommand(_ => { this.window.Close(); });
            this.Maximize = new RelayCommand(_ => { this.window.WindowState ^= WindowState.Maximized; });
            this.Minimize = new RelayCommand(_ => { this.window.WindowState = WindowState.Minimized; });
            this.ShowSystemMenu = new RelayCommand(_ => { SystemCommands.ShowSystemMenu(this.window, GetMousePosition()); });
            this.ImportData = new RelayCommand(_ => { this.CurrentView = this.dataVM; });
            this.Clustering = new RelayCommand(_ => { this.CurrentView = this.clusteringVM; });
            this.Representation = new RelayCommand(_ => { this.CurrentView = this.representationVM; });
        }

        public ICommand Close { get; private set; }
        public ICommand Maximize { get; private set; }
        public ICommand Minimize { get; private set; }
        public ICommand ShowSystemMenu { get; private set; }
        public ICommand ImportData { get; private set; }
        public ICommand Clustering { get; private set; }
        public ICommand Representation { get; set; }
        public object CurrentView
        {
            get { return this.currentView; }
            set
            {
                this.currentView = value;
                this.OnPropertyChanged(nameof(currentView));
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}

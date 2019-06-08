using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UINetFx.ViewModels;

namespace UINetFx.Views
{
    /// <summary>
    /// Interaction logic for RepresentationView.xaml
    /// </summary>
    public partial class RepresentationView : UserControl
    {
        public RepresentationView()
        {
            InitializeComponent();

            this.DataContext = new RepresentationViewModel();
        }
    }
}

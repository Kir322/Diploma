using Diploma.Core.Data;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Windows.Input;
using UINetFx.Commands;

namespace UINetFx.ViewModels
{
    public class DataImportViewModel : BaseViewModel
    {
        private DataView dataView;
        private string fileName;

        public DataImportViewModel()
        {
            this.ImportFile = new RelayCommand(this.Import);
        }

        public DataView Data
        {
            get => this.dataView;
            set
            {
                this.dataView = value;
                this.OnPropertyChanged(nameof(this.Data));
            }
        }
        public string FileName { get { return this.fileName; } set { this.fileName = value; this.OnPropertyChanged(nameof(this.fileName)); } }

        public ICommand ImportFile { get; }

        private void Import(object _)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Import Data to Clustering App"
            };

            if (dialog.ShowDialog().Value)
            {
                this.FileName = Path.GetFileNameWithoutExtension(dialog.FileName);

                this.Data = DataFrame.ReadFromCsv(dialog.FileName).ToDataTable().DefaultView;
            }
        }
    }
}

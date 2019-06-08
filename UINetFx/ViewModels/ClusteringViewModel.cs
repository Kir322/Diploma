using System;

namespace UINetFx.ViewModels
{
    public partial class ClusteringViewModel : BaseViewModel
    {
        private int maxiterations = 100;
        private float error = 1e-5f;
        private int numberOfClusters = 3;
        private CentroidDeterminingAlgorithmType cdat = CentroidDeterminingAlgorithmType.RandomPartitioning;
        private bool normalize = true;

        public int MaxIterations
        {
            get => this.maxiterations;
            set
            {
                this.maxiterations = value;
                this.OnPropertyChanged(nameof(this.maxiterations));
            }
        }

        public float Error
        {
            get => this.error;
            set
            {
                this.error = value;
                this.OnPropertyChanged(nameof(this.error));
            }
        }

        public int NumberOfClusters
        {
            get => this.numberOfClusters;
            set
            {
                this.numberOfClusters = value;
                this.OnPropertyChanged(nameof(this.numberOfClusters));
            }
        }

        public CentroidDeterminingAlgorithmType CentroidDeterminingAlgorithm
        {
            get => this.cdat;
            set
            {
                this.cdat = value;
                this.OnPropertyChanged(nameof(this.cdat));
            }
        }

        public bool Normalize
        {
            get => this.normalize;
            set
            {
                this.normalize = value;
                this.OnPropertyChanged(nameof(this.normalize));
            }
        }
    }
}
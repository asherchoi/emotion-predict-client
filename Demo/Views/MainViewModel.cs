using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect.Face;

using Demo.Models;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using System.Windows.Media;

using System.Windows.Controls;
using LiveCharts;

using Demo.Data;

namespace Demo.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private HdFaceTrackingModel model = new HdFaceTrackingModel();
        //private Image[] = an, fe, ha, ne, sa, su;

        public MainViewModel()
        {
            this.model.PropertyChanged += OnModelPropertyChanged;
        }

        #region "Property"
        
        //public MeshGeometry3D Geometry3d
        //{
        //    get { return this.model.Geometry3d; }
        //}

        //public string FaceModelBuilderStatus
        //{
        //    get { return this.model.FaceModelBuilderStatus; }
        //}

        //public string FaceModelCaptureStatus
        //{
        //    get { return this.model.FaceModelCaptureStatus; }
        //}

        // public IReadOnlyDictionary<FaceShapeAnimations, float> AnimationUnits
        //{
        //    get { return this.model.AnimationUnits; }
        //}
        
        public dynamic EmotionUnits
        {
            get { return this.model.EmotionUnits; }
        }

        public ImageSource ImageSource
        {
            get { return this.model.imageSource; }
        }

        public SeriesCollection SeriesCollection { get { return this.model.SeriesCollection; } }
        public string[] Labels { get { return this.model.Labels; } }
        public Func<double, string> Formatter { get { return this.model.Formatter; } }

        //public Brush SkinColor
        //{
        //    get { return new SolidColorBrush(this.model.SkinColor); }
        //}

        //public Brush HairColor
        //{
        //    get { return new SolidColorBrush(this.model.HairColor); }
        //}
        //public List<string> Labels
        //{
        //    get
        //    {
        //        return this.model.labels;
        //    }
        //}

        //public SeriesCollection SeriesBar
        //{
        //    get
        //    {
        //        return this.model.seriesBar;
        //    }

        //}

        #endregion

        public void StartCommand()
        {
            this.model.Start();
        }

        public void LoadedCommand()
        {
            this.model.Loaded();
            //
        }

        public void StopCommand()
        {
            this.model.Stop();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if(propertyName!=null)
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}

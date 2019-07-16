using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiveCharts;
using Microsoft.Kinect.Face;
namespace Demo
{
    class ViewModelBase : INotifyPropertyChanged
    {
        List<string> labels = new List<string>();
        List<double> data = new List<double>();
        SeriesCollection seriesBar = new SeriesCollection();
        SeriesCollection seriesPie = new SeriesCollection();

        public List<string> Labels
        {
            get
            {
                return labels;
            }

            set
            {
                labels = value;
                RaisePropertyChanged("Labels");
            }
        }

        public SeriesCollection SeriesBar
        {
            get
            {
                return seriesBar;
            }

            set
            {
                seriesBar = value;
                RaisePropertyChanged("SeriesBar");
            }
        }

        public SeriesCollection SeriesPie
        {
            get
            {
                return seriesPie;
            }

            set
            {
                seriesPie = value;
                RaisePropertyChanged("SeriesPie");
            }
        }

        public ViewModelBase(string[] label)
        {
            data = new List<double>();
            Labels = new List<string>();

            foreach(string temp in label)
            {
                Labels.Add(temp);
            }
        }

        /*
        public void UpdateModel(IReadOnlyDictionary<FaceShapeAnimations, float> AUs, string title)
        {
            ChartValues<double> cv = new ChartValues<double>();

            foreach(KeyValuePair<FaceShapeAnimations, float> temp in AUs)
            {
                this.data.Add((double)temp.Value);
            }

            cv.AddRange(data);

            //var barSeries = new Bar
        }
        */

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}

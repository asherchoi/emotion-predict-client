using System;
using System.Windows.Controls;
using LiveCharts;


namespace Demo.Graph
{
    /// <summary>
    /// PiGraph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PiGraph : UserControl
    {
        public PiGraph()
        {
            InitializeComponent();
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = this;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }
    }
}

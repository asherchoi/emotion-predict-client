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
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public event Action<string> UserName;
        public event Action<string> FrameRate;

        public SettingWindow()
        {
            InitializeComponent();

            ////fire the event
            //if (UserName != null)
            //    this.UserName(this.NameAnswer.Text);
            //if (FrameRate != null)
            //    this.FrameRate("fps5");  // Default fps5
           
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (UserName != null)
                this.UserName(this.NameAnswer.Text);
            if (FrameRate != null)
                this.FrameRate(FrameRateState());           
           
            this.DialogResult = true;
        }

        private string FrameRateState()
        {
            if (this.fps1.IsChecked == true)
                return "fps1";
            if (this.fps5.IsChecked == true)
                return "fps5";
            if (this.fps10.IsChecked == true)
                return "fps10";

            return "fps1"; //Default
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.UserName("Default");
            this.FrameRate("fps1");
            this.DialogResult = true;
        }

        private void fps5_Click(object sender, RoutedEventArgs e)
        {
            this.fps10.IsChecked = false;
            this.fps1.IsChecked = false;
        }

        private void fps10_Click(object sender, RoutedEventArgs e)
        {
            this.fps5.IsChecked = false;
            this.fps1.IsChecked = false;
        }

        private void fps1_Click(object sender, RoutedEventArgs e)
        {
            this.fps5.IsChecked = false;
            this.fps10.IsChecked = false;
        }

       
    }
}

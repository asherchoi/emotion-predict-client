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
    /// DemoSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DemoSettingWindow : Window
    {
        public event Action<string> UserName;
        public event Action<string> FrameRate;
        public event Action<string> SlideEmotion;
        public event Action<string> SlideState;

        public DemoSettingWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (UserName != null)
                this.UserName(this.NameAnswer.Text);

            if (SlideEmotion != null)
                this.SlideEmotion(SlideEmotionState());

            if (SlideState != null)
                this.SlideState(_SlideState());

            this.FrameRate("5fps");

            this.DialogResult = true;
        }

        private string _SlideState()
        {
            if (this.Image.IsChecked == true)
                return "Image";
            if (this.Video.IsChecked == true)
                return "Video";

            return "Image"; //Default
        }

        private string SlideEmotionState()
        {
            if (this.Neutral.IsChecked == true)
                return "Neutral";
            if (this.Happy.IsChecked == true)
                return "Happy";
            if (this.Surprise.IsChecked == true)
                return "Surprise";
            if (this.Angry.IsChecked == true)
                return "Angry";
            if (this.Sad.IsChecked == true)
                return "Sad";
            if (this.Disgust.IsChecked == true)
                return "Disgust";
            if (this.Contempt.IsChecked == true)
                return "Contempt";
            if (this.Fear.IsChecked == true)
                return "Fear";
            if (this.FacialExpression.IsChecked == true)
                return "FacialExpression";

            return "Neutral"; //Default
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.UserName("Default");
            this.FrameRate("fps5");
            this.DialogResult = true;
        }

        #region "Demo Style"
        private void Image_Click(object sender, RoutedEventArgs e)
        {
            this.Video.IsChecked = false;
        }

        private void Video_Click(object sender, RoutedEventArgs e)
        {
            this.Image.IsChecked = false;
        }
        #endregion

        #region "Related Emotion"
        private void Neutral_Click(object sender, RoutedEventArgs e)
        {
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Happy_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Surprise_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Angry_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Sad_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Disgust_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Contempt_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Fear.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Fear_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.FacialExpression.IsChecked = false;
        }

        private void Expression_Click(object sender, RoutedEventArgs e)
        {
            this.Neutral.IsChecked = false;
            this.Happy.IsChecked = false;
            this.Surprise.IsChecked = false;
            this.Angry.IsChecked = false;
            this.Sad.IsChecked = false;
            this.Disgust.IsChecked = false;
            this.Contempt.IsChecked = false;
            this.Fear.IsChecked = false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Demo.ViewModels;
using Demo.Models;
using Demo.Graph;

using LiveCharts.Wpf;
using System.ComponentModel;
using LiveCharts;

namespace Demo
{
    public partial class MainWindow : Window
    {
        #region "variables"
        private MainViewModel ViewModel = new MainViewModel();

        /// <summary>
        /// Face frame counter
        /// </summary>
        public static int FaceFrameCount = 1;
       
        /// <summary>
        /// frame rate state
        /// Default = 5 frame per seconds
        /// </summary>
        public static string FrameRateState = "1fps"; //Default: 5fps

        /// <summary>
        /// record state
        /// </summary>
        public static bool RecordStarted;

        /// <summary>
        /// Emotion state
        /// </summary>
        public static string Emotion = "End_Sampling";

        /// <summary>
        /// Recognition Emotion State
        /// </summary>
        public static string RecogEmotion = "None";

        /// <summary>
        /// User name
        /// </summary>
        public static string UserName = "Default"; // Default

        /// <summary>
        /// Image Slide information
        /// </summary>
        public static string SlideEmotion = "Default"; // Default

        /// <summary>
        /// Slide State
        /// </summary>
        public static string SlideState = "Default"; // Default
        
        /// <summary>
        /// Image Source List
        /// </summary>
        public List<string> SrcList;
        public int CurrSrc = 0;

        #endregion

        #region "properties"
        
        /// <summary>
        /// Stop Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            RecordStarted = false;
            Emotion = "End_Sampling";
            SrcList.Clear();
            CurrSrc = 0;
            //Process.Start("ffmpeg.exe", "-framerate 10 -i ./img/%d.jpeg -c:v libx264 -r 30 -pix_fmt yuv420p kinect_video.mp4");
        }

        /// <summary>
        /// Start Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            /*
            if(UserName == "Default")
            {
                MessageBoxResult Result = System.Windows.MessageBox.Show(this, "이름을 먼저 입력해주세요.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Warning);
                if(Result == MessageBoxResult.OK)
                {
                    UserName = "Default";
                    FrameRateState = "1fps";
                    RecordStarted = false;
                } 
            }
            else
            {
                if ((RecordStarted == false) && (HdFaceTrackingModel.CSVDataFile == null))
                {
                    HdFaceTrackingModel.CSVDataFile = new FileIO.CSVFileCreater();
                    //HdFaceTrackingModel.CSVDataFile.FirstRow();
                }
                FaceFrameCount = 1;
                RecordStarted = true;
                Emotion = SlideEmotion;
                SrcList.Clear();
                CurrSrc = 0;
                //FileList_Call();
            } 
            */
            if ((RecordStarted == false) && (HdFaceTrackingModel.CSVDataFile == null))
            {
                HdFaceTrackingModel.CSVDataFile = new FileIO.CSVFileCreater();
                //HdFaceTrackingModel.CSVDataFile.FirstRow();
            }
            FaceFrameCount = 1;
            RecordStarted = true;
            Emotion = SlideEmotion;
            SrcList.Clear();
            CurrSrc = 0;
  
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow Setting = new SettingWindow();
            Setting.UserName += value => UserName = value;
            Setting.FrameRate += value => FrameRateState = value;
            Setting.ShowDialog();
        }

        private void Demo_Setting_Click(object sender, RoutedEventArgs e)
        {
            DemoSettingWindow Setting = new DemoSettingWindow();

            Setting.UserName += value => UserName = value;
            Setting.SlideEmotion += value => SlideEmotion = value;
            Setting.FrameRate += value => FrameRateState = value;
            Setting.SlideState += value => SlideState = value;
            Setting.ShowDialog();
        }
        #endregion
        
        public MainWindow()
        {
            InitializeComponent();

            this.ViewModel.StartCommand();
            this.DataContext = this.ViewModel;

            this.InitializeComponent();
            SrcList = new List<string>();

            // start
            this.start_button.Click += Start_Click;
            // stop
            this.stop_button.Click += Stop_Click;
            //FileList_Call();
           

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LoadedCommand();
           
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ViewModel.StopCommand();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
        }

        /*
        private void FileList_Call()
        {
            string path = @"Images\" + Emotion+"\\";
            DirectoryInfo dir = new DirectoryInfo(path);
        }
        */

        private void PrevPic_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrSrc == 0)
                CurrSrc = SrcList.Count - 1;
            else
                CurrSrc--;
            DoubleAnimation dblAnim = new DoubleAnimation();
            dblAnim.From = 0.0;
            dblAnim.To = 1.0;
            dblAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
        }

        
        private void NextPic_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrSrc == SrcList.Count - 1)
                CurrSrc = 0;
            else
                CurrSrc++;
            DoubleAnimation dblAnim = new DoubleAnimation();
            dblAnim.From = 0.0;
            dblAnim.To = 1.0;
            dblAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
        }
    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using LiveCharts;
using LiveCharts.Wpf;


namespace Demo.FileIO
{
    class CSVFileCreater
    {
        private FileStream fs = null;
        private StreamWriter sw = null;

        private string FileName = null;
        private string FilePath = null;

        private char[] CharArr = new char[] { ',', '\r', '\n' };
        private string StringFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}";
        private string data_string = null;

        public static string UNIXTIME = null;

        private const int DelimiterIndex = 0;
        private const int NewLine = 2;

        public CSVFileCreater()
        {
            this.FileName = CreateFileName();
            //this.FilePath = "./Data/"+ this.FileName + ".csv";
            //if((!MainWindow.RecordStarted)&&(this.fs == null))this.fs = new FileStream(FilePath, FileMode.OpenOrCreate);
            //this.sw = new StreamWriter(this.fs, System.Text.Encoding.Default);
        }

        public void CSV_Close()
        {
            //this.sw.Flush();
            //this.sw.Dispose();
            //this.fs.Close();
        }

        public void FirstRow()
        {
             var newLine = string.Format(this.StringFormat, 
                    "UNIX_Time", "Name", "JawOpened", "LipPucker", "JawSlideRight", "LipStretcherRight",
                    "LipStretcherLeft", "LipCornerPullerLeft", "LipCornerPullerRight", "LipCornerDepressorLeft", "LipCornerDepressorRight", "LeftcheekPuff",
                    "RightcheekPuff", "LefteyeClosed", "RighteyeClosed", "RighteyebrowLowerer", "LefteyebrowLowerer", "LowerlipDepressorLeft", 
                    "LowerlipDepressorRight", "FaceYaw", "FacePitch", "FaceRoll", "Emotion");

            this.sw.WriteLine(newLine);
        }
        public string CSV_Writer(string UserName, IReadOnlyDictionary<FaceShapeAnimations, float> AUs , string FaceOrientation, string Emotion)
        {
            string feature_data = null;
            this.data_string = string.Empty;
            
            UNIXTIME = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();

            this.data_string = UNIXTIME + "," + UserName + ",";

            foreach (KeyValuePair<FaceShapeAnimations, float> temp in AUs)
            {
                this.data_string += (temp.Value + ",");
                feature_data += (temp.Value + ",");
            }
            this.data_string += (FaceOrientation);
            this.data_string += Emotion;
            
            this.sw.WriteLine(this.data_string);
            this.sw.Flush();

            feature_data = feature_data.Remove(feature_data.Length - 1);

            return feature_data;
            
        }
        
        private string CreateFileName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-") + MainWindow.FrameRateState + "-" + MainWindow.UserName;
        }
    }
}
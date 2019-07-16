using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Demo.FileIO;
using Demo.Communications;

using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

using Demo.Data;
using System.Text;

namespace Demo.Models
{
    class HdFaceTrackingModel : UserControl, INotifyPropertyChanged
    {


        #region "variable"
        private object lockObject = new object();
        byte[] croppedFaceArray;
        byte[] croppedFaceArraySize;
        private const double FaceRotationIncrementInDegrees = 5.0;

        //int[] points = {
        //    0, 4, 8, 10, 14, 18, 19, 24, 28, 91, 140, 151, 156, 210, 222, 241, 346, 412, 458, 469, 674,
        //    687, 731, 758, 772, 783, 803, 843, 849, 933, 1072, 1090, 1104, 1117, 1279, 1286, 1307, 1327
        //};

        /// <summary>
        /// Thickness of face bounding box and face points
        /// </summary>
        private const double DrawFaceShapeThickness = 8;

        /// <summary>
        /// Font size of face property text 
        /// </summary>
        private const double DrawTextFontSize = 40;

        /// <summary>
        /// Radius of face point circle
        /// </summary>
        private const double FacePointRadius = 1.0;

        /// <summary>
        /// Text layout offset in X axis
        /// </summary>
        private const float TextLayoutOffsetX = -0.1f;

        /// <summary>
        /// Text layout offset in Y axis
        /// </summary>
        private const float TextLayoutOffsetY = -0.15f;

        private KinectSensor kinect;

        // private MultiSourceFrameReader reader;

        private BodyFrameReader bodyFrameReader;

        /// <summary>
        /// Array to store bodies
        /// </summary>
        private Body[] bodies;

        /// <summary>
        /// The currently tracked body
        /// </summary>
        private Body currentTrackedBody = null;

        /// <summary>
        /// Number of bodies tracked
        /// </summary>
        private int bodyCount;

        private FaceAlignment faceAlignment = null;
        private FaceModel faceModel = null;

        private FaceFrameSource faceFrameSource = null;
        private FaceFrameReader faceFrameReaders = null;

        private HighDefinitionFaceFrameSource hdFaceFrameSources = null;
        private HighDefinitionFaceFrameReader hdFaceFrameReaders = null;

        /// <summary>
        /// Storage for face frame results
        /// </summary>
        private FaceFrameResult faceFrameResults = null;

        // Coordinate mapper to map one type of point to another
        private CoordinateMapper coordinateMapper = null;

        // Reader for color frames
        private ColorFrameReader colorFrameReader = null;

        // Bitmap to display
        private WriteableBitmap colorBitmap = null;

        /// <sumamry>
        /// Width of display (color space)
        /// </sumamry>
        private int displayWidth;

        /// <summary>
        /// Height of display (color space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// Display rectangle
        /// </summary>
        private Rect displayRect;

        /// <summary>
        /// List of brushes for each face tracked
        /// </summary>
        private Brush faceBrush;

        private static string textFaceTrackedString = "test";


        /// <summary>
        /// Formatted text to indicate that there are no bodies/faces tracked in the FOV
        /// </summary>
        private FormattedText textFaceNotTracked = new FormattedText(
                        "얼굴이 인식되지 않습니다. 손을 흔들어보세요.",
                        CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Georgia"),
                        DrawTextFontSize,
                        Brushes.Red);

        private FormattedText textFaceTracked1 = new FormattedText(
                "당신은 지금",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Georgia"),
                DrawTextFontSize,
                Brushes.Blue);

        private FormattedText textFaceTracked2 = new FormattedText(
                "상태입니다.",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Georgia"),
                DrawTextFontSize,
                Brushes.Blue);
        private FormattedText textFaceTracked = null;

        /// <summary>
        /// Text layout for the no face tracked message
        /// </summary>
        private Point textLayoutFaceNotTracked = new Point(10.0, 10.0);

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        public DrawingImage imageSource;

        public BitmapImage an;
        public BitmapImage fe;
        public BitmapImage ha;
        public BitmapImage ne;
        public BitmapImage sa;
        public BitmapImage su;
        /// <summary>
        /// data string
        /// </summary>
        public string data_string = string.Empty;

        /// <summary>
        /// recognition result string
        /// </summary>
        public string recognition_string = string.Empty;

        /// <summary>
        /// Craete and Open the file
        /// </summary>
        public static CSVFileCreater CSVDataFile = null;

        /// <summary>
        /// Create TCP Object
        /// </summary>
        private Client client = null;

        /// <summary>
        /// receiver string buffer
        /// </summary>
        public string receive_data;

        // for Save the image/video
        private byte[] colorData;

        //private BitmapSource bitmapSource;

        private Object _lock = null;
        //IReadOnlyDictionary<FaceShapeAnimations, float> AUs = null;

        public List<string> labels = new List<string>();
        public SeriesCollection seriesBar = new SeriesCollection();

        public int num = 0;
        private List<string> data = new List<string>();
        private int faceHeight;
        private int faceWidth;
        private float depth;
        #endregion

        #region "properties"

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        IDictionary<FaceEmotions, float> tempDict = new Dictionary<FaceEmotions, float>()
            {
                {FaceEmotions.neutral, 0 },
                {FaceEmotions.fear, 1 },
                {FaceEmotions.happy, 2 },
                {FaceEmotions.angry, 3 },
                {FaceEmotions.sad, 4 },
                {FaceEmotions.surprise, 5 },
            };
        private RectI faceBoxSource;

        public dynamic EmotionUnits
        {
            get
            {
                //Console.WriteLine(MainWindow.Emotion);
                if ( (this.colorBitmap != null) && (MainWindow.Emotion != "End_Sampling") )
                {
                    // crop the face image
                    CroppedBitmap croppedFaceImage = new CroppedBitmap(this.colorBitmap, new Int32Rect(this.faceBoxSource.Left,
                        this.faceBoxSource.Top,
                        this.faceBoxSource.Right - this.faceBoxSource.Left,
                        this.faceBoxSource.Bottom - this.faceBoxSource.Top));
                    // faceBoxSource.Left - 75, faceBoxSource.Top - 75, faceBoxSource.Right - faceBoxSource.Left + 150, faceBoxSource.Bottom - faceBoxSource.Top + 150
                    // convert to grayscale
                    FormatConvertedBitmap grayFaceImage = new FormatConvertedBitmap();
                    grayFaceImage.BeginInit();
                    grayFaceImage.Source = croppedFaceImage;
                    grayFaceImage.DestinationFormat = PixelFormats.Gray32Float;
                    grayFaceImage.EndInit();

                    // This code save the cropped image (for test)
                    //string filePath = String.Format(@"C:\imgtest\testimage_{0}.jpg", a++);
                    //FileStream fStream = new FileStream(filePath, FileMode.Create);
                    //encoder.Save(fStream);

                    /*
                    lock (lockObject)
                    {
                        // convert to byte array
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(grayFaceImage));
                        using (MemoryStream ms = new MemoryStream())
                        {
                            encoder.Save(ms);
                            this.croppedFaceArray = ms.ToArray();
                            ms.Close();
                        }
                    }
                    */
                    // convert to byte array
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(grayFaceImage));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        this.croppedFaceArray = ms.ToArray();
                        ms.Close();
                    }

                }


                receive_data = null;
                char[] delimiterChars = { ':', ',', ' ', '}' };

                if (this.croppedFaceArray == null)
                {
                    return null;
                }

                lock(lockObject)
                {
                    croppedFaceArraySize = BitConverter.GetBytes(this.croppedFaceArray.Length);
                    //Console.WriteLine(BitConverter.ToInt32(this.croppedFaceArraySize, 0));
                    this.client.SendByte(this.croppedFaceArraySize, this.croppedFaceArray);
                    receive_data = this.client.ReceiveByte();
                }
                this.croppedFaceArray = null;
                this.croppedFaceArraySize = null;
                //Console.WriteLine(receive_data);
                string[] splitTest = receive_data.Split(delimiterChars);
                this.tempDict = ConvertDict(splitTest);
                DrawingGraph(this.tempDict);
                this.data.Clear();

                //Dictionary<FaceEmotions, float> dict_rank = this.tempDict.OrderBy(num => num.Value);
                var items = this.tempDict.OrderByDescending(num => num.Value);
                var maxKey = this.tempDict.FirstOrDefault(x => x.Value == this.tempDict.Values.Max()).Key;
                MainWindow.RecogEmotion = RecogEmotion(maxKey);

                return items;
            }
        }   

        private string RecogEmotion(FaceEmotions d)
        {        
            if (d == FaceEmotions.angry)
                return "화남";
            if (d == FaceEmotions.fear)
                return "두려움";
            if (d == FaceEmotions.happy)
                return "행복";
            if (d == FaceEmotions.neutral)
                return "무표정";
            if (d == FaceEmotions.sad)
                return "슬픔";
            if (d == FaceEmotions.surprise)
                return "놀람";
            return "인식되지 않음";
        }

        private void DrawingGraph(IDictionary<FaceEmotions, float> drawingData)
        {
            float outValue;
            this.SeriesCollection[0].Values.Clear();
            for (int i = 0; i < 6; i++)
            {
                drawingData.TryGetValue((FaceEmotions)i, out outValue);

                SeriesCollection[0].Values.Insert(i, (double)outValue);
            }
        }

        private IDictionary<FaceEmotions, float> ConvertDict(string[] data)
        {
            int i = 0;

            IDictionary<FaceEmotions, float> temp = new Dictionary<FaceEmotions, float>();

            foreach (string s in data)
            {
                if (s.Contains("angry"))
                {
                    temp.Add(FaceEmotions.angry, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (s.Contains("fear"))
                {
                    temp.Add(FaceEmotions.fear, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (s.Contains("happy"))
                {
                    temp.Add(FaceEmotions.happy, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (s.Contains("neutral"))
                {
                    temp.Add(FaceEmotions.neutral, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (s.Contains("sad"))
                {
                    temp.Add(FaceEmotions.sad, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (s.Contains("surprise"))
                {
                    temp.Add(FaceEmotions.surprise, float.Parse(data[i + 2], CultureInfo.InvariantCulture.NumberFormat));
                }
                i++;
            }
            return temp;

        }

        #endregion

        #region "test"
        //public SeriesCollection SeriesCollection { get; set; }
        //public string[] Labels { get; set; }
        //public Func<double, string> Formatter { get; set; }
        #endregion

        #region "code"
        public void Start()
        {
            Initialize();

        }

        public new void Loaded()
        {
            if (this.faceFrameReaders != null)
            {
                // wire handler for face frame arrival
                this.faceFrameReaders.FrameArrived += this.Reader_FaceFrameArrived;
            }
            if (this.hdFaceFrameReaders != null)
            {
                // wire handler for face frame arrival
                this.hdFaceFrameReaders.FrameArrived += this.OnFaceFrameArrived;
            }

            if (this.bodyFrameReader != null)
            {
                // wire handler for body frame arrival
                this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;
            }

        }
        public void Stop()
        {

            MainWindow.RecordStarted = false;
            //CSVDataFile.CSV_Close();

            SeriesCollection[0].OnSeriesUpdatedFinish();

            if (this.faceFrameReaders != null)
            {
                // FaceFrameReader is IDisposable
                this.faceFrameReaders = null;
            }

            if (this.hdFaceFrameReaders != null)
            {
                // FaceFrameReader is IDisposable
                this.hdFaceFrameReaders.Dispose();
                this.hdFaceFrameReaders = null;
            }

            if (this.faceFrameSource != null)
            {
                // FaceFrameSource is IDisposable
                this.faceFrameSource = null;
            }

            if (CSVDataFile != null)
                CSVDataFile.CSV_Close();

            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinect != null)
            {
                this.kinect.Close();
                this.kinect = null;
            }
        }



        private void Initialize()
        {

            this.kinect = KinectSensor.GetDefault();

            if (kinect == null) return;

            this.coordinateMapper = this.kinect.CoordinateMapper;

            // open the reader for the color frames
            this.colorFrameReader = this.kinect.ColorFrameSource.OpenReader();

            // wire handler for frame arrival
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription frameDescription = this.kinect.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // for image / video save
            uint frameSize = frameDescription.BytesPerPixel * frameDescription.LengthInPixels;
            this.colorData = new byte[frameSize];

            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(frameDescription.Width, frameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            // set the display specifics
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;
            this.displayRect = new Rect(0.0, 0.0, this.displayWidth, this.displayHeight);

            // open the reader for the body frames
            this.bodyFrameReader = this.kinect.BodyFrameSource.OpenReader();

            // wire handler for body frame arrival
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // set the  maximum number of bodies that would be tracked by Kinect
            this.bodyCount = this.kinect.BodyFrameSource.BodyCount;

            this.hdFaceFrameSources = new HighDefinitionFaceFrameSource(this.kinect);

            // create the face frame source with the required face frame features and an initial tracking Id of 0
            this.faceFrameSource = new FaceFrameSource(this.kinect, 0,
                FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.MouthOpen
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.BoundingBoxInColorSpace);
            this.faceFrameReaders = this.faceFrameSource.OpenReader();
            
            this.hdFaceFrameSources = new HighDefinitionFaceFrameSource(this.kinect);

            // open the corresponding reader
            this.faceFrameReaders = this.faceFrameSource.OpenReader();
            this.hdFaceFrameReaders = this.hdFaceFrameSources.OpenReader();

            this.faceModel = new FaceModel();
            this.faceAlignment = new FaceAlignment();
            // initialize the components (controls) of the window
            //this.InitializeComponent();


            // populate face result colors = one for each face index
            this.faceBrush = Brushes.White;

            this.kinect.Open();

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our iamge control
            this.imageSource = new DrawingImage(this.drawingGroup);
            this.an = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/Anger.png", UriKind.Relative));
            this.fe = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/Fear.png", UriKind.Relative));
            this.ha = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/happy.png", UriKind.Relative));
            this.ne = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/Neutral.png", UriKind.Relative));
            this.sa = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/Sad.png", UriKind.Relative));
            this.su = new BitmapImage(new Uri("C:/Users/Kyoungsoo Park/Desktop/client2/Demo/Demo/Images/Emoticon/Surprise.png", UriKind.Relative));

            SeriesCollection = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "Predict Probability",
                    Values = new ChartValues<double> { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                    DataLabels = true
                }
            };

            SeriesCollection[0].OnSeriesUpdateStart();

            Labels = new[] { "Neutral", "Fear", "Happy", "Angry", "Sad", "Surpirse" };
            //Labels = new[] { "Joy", "Calm", "Sad", "Stress" };

            //Labels = tempDict.Keys.ToArray();
            Formatter = value => value.ToString("P");

            // object init
            this._lock = new Object();

            // Socket Communication
            this.client = new Client("advice.hufs.ac.kr", 50001);
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"> object sending the event </param>
        /// <param name="e"> event arguments </param>
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // ColorFrame is IDisposable
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Bgra);

                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    // Createing BitmapSource
                    var bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel) / 8;
                    var stride = bytesPerPixel * colorFrame.FrameDescription.Width;

                    //if (MainWindow.RecordStarted)
                    //{
                    //
                    //    this.bitmapSource = BitmapSource.Create(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null, colorData, stride);
                    //}

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));

                        }

                        this.colorBitmap.Unlock();
                    }

                }
            }
        }

        /// <summary>
        /// Handles the face frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame == null || !faceFrame.IsTrackingIdValid)
                {
                    return;
                }
                if (this.ValidateFaceBoxAndPoints(faceFrame.FaceFrameResult))
                {
                    this.faceFrameResults = faceFrame.FaceFrameResult;
                }
                else
                {
                    // indicates that the latest face frame result from this reader is invalid
                    this.faceFrameResults = null;
                }
            }
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var frame = e.FrameReference.AcquireFrame();
            if (frame == null) return;

            // To get the frame with respect to BodyFrame
            using (var bodyFrame = frame) 
            {
                if (bodyFrame != null)
                {
                    
                    this.bodies = new WeakReference(new Body[bodyFrame.BodyCount]).Target as Body[];
                    
                    // Storing the skeleton data(update body data)
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    
                    //Console.WriteLine(bodyFrame.BodyCount);
                    this.currentTrackedBody = new WeakReference(this.bodies.Where(b => b.IsTracked).FirstOrDefault()).Target as Body;

                    if (this.currentTrackedBody != null)
                    {
                        this.AssignTrackingId(this.currentTrackedBody.TrackingId);
                        //get depth
                        var head = this.currentTrackedBody.Joints[JointType.Head];
                        this.depth = head.Position.Z;
                        //Console.WriteLine(head.ToString());
                    }
                    
                    
                    this.currentTrackedBody = null;

                    using (DrawingContext dc = this.drawingGroup.Open())
                    {
                        // draw background color background
                        dc.DrawImage(this.colorBitmap, this.displayRect);
                        //dc.DrawImage(this.an, new Rect(50, 50, 100, 100));
                        bool drawFaceResult = false;
                        
                        // check if a valid face is trakced in this face source
                        if (this.faceFrameSource.IsTrackingIdValid)
                        {
                            // check if we have valid face frame results
                            if (this.faceFrameResults != null)
                            {
                                // draw face frame results
                                if (MainWindow.RecordStarted == true)
                                    this.DrawFaceFrameResults(this.faceFrameResults, dc);

                                if (!drawFaceResult)
                                {
                                    drawFaceResult = true;
                                }
                            }
                        }
                        else
                        {
                            // check if the corresponding body is tracked
                            if (this.currentTrackedBody != null && this.currentTrackedBody.IsTracked)
                            {
                                textFaceTrackedString = "Tracking ID:" + this.faceFrameSource.TrackingId.ToString();
                                this.textFaceTracked = new FormattedText(
                                    textFaceTrackedString,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Georgia"),
                                    DrawTextFontSize,
                                    Brushes.Red);
                            }
                        }

                        if (!drawFaceResult)
                        {
                            // if no faces were drawn then this indicates one of the following:
                            // a body was not tracked
                            // a body was tracked but the corresponding face was not tracked
                            // a body and the dorresponding face was tracked though the face box or the face points were not valid
                            dc.DrawText(
                                this.textFaceNotTracked,
                                this.textLayoutFaceNotTracked);

                        }
                        else if (textFaceTracked != null)
                        {
                            dc.DrawText(
                                this.textFaceTracked,
                                this.textLayoutFaceNotTracked);
                            //this.notTrackedCount = 0;
                        }

                        this.drawingGroup.ClipGeometry = new RectangleGeometry(this.displayRect);
                        OnPropertyChanged("ImageSource");
                    }

                }
            }
        }

        private void AssignTrackingId(ulong bodyTrackingId)
        {
            if (this.faceFrameSource != null)
                if (!this.faceFrameSource.IsTrackingIdValid)
                    this.faceFrameSource.TrackingId = bodyTrackingId;

            if (this.hdFaceFrameSources != null)
                if (!this.hdFaceFrameSources.IsTrackingIdValid)
                    this.hdFaceFrameSources.TrackingId = bodyTrackingId;
        }


        /// <summary>
        /// Draws face frame results
        /// </summary>
        /// <param name="faceIndex"></param>
        /// <param name="faceResult"></param>
        /// <param name="drawingContext"></param>
        private void DrawFaceFrameResults(FaceFrameResult faceResult, DrawingContext drawingContext)
        {
            // choose the brush based on the face index
            Brush drawingBrush = this.faceBrush;

            Pen drawingPen = new Pen(drawingBrush, DrawFaceShapeThickness);

            // draw the face bounding box
            this.faceBoxSource = faceResult.FaceBoundingBoxInColorSpace;
            Rect faceBox = new Rect(this.faceBoxSource.Left-20,
                                    this.faceBoxSource.Top-20,
                                    this.faceBoxSource.Right - this.faceBoxSource.Left+40,
                                    this.faceBoxSource.Bottom - this.faceBoxSource.Top+40);
            drawingContext.DrawRectangle(null, drawingPen, faceBox);

            //Console.WriteLine("width");
            //Console.WriteLine(faceBoxSource.Right - faceBoxSource.Left);
            //Console.WriteLine("Heith");
            //Console.WriteLine(faceBoxSource.Bottom - faceBoxSource.Top);
            this.faceHeight = this.faceBoxSource.Right - this.faceBoxSource.Left;
            this.faceWidth = this.faceBoxSource.Bottom - this.faceBoxSource.Top;


            string faceText = "감정상태: " + MainWindow.RecogEmotion + "\n";
            faceText += "사진크기: " + this.faceWidth + "x" + this.faceHeight + "pixels\n";
            faceText += "카메라와의 거리: " + this.depth + "m\n";
            // extract each face property information and store it in faceText
            if (faceResult.FaceProperties != null)
            {
                    foreach (var item in faceResult.FaceProperties)
                {
                    if (item.Key.ToString() == "Happy" || item.Key.ToString() == "Engaged" || item.Key.ToString() == "MouthMoved")
                    {
                        continue;
                    }
                    //faceText += item.Key.ToString() + ": ";

                    // consider a "maybe" as a "no" to restrict 
                    // the detection result refresh rate
                    if (item.Value == DetectionResult.Maybe)
                    {
                        //faceText += DetectionResult.No + "\n";
                        continue;
                    }
                    else
                    {
                        if ( (item.Key.ToString() == "WearingGlasses") && item.Value.ToString() == "Yes")
                        {
                            faceText += "안경착용 중\n";
                        }
                        if ((item.Key.ToString() == "LeftEyeClosed") && item.Value.ToString() == "Yes")
                        {
                            faceText += "왼쪽 눈감음\n";
                        }
                        if ((item.Key.ToString() == "RightEyeClosed") && item.Value.ToString() == "Yes")
                        {
                            faceText += "오른쪽 눈감음\n";
                        }
                        if ((item.Key.ToString() == "MouthOpen") && item.Value.ToString() == "Yes")
                        {
                            faceText += "입 열림\n";
                        }
                        if ((item.Key.ToString() == "LookingAway") && item.Value.ToString() == "Yes")
                        {
                            faceText += "측면응시 중";
                        }
                        //faceText += item.Value.ToString() + "\n";
                    }
                }
            }

            // render the face propertyW
            Point faceTextLayout;
            if (this.GetFaceTextPositionInColorSpace(out faceTextLayout))
            {
                faceTextLayout.X = faceTextLayout.X + 270;
                faceTextLayout.Y = faceTextLayout.Y-220;
                drawingContext.DrawText(
                        new FormattedText(
                            faceText,
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Consolas"),
                            DrawTextFontSize,
                            drawingBrush),
                            faceTextLayout);
                //Console.WriteLine(faceTextLayout.ToString());
                //drawingContext.DrawContext("dsad", new Rect(20, 20, 100, 100));
                if (MainWindow.RecogEmotion != "None")
                {
                    drawingContext.DrawText(this.textFaceTracked1, new Point(50.0, 30.0));
                    if (MainWindow.RecogEmotion == "화남")
                    {
                        drawingContext.DrawImage(this.an, new Rect(270, 10, 80, 80));
                    }
                    else if (MainWindow.RecogEmotion == "두려움")
                    {
                        drawingContext.DrawImage(this.fe, new Rect(270, 10, 80, 80));
                    }
                    else if (MainWindow.RecogEmotion == "행복")
                    {
                        drawingContext.DrawImage(this.ha, new Rect(270, 10, 80, 80));
                    }
                    else if (MainWindow.RecogEmotion == "무표정")
                    {
                        drawingContext.DrawImage(this.ne, new Rect(270, 10, 80, 80));
                    }
                    else if (MainWindow.RecogEmotion == "슬픔")
                    {
                        drawingContext.DrawImage(this.sa, new Rect(270, 10, 80, 80));
                    }
                    else if (MainWindow.RecogEmotion == "놀람")
                    {
                        drawingContext.DrawImage(this.su, new Rect(270, 10, 80, 80));
                    }
                    drawingContext.DrawText(this.textFaceTracked2, new Point(360.0, 30.0));
                }
            }

            //this.imageDrawings = null;
            //this.di
            //this.di.Rect = new Rect(75, 75, 100, 100);
            //this.di.ImageSource = new BitmapImage(
            //    new Uri(@"Images/Emotion/Anger.png", UriKind.Relative));
            //this.imageDrawings.Children.Add(di);

        }




        /// <summary>
        /// Computes the face result text position by adding an offset to the corresponding
        /// body's head joint in camera space and then by projecting it to screen space
        /// </summary>
        /// <param name="faceIndex"></param>
        /// <param name="faceTextLayout"></param>
        /// <returns></returns>
        private bool GetFaceTextPositionInColorSpace(out Point faceTextLayout)
        {
            faceTextLayout = new Point();
            bool isLayoutValid = false;

            Body trackedBody = null;

            foreach (Body body in bodies)
            {
                if (body.TrackingId == this.faceFrameResults.TrackingId)
                    trackedBody = body;
            }

            if (trackedBody != null && trackedBody.IsTracked)
            {
                var headJoint = trackedBody.Joints[JointType.Head].Position;

                CameraSpacePoint textPoint = new CameraSpacePoint()
                {
                    X = headJoint.X + TextLayoutOffsetX,
                    Y = headJoint.Y + TextLayoutOffsetY,
                    Z = headJoint.Z
                };

                ColorSpacePoint textPointInColor = this.coordinateMapper.MapCameraPointToColorSpace(textPoint);

                faceTextLayout.X = textPointInColor.X;
                faceTextLayout.Y = textPointInColor.Y;
                isLayoutValid = true;
            }

            return isLayoutValid;
        }

        private void OnFaceFrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {

            using (var faceFrame = e.FrameReference.AcquireFrame())
            {

                if (faceFrame == null || !faceFrame.IsFaceTracked) return;

                // Update Face Alignment
                faceFrame.GetAndRefreshFaceAlignmentResult(this.faceAlignment);

                MainWindow.FaceFrameCount++;
                // Update the Animation Unit
                //Console.WriteLine(MainWindow.FaceFrameCount);
                //Console.WriteLine(FrameRateStateToInt(MainWindow.FrameRateState)); //30frame per sec = 30frame / sec
                if ((MainWindow.RecordStarted == true) && (MainWindow.FaceFrameCount % 30 == 0)) // (FrameRateStateToInt(MainWindow.FrameRateState) == 0)
                {
                    //Console.WriteLine(System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    OnPropertyChanged("EmotionUnits");
                    MainWindow.FaceFrameCount = 1;
                }
            }
        }


        private int FrameRateStateToInt(string FrameRate)
        {
            if (MainWindow.FrameRateState == "10fps")
                return 3;
            else if (MainWindow.FrameRateState == "5fps")
                return 6;
            else if (MainWindow.FrameRateState == "1fps")
                return 30;
            else
                return 30;
        }

        /// <summary>
        /// Validates face bounding box and face points to be within screen space
        /// </summary>
        /// <param name="faceResult"></param>
        /// <returns></returns>
        private bool ValidateFaceBoxAndPoints(FaceFrameResult faceResult)
        {
            bool isFaceValid = faceResult != null;

            if (isFaceValid)
            {
                var faceBox = faceResult.FaceBoundingBoxInColorSpace;
                if (faceBox != null)
                {

                    // check is we have a valid rectagle within the bounds of the screen space
                    isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
                                  (faceBox.Bottom - faceBox.Top) > 0 &&
                                  faceBox.Right <= this.displayWidth &&
                                  faceBox.Bottom <= this.displayHeight;
                }

                if (isFaceValid)

                {
                    var facePoints = faceResult.FacePointsInColorSpace;
                    if (facePoints != null)
                    {
                        foreach (PointF pointF in facePoints.Values)
                        {
                            // check if we have a valid face point within the bounds of the screen space
                            bool isFacePointValid = pointF.X > 0.0f &&
                                                    pointF.Y > 0.0f &&
                                                    pointF.X < this.displayWidth &&
                                                    pointF.Y < this.displayHeight;

                            if (!isFacePointValid)
                            {
                                isFaceValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            return isFaceValid;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
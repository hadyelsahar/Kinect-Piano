using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.Media;

namespace skeleton_example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Runtime nui = new Runtime();

        public MainWindow()
        {
            InitializeComponent();
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);

            nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseDepth | RuntimeOptions.UseColor); 

            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth); 

        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage myimage = e.ImageFrame.Image;
            image1.Source = BitmapSource.Create(myimage.Width, myimage.Height, 96, 96, PixelFormats.Bgr32, null, myimage.Bits, myimage.Width * myimage.BytesPerPixel);
        }

        /// <summary>
        /// the event handler for the skeleton frame ready event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allskeletons = e.SkeletonFrame;
            SkeletonData skeleton = (from s in allskeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();

            setpositionellipse(ellipse1,skeleton.Joints[JointID.Head]);
            setpositionellipse(ellipse2, skeleton.Joints[JointID.HandLeft]);
            setpositionellipse(ellipse3, skeleton.Joints[JointID.HandRight]);
            setpositionellipse(ellipse4, skeleton.Joints[JointID.FootLeft]);
            setpositionellipse(ellipse5, skeleton.Joints[JointID.FootRight]);

            getCoordinates_Playsound(skeleton.Joints[JointID.HandLeft]);

        }

        

        private void getCoordinates_Playsound(Joint leftFoot)
        {

            //Decimal.Round((decimal)(leftFoot.Position.X),2) is to round the Decimal number  to the nearest 2 decimals 

            leftTextBlock.Text = "X= " + Decimal.Round((decimal)(leftFoot.Position.X), 2) + " , Y= " + Decimal.Round((decimal)(leftFoot.Position.Y), 2) + " , Z= " + Decimal.Round((decimal)(leftFoot.Position.Z), 2);
            
            if (leftFoot.Position.Y < 0.2f)
            {
                // left hand x pos varies from  -1>>2 we add +2  to make it 1>>2 and freq from 600>>1200
                Console.Beep((int)((leftFoot.Position.X+2)/1*600),500);
            }

        }




        private void setpositionellipse(FrameworkElement ellipse , Joint joint )

        {
            // it sets the end of the skeleton coordinates  which means the maximum x for the skeleton is 0.5 x , but still it can get more than 0.5 but skeleton wont move 
            var scaledjoint = joint.ScaleTo(640, 480, 1f, 1f); 
            Canvas.SetLeft(ellipse, scaledjoint.Position.X);
            Canvas.SetTop(ellipse, scaledjoint.Position.Y);
        }


        /// <summary>
        /// the event handler for the depth frame ready event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            image2.Source = e.ImageFrame.ToBitmapSource();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }
    }
}

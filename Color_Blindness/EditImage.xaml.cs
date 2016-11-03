using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.Net.Http;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;


namespace Color_It
{
    public sealed partial class EditImage : Page
    {
        CoreApplicationView view;
        String ImagePath;

        static readonly long cycleDuration = TimeSpan.FromSeconds(3).Ticks;

        FileOpenPicker filePicker;
        WriteableBitmap bmp_rgbp;
        double a = 0, b = 0, c = 0;
        double value1 = 0, value2 = 0, value3 = 0;

        StorageFile storageFile;
        WriteableBitmap bitmapImage;
        String deficiency = "";

        // Access local app storage
        StorageFolder localFolder = null;
        int localCounter = 0;
        const string filename = "deficiency.txt";

        public EditImage()
        {
            this.InitializeComponent();
            Application.Current.DebugSettings.EnableFrameRateCounter = true;
            view = CoreApplication.GetCurrentView();
            localFolder = ApplicationData.Current.LocalFolder;
        }

        public class bitmapClass
        {
            public byte[] bitmapStream
            {
                get;
                set;
            }

            public string deficiency
            {
                get;
                set;
            }

            public int width
            {
                get;
                set;
            }
            public int height
            {
                get;
                set;
            }
        }

        async void Read_Local_Counter()
        {
            StorageFile file = await localFolder.GetFileAsync(filename);
            string text = await FileIO.ReadTextAsync(file);
            localCounter = int.Parse(text);
            deficiency = (localCounter == 1 ? "Protanopia" : localCounter == 2 ? "Deuteranopia" : "Normal Vision");
            ModifiedDesc.Text = ModifiedDesc.Text + "( " + deficiency + " adjusted )";
        }

        private void load_Click(object sender, RoutedEventArgs e)//load
        {
            FilteredImage.Source = null;
            FilterButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

            ImagePath = string.Empty;
            filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;
        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFile = args.Files[0];
                var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                bitmapImage = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(500,500) ;
                bitmapImage = await BitmapFactory.New(1, 1).FromStream(stream);
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                var newObj = await BitmapFactory.New(15, 15).FromStream(stream);
                OriginalImage.Source = newObj;
            }
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Read_Local_Counter();
        }

        async private void filter_Click(object sender, RoutedEventArgs e)
        {
            var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            bitmapClass objClass = new bitmapClass();
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56564/api/daltonize");
            System.IO.Stream ioStream = stream.AsStream();
            Debug.WriteLine("Image size(hxw): " + bitmapImage.PixelHeight + ": " + bitmapImage.PixelWidth);
            byte[] buffer = new byte[bitmapImage.PixelHeight*bitmapImage.PixelWidth];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = ioStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                ms.ToArray();
            }

            StreamContent content = new StreamContent(ioStream);
            objClass.bitmapStream = buffer;
            objClass.deficiency = deficiency;
            objClass.height = bitmapImage.PixelHeight;
            objClass.width = bitmapImage.PixelWidth;

            HttpResponseMessage result = await client.PostAsJsonAsync(client.BaseAddress, objClass);
            var myobject = await result.Content.ReadAsAsync<bitmapClass>();
        
            MemoryStream stream1 = new MemoryStream();
            stream1.Write(myobject.bitmapStream, 0, myobject.bitmapStream.Length);
            stream1.AsRandomAccessStream();

            var ms1 = new MemoryStream(myobject.bitmapStream).AsRandomAccessStream();
            bitmapImage = await BitmapFactory.New(1, 1).FromStream(ms1);
            FilteredImage.Source = bitmapImage;

        }//end of button clcik
    }
}

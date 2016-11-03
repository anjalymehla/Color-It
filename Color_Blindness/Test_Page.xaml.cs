using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.Storage;

namespace Color_It
{

    public sealed partial class Test_Page : Page
    {
        BitmapImage img = new BitmapImage();
        int imageno = 7;
        StorageFolder localFolder = null;
        int localCounter = 0;
        const string filename = "deficiency.txt";
        String str;
        public Test_Page()
        {
            this.InitializeComponent();
            localFolder = ApplicationData.Current.LocalFolder;
        }
        async void Write_Local_Counter(int a)
        {
            StorageFile file = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, a.ToString());
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            string s = "/Assets/" + imageno + ".png";
            img.UriSource = new Uri(this.BaseUri, s);
            TestImage.Source = img;
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

        private async Task normalVisionContinue()
        {
            String msg = "Continue testing";
            MessageDialog dl = new MessageDialog(msg);
            await dl.ShowAsync();
            imageno++;
            InputTestBox.Text = "";
            string str = "Assets/" + imageno + ".png";
            img.UriSource = new Uri(this.BaseUri, str);
            TestImage.Source = img;
        }

        private async Task normalVisionFinal()
        {
            String msg = "You have normal vision";
            MessageDialog dl = new MessageDialog(msg);
            await dl.ShowAsync();
            Write_Local_Counter(0);
            this.Frame.Navigate(typeof(MainPage));
        }
        private async Task protanopia()
        {
            String msg = "Protanopia deficiency";
            MessageDialog dl = new MessageDialog(msg);
            await dl.ShowAsync();
            Write_Local_Counter(1);
            this.Frame.Navigate(typeof(MainPage), "Protanopia");
        }

        private async Task deuteranopia()
        {
            String msg = "Deuteranopia deficiency";
            MessageDialog dl = new MessageDialog(msg);
            await dl.ShowAsync();
            Write_Local_Counter(2);
            this.Frame.Navigate(typeof(MainPage), "Deuteranopia");
        }

        private async Task invalidInput()
        {
            String msg = "Try again.";
            MessageDialog dl = new MessageDialog(msg);
            await dl.ShowAsync();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);
            writeableBmp.GetBitmapContext();
            String input = InputTestBox.Text;

            switch (imageno)
            {
                case 7:
                    if (input.Equals("96"))
                        await normalVisionContinue();
                    else if (input.Equals("6"))
                        await protanopia();
                    else if (input.Equals("9"))
                        await deuteranopia();
                    else await invalidInput();
                    break;
                case 8:
                    if (input.Equals("42"))
                        await normalVisionContinue();
                    else if (input.Equals("2"))
                        await protanopia();
                    else if (input.Equals("4"))
                        await deuteranopia();
                    else await invalidInput();
                    break;
                case 9:
                    if (input.Equals("35"))
                        await normalVisionContinue();
                    else if (input.Equals("5"))
                        await protanopia();
                    else if (input.Equals("3"))
                        await deuteranopia();
                    else await invalidInput();
                    break;
                case 10:
                    if (input.Equals("26"))
                        await normalVisionFinal();
                    else if (input.Equals("6"))
                        await protanopia();
                    else if (input.Equals("2"))
                        await deuteranopia();
                    else await invalidInput();
                    break;
            }

        }
    }
}

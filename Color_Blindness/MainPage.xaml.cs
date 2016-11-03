using System;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Color_It
{
    public sealed partial class MainPage : Page
    {

        StorageFolder localFolder = null;
        int localCounter = 0;
        const string filename = "deficiency.txt";
        String str;


        async void Read_Local_Counter()
        {
            try
            {
                StorageFile file = await localFolder.GetFileAsync(filename);
                string text = await FileIO.ReadTextAsync(file);
                localCounter = int.Parse(text);
                if (localCounter == 0 || localCounter == 1 || localCounter == 2)
                    EditImageButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                str = (localCounter == 1 ? "Protanopia deficiency" : localCounter == 2 ? "Deuteranopia deficiency" : "Normal Vision");
                Desc.Text = "So you've taken the test.\n" + str + " detected.";
                TakeTestButton.Content = "Test Again";
            }
            catch (Exception)
            {
                Desc.Text = "Take the test to identify deficiency.\n";
                EditImageButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            localFolder = ApplicationData.Current.LocalFolder;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Read_Local_Counter();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            // Do nothing, becuase start page of the app.
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)//test
        {
            this.Frame.Navigate(typeof(Test_Page));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)//edit
        {
            this.Frame.Navigate(typeof(EditImage));
        }
    }
}
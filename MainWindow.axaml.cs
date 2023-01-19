using System;
using System.IO;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using QRCoder;

namespace HGDCabinetLauncher
{
    public partial class MainWindow : Window
    {
        private readonly AvaloniaList<ListBoxItem> _uiList = new(); //list for populating the ui with
        //instance of the GameFinder class for indexing metafiles and running detected games
        private readonly GameFinder _finder = new(); 
        //generate qr codes for website visiting
        private readonly QRCodeGenerator _gen = new();


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#else
            this.WindowState = WindowState.FullScreen;
#endif
        }

        //update interface when new item is selected
        private void uiList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            name.Text = _finder.GameList[uiList.SelectedIndex].Name;
            desc.Text = "Description:\n" + _finder.GameList[uiList.SelectedIndex].Desc;
            ver.Text = "Version:\n" + _finder.GameList[uiList.SelectedIndex].Version;
            authors.Text = "Author(s):\n" + _finder.GameList[uiList.SelectedIndex].Authors;
            //set data for link opening stuff via click event
            link.Tag = _finder.GameList[uiList.SelectedIndex].Link;

            //keep tabs on functions like setting images and generating qr codes that may fail
            try
            {
                //construct bitmap with full path to image
                IImage img = new Bitmap(
                    _finder.GameList[uiList.SelectedIndex].ExecLoc + 
                    _finder.GameList[uiList.SelectedIndex].ImgDir);
                
                this.gameImg.Source = (img);
            }
            catch (FileNotFoundException err)
            {
                Console.WriteLine("failed to set reference image! using fallback...");
                Console.WriteLine(err.Message);
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                //use embedded fallback resource
                this.gameImg.Source = new Bitmap(assets.Open(new Uri("resm:HGDCabinetLauncher.logoHGDRast.png")));
            }

            try
            {
                //generate qr code for current game's link
                QRCodeData codeData = _gen.CreateQrCode(
                    _finder.GameList[uiList.SelectedIndex].Link,
                    QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrImage = new(codeData);
                byte[] graphic = qrImage.GetGraphic(10);

                using MemoryStream ms = new(graphic);
                Bitmap bmp = new(ms);
                this.qrImage.Source = bmp;
            }
            catch (Exception err)
            {
                Console.WriteLine("failed to create qr code, using fallback...");
                Console.WriteLine(err.Message);
                
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                this.qrImage.Source = new Bitmap(assets.Open(new Uri("resm:HGDCabinetLauncher.logoHGDRast.png")));
            }
        }

        //build new avalonia list for listbox once it's loaded in
        private void uiList_OnLoaded(object? sender, RoutedEventArgs e)
        {
            foreach (var gameData in _finder.GameList)
            {
                _uiList.Add(new() {Content = gameData.Name});
            }

            uiList.Items = _uiList;
            uiList.SelectedIndex = 0; //forces a selection so focus is on the listbox
        }

        //play the game
        private void buttonPlay(object? sender, RoutedEventArgs e)
        {
            _finder.playGame(uiList.SelectedIndex);
        }

        //unused, for opening a browser upon button click
        private void Link_OnClick(object? sender, RoutedEventArgs e)
        {
            //open link to site, borrowed from stack overflow since C# lacks a standard way of opening links
            //(wish this was abstracted in a dotnet core class like with directories)
            //this is now disabled since it would be dumb to open a browser on an arcade cabinet
            //may bring it back if I introduce builds meant to work on systems besides the cabinet

            //  if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // {
            //     Process.Start(new ProcessStartInfo((string) this.link.Tag) { UseShellExecute = true });
            // }
            // else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // {
            //     Process.Start("xdg-open", (string) this.link.Tag);
            // }
            // else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // {
            //     Process.Start("open", (string) this.link.Tag);
            // }
        }

        //handling input for moving along the list of games in the interface
        private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
        {
            //because I hate how this toolkit handles
            //focus since focusing the listbox is not the same as focusing the listbox items
            switch (e.Key)
            {
                case Key.W:
                    if (uiList.SelectedIndex > 0) this.uiList.SelectedIndex -= 1;
                    break;
                case Key.S:
                    if (uiList.SelectedIndex < uiList.ItemCount - 1) this.uiList.SelectedIndex += 1;
                    break;
            }
        }
    }
}
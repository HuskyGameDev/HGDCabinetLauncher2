using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;

namespace HGDCabinetLauncher
{
    public partial class MainWindow : Window
    {
        private AvaloniaList<ListBoxItem> uiList = new();
        private GameFinder finder;
        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            finder = new();
        }

        private void GameList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //probably don't need this method at all, will remove later
            ListBox item = (ListBox) sender;
            Console.WriteLine($"selection: {item.SelectedIndex}");
            
            this.name.Text = finder.gameList[item.SelectedIndex].name;
            this.desc.Text = "Description:\n" + finder.gameList[item.SelectedIndex].desc;
            this.ver.Text = "Version:\n" + finder.gameList[item.SelectedIndex].version;
            this.authors.Text = "Author(s):\n" + finder.gameList[item.SelectedIndex].authors;
            //set data for link opening stuff via click event
            this.link.Tag = finder.gameList[item.SelectedIndex].link;
        }

        //build new avalonia list for listbox once it's properly loaded in
        //done through this so we get a proper reference to the listbox that won't be null
        private void GameList_OnLoaded(object? sender, RoutedEventArgs e)
        {
            ListBox item = (ListBox) sender;

            foreach (var gameData in finder.gameList)
            {
                uiList.Add(new (){Content = gameData.name});
            }
            
            item.Items = uiList;
            item.SelectedIndex = 0; //forces a selection so focus is on the listbox
        }

        private void ButtonPlay(object? sender, RoutedEventArgs e)
        {
            ListBox item = this.FindControl<ListBox>("gameList");
            finder.playGame(item.SelectedIndex);
        }

        private void GameList_OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            ListBox item = (ListBox) sender;
            finder.playGame(item.SelectedIndex);
        }

        private void Link_OnClick(object? sender, RoutedEventArgs e)
        {
            //open link to site, borrowed from stack overflow since C# lacks a standard way of opening links
            //(wish this was abstracted in a dotnet core class like with directories)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo((string) this.link.Tag) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", (string) this.link.Tag);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", (string) this.link.Tag);
            }
        }
    }
}

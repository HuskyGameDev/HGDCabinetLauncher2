using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Linq;
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
        }

        private void InitializeComponent()
        {
            finder = new();
            AvaloniaXamlLoader.Load(this);
        }

        private void GameList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //probably don't need this method at all, will remove later
            //ListBox item = (ListBox) sender;
            //Console.WriteLine(item.SelectedIndex);
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
    }
}

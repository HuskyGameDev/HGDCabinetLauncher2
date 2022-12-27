using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace HGDCabinetLauncher
{
    public partial class MainWindow : Window
    {
        private AvaloniaList<ListBoxItem> gamesList = new();
        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void GameList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ListBox item = (ListBox) sender;
            if(item.DataContext != null) Console.WriteLine(item.DataContext);
        }

        private void GameList_OnLoaded(object? sender, RoutedEventArgs e)
        {
            ListBox item = (ListBox) sender;
            GameFinder finder = new();
            
            gamesList.Add(new(){Content = "hi"});
            gamesList.Add(new (){Content = "test"});
            item.Items = gamesList;
            item.SelectedIndex = 0;
        }
    }
}

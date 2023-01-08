using System;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Reflection;
using System.Threading;
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
            Console.WriteLine(this.panel.Name);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void GameList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //probably don't need this method at all, will remove later
            ListBox item = (ListBox) sender;
            Console.WriteLine($"selection: {item.SelectedIndex}");
            if (!this.IsLoaded) return;
            this.name.Text = finder.gameList[item.SelectedIndex].name;
            this.desc.Text = finder.gameList[item.SelectedIndex].desc;
            this.ver.Text = finder.gameList[item.SelectedIndex].version;
            this.link.Text = finder.gameList[item.SelectedIndex].link;
            this.authors.Text = finder.gameList[item.SelectedIndex].authors;
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

        private void Control_OnLoaded(object? sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel) sender;
            AvaloniaList<IControl>.Enumerator boxes = panel.Children.GetEnumerator();
            foreach (TextBox box in panel.Children)
            {
                box.Text = finder.gameList[0].name;
            }

            Type t = typeof(GameMeta);
            PropertyInfo[] props = typeof(GameMeta).GetProperties(BindingFlags.Public|BindingFlags.Instance);
            Console.WriteLine(props.Length);
            foreach (PropertyInfo p in typeof(GameMeta).GetProperties())
            {
                Console.WriteLine(p.Name);
            }
            for (int i = 0; i < props.Length; i++)
            {
                Console.WriteLine(props[i].GetValue(finder.gameList[i]) );
            }
            // this.name.Text = finder.gameList[0].name;
            // this.desc.Text = finder.gameList[0].desc;
            // this.ver.Text = finder.gameList[0].version;
            // this.link.Text = finder.gameList[0].link;
            // this.authors.Text = finder.gameList[0].authors;
        }
    }
}

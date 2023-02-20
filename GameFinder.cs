using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using QRCoder;

namespace HGDCabinetLauncher;

public class GameFinder
{
    
    [DllImport("user32.dll")]
    private static extern int SetForegroundWindow(int hwnd);

    public GameMeta[] GameList { get; } //list of game metadata like file paths and information to display
    private bool isRunning; //used to ensure only one game is running at any given time

    public bool getRunning()
    {
        return isRunning;
    } 
    
    public GameFinder()
    {
        
        Console.WriteLine("Indexing game metafiles!");
        
        /*
         desktop is used instead of documents because on
         linux C# regards ~/ as the documents folder instead of ~/Documents, maybe some distros don't have ~/Documents
         but this just seems like lazy/poorly implemented code to me :/
        */
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //attempt to create games folder if it does not already exist, catch error if failure
        try
        {
            Directory.CreateDirectory(desktop + "/Games");
        }
        catch (Exception err)
        {
            Console.WriteLine("Could not create or access games folder!");
            Console.WriteLine(err.Message);
            GameList = Array.Empty<GameMeta>();
            return;
        }

        string[] fileList = Directory.GetFiles
        (
            (desktop + "/Games"),
            "meta.json", SearchOption.AllDirectories
        ); //look for any meta.json files within a Games folder on the Desktop

        GameList = new GameMeta[fileList.Length];

        QRCodeGenerator codeGen = new();

        for (int i = 0; i < fileList.Length; i++)
        {
            string metaStr = File.ReadAllText(fileList[i]);
            //create empty game meta if deserialization goes sideways
            GameList[i] = JsonConvert.DeserializeObject<GameMeta>(metaStr) ?? new GameMeta();
            GameList[i].ExecLoc = fileList[i][..(fileList[i].Length - 9)];
            
            //load in sample image for game and store it
            try
            {
                //construct bitmap with full path to image
                IImage img = new Bitmap(
                    GameList[i].ExecLoc +
                    GameList[i].ImgDir);

                GameList[i].gameImage = (img);
            }
            catch (FileNotFoundException err)
            {
                Console.WriteLine("failed to set reference image! using fallback...");
                Console.WriteLine(err.Message);
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                //use embedded fallback resource
                GameList[i].gameImage = new Bitmap(assets.Open(new Uri("resm:HGDCabinetLauncher.logoHGDRast.png")));
            }
            //generate qr code based on metadata link and store it
            try
            {
                //generate qr code for current game's link
                QRCodeData codeData = codeGen.CreateQrCode(
                    GameList[i].Link,
                    QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrImage = new(codeData);
                byte[] graphic = qrImage.GetGraphic(10);

                using MemoryStream ms = new(graphic);
                GameList[i].qrImage = new Bitmap(ms);
                //dispose ALL the objects!
                ms.Dispose();
                qrImage.Dispose();
                codeData.Dispose();
                graphic = null;
            }
            catch (Exception err)
            {
                Console.WriteLine("failed to create qr code, using fallback...");
                Console.WriteLine(err.Message);

                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                GameList[i].qrImage = new Bitmap(assets.Open(new Uri("resm:HGDCabinetLauncher.logoHGDRast.png")));
            }
            codeGen.Dispose();
            
            Console.WriteLine($"found data for: {GameList[i].Name}");
        }

        Console.WriteLine($"Index complete! Found {GameList.Length} games");
    }

    //false if there was an error running the game
    public async void playGame(int index)
    {
        if (isRunning) return;
        Console.WriteLine($"Starting game index {index} name \"{GameList[index].Name}\"");
        isRunning = true;
        using Process gameProcess = new();
        try
        {
            //for linux make sure binaries have the execution bit set
            
            gameProcess.StartInfo.UseShellExecute = false;
            gameProcess.StartInfo.WorkingDirectory = GameList[index].ExecLoc;
            gameProcess.StartInfo.FileName = GameList[index].ExecLoc + GameList[index].Exec;
            gameProcess.StartInfo.CreateNoWindow = true;
            gameProcess.Start(); //I hope you have your file associations correct!
            SetForegroundWindow((int) gameProcess.MainWindowHandle);

            //using async and await calls instead of the exit event since the
            //event fails to fire if this method finishes, defeating the purpose of using an event at all
        }
        catch (Exception e)
        {
            Console.WriteLine("process had an error!");
            Console.WriteLine(e.Message);
        }
        
        await gameProcess.WaitForExitAsync();
        Console.WriteLine("process exited!");

        isRunning = false;
    }
}
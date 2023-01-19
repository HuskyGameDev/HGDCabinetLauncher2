using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace HGDCabinetLauncher;

public class GameFinder
{
    public GameMeta[] GameList { get; } //list of game metadata like file paths and information to display
    private bool _isRunning; //used to ensure only one game is running at any given time

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

        for (int i = 0; i < fileList.Length; i++)
        {
            string metaStr = File.ReadAllText(fileList[i]);
            //create empty game meta if deserialization goes sideways
            GameList[i] = JsonConvert.DeserializeObject<GameMeta>(metaStr) ?? new GameMeta();
            GameList[i].ExecLoc = fileList[i][..(fileList[i].Length - 9)];
            Console.WriteLine($"found data for: {GameList[i].Name}");
        }

        Console.WriteLine($"Index complete! Found {GameList.Length} games");
    }

    //false if there was an error running the game
    public async void playGame(int index)
    {
        if (_isRunning) return;
        Console.WriteLine($"Starting game index {index} name \"{GameList[index].Name}\"");
        _isRunning = true;
        try
        {
            //for linux make sure binaries have the execution bit set
            using Process gameProcess = new();
            gameProcess.StartInfo.UseShellExecute = false;
            gameProcess.StartInfo.WorkingDirectory = GameList[index].ExecLoc;
            gameProcess.StartInfo.FileName = GameList[index].ExecLoc + GameList[index].Exec;
            gameProcess.StartInfo.CreateNoWindow = false;
            gameProcess.Start(); //I hope you have your file associations correct!

            //using async and await calls instead of the exit event since the
            //event fails to fire if this method finishes, defeating the purpose of using an event at all
            await gameProcess.WaitForExitAsync();
            Console.WriteLine("process exited!");
        }
        catch (Exception e)
        {
            Console.WriteLine("process crashed!");
            Console.WriteLine(e.Message);
        }

        _isRunning = false;
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HGDCabinetLauncher;

public class GameFinder
{
    public GameMeta[] gameList { get; }
    private bool isRunning; //used to ensure only one game is running at any given time

    public GameFinder()
    {
        Console.WriteLine("Indexing game metafiles!");
        
        /*
         desktop is used instead of documents because on
         linux C# regards ~/ as the documents folder instead of ~/Documents, maybe some distros don't have ~/Documents
         but this just seems like lazy code to me :/
        */
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string[] fileList = Directory.GetFiles
        (
            (desktop + "/Games"),
            "meta.json", SearchOption.AllDirectories
        ); //look for any meta.json files within a Games folder on the Desktop

        gameList = new GameMeta[fileList.Length];

        for (int i = 0; i < fileList.Length; i++)
        {
            string metaStr = File.ReadAllText(fileList[i]);
            gameList[i] = JsonConvert.DeserializeObject<GameMeta>(metaStr);
            gameList[i].execLoc = fileList[i].Substring(0, fileList[i].Length - 9);
            Console.WriteLine($"found data for: {gameList[i].name}");
        }

        Console.WriteLine($"Index complete! Found {gameList.Length} games");
    }

    //false if there was an error running the game
    public async void playGame(int index)
    {
        if (isRunning) return;
        Console.WriteLine($"Starting game index {index} name \"{gameList[index].name}\"");
        isRunning = true;
        try
        {
            //for linux make sure binaries have the execution bit set
            using Process gameProcess = new();
            gameProcess.StartInfo.UseShellExecute = false;
            gameProcess.StartInfo.WorkingDirectory = gameList[index].execLoc;
            gameProcess.StartInfo.FileName = gameList[index].execLoc + gameList[index].exec;
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

        isRunning = false;
    }
}
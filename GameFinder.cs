using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace HGDCabinetLauncher;

public class GameFinder
{
    public GameMeta[] gameList { get; }

    public GameFinder()
    {
        Console.WriteLine("Indexing games!");
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string[] fileList = Directory.GetFiles((desktop + "/Games"), "meta.json", SearchOption.AllDirectories);
        gameList = new GameMeta[fileList.Length];
        
        for(int i = 0; i < fileList.Length; i++)
        {
            string metaStr = File.ReadAllText(fileList[i]);
            gameList[i] = JsonConvert.DeserializeObject<GameMeta>(metaStr);
            gameList[i].execLoc = fileList[i].Substring(0, fileList[i].Length - 9);
            Console.WriteLine($"found: {gameList[i].name}");
        }
        Console.WriteLine($"Index complete! Found {gameList.Length} games");
    }
    
    public GameFinder(string directory)
    {
        //may or may not use this, more likely to make a settings.json or launch arg to control directory scans
    }

    //false if there was an error running the game
    public bool playGame(int index)
    {
        Console.WriteLine($"Starting game index {index} name \"{gameList[index].name}\"");
        
        try
        {
            //for linux make sure binaries have the execution bit set
            using Process gameProcess = new();
            gameProcess.StartInfo.UseShellExecute = false;
            gameProcess.StartInfo.WorkingDirectory = gameList[index].execLoc;
            gameProcess.StartInfo.FileName = gameList[index].execLoc + gameList[index].exec;
            gameProcess.StartInfo.CreateNoWindow = false;
            gameProcess.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        
        return true;
    }
}
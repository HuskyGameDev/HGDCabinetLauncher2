using System;
using System.IO;
using Newtonsoft.Json;

namespace HGDCabinetLauncher;

public class GameFinder
{
    private GameMeta[] gameList { get; }

    public GameFinder()
    {
        string[] metaList = Directory.GetFiles("/home/soup/Documents/GitHub/HGDCabinetLauncher2/Games/", "meta.json", SearchOption.AllDirectories);
        gameList = new GameMeta[metaList.Length];
        for(int i = 0; i < metaList.Length; i++)
        {
            Console.WriteLine(metaList[i]);
            string metaStr = File.ReadAllText(metaList[i]);
            gameList[i] = JsonConvert.DeserializeObject<GameMeta>(metaStr);
            Console.WriteLine(gameList[i].name);
        }
    }
    
    public GameFinder(string directory)
    {
        
    }
}
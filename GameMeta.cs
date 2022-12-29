namespace HGDCabinetLauncher;

public class GameMeta
{
    public string name;     //name of game
    public string exec;     //name of executable plus extension
    public string execLoc;  //folder exec resides in, determined at runtime
    //these are all vanity and self explanatory, if missing the launcher will have a placeholder
    public string desc;
    public string version;
    public string link;     //as in website link
    public string authors;
    public string iconDir;
    public string imgDir;
}
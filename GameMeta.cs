using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace HGDCabinetLauncher;

public class GameMeta
{
    public string? Name;     //name of game
    public string? Desc;
    public string? Version;
    public string? Link;     //as in website link
    public string? Authors;

    public string? Exec;     //name of executable plus extension
    public string? ExecLoc;  //folder exec resides in, determined at runtime
    public string? IconDir;
    public string? ImgDir;

    public Bitmap qrImage;
    public IImage gameImage;
}
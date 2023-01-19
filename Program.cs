using System;
using System.IO;
using Avalonia;
using Avalonia.Svg;

namespace HGDCabinetLauncher
{
    internal static class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        // ReSharper disable once InconsistentNaming
        public static void Main(string[] args)
        {
#if !DEBUG
            using StreamWriter logWriter = new("log.txt", append:true);
            logWriter.AutoFlush = true;
            logWriter.WriteLine($"Logfile for HGD arcade cabinet, generated on {DateTime.Now.ToString()}");
            Console.SetOut(logWriter);
#endif
            try
            {
                buildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception err)
            {
                Console.WriteLine("Launcher crash! dumping log and exiting...");
                Console.WriteLine(err);
#if !DEBUG
                logWriter.Flush();
                logWriter.Close();
#endif
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder buildAvaloniaApp()
        {
            GC.KeepAlive(typeof(SvgImageExtension).Assembly);
            GC.KeepAlive(typeof(Avalonia.Svg.Svg).Assembly);
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }
}
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Utils;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using CmlLib.Core.Downloader;
using CmlLib.Core.Files;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Console_Launcher
{
    class Program
    {
        static Process Minecraft;


        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.Main());
        }
        static string GetJava()
        {
            string java_home = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(java_home))
            {
                foreach (string str in Environment.GetEnvironmentVariable("PATH").Split(";"))
                {
                    if (str.Contains("Oracle\\Java\\javapath"))
                        return str + "\\javaw.exe";
                }
            }

            return java_home + "\\bin\\javaw.exe";
        }

        static int GetMemory()
        {
            return (int)Math.Floor((double) GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1048576.0);
        }

        static async Task Main(string[] args)
        {

            MinecraftPath data_folder = new MinecraftPath("data");
            string data_path = data_folder.ToString() + "\\";

            UserConfig cfg = new UserConfig(data_path + "launcher.json");

            if (string.IsNullOrEmpty(cfg.JavaPath))
                cfg.JavaPath = GetJava();

            while (string.IsNullOrEmpty(cfg.Nickname) || cfg.Nickname.Length < 5)
            {
                Console.Write("Enter nickname: ");
                cfg.Nickname = Regex.Replace(Console.ReadLine(), "\\s+", "");
            }

            if (cfg.Memory == null)
            {
                cfg.Memory = new int[2];
                int mem = GetMemory();
                if (mem > 4096)
                    cfg.Memory[0] = 512;
                else
                    cfg.Memory[0] = 256;

                cfg.Memory[1] = (int) Math.Floor(mem / 2.6);
            }

            while (string.IsNullOrEmpty(cfg.Version) || cfg.Version.Length < 2)
            {
                Console.Write("Enter version: ");
                cfg.Version = Regex.Replace(Console.ReadLine(), "\\s+", "");
            }

            cfg.Save();

            Console.WriteLine("Nickname: {0}\nJava: {1}\nMemory:\n\tMax: {2} MB\n\tMin: {3}MB\n", cfg.Nickname, cfg.JavaPath, cfg.Memory[0], cfg.Memory[1]);
            Console.WriteLine("Skin: https://skin.prinzeugen.net/ \n");

            //IDownloader downloader = new SequenceDownloader();
            //Progress<DownloadFileChangedEventArgs> fileProgress = new Progress<DownloadFileChangedEventArgs>(e => FileChanged?.Invoke(e));

            //LibraryChecker libraryChecker = new LibraryChecker();
            //DownloadFile[] files = libraryChecker.CheckFiles(data_folder, new Array(), fileProgress);

            CMLauncher launcher = new CMLauncher(data_folder);

            launcher.FileChanged += (e) =>
            {
                Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
            };

            var launchOption = new MLaunchOption
            {
                MinimumRamMb = cfg.Memory[0],
                MaximumRamMb = cfg.Memory[1],
                Session = MSession.GetOfflineSession(cfg.Nickname),
                JavaPath = cfg.JavaPath,
                GameLauncherName = "Console Launcher",
                //ServerIp = "mc.hypixel.net"
            };

            Minecraft = await launcher.CreateProcessAsync(cfg.Version, launchOption);
            
            ProcessUtil util = new ProcessUtil(Minecraft);
            util.OutputReceived += (s, str) => Console.WriteLine(str);//log4j.Print(str);
            util.Exited += new EventHandler(minecraft_Exited);

            util.StartWithEvents();
            Minecraft.WaitForExit();
            if (true) {

            }
        }

        static void minecraft_Exited(object sender, System.EventArgs e)
        {
            Console.WriteLine(
                $"Exit time    : {Minecraft.ExitTime}\n" +
                $"Exit code    : {Minecraft.ExitCode}\n" +
                $"Elapsed time : {Math.Round((Minecraft.ExitTime - Minecraft.StartTime).TotalMilliseconds)}");
        }

    }
}

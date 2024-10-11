using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Steam_Icons_Fix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> files = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (Directory.Exists(args[i]))
                {
                    files.AddRange(Tool.SearchDirectory(args[i], true, new string[] { "*.url" }));
                }
                else if (File.Exists(args[i]))
                {
                    files.Add(args[i]);
                }
            }

            for (int i = 0; i < files.Count; i++)
            {
                string appId = "";
                FileInfo path = null;
                string iconHash = "";

                bool validSteamShortcut = false;

                string[] lines = File.ReadAllLines(files[i]);
                foreach (string line in lines)
                {
                    if (line.StartsWith("URL="))
                    {
                        if (line.Contains("steam://rungameid/"))
                        {
                            validSteamShortcut = true;
                        }
                        else
                        {
                            break;
                        }

                        appId = line.Split('/').Last();
                    }
                    else if (line.StartsWith("IconFile="))
                    {
                        path = new FileInfo(line.Split('=').Last());

                        if (!path.FullName.ToLower().EndsWith(".ico"))
                        {
                            validSteamShortcut = false;
                            break;
                        }

                        iconHash = Path.GetFileNameWithoutExtension(path.FullName);
                    }
                }

                if (!validSteamShortcut)
                {
                    continue;
                }

                if (appId != "" && path != null && iconHash != "")
                {
                    try
                    {
                        path.Directory.Create();
                        Steam.DownloadIconAsync(appId, iconHash, path.FullName).Wait();
                        Console.WriteLine("Success: " + Path.GetFileNameWithoutExtension(files[i]));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed (" + ex.Message + "): " + Path.GetFileNameWithoutExtension(files[i]));
                    }
                }
                else
                {
                    Console.WriteLine("Failed: " + Path.GetFileNameWithoutExtension(files[i]));
                }
            }

            Console.WriteLine("");
            Console.Write("Done");
            Console.ReadKey();
        }
    }
}

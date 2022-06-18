/*
 * 6/17/22 11:33 PM
 * PlexoDev/PlexoLauncherMain/Program.cs
 * Main launcher for first time installations of Plexo
 * pos0 2022
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PlexoLauncherMain
{
    class Program
    {
        private static string currentVersion = ""; // we get this later on
        private static string tempPath = Path.GetTempPath();
        private static string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string getStringFromUrl(string url)
        {
            using(WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
        private static string generateRandString(int length = 8)
        {
            const string chars = "ABCDEF0123456789";

            Random random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }
        private static async Task downloadFile(string url, string location)
        {
            using(WebClient wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(url, location);
            }
        }
        private static string generateBaseUrl(int type)
        {
            switch(type)
            {
                case 0: // plexxo.xyz
                    return "http://plexxo.xyz";
                case 1: // setup.plexxo.xyz
                    return "http://setup.plexxo.xyz";
                case 2: // api.plexxo.xyz
                    return "http://api.plexxo.xyz";
                default: // plexxo.xyz
                    return "http://plexxo.xyz";
            }
        }
        private static void registerInRegistry()
        {
            // Prelauncher
            RegistryKey key = Registry.ClassesRoot.CreateSubKey("plexo-prelaunch14L");
            key.SetValue("", "URL: PlexoPreLaunch14L Protocol");
            key.SetValue("URL Protocol", "");

            RegistryKey key1 = key.CreateSubKey("DefaultIcon");
            key1.SetValue("", localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe");

            RegistryKey key2 = key.CreateSubKey("shell");
            RegistryKey key3 = key2.CreateSubKey("open");

            RegistryKey key4 = key3.CreateSubKey("command");
            key4.SetValue("", "\"" + localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe" + "\" %1");

            Console.WriteLine("Created all keys for Prelauncher.");

            // Actual launcher
            RegistryKey key5 = Registry.ClassesRoot.CreateSubKey("ple14l-player");
            key5.SetValue("", "URL: Ple14L Protocol");
            key5.SetValue("URL Protocol", "");

            RegistryKey key6 = key.CreateSubKey("DefaultIcon");
            key6.SetValue("", localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe");

            RegistryKey key7 = key.CreateSubKey("shell");
            RegistryKey key8 = key7.CreateSubKey("open");

            RegistryKey key9 = key8.CreateSubKey("command");
            key9.SetValue("", "\"" + localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe" + "\" %1");

            Console.WriteLine("Created all keys for actual launcher.");
            Console.WriteLine("Created all keys successfully!");
        }
        private static void process_Exited(object sender, EventArgs args)
        {
            // we need to make sure that all of these actually exist before we create keys in the registry
            if(File.Exists(localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe")
                && File.Exists(localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe"))
            {
                Console.WriteLine("Plexo installation part one was a success, let's try to register our keys in the registry.");
                registerInRegistry();
            } else
            {
                Console.WriteLine("Cannot continue, as Plexo installation part one failed.");
                Environment.Exit(0);
            }
        }
        static async Task Main(string[] args)
        {
            string bootstrapperExePath = tempPath + "\\PlexoPlayerLauncher-" + generateRandString() + ".exe";
            currentVersion = getStringFromUrl(generateBaseUrl(1) + "/version");
            Console.WriteLine("Downloading Bootstrapper...");
            await downloadFile(generateBaseUrl(1) + "/Roblox.exe", bootstrapperExePath);
            if(File.Exists(bootstrapperExePath))
            {
                Console.WriteLine("Bootstrapper found!");

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = bootstrapperExePath;

                Process process = new Process();
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(process_Exited);
                process.Start();

                Console.WriteLine("Started Bootstrapper");
            } else
            {
                Console.WriteLine("Cannot continue with installation, as our Bootstrapper was not found.");
            }
            //Environment.Exit(0);
        }
    }
}

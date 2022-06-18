﻿/*
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
using System.Windows;
using Microsoft.Win32;

namespace PlexoLauncherMain
{
    class Program
    {
        private static string currentVersion = ""; // we get this later on
        private static string tempPath = Path.GetTempPath();
        private static string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static RegistryKey softwareClasses = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Classes", true);
        private static bool uninstalling = false;
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
                try
                {
                    await wc.DownloadFileTaskAsync(url, location);
                } catch (WebException e)
                {
                    MessageBox.Show("Plexo was not downloaded. Error: " + e.Message + " Ask for help in the Plexo discord, if needed.", "Error");
                }
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
            RegistryKey key = softwareClasses.CreateSubKey("plexo-prelaunch14l", true);
            key.SetValue("", "URL: PlexoPreLaunch14L Protocol");
            key.SetValue("URL Protocol", "");

            RegistryKey key1 = key.CreateSubKey("DefaultIcon", true);
            key1.SetValue("", localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe");

            RegistryKey key2 = key.CreateSubKey("shell", true);
            RegistryKey key3 = key2.CreateSubKey("open", true);

            RegistryKey key4 = key3.CreateSubKey("command", true);
            key4.SetValue("", "\"" + localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe" + "\" %1");

            Console.WriteLine("Created all keys for Prelauncher.");

            // Actual launcher
            RegistryKey key5 = softwareClasses.CreateSubKey("ple14l-player", true);
            key5.SetValue("", "URL: Ple14L Protocol");
            key5.SetValue("URL Protocol", "");

            RegistryKey key6 = key5.CreateSubKey("DefaultIcon", true);
            key6.SetValue("", localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe");

            RegistryKey key7 = key5.CreateSubKey("shell", true);
            RegistryKey key8 = key7.CreateSubKey("open", true);

            RegistryKey key9 = key8.CreateSubKey("command", true);
            key9.SetValue("", "\"" + localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe" + "\" %1");

            // Let's close everything now
            key.Close();
            key1.Close();
            key2.Close();
            key3.Close();
            key4.Close();
            key5.Close();
            key6.Close();
            key7.Close();
            key8.Close();
            key9.Close();

            Console.WriteLine("Created all keys for actual launcher.");
            Console.WriteLine("Created all keys successfully!");
        }
        private static bool regKeyExists(RegistryKey baseKey, string subKeyName)
        {
            RegistryKey ret = baseKey.OpenSubKey(subKeyName);

            return ret != null;
        }
        private static void process_Exited(object sender, EventArgs args)
        {
            // we need to make sure that all of these actually exist before we create keys in the registry
            if(File.Exists(localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\RobloxPlayerLauncher.exe")
                && File.Exists(localAppDataPath + "\\Ple14L\\Versions\\" + currentVersion + "\\PlexoParseLauncherUri.exe"))
            {
                Console.WriteLine("Plexo installation part one was a success, let's try to register our keys in the registry.");
                try
                {
                    registerInRegistry();
                    MessageBox.Show("Plexo Install was successful.", "Success");
                } catch (Exception e)
                {
                    MessageBox.Show("Plexo Install failed whilst writing registry keys. Error: " + e.Message, "Error");
                }
            } else
            {
                Console.WriteLine("Cannot continue, as Plexo installation part one failed.");
                MessageBox.Show("Plexo was not completely downloaded. Try installing again. If the issue persists, ask for help in the Discord.", "Error");
                Environment.Exit(0);
            }
        }
        private static void checkIfInstalled()
        {
            if (regKeyExists(softwareClasses, "plexo-prelaunch14l") || regKeyExists(softwareClasses, "ple14l-player"))
            {
                Console.WriteLine("Plexo is already installed!");
                MessageBox.Show("Plexo is already installed!", "Error");
                Environment.Exit(0);
            }
        }
        static async Task Main(string[] args)
        {
            checkIfInstalled();

            foreach(string arg in args)
            {
                switch(arg.ToLower())
                {
                    case "-uninstall":
                        Console.WriteLine("Uninstalling argument found");
                        uninstalling = true;
                        break;
                    default:
                        break;
                }
            }

            if(!uninstalling)
            {
                string bootstrapperExePath = tempPath + "\\PlexoPlayerLauncher-" + generateRandString() + ".exe";
                currentVersion = getStringFromUrl(generateBaseUrl(1) + "/version");

                Console.WriteLine("Downloading Bootstrapper...");
                await downloadFile(generateBaseUrl(1) + "/Roblox.exe", bootstrapperExePath);

                if (File.Exists(bootstrapperExePath))
                {
                    Console.WriteLine("Bootstrapper found!");

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = bootstrapperExePath;

                    Process process = new Process();
                    process.EnableRaisingEvents = true;
                    process.Exited += new EventHandler(process_Exited);
                    process.StartInfo = info;
                    process.Start();
                    process.WaitForExit();

                    Console.WriteLine("Started Bootstrapper");
                }
                else
                {
                    Console.WriteLine("Cannot continue with installation, as our Bootstrapper was not found.");
                    MessageBox.Show("Plexo Install was unsuccessful. Bootstrapper was not found.", "Error");
                }
            } else
            {
                Console.WriteLine("Uninstalling...");

                if(Directory.Exists(localAppDataPath + "\\Ple14L"))
                {
                    Directory.Delete(localAppDataPath + "\\Ple14L", true);
                }

                if(regKeyExists(softwareClasses, "plexo-prelaunch14l") && regKeyExists(softwareClasses, "ple14l-player"))
                {
                    softwareClasses.DeleteSubKey("plexo-prelaunch14l");
                }

                MessageBox.Show("Plexo Uninstall was successful.", "Uninstalled");
            }
            //Environment.Exit(0);
        }
    }
}

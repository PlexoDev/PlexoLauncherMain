using System;
using System.Collections.Generic;
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
        private static string getStringFromUrl(string url)
        {
            using(WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
        private static void downloadFile(string url, string location)
        {
            using(WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, location);
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
        static void Main(string[] args)
        {
            currentVersion = getStringFromUrl(generateBaseUrl(1) + "/version");
            downloadFile(generateBaseUrl(1) + "/Roblox.exe", tempPath);
            // :troll:
        }
    }
}

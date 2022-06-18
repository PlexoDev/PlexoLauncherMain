using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexoLauncherMain
{
    class Program
    {
        private static string currentVersion = "version-b017cc18d6f77d3d";
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
                default:
                    return "http://plexxo.xyz";
            }
        }
        static void Main(string[] args)
        {
        }
    }
}

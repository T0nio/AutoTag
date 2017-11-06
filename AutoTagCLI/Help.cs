using System;

namespace AutoTagCLI
{
    public static class Help
    {
        public static void DisplayHelp()
        {
            Console.WriteLine("autotag 0.0.94 - (C) 2017");
            Console.WriteLine("Released under the GNU GPL.");
            Console.WriteLine();
            Console.WriteLine("-d DIRECTORY        - Select the directory to analyse");
            Console.WriteLine("-dd DIRECTORY       - The destination directory.");
            Console.WriteLine("-r                  - Enable music recognition");
            Console.WriteLine("-m                  - Enable reorganisation by moving");
            Console.WriteLine("-c                  - Enable reorganisation by moving");
            Console.WriteLine("--help              - This help");
            
        }    
    }
}
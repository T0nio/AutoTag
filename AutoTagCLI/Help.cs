using System;

namespace AutoTagCLI
{
    public static class Help
    {
        public static void DisplayHelp()
        {
            Console.WriteLine("autotag 0.0.84 - (C) 2017");
            Console.WriteLine("Released under the GNU GPL.");
            Console.WriteLine();
            Console.WriteLine("-d DIRECTORY        - Select the directory to analyse");
            Console.WriteLine("-ro                 - Recognize only mode");
            Console.WriteLine("-rm                 - Recognize and move");
            Console.WriteLine("-rc                 - Recognize and copy");
            Console.WriteLine("-dd DIRECTORY       - The destination directory.");
            Console.WriteLine("--help              - This help");
            
        }    
    }
}
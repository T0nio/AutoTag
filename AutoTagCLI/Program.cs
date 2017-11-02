using System;
using System.IO;
using AutoTagLib;

namespace AutoTagCLI
{
    class Program
    {
        public enum ACTION {
            noaction, help, recognizeonly, recognizeorganizemv, recognizeorganizecp
        }
        
        static void Main(string[] args)
        {
            
            
            int i = 0;

            string directory = "";
            string destinationDirectory = "";

            ACTION action = ACTION.noaction;
            
            while (i < args.Length)
            {
                if (action != ACTION.help)
                {
                    switch (args[i])
                    {
                        case "--help":
                            i = args.Length;
                            action = ACTION.help;
                            break;

                        case "-d":
                            i++;
                            directory = args[i];
                            break;

                        case "-dd":
                            i++;
                            destinationDirectory = args[i];
                            break;
                        
                        case "-ro":
                            action = ACTION.recognizeonly;
                            break;
                            
                        case "-rm":
                            action = ACTION.recognizeorganizemv;
                            break;
                            
                        case "-rc":
                            action = ACTION.recognizeorganizecp;
                            break;
                    }
                    i++;
                }
                else
                {
                    i = args.Length;
                }
            }
            if (action == ACTION.noaction)
            {
                action = ACTION.help;
            }

            if (action == ACTION.help)
            {
                Help.DisplayHelp();
            }
            else
            {
                if ((action == ACTION.recognizeorganizecp || action == ACTION.recognizeorganizemv) &&
                    (directory == "" || destinationDirectory == ""))
                {
                    Console.WriteLine("Please enter a directory to filter AND a destination directory.");
                }
                else
                {
                    var myMusics = new MusicsLib();
                    //musicList.Extensions = myNewExtensions;
                    try
                    {
                        // Add possibility of just reorganize
                        
                        myMusics.LoadFromFolder(directory);
                        
                        myMusics.ReadTags();
                        
                        myMusics.WriteTags();

                        if (action != ACTION.recognizeonly)
                        {
                            myMusics.Reorganize(destinationDirectory, action == ACTION.recognizeorganizecp);
                        }
                         
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Directory name invalid");
                        throw;
                    }
                }
            }
        }
    }
}

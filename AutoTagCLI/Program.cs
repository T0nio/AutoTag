using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using AutoTagLib;
using AutoTagLib.ErrorManager;

namespace AutoTagCLI
{
    class Program
    {
        public enum ACTION {
            help, recognize, move, copy
        }

        static void Main(string[] args)
        {
            var errorManager = (IErrorManager)CLIErrorManager.GetInstance();
            Lookup.GetInstance().Register(typeof(IErrorManager), errorManager);
           
            
            int i = 0;
            string directory = string.Empty;
            string destinationDirectory = string.Empty;
            List<ACTION> action = new List<ACTION>();

            while (i < args.Length)
            {
                // When we triggered the help action, we don't do nothing more
                if (action.IndexOf(ACTION.help) == -1)
                {
                    switch (args[i])
                    {
                        case "--help":
                            i = args.Length;
                            action.Add(ACTION.help);
                            break;

                        case "-d":
                            i++;
                            directory = args[i];
                            break;

                        case "-dd":
                            i++;
                            destinationDirectory = args[i];
                            break;
                            
                        default:
                            if (args[i][0] == '-')
                            {
                                foreach (var options in args[i].Substring(1))
                                {
                                    switch (options)
                                    {
                                        case 'r':
                                            action.Add(ACTION.recognize);
                                            break;
                                        
                                        case 'm':
                                            action.Add(ACTION.move);
                                            break;
                                        
                                        case 'c':
                                            action.Add(ACTION.copy);
                                            break;
                                        default:
                                            action.Add(ACTION.help);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                action.Add(ACTION.help);
                            }
                            break;
                    }
                    i++;
                }
                else
                {
                    i = args.Length;
                }
            }
            if(action.Count == 0)
            {
                action.Add(ACTION.help);
            }
    
            if (action.Contains(ACTION.help))
            {
                Help.DisplayHelp();
            }
            else
            {
                if (action.Contains(ACTION.recognize) && directory == String.Empty)
                {
                    Console.WriteLine("Please enter a directory to explore.");
                }
                else if (action.IndexOf(ACTION.copy) >= 0 && action.IndexOf(ACTION.move) >= 0)
                {
                    Console.WriteLine("Please chose copy OR move operation.");
                }
                else if((action.IndexOf(ACTION.copy) >= 0 || action.IndexOf(ACTION.move) >= 0) && destinationDirectory == String.Empty){
                    Console.WriteLine("Please enter a directory to filter AND a destination directory.");
                }
                else
                {
                    var myMusics = new MusicsLib();
                    //musicList.Extensions = myNewExtensions;
                    try
                    {
                        Console.WriteLine("Loading list of files");


                        myMusics.LoadFromFolder(directory);
                        if (action.IndexOf(ACTION.recognize) >= 0)
                        {
                            Console.WriteLine("Recognizing tags");
                            myMusics.ReadTags();
                            Console.WriteLine("Writing tags");
                            myMusics.WriteTags();
                        }

                        if (action.IndexOf(ACTION.copy) >= 0)
                        {
                            Console.WriteLine("Copying files to new path");
                            myMusics.Reorganize(destinationDirectory, true);
                        }

                        if (action.IndexOf(ACTION.move) >= 0)
                        {
                            Console.WriteLine("Moving files to new path");
                            myMusics.Reorganize(destinationDirectory, false);
                        }
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("Please enter a valid input directory name");
                    }
                    catch (System.Net.WebException e)
                    {
                        Console.WriteLine("Net Exption déso");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        Console.WriteLine("Finish !");
                    }
                }
            }
        }
    }
}

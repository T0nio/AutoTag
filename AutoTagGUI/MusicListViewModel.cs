﻿using AutoTagGUI.Utils;
using AutoTagLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace AutoTagGUI
{
    public class MusicListViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string str="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

        #endregion

        #region Constructor

        public MusicListViewModel()
        {
            MusicLibrary = new MusicLists();
            ReorganizeFormat = $"%Artist%{Path.DirectorySeparatorChar}%Album%{Path.DirectorySeparatorChar}%Track% - %Title%.mp3";
            MusicLibraryFolder = @"D:\Music\The Who";
            MusicLibrary.FillDict(MusicLibraryFolder);
        }

        #endregion

        #region Properties

        public static readonly string NoFolderSelected = "No target folder is selected";

        public List<BoolStringClass> AllowedExtensions {
            get
            {
                return new List<BoolStringClass>
                {
                    new BoolStringClass {IsSelected = true, Content = ".mp3" },
                    new BoolStringClass {IsSelected = false, Content = ".wav" },
                    new BoolStringClass {IsSelected = false, Content = ".flac" },
                    new BoolStringClass {IsSelected = false, Content = ".aac"},
                    new BoolStringClass {IsSelected = false, Content = ".wma"},
                    new BoolStringClass {IsSelected = false, Content = ".ogg"},
                };
            }
        }

        public bool MusicLibraryLoaded
        {
            get
            {
                return MusicLibraryFolder != null && MusicLibraryFolder != String.Empty;
            }
            set
            {
                if (value != MusicLibraryLoaded)
                {
                    MusicLibraryLoaded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private MusicLists _musicLibrary;
        public MusicLists MusicLibrary
        {
            get
            {
                return _musicLibrary;
            }

            set
            {
                if (value != _musicLibrary)
                {
                    _musicLibrary = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _musicLibraryFolder;
        public string MusicLibraryFolder {
            get
            {
                return _musicLibraryFolder;
            }

            set
            {
                if (value != _musicLibraryFolder)
                {
                    _musicLibraryFolder = value;
                    NotifyPropertyChanged();
                    MusicLibrary.FillDict(MusicLibraryFolder);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibraryLoaded"));
                }
            }
        }

        private string _musicLibraryTargetFolder;
        public string MusicLibraryTargetFolder {
            get
            {
                return _musicLibraryTargetFolder;
            }

            set
            {
                if (value != _musicLibraryTargetFolder)
                {
                    _musicLibraryTargetFolder = value;
                    NotifyPropertyChanged();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReorganizeIsEnabled"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompleteReorganizeFormat"));
                }
            }
        }

        public bool ReorganizeIsEnabled {
            get
            {
                return MusicLibraryLoaded && (MusicLibraryTargetFolder != null);
            }
        }

        private string _reorganizeFormat;
        public string ReorganizeFormat {
            get
            {
                return _reorganizeFormat;
            }
            set
            {
                if (value != _reorganizeFormat)
                {
                    _reorganizeFormat = value;
                    NotifyPropertyChanged();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompleteReorganizeFormat"));
                }
            }
        }

        public string CompleteReorganizeFormat {
            get
            {
                if (MusicLibraryTargetFolder != null)
                {
                    return $"{MusicLibraryTargetFolder}{Path.DirectorySeparatorChar}{ReorganizeFormat}";
                }
                return "No target folder is selected";
            }
        }

        #endregion

        #region Commands

        public ICommand LoadTargetFolder
        {
            get
            {
                return (ICommand) new RelayCommand<MusicLists>((library) =>
                {
                    FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                    openDirDialog.ShowDialog();
                    if (openDirDialog.SelectedPath != String.Empty)
                    {
                        MusicLibraryTargetFolder = openDirDialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand LoadSourceFolder
        {
            get
            {
                return new RelayCommand<MusicLists>((library) =>
                {
                    FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                    openDirDialog.ShowDialog();
                    if (openDirDialog.SelectedPath != String.Empty)
                    {
                        MusicLibraryFolder = openDirDialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand WriteTags
        {
            get
            {
                return new RelayCommand<MusicLists>((library) =>
                {
                    MusicLibrary.WriteTags();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                });
            }
        }

        public ICommand Reorganize
        {
            get
            {
                return new RelayCommand<MusicLists>((library) =>
                {
                    MusicLibrary.Reorganization();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                });
            }
        }

        #endregion
    }
}
using AutoTagGUI.Utils;
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
            this.MusicLibrary = new MusicsLib();
            this.ReorganizeFormat = $"%Artist%{Path.DirectorySeparatorChar}%Album%{Path.DirectorySeparatorChar}%Track% - %Title%.mp3";
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

        public List<string> SelectedExtensions
        {
            get
            {
                List<string> toReturn = new List<string>();
                foreach (BoolStringClass extension in this.AllowedExtensions)
                {
                    if (extension.IsSelected)
                    {
                        toReturn.Add(extension.Content);
                    }
                }
                return toReturn;
            }
        }

        public bool MusicLibraryLoaded
        {
            get { return this.MusicLibraryFolder != null && this.MusicLibraryFolder != String.Empty; }
            set
            {
                if (value != MusicLibraryLoaded)
                {
                    this.MusicLibraryLoaded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private MusicsLib _musicLibrary;
        public MusicsLib MusicLibrary
        {
            get { return _musicLibrary; }
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
            get {  return _musicLibraryFolder; }
            set
            {
                if (value != _musicLibraryFolder)
                {
                    _musicLibraryFolder = value;
                    NotifyPropertyChanged();

                    this.MusicLibrary.Extensions = this.SelectedExtensions;
                    this.MusicLibrary.LoadFromFolder(MusicLibraryFolder);
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MusicLibrary"));
                        PropertyChanged(this, new PropertyChangedEventArgs("MusicLibraryLoaded"));
                    }

                }
            }
        }

        private string _musicLibraryTargetFolder;
        public string MusicLibraryTargetFolder {
            get { return _musicLibraryTargetFolder; }
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

        public bool ReorganizeIsEnabled { get { return MusicLibraryLoaded && (MusicLibraryTargetFolder != null); } }

        private bool _copyFiles;
        public bool CopyFiles {
            get { return _copyFiles; }
            set
            {
                if (value != _copyFiles)
                {
                    _copyFiles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _reorganizeFormat;
        public string ReorganizeFormat {
            get { return _reorganizeFormat; }
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
                if (this.MusicLibraryTargetFolder != null)
                {
                    return $"{this.MusicLibraryTargetFolder}{Path.DirectorySeparatorChar}{this.ReorganizeFormat}";
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
                return (ICommand) new RelayCommand<MusicsLib>((library) =>
                {
                    FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                    openDirDialog.ShowDialog();
                    if (openDirDialog.SelectedPath != String.Empty)
                    {
                        this.MusicLibraryTargetFolder = openDirDialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand LoadSourceFolder
        {
            get
            {
                return new RelayCommand<MusicsLib>((library) =>
                {
                    FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                    openDirDialog.ShowDialog();
                    if (openDirDialog.SelectedPath != String.Empty)
                    {
                        this.MusicLibraryFolder = openDirDialog.SelectedPath;
                    }
                });
            }
        }

        public ICommand WriteTags
        {
            get
            {
                return new RelayCommand<MusicsLib>((library) =>
                {
                    this.MusicLibrary.ReadTags();
                    this.MusicLibrary.WriteTags();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                });
            }
        }

        public ICommand Reorganize
        {
            get
            {
                return new RelayCommand<MusicsLib>((library) =>
                {
                    MusicLibrary.Reorganize(this.CompleteReorganizeFormat, this.CopyFiles);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                });
            }
        }

        #endregion
    }
}
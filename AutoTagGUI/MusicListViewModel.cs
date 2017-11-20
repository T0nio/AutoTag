using AutoTagGUI.Utils;
using AutoTagLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
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
            this.ProgressBarVisibility = Visibility.Hidden;
            this.CurrentOperation = "";
        }

        #endregion

        #region Properties

        private static Thread _backgroundThread = new Thread(() => { });
        private readonly static GUIErrorManager _guiErrorManager = (GUIErrorManager)GUIErrorManager.GetInstance();

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

        private Visibility _progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                if(value != _progressBarVisibility)
                {
                    this._progressBarVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _currentOperation;
        public string CurrentOperation {
            get { return _currentOperation; }
            set
            {
                if (value != _currentOperation)
                {
                    _currentOperation = value;
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

        #region Methods

        private void ShowProgressBar(string msg)
        {
            this.ProgressBarVisibility = Visibility.Visible;
            this.CurrentOperation = msg;
        }

        private void HideProgressBar()
        {
            this.ProgressBarVisibility = Visibility.Hidden;
            this.CurrentOperation = "";
        }

        #endregion

        #region Commands

        public ICommand LoadTargetFolder
        {
            get
            {
                return (ICommand) new RelayCommand<MusicsLib>((library) =>
                {
                    if (_backgroundThread.ThreadState != ThreadState.Running)
                    {
                        FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                        openDirDialog.ShowDialog();
                        if (openDirDialog.SelectedPath != String.Empty)
                        {
                            this.ShowProgressBar("Loading Target Folder");
                            _backgroundThread = new Thread(() =>
                            {
                                this.MusicLibraryTargetFolder = openDirDialog.SelectedPath;
                                this.HideProgressBar();
                                _guiErrorManager.WaitErrorManager();
                                _guiErrorManager.ClearErrorManager();
                            });
                            _backgroundThread.Start();
                        }
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
                    if (_backgroundThread.ThreadState != ThreadState.Running)
                    {
                        FolderBrowserDialog openDirDialog = new FolderBrowserDialog();
                        openDirDialog.ShowDialog();
                        if (openDirDialog.SelectedPath != String.Empty)
                        {
                            this.ShowProgressBar("Loading Source Folder");
                            _backgroundThread = new Thread(() =>
                            {
                                this.MusicLibraryFolder = openDirDialog.SelectedPath;
                                this.HideProgressBar();
                                _guiErrorManager.WaitErrorManager();
                                _guiErrorManager.ClearErrorManager();
                                System.Windows.Forms.MessageBox.Show("Your music library has been loaded !", "Library loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            });
                            _backgroundThread.Start();
                        }
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
                    if (_backgroundThread.ThreadState != ThreadState.Running)
                    {
                        this.ShowProgressBar("Writing tags in loaded library");
                        _backgroundThread = new Thread(() =>
                        {
                            bool tagsRead = this.MusicLibrary.ReadTags();
                            this.MusicLibrary.WriteTags();
                            this.HideProgressBar();
                            _guiErrorManager.WaitErrorManager();
                            _guiErrorManager.ClearErrorManager();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                            if (tagsRead)
                            {
                                System.Windows.Forms.MessageBox.Show("All tags for your music library have been written !", "Tags written", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        });
                        _backgroundThread.Start();
                    }
                });
            }
        }

        public ICommand Reorganize
        {
            get
            {
                return new RelayCommand<MusicsLib>((library) =>
                {
                    if (_backgroundThread.ThreadState != ThreadState.Running)
                    {
                        this.ShowProgressBar("Reorganizing your library");
                        _backgroundThread = new Thread(() =>
                        {
                            MusicLibrary.Reorganize(this.CompleteReorganizeFormat, this.CopyFiles);
                            this.HideProgressBar();
                            _guiErrorManager.WaitErrorManager();
                            _guiErrorManager.ClearErrorManager();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MusicLibrary"));
                            System.Windows.Forms.MessageBox.Show("Your music library has been reorganized !", "Library reoarginzed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                        _backgroundThread.Start();
                    }
                });
            }
        }

        #endregion
    }
}
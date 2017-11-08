using AutoTagLib.ErrorManager;
using System.Windows;

namespace AutoTagGUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var errorManager = (IErrorManager)GUIErrorManager.GetInstance();
            Lookup.GetInstance().Register(typeof(IErrorManager), errorManager);
            InitializeComponent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Memory.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Memory.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Highscore : Page
    {
        private static SQLiteDb db = new SQLiteDb();

        public Highscore()
        {
            db.Create();
            this.InitializeComponent();
            this.CurrentCardList = db.GetTopHighscores();
            this.DataContext = this;
        }

        public static void AddHighscore(String name, int time, int incorrectPairs, int points)
        {
            db.AddHighscore(name, time, incorrectPairs, points);
            ToastHelper.PopToast("Memory", name + " has a new highscore in memory!");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string parameter = e.Parameter as string;

        }

        public List<Score> CurrentCardList { get; set; }
    }
}

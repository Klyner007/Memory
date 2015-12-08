using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Memory.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Memory.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private Dictionary<int, string> memoryDictionary;

        private String _scorePlayerOne = "0";
        private String _scorePlayerTwo = "0";

        public string ScorePlayerTwo
        {
            get { return _scorePlayerTwo; }
            set
            {
                if (value != _scorePlayerTwo)
                {
                    _scorePlayerTwo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private static BitmapImage backgroundImage;
        private Stopwatch stopwatch;
        private DispatcherTimer timer;
        private List<Button> buttonCollection;

        public Game()
        {
            InitializeComponent();
            InitializeCards();
            InitializeTimers();

            this.DataContext = new Score()
            {
                playerOneScore = 0,
                playerTwoScore = 0
            };

            _scorePlayerOne = "0";
            _scorePlayerTwo = "1";

            ScorePlayerTwo = "2";

            buttonCollection = gameGrid.Children.OfType<Button>().ToList();

            foreach (Button button in buttonCollection)
            {
                button.Content = new BitmapImage(new Uri(memoryDictionary[0]));
            }
        }

        private void InitializeTimers()
        {
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Tick += TimerOnTick;
            stopwatch.Start();
            timer.Start();
        }

        private void TimerOnTick(object sender, object o)
        {
            timerTextBlock.Text = stopwatch.Elapsed.Minutes.ToString("00") + ":" + stopwatch.Elapsed.Seconds.ToString("00");
        }
        
        private int[,] cardArray = new int[6, 2];

        private void InitializeCards()
        {
            string imagesSubFolder = @"Assets\Cards\";
            string installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            memoryDictionary = new Dictionary<int, string>();
            string[] imagesFullPaths = Directory.GetFiles(Path.Combine(installationFolder, imagesSubFolder));

            int id = 0;

            foreach (string imagePath in imagesFullPaths)
            {
                memoryDictionary.Add(id, imagePath);
                id++;
            }

            //radom card generator
            int[] aantalkaarten = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var naam = aantalkaarten.ToList();
            var random = new Random();
            var randomvalue = 0;
            for (var i = 0; i < 6; i++)
            {
                for (var a = 0; a < 2; a++)
                {
                    if (naam.Count == 1)
                    {
                        cardArray[i, a] = naam.First();
                        break;
                    }
                    randomvalue = random.Next(1, naam.Count);
                    var uniekvalue = naam.ToArray()[randomvalue];
                    naam.Remove(uniekvalue);
                    cardArray[i, a] = uniekvalue;
                }
            }
            bool vs = true;
            vs = false;
        }
        
        private int Matchcard(int cardnumber)
        {
            //todo dynamic array size
            for (var i = 0; i < 6; i++)
                for (var a = 0; a < 2; a++)
                    if (cardArray[i, a] == cardnumber)
                        return i;
            return -1;
        }

        private bool _firstOpenCard = true;
        private Button _cardOneButton;
        private bool playerOneTurn = true;
        private int[] playersGameScore = new int[2];
        private int pairsFound = 0;

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            var cardButton = (Button)sender;

            if (cardButton.Content == null)
            {
                if (_firstOpenCard)
                {

                    _firstOpenCard = false;
                    _cardOneButton = cardButton;
                    cardButton.Content = new Image
                    {
                        Source =
                            new BitmapImage(
                                new Uri(memoryDictionary[Matchcard(Convert.ToInt32(cardButton.Name.Substring(4)))]))
                    };

                }
                else
                {
                    if (_cardOneButton.Name != cardButton.Name)
                    {
                        cardButton.Content = new Image
                        {
                            Source =
                                new BitmapImage(
                                    new Uri(memoryDictionary[Matchcard(Convert.ToInt32(cardButton.Name.Substring(4)))]))
                        };
                        _firstOpenCard = true;
                        //this.IsEnabled = false;
                        //2dezelfde
                        if (Matchcard(Convert.ToInt32(cardButton.Name.Substring(4))) ==
                            Matchcard(Convert.ToInt32(_cardOneButton.Name.Substring(4))))
                        {
                            pairsFound += 1;

                            if (pairsFound == (cardArray.Length/2))
                            {
                                // todo score toevoegen? kaarten laten verdwijnen
                                //_cardOneButton.Content =;
                                //cardButton.Content =;
                                if (playerOneTurn)
                                {
                                    _scorePlayerOne = (Convert.ToInt32(_scorePlayerOne) + 1).ToString();
                                }
                                else
                                {
                                    _scorePlayerTwo = (Convert.ToInt32(_scorePlayerTwo) + 1).ToString();
                                }
                            }

                        }
                        else
                        {
                            await Test(cardButton, _cardOneButton);
                        }
                    }
                    playerOneTurn = !playerOneTurn;
                }
            }
        }

        internal static async Task Test(Button cardOneButton, Button cardButton)
        {
            await Task.Delay(550);
            cardOneButton.Content = null;
            cardButton.Content = null;
        }

        private void timerTextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        //public class MediaPlayer
        //{
        //    private void addSound()
        //    {
        //        BackgroundMediaPlayer.Current.Play();
        //    }

        //    public void ToggleSound()
        //    {
        //        //BackgroundMediaPlayer player 

        //        if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
        //        {
        //            BackgroundMediaPlayer.Current.Pause();
        //        }
        //        else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
        //        {
        //            BackgroundMediaPlayer.Current.Play();
        //        }
        //    }
        //}
    }
}

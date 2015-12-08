using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
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
        private static BitmapImage backgroundImage;
        private Stopwatch stopwatch;
        private Stopwatch stopwatch_2;
        private DispatcherTimer timer;
        private List<Button> buttonCollection;
        private PlayerScore s = new PlayerScore();

        public Game()
        {
            InitializeTimers();
            
            this.DataContext = s;
            
            s.GameScorePlayerOne = 0;
            s.TimePlayerOne = 0;
            s.TotalScorePlayerOne = 0;
            s.WrongChoicesPlayerOne = 0;

            s.GameScorePlayerTwo = 0;
            s.TimePlayerTwo = 0;
            s.TotalScorePlayerTwo = 0;
            s.WrongChoicesPlayerTwo = 0;

            InitializeNewGame();
        }

        private void InitializeNewGame()
        {
            InitializeComponent();
            InitializeCards();
            UpdateUi();

            pairsFound = 0;

            buttonCollection = buttonPanel1.Children.OfType<Button>().ToList();
            buttonCollection.AddRange(buttonPanel2.Children.OfType<Button>().ToList());
            buttonCollection.AddRange(buttonPanel3.Children.OfType<Button>().ToList());
            buttonCollection.AddRange(buttonPanel4.Children.OfType<Button>().ToList());

            foreach (Button button in buttonCollection)
            {
                button.Content = null;//new BitmapImage(new Uri(memoryDictionary[0]));
            }

            gameFinished = false;
        }
        
        private void InitializeTimers()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch_2 = new Stopwatch();
            stopwatch_2.Start();
            timer = new DispatcherTimer();
            timer.Tick += TimerOnTick;
            timer.Start();
        }

        private void TimerOnTick(object sender, object o)
        {
            if (stopwatch_2.Elapsed.Seconds == 1)
            {
                s.TimePlayerOne = playerOneTurn ? s.TimePlayerOne + 1 : s.TimePlayerOne;
                s.TimePlayerTwo = !playerOneTurn ? s.TimePlayerTwo + 1 : s.TimePlayerTwo;
                stopwatch_2.Reset();
                stopwatch_2.Start();
            }
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
            for (var i = 0; i < 6; i++)
            {
                for (var a = 0; a < 2; a++)
                {
                    if (naam.Count == 1)
                    {
                        cardArray[i, a] = naam.First();
                        break;
                    }
                    var randomvalue = random.Next(1, naam.Count);
                    var uniekvalue = naam.ToArray()[randomvalue];
                    naam.Remove(uniekvalue);
                    cardArray[i, a] = uniekvalue;
                }
            }
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
        private int pairsFound = 0;
        private bool gameFinished;

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            s.GameScorePlayerOne = s.GameScorePlayerOne + 1;
            var cardButton = (Button)sender;
            if (!gameFinished)
            {
                handleClick(cardButton);
            }
            else
            {
                s.TotalScorePlayerOne = s.GameScorePlayerOne > 2 ? s.TotalScorePlayerOne + 1 : s.TotalScorePlayerOne;
                s.TotalScorePlayerTwo = s.TotalScorePlayerTwo > 2 ? s.TotalScorePlayerTwo + 1 : s.TotalScorePlayerTwo;
                
                if (s.TotalScorePlayerOne > 2)
                {
                    Memory.Views.Highscore.AddHighscore(textBlock.Text, s.TimePlayerTwo, s.WrongChoicesPlayerTwo, s.TotalScorePlayerOne);
                    Frame.Navigate(typeof(Highscore));
                }
                else if (s.TotalScorePlayerTwo > 2)
                {
                    Memory.Views.Highscore.AddHighscore(textBlock1.Text, s.TimePlayerTwo, s.WrongChoicesPlayerTwo, s.TotalScorePlayerTwo);
                    Frame.Navigate(typeof(Highscore));
                }

                playerOneTurn = !playerOneTurn;
                InitializeNewGame();
            }
        }

        private async void handleClick(Button cardButton)
        {
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
                            if (playerOneTurn)
                            {
                                s.GameScorePlayerOne = s.GameScorePlayerOne + 1;
                            }
                            else
                            {
                                s.GameScorePlayerTwo = s.GameScorePlayerTwo + 1;
                            }

                            if (pairsFound == (cardArray.Length / 2))
                            {
                                gameFinished = true;
                            }
                        }
                        else
                        {
                            if (playerOneTurn)
                            {
                                s.WrongChoicesPlayerOne = s.WrongChoicesPlayerOne + 1;
                            }
                            else
                            {
                                s.WrongChoicesPlayerTwo = s.WrongChoicesPlayerTwo + 1;
                            }
                            await Test(cardButton, _cardOneButton);
                            playerOneTurn = !playerOneTurn;
                        }
                    }
                    UpdateUi();
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

        private void UpdateUi()
        {
            arrowPlayerOneImage.Visibility = playerOneTurn ? Visibility.Visible : Visibility.Collapsed;
            arrowPlayerTwoImage.Visibility = !playerOneTurn ? Visibility.Visible : Visibility.Collapsed;
            arrowPlayerOneImage_2.Visibility = playerOneTurn ? Visibility.Visible : Visibility.Collapsed;
            arrowPlayerTwoImage_2.Visibility = !playerOneTurn ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] parameter = e.Parameter as string[];
            textBlock.Text = parameter[0];
            textBlock1.Text = parameter[1];
        }

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

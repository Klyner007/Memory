using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;
using Memory.Helpers;
using Memory.Views;
using Memory.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Memory.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MessageWebSocket messageWebSocket;
        private DataWriter messageWriter;

        public MainPage()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isConnected = NetworkController.GetInstance().IsConnected;
            if (tbPlayerOne.Text.Length > 0 && isConnected)
            {
                Frame.Navigate(typeof(Game), new string[]
                {
                    tbPlayerOne.Text
                }
                );
            }
            else if(isConnected)
            {
                tbPlayerOne.Text = "Player 1";
            }
            else if(tbIpAddress.Text.Length > 0)
            {
                NetworkController.GetInstance().GetConnection(tbIpAddress.Text);
            }
        } 

        private void exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void about_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (About));
        }

        private void highscore_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Highscore));
        }
    }
}

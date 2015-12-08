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
            GetConnection();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbPlayerOne.Text.Length > 0 && tbPlayerTwo.Text.Length > 0)
            {
                Frame.Navigate(typeof(Game), new string[]
                {
                    tbPlayerOne.Text,
                    tbPlayerTwo.Text
                }
                );
            }
            else
            {
                tbPlayerOne.Text = "Player 1";
                tbPlayerTwo.Text = "Player 2";
            }
        }

        private bool TryGetUri(string uriString, out Uri uri)
        {
            uri = null;

            Uri webSocketUri;
            if (!Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out webSocketUri))
            {
                System.Diagnostics.Debug.WriteLine("Error: Invalid URI");
                return false;
            }

            // Fragments are not allowed in WebSocket URIs.
            if (!String.IsNullOrEmpty(webSocketUri.Fragment))
            {
                System.Diagnostics.Debug.WriteLine("Error: URI fragments not supported in WebSocket URIs.");
                return false;
            }

            // Uri.SchemeName returns the canonicalized scheme name so we can use case-sensitive, ordinal string
            // comparison.
            if ((webSocketUri.Scheme != "ws") && (webSocketUri.Scheme != "wss"))
            {
                System.Diagnostics.Debug.WriteLine("Error: WebSockets only support ws:// and wss:// schemes.");
                return false;
            }

            uri = webSocketUri;

            return true;
        }

        private void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Closed; Code: " + args.Code + ", Reason: " + args.Reason);

            if (messageWebSocket != null)
            {
                messageWebSocket.Dispose();
                messageWebSocket = null;
            }
        }

        private async void GetConnection()
        {   
            System.Diagnostics.Debug.WriteLine("Starting... ");
            try
            {
                if (messageWebSocket == null)
                {
                    Uri server;
                    if (!TryGetUri("ws://127.0.0.1:1337", out server))
                    {
                        return;
                    }

                    //Server is now build..
                    messageWebSocket = new MessageWebSocket();
                    messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
                    messageWebSocket.MessageReceived += MessageReceived;

                    // Dispatch close event on UI thread. This allows us to avoid synchronizing access to messageWebSocket.
                    messageWebSocket.Closed += async (senderSocket, args) =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Closed(senderSocket, args));
                    };

                    await messageWebSocket.ConnectAsync(server);
                    messageWriter = new DataWriter(messageWebSocket.OutputStream);
                    SendMessage(messageWriter, "Hallo Verbinding");

                    await messageWriter.StoreAsync();
                }
                else if (messageWriter != null)
                {
                    SendMessage(messageWriter, "no msg");
                    await messageWriter.StoreAsync();
                }
            }
            catch (Exception er)
            {
                if (messageWebSocket != null)
                {
                    messageWebSocket.Dispose();
                    messageWebSocket = null;
                }
                WebErrorStatus status = WebSocketError.GetStatus(er.GetBaseException().HResult);
            }
        }

        private void SendMessage(DataWriter writer, String message)
        {
            writer.WriteString(message);
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Message Received; Type: " + args.MessageType);
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    string read = reader.ReadString(reader.UnconsumedBufferLength);

                    //Convert to JSON
                    if (read != "")
                    {

                    }
                    System.Diagnostics.Debug.WriteLine(read);
                }
            }
            catch (Exception ex) // For debugging
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);

                if (status == WebErrorStatus.Unknown)
                {
                    throw;
                }

                // Normally we'd use the status to test for specific conditions we want to handle specially,
                // and only use ex.Message for display purposes.  In this sample, we'll just output the
                // status for debugging here, but still use ex.Message below.
                System.Diagnostics.Debug.WriteLine("Error: " + status);

                System.Diagnostics.Debug.WriteLine(ex.Message);
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

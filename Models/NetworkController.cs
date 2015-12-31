using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.Web;

namespace Memory.Models
{
    class NetworkController
    {
        private static NetworkController instance;

        public static NetworkController GetInstance()
        {
            if (instance == null)
            {
                instance = new NetworkController();
            }
            return instance;
        }

        private MessageWebSocket messageWebSocket;

        public MessageWebSocket MessageWebSocket
        {
            get
            {
                return messageWebSocket;
            }
            set
            {
                messageWebSocket = value;
            }
        }

        private DataWriter messageWriter;

        public DataWriter MessageWriter
        {
            get
            {
                return messageWriter;
            }
            set
            {
                messageWriter = value;
            }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        private NetworkController()
        {
            messageWebSocket = null;
            isConnected = false;
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
            isConnected = false;

            if (messageWebSocket != null)
            {
                messageWebSocket.Dispose();
                messageWebSocket = null;
            }
        }

        public async void GetConnection(String host)
        {
            System.Diagnostics.Debug.WriteLine("Starting... ");
            try
            {
                if (messageWebSocket == null)
                {
                    Uri server;
                    if (!TryGetUri("ws://" + host, out server))
                    {
                        return;
                    }

                    //Server is now build..
                    messageWebSocket = new MessageWebSocket();
                    messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
                    messageWebSocket.MessageReceived += MessageReceived;

                    // Dispatch close event on UI thread. This allows us to avoid synchronizing access to messageWebSocket.
                    messageWebSocket.Closed += (senderSocket, args) =>
                    {
                        //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Closed(senderSocket, args));
                        Closed(senderSocket, args);
                    };

                    await messageWebSocket.ConnectAsync(server);
                    isConnected = true;
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
    }
}

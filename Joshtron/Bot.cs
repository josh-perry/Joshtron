using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Discord;
using Discord.Commands;

namespace Joshtron
{
    public class Bot
    {
        /// <summary>
        /// Discord.Net DiscordClient object. Primary means of interacting with discord.
        /// </summary>
        private readonly DiscordClient _client;

        /// <summary>
        /// Secret bot key needed to use the Discord API.
        /// </summary>
        private string _token;

        /// <summary>
        /// LogMessageReceived handler for logging to the UI etc.
        /// </summary>
        /// <param name="datetime">Date/time of message</param>
        /// <param name="message">Message</param>
        public delegate void LogMessageReceivedHandler(DateTime datetime, string message);

        /// <summary>
        /// Event for logging to the UI etc.
        /// </summary>
        public event LogMessageReceivedHandler LogMessageReceived;

        /// <summary>
        /// Default constructor. Sets up events and connects to discord.
        /// </summary>
        public Bot()
        {
            try
            {
                LoadToken();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bot token from file!\n\n{ex.Message}");
                Environment.Exit(1);
            }

            _client = new DiscordClient();
            _client.MessageReceived += ClientOnMessageReceived;

            _client.UsingCommands(x =>
            {
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            _client.GetService<CommandService>().CreateCommand("hug")
                .Description("Hug a person.")
                .Parameter("HuggedPerson")
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"/me hugs {e.GetArg("HuggedPerson")}. :hearts:");
                });

            new Task(async () => await _client.Connect(_token)).Start();
        }

        /// <summary>
        /// Load bot token from a file.
        /// </summary>
        private void LoadToken()
        {
            _token = File.ReadAllText("secrets.txt");

            if (String.IsNullOrWhiteSpace(_token))
            {
                throw new Exception("Token is null or empty!");
            }
        }

        /// <summary>
        /// Event handler for message received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClientOnMessageReceived(object sender, MessageEventArgs e)
        {
            // Log message.
            LogMessageReceived?.Invoke(e.Message.Timestamp, e.Message.ToString());

            // Ignore self.
            if (e.Message.IsAuthor)
                return;

            // Lee is a special case.
            if (LeeDetector.IsLee(e.Message.User))
            {
                var msg = e.Message.ToString().ToLower();
                if (msg.Contains("meme") || msg.Contains("meming"))
                {
                    await e.Channel.SendTTSMessage("What a memelord.");
                }
            }

            // Only respond if user is talking to me.
            if (!e.Channel.IsPrivate && !e.Message.IsMentioningMe())
                return;
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="message"></param>
        protected virtual void OnMessageReceivedLogHandler(DateTime datetime, string message)
        {
            Console.WriteLine($"[{datetime}] {message}");
            LogMessageReceived?.Invoke(datetime, message);
        }
    }
}

using System;

namespace Joshtron
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Default constructor. Sets up a new bot and it's event handlers.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            LogTextBox.TextChanged += delegate { LogTextBox.ScrollToEnd(); };

            var bot = new Bot();
            bot.LogMessageReceived += BotOnLogMessageReceived;
        }

        /// <summary>
        /// Message received event handler. Updates the UI.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="message"></param>
        private void BotOnLogMessageReceived(DateTime datetime, string message)
        {
            Dispatcher.InvokeAsync(() => LogTextBox.AppendText($"[{datetime}] {message}{Environment.NewLine}"));
        }
    }
}

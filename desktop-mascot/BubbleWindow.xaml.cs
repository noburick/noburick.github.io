using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopMascot.Models;
using DesktopMascot.Services;

namespace DesktopMascot
{
    /// <summary>
    /// 吹き出しチャットウィンドウ
    /// Step 3: 吹き出しUI
    /// Step 4: ChatGPT API連携
    /// Step 5: emotionによる表情切り替え
    /// Step 6: 会話履歴の保持
    /// </summary>
    public partial class BubbleWindow : Window
    {
        private readonly MainWindow _mascot;
        private readonly ChatService _chatService;

        // UI へバインドする会話履歴コレクション (Step 6)
        public ObservableCollection<ChatBubbleItem> ChatItems { get; } = new();

        public BubbleWindow(MainWindow mascot)
        {
            _mascot = mascot;
            InitializeComponent();

            // コンバーターをリソースに登録
            Resources["BoolToVisibilityConverter"]         = new BoolToVisibilityConverter();
            Resources["BoolToInversedVisibilityConverter"] = new BoolToInversedVisibilityConverter();

            // チャット履歴をバインド
            ChatHistory.ItemsSource = ChatItems;

            // Step 4: ChatService を初期化（API キー読み込み）
            var apiKey = ConfigService.LoadApiKey();
            var model  = ConfigService.LoadModel();
            _chatService = new ChatService(apiKey, model);
        }

        // ──────────────────────────────────────────────
        // メッセージ送信処理 (Step 4, 5, 6)
        // ──────────────────────────────────────────────

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync();
        }

        private async void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(InputBox.Text))
                await SendMessageAsync();
        }

        private async Task SendMessageAsync()
        {
            var userText = InputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userText)) return;

            // 入力欄をクリアし、送信ボタンを無効化
            InputBox.Text    = string.Empty;
            SendButton.IsEnabled = false;

            // ユーザーメッセージを履歴に追加 (Step 6)
            AppendMessage(userText, isUser: true);

            try
            {
                // Step 4: ChatGPT API を呼び出す
                var response = await _chatService.SendMessageAsync(userText);

                // Step 5: emotion に応じてキャラクター表情を変更
                _mascot.SetEmotion(response.Emotion);

                // マスコットの返答を履歴に追加 (Step 6)
                AppendMessage(response.Message, isUser: false);
            }
            catch (Exception ex)
            {
                AppendMessage($"[エラー] {ex.Message}", isUser: false);
                _mascot.SetEmotion("troubled");
            }
            finally
            {
                SendButton.IsEnabled = true;
                InputBox.Focus();
            }
        }

        private void AppendMessage(string text, bool isUser)
        {
            ChatItems.Add(new ChatBubbleItem { Text = text, IsUser = isUser });

            // 最新メッセージまでスクロール
            ChatScrollViewer.ScrollToBottom();
        }

        // ──────────────────────────────────────────────
        // ウィンドウ操作
        // ──────────────────────────────────────────────

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }

    // ──────────────────────────────────────────────
    // 値コンバーター（インラインで定義）
    // ──────────────────────────────────────────────

    public class BoolToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToInversedVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}

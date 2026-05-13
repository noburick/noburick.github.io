using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DesktopMascot
{
    /// <summary>
    /// メインウィンドウ：透明背景のキャラクター表示ウィンドウ
    /// Step 1: 透明背景 + キャラクター画像表示
    /// Step 2: 常に最前面 + ドラッグ移動
    /// </summary>
    public partial class MainWindow : Window
    {
        private BubbleWindow? _bubbleWindow;

        // emotion名 -> 画像相対パスのマッピング
        private readonly Dictionary<string, string> _emotionImages = new()
        {
            { "normal",    "assets/character/normal.png"    },
            { "smile",     "assets/character/smile.png"     },
            { "troubled",  "assets/character/troubled.png"  },
            { "angry",     "assets/character/angry.png"     },
            { "surprised", "assets/character/surprised.png" },
        };

        public MainWindow()
        {
            InitializeComponent();
            SetEmotion("normal");
        }

        // ──────────────────────────────────────────────
        // Step 1: キャラクター画像の表示・切り替え
        // ──────────────────────────────────────────────

        /// <summary>
        /// emotion に対応するキャラクター画像を切り替える。
        /// 画像ファイルが存在しない場合は何もしない。
        /// </summary>
        public void SetEmotion(string emotion)
        {
            if (!_emotionImages.TryGetValue(emotion, out var relativePath))
                relativePath = _emotionImages["normal"];

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (!File.Exists(fullPath))
            {
                // 画像未配置の場合はフォールバック（何も表示しない）
                CharacterImage.Source = null;
                return;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource     = new Uri(fullPath);
            bitmap.CacheOption   = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            CharacterImage.Source = bitmap;
        }

        // ──────────────────────────────────────────────
        // Step 2: ドラッグ移動
        // ──────────────────────────────────────────────

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove(); // ウィンドウをドラッグ移動
        }

        // ──────────────────────────────────────────────
        // Step 3: 吹き出しウィンドウの位置連動
        // ──────────────────────────────────────────────

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SyncBubblePosition();
        }

        /// <summary>吹き出しをマスコットの右隣に配置する</summary>
        public void SyncBubblePosition()
        {
            if (_bubbleWindow is { IsVisible: true })
            {
                _bubbleWindow.Left = Left + Width + 8;
                _bubbleWindow.Top  = Top;
            }
        }

        // ──────────────────────────────────────────────
        // コンテキストメニューイベント
        // ──────────────────────────────────────────────

        private void OpenChat_Click(object sender, RoutedEventArgs e)
        {
            if (_bubbleWindow is null || !_bubbleWindow.IsVisible)
            {
                _bubbleWindow = new BubbleWindow(this);
                _bubbleWindow.Show();
                SyncBubblePosition();
            }
            else
            {
                _bubbleWindow.Activate();
            }
        }

        private void ToggleTopmost_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            TopmostMenuItem.IsChecked = Topmost;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

using System.IO;
using System.Text.Json;
using DesktopMascot.Models;

namespace DesktopMascot.Services
{
    /// <summary>
    /// APIキーとモデル名を環境変数または appsettings.json から読み込む
    /// Step 4: APIキーを安全に管理する
    /// </summary>
    public static class ConfigService
    {
        private const string EnvKeyName = "OPENAI_API_KEY";
        private const string ConfigFile = "appsettings.json";

        private static AppSettings? _cached;

        private static AppSettings LoadSettings()
        {
            if (_cached is not null) return _cached;

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile);
            if (!File.Exists(configPath)) return _cached = new AppSettings();

            try
            {
                var json = File.ReadAllText(configPath);
                _cached = JsonSerializer.Deserialize<AppSettings>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new AppSettings();
            }
            catch (Exception ex) when (ex is JsonException or IOException)
            {
                // 設定ファイルの読み込み・パース失敗時は空の設定を使用する
                _cached = new AppSettings();
            }

            return _cached;
        }

        /// <summary>
        /// APIキーを取得する。
        /// 優先順位: 環境変数 OPENAI_API_KEY > appsettings.json の OpenAI.ApiKey
        /// </summary>
        public static string LoadApiKey()
        {
            var envKey = Environment.GetEnvironmentVariable(EnvKeyName);
            if (!string.IsNullOrWhiteSpace(envKey)) return envKey;

            return LoadSettings().OpenAI?.ApiKey ?? string.Empty;
        }

        /// <summary>
        /// 使用するモデル名を取得する。デフォルトは gpt-4o-mini。
        /// </summary>
        public static string LoadModel()
        {
            return LoadSettings().OpenAI?.Model ?? "gpt-4o-mini";
        }
    }
}

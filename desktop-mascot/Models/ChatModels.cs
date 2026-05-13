namespace DesktopMascot.Models
{
    /// <summary>
    /// ChatGPT API への送受信メッセージ形式
    /// </summary>
    public class ChatMessage
    {
        public string Role    { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// ChatGPT API から受け取るチャット返答（JSON パース後）
    /// Step 5: emotion フィールドで表情を判定する
    /// </summary>
    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        /// <summary>normal / smile / troubled / angry / surprised</summary>
        public string Emotion { get; set; } = "normal";
    }

    /// <summary>
    /// UI の会話履歴バブルにバインドするデータモデル
    /// Step 6: 会話履歴
    /// </summary>
    public class ChatBubbleItem
    {
        public string Text   { get; set; } = string.Empty;
        /// <summary>true = ユーザー発言, false = マスコット発言</summary>
        public bool   IsUser { get; set; }
    }

    // ──────────────────────────────────────────────
    // OpenAI API レスポンス逆シリアライズ用クラス
    // ──────────────────────────────────────────────

    public class OpenAICompletionResponse
    {
        public List<OpenAIChoice>? Choices { get; set; }
    }

    public class OpenAIChoice
    {
        public OpenAIMessage? Message { get; set; }
    }

    public class OpenAIMessage
    {
        public string? Content { get; set; }
    }

    // ──────────────────────────────────────────────
    // appsettings.json 読み込み用クラス
    // ──────────────────────────────────────────────

    public class AppSettings
    {
        public OpenAISettings? OpenAI { get; set; }
    }

    public class OpenAISettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model  { get; set; } = "gpt-4o-mini";
    }
}

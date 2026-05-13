using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DesktopMascot.Models;

namespace DesktopMascot.Services
{
    /// <summary>
    /// ChatGPT API との通信を担当するサービスクラス
    /// Step 4: API 連携
    /// Step 5: emotion を含む JSON レスポンスを解析
    /// Step 6: 会話履歴を保持
    /// </summary>
    public class ChatService
    {
        private const string ApiEndpoint = "https://api.openai.com/v1/chat/completions";

        private static readonly HttpClient _httpClient = new();
        private readonly string     _apiKey;
        private readonly string     _model;

        // Step 6: 会話履歴（システムプロンプト含む）
        private readonly List<ChatMessage> _history = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };

        public ChatService(string apiKey, string model = "gpt-4o-mini")
        {
            _apiKey = apiKey;
            _model  = model;

            // システムプロンプトを会話履歴の先頭に追加 (Step 5)
            _history.Add(new ChatMessage
            {
                Role    = "system",
                Content = """
                    あなたは親切で明るいデスクトップマスコットアシスタントです。
                    ユーザーの質問や話しかけに日本語で短く返答してください。
                    返答は必ず次の JSON 形式のみで出力してください（それ以外のテキストは絶対に含めないこと）:
                    {"message": "返答テキスト", "emotion": "感情"}
                    emotion には以下の5種類のいずれかを選んでください:
                    - normal   : 普通の返答
                    - smile    : 楽しい・嬉しい・ポジティブな内容
                    - troubled : 困っている・わからない・難しい内容
                    - angry    : 怒り・批判・不快な内容
                    - surprised: 驚き・意外な内容
                    """,
            });
        }

        /// <summary>
        /// ユーザーメッセージを送信し、返答と emotion を受け取る。
        /// </summary>
        public async Task<ChatResponse> SendMessageAsync(string userMessage)
        {
            // ユーザーメッセージを履歴に追加 (Step 6)
            _history.Add(new ChatMessage { Role = "user", Content = userMessage });

            // リクエストボディを構築
            var requestBody = new
            {
                model           = _model,
                messages        = _history.Select(m => new { role = m.Role, content = m.Content }),
                response_format = new { type = "json_object" }, // JSON モード強制
                max_tokens      = 512,
                temperature     = 0.7,
            };

            var json    = JsonSerializer.Serialize(requestBody, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API 呼び出し（リクエスト毎に Authorization ヘッダーを付与）
            using var request = new HttpRequestMessage(HttpMethod.Post, ApiEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = content;

            var httpResponse = await _httpClient.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();

            var responseJson = await httpResponse.Content.ReadAsStringAsync();

            // OpenAI レスポンスを解析
            var completion = JsonSerializer.Deserialize<OpenAICompletionResponse>(responseJson, JsonOptions);
            var assistantContent = completion?.Choices?.FirstOrDefault()?.Message?.Content
                                   ?? "{\"message\": \"返答を取得できませんでした。\", \"emotion\": \"troubled\"}";

            // アシスタントの返答を履歴に追加 (Step 6)
            _history.Add(new ChatMessage { Role = "assistant", Content = assistantContent });

            // Step 5: JSON から message と emotion を抽出
            try
            {
                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(assistantContent, JsonOptions);
                if (chatResponse is not null && !string.IsNullOrWhiteSpace(chatResponse.Message))
                    return chatResponse;
            }
            catch (JsonException)
            {
                // JSON パース失敗時はそのまま返答テキストとして扱う
            }

            return new ChatResponse { Message = assistantContent, Emotion = "normal" };
        }

        /// <summary>
        /// 会話履歴をリセットする（システムプロンプトは維持）。
        /// </summary>
        public void ClearHistory()
        {
            // インデックス 0 はシステムプロンプトなので残す
            if (_history.Count > 1)
                _history.RemoveRange(1, _history.Count - 1);
        }
    }
}

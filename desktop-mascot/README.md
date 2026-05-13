# 🐱 DesktopMascot — Windows デスクトップマスコットアプリ

ChatGPT と会話できる Windows デスクトップマスコットアプリです。  
透明背景ウィンドウにキャラクター画像を表示し、吹き出しUI でチャットします。  
返答の感情（emotion）に合わせてキャラクターの表情が変わります。

---

## 🗂 プロジェクト構成

```
desktop-mascot/
├── DesktopMascot.csproj        # C# / WPF プロジェクトファイル
├── App.xaml / App.xaml.cs      # アプリケーション起動定義
├── MainWindow.xaml / .cs       # 透明ウィンドウ・キャラクター表示 (Step 1-2)
├── BubbleWindow.xaml / .cs     # 吹き出しチャットUI (Step 3, 6)
├── Models/
│   └── ChatModels.cs           # データモデル（ChatMessage, ChatResponse 等）
├── Services/
│   ├── ChatService.cs          # ChatGPT API 通信 (Step 4-5)
│   └── ConfigService.cs        # APIキー読み込み (Step 4)
├── appsettings.json            # 設定ファイル（APIキー等）
└── assets/
    └── character/
        ├── normal.png          # 通常表情
        ├── smile.png           # 笑顔
        ├── troubled.png        # 困り顔
        ├── angry.png           # 怒り
        └── surprised.png       # 驚き
```

---

## ⚙️ 技術構成

| 項目       | 内容                        |
|------------|----------------------------|
| 言語       | C# 12                      |
| フレームワーク | .NET 8 + WPF             |
| 外部ライブラリ | なし（標準ライブラリのみ） |
| AI         | OpenAI ChatGPT API          |

**WPF を選んだ理由**
- Windows デスクトップアプリとして最も自然
- `AllowsTransparency` + `WindowStyle="None"` で透明ウィンドウが簡単
- 追加ライブラリなしで完結する
- .exe として配布しやすい

---

## 🚀 セットアップ

### 前提

- Windows 10 / 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) のインストール

### 手順

1. **リポジトリのクローン or フォルダをダウンロード**

2. **キャラクター画像を配置**

   `assets/character/` に以下の5ファイルを置いてください（PNG 推奨、背景透過）。

   ```
   normal.png / smile.png / troubled.png / angry.png / surprised.png
   ```

   > 💡 [いらすとや](https://www.irasutoya.com/) などのフリー素材が利用できます。

3. **APIキーの設定**（どちらか一方でOK）

   **方法 A: 環境変数（推奨・安全）**
   ```powershell
   $env:OPENAI_API_KEY = "sk-..."
   dotnet run
   ```

   **方法 B: appsettings.json**
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-...",
       "Model": "gpt-4o-mini"
     }
   }
   ```
   > ⚠️ `appsettings.json` を Git にコミットしないように `.gitignore` に追加してください。

4. **実行**

   ```powershell
   cd desktop-mascot
   dotnet run
   ```

---

## 🎮 使い方

| 操作 | アクション |
|------|-----------|
| 左クリック + ドラッグ | マスコットを移動 |
| 右クリック | メニューを開く |
| メニュー「チャットを開く」| 吹き出しチャットを表示 |
| メニュー「最前面固定」 | 最前面表示のオン/オフ |
| Enter / 送信ボタン | メッセージ送信 |

---

## 🎨 emotion による表情切り替え（Step 5）

ChatGPT への**システムプロンプト**に以下を含めています：

> 返答は必ず `{"message": "...", "emotion": "..."}` の JSON 形式で出力してください。

| emotion 値  | 表示画像        | 使われる場面           |
|------------|----------------|----------------------|
| `normal`   | normal.png     | 普通の返答            |
| `smile`    | smile.png      | 楽しい・嬉しい内容     |
| `troubled` | troubled.png   | 困っている・難しい内容  |
| `angry`    | angry.png      | 怒り・批判的な内容     |
| `surprised`| surprised.png  | 驚き・意外な内容       |

---

## 📦 Step 7: exe 化・配布方法

### 単一 exe ファイルとして発行

```powershell
cd desktop-mascot
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

出力先: `bin/Release/net8.0-windows/win-x64/publish/DesktopMascot.exe`

### 配布に含めるファイル

```
DesktopMascot.exe
appsettings.json        ← APIキーを記入したもの（または不要なら省略）
assets/
  character/
    normal.png
    smile.png
    troubled.png
    angry.png
    surprised.png
```

> 💡 `--self-contained true` を使うと、配布先に .NET ランタイムのインストールが不要です。  
> ただし exe サイズが約 100MB 程度になります。小さくしたい場合は `--self-contained false` にして .NET 8 Desktop Runtime を別途インストールしてもらう方法もあります。

### スタートアップ起動の設定（オプション）

Windows スタートアップに登録するには：

```powershell
$exePath = "C:\path\to\DesktopMascot.exe"
$regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Set-ItemProperty -Path $regPath -Name "DesktopMascot" -Value $exePath
```

---

## 🛡 セキュリティ注意事項

- `appsettings.json` に直接 APIキーを書く場合は Git にコミットしないでください。
- `.gitignore` に `appsettings.json` を追加することを推奨します。
- 環境変数 `OPENAI_API_KEY` を使うのが最も安全です。

---

## 📋 実装ステップ一覧

| Step | 内容                           | 対応ファイル                     |
|------|-------------------------------|--------------------------------|
| 1    | 透明背景 + キャラクター画像表示  | MainWindow.xaml / .cs          |
| 2    | 常に最前面 + ドラッグ移動        | MainWindow.xaml / .cs          |
| 3    | 吹き出しUI追加                 | BubbleWindow.xaml / .cs        |
| 4    | ChatGPT API連携               | ChatService.cs / ConfigService.cs |
| 5    | emotion による表情切り替え      | ChatService.cs / BubbleWindow.xaml.cs |
| 6    | 会話履歴の保持                 | ChatService.cs / BubbleWindow.xaml.cs |
| 7    | exe 化・配布方法               | このREADME参照                  |

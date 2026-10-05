# noburick.github.io

## GitHub Pages 公開手順

1. GitHub のリポジトリ設定を開く
2. **Pages** を開く
3. **Build and deployment** の Source を `Deploy from a branch` に設定
4. Branch を `main` / `/ (root)` に設定して保存
5. 数分後に `https://noburick.github.io/` で公開を確認

## ホームページの編集

- `apps.js` の `appCatalog` で作品名・説明・状態・画像・リンクを管理します。
- `detailHref` を設定すると、ストアリンクと並べて詳細ページへのボタンを表示します。
- 開発中で `href` が空の作品は `Coming Soon` を表示します。
- `index.html` はトップ、作品一覧、開発について、Xへの導線を含みます。
- `navigation.js` はスマホ用メニューを制御します。JavaScriptを無効にしてもナビゲーションと作品詳細リンクを使えます。
- `fonts/` は手書き風見出しの Kalam と本文の Noto Sans JP（掲載テキスト・かな・英数字に絞った軽量版）です。両フォントのライセンスを同梱しています。未収録文字は端末の日本語フォントへフォールバックします。
- `design/preview-mobile.webp` と `design/preview-desktop.webp` は確認時の画面です。

### イラスト

画像生成ツールで、承認済みのデザイン見本に合わせて作成した装飾イラストです。キャラクターの公式素材としては扱っていません。

- `images/realm-hero-walk.webp`: トップページ用。ひなたと3匹の猫が庭の小道を歩く横長イラスト。PCでは右側へコピーを重ね、900px以下では画像全体を上部に表示します。
- `images/realm-hero.webp`: 淡い空、海辺の町、植物と石垣から顔を出す4匹の猫。文字を重ねる中央部分には余白。
- `images/realm-hero-mobile.webp`: 同じ場面の縦長版。4匹すべてがスマホでも見える構図。
- `images/realm-workshop.webp`: 植物、本、マグカップがある日差しの柔らかい窓辺で眠る猫。

生成指示の共通条件：温かい水彩・ガッシュの絵本風、クリーム色と深緑、UI・文字・ロゴを含まない背景素材。

## アクセス解析（Google Analytics 4）

- 測定ID：`G-1MSWYBR5F8`
- トップページと `apps/hurimuki/`、`apps/irasutoya-escape/`、`apps/nekotomo/` の各紹介ページに Google タグを設置しています。
- 新しい紹介ページを追加するときは、既存ページと同じタグを `<head>` 直後に1回だけ追加してください。
- GA4 の拡張計測で外部リンクのクリックを有効にすると、App Store・Google Play・X等へのクリックを `click` イベントとして集計できます。App Storeへの遷移は `link_domain = apps.apple.com` で絞り込みます。
- 動作確認：公開サイトを開き、GA4 のリアルタイムレポートでアクセスを確認します。広告ブロッカー等により計測されない場合があります。

const STATUS = Object.freeze({
  released: "released",
  testing: "testing",
  developing: "developing"
});

const appCatalog = [
  {
    title: "ねことも！ポモドーロ",
    summary: "猫たちと一緒に集中時間を楽しむポモドーロタイマー",
    status: STATUS.released,
    image: "./images/nekotomo-card.png",
    href: "./apps/nekotomo/"
  },
  {
    title: "いらすとやからの脱出",
    summary: "いらすとやの世界を探索する脱出ゲーム",
    status: STATUS.released,
    image: "./images/irasutoya-card.png",
    href: "https://apps.apple.com/jp/app/id6747669718"
  },
  {
    title: "ふりむきミリオンキャット",
    summary: "振り向く猫を集めるカジュアルゲーム",
    status: STATUS.testing,
    image: "./images/millioncat-card.png",
    href: "./apps/hurimuki/"
  },
  {
    title: "ウチ猫冒険記",
    summary: "猫たちの大冒険が始まる",
    status: STATUS.developing,
    image: "./images/uchineko-card.png",
    href: ""
  }
];

const statusMap = {
  [STATUS.released]: {
    label: "🟢 配信中",
    className: "status-badge--released"
  },
  [STATUS.testing]: {
    label: "🟡 テスター募集中",
    className: "status-badge--testing"
  },
  [STATUS.developing]: {
    label: "🔵 開発中",
    className: "status-badge--developing"
  }
};

const appsRoot = document.querySelector("#apps");
const template = document.querySelector("#app-card-template");

function getButtonLabel(app) {
  if (app.status === STATUS.testing) {
    return "詳細";
  }

  if (app.status === STATUS.developing) {
    return "開発中";
  }

  if (getUrlHost(app.href) === "apps.apple.com") {
    return "App Store";
  }

  if (getUrlHost(app.href) === "play.google.com") {
    return "Google Play";
  }

  return "詳細";
}

function getUrlHost(href) {
  try {
    return new URL(href, window.location.href).host;
  } catch {
    return "";
  }
}

function createAction(app) {
  const label = getButtonLabel(app);

  if (!app.href) {
    const button = document.createElement("span");
    button.className = "button button--disabled";
    button.textContent = label;
    return button;
  }

  const link = document.createElement("a");
  link.className = "button";
  link.href = app.href;
  link.textContent = label;

  if (/^https?:\/\//.test(app.href)) {
    link.target = "_blank";
    link.rel = "noopener noreferrer";
  }

  return link;
}

appCatalog.forEach((app) => {
  const fragment = template.content.cloneNode(true);
  const badge = fragment.querySelector(".status-badge");
  const image = fragment.querySelector("img");
  const title = fragment.querySelector(".app-card__title");
  const summary = fragment.querySelector(".app-card__summary");
  const actions = fragment.querySelector(".app-card__actions");
  const status = statusMap[app.status];

  badge.textContent = status.label;
  badge.classList.add(status.className);
  image.src = app.image;
  image.alt = `${app.title}の代表画像`;
  title.textContent = app.title;
  summary.textContent = app.summary;
  actions.append(createAction(app));

  appsRoot.append(fragment);
});

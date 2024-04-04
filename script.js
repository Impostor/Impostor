const DEFAULT_PORT_HTTP = 22023;
const DEFAULT_PORT_HTTPS = 443;

/**
 * @template {HTMLElement} T
 * @param {string} elementId
 * @returns {T}
 */
function getElementByIdOrThrow(elementId) {
    const element = document.getElementById(elementId);
    if (element === null) throw new Error(`Couldn't find the element with id ${elementId}`);
    return /** @type {T} */ (element);
}

const addressInput = /** @type {HTMLInputElement} */ (getElementByIdOrThrow("address"));
const portInput = /** @type {HTMLInputElement} */ (getElementByIdOrThrow("port"));
const nameInput = /** @type {HTMLInputElement} */ (getElementByIdOrThrow("name"));
const protocolSelect =
    /** @type {Omit<HTMLSelectElement, "value"> & { value: Protocol; }} */
    (getElementByIdOrThrow("protocol"));

/**
 * @typedef {{ protocol: Protocol, address: string, port: number, name: string, url: string }} ServerInfo
 * @typedef {"desktop" | "android" | "ios"} Platform
 * @typedef {"https" | "http"} Protocol
 */

/**
 * @returns {ServerInfo}
 */
function parseForm() {
    const protocol = protocolSelect.value;
    const address = addressInput.value.trim();
    const port = parseInt(portInput.value) || (protocol === "http" ? DEFAULT_PORT_HTTP : DEFAULT_PORT_HTTPS);
    const name = nameInput.value || "Impostor";

    return { protocol, address, port, name, url: `${protocol}://${address}` };
}

function downloadAsync() {
    const serverInfo = parseForm();
    if (!serverInfo.address) {
        return false;
    }

    const json = generateRegionInfo(serverInfo);

    console.log(`Saving ${json}`);

    const blob = new Blob([json], { type: "text/plain" });
    saveFile(blob, "regionInfo.json");

    return false;
}

function openApp() {
    const serverInfo = parseForm();
    if (!serverInfo.address) {
        return false;
    }

    const params = new URLSearchParams({
        servername: serverInfo.name,
        serverport: serverInfo.port.toString(),
        serverip: serverInfo.url,
        usedtls: false.toString(),
    });

    const url = `amongus://init?${params.toString()}`;

    console.log(`Opening ${url}`);
    window.location.href = url;

    return false;
}

/** @type {Record<Platform, HTMLAnchorElement>} */
const platformButtons = {
    desktop: getElementByIdOrThrow("desktop"),
    android: getElementByIdOrThrow("android"),
    ios: getElementByIdOrThrow("ios"),
};

/** @type {?Platform} */
let currentPlatform = null;
let httpsSetExplicitly = false;

/**
 * @param {Platform} platform
 * @param {boolean} value
 */
function setEnabled(platform, value) {
    for (const e of /** @type {NodeListOf<HTMLElement>} */ (document.querySelectorAll(`.${platform}-support`))) {
        e.style.display = value ? "block" : "none";
    }
}

/**
 * @param {Platform} platform
 */
function setPlatform(platform) {
    if (currentPlatform === platform) {
        return;
    }

    if (currentPlatform) {
        setEnabled(currentPlatform, false);
        platformButtons[currentPlatform].classList.remove("text-primary");
    }

    // HTTPS is mandatory on ios/android
    if (platform === "android" || platform === "ios") {
        httpsSetExplicitly = protocolSelect.value === "https";
        protocolSelect.value = "https";
        protocolSelect.disabled = true;
        setPortIfDefault("https");
    } else {
        protocolSelect.disabled = false;
        if (!httpsSetExplicitly) {
            protocolSelect.value = "http";
            setPortIfDefault("http");
        }
    }

    setEnabled(platform, true);
    platformButtons[platform].classList.add("text-primary");

    currentPlatform = platform;
}

/**
 * @param {Protocol} protocol
 */
function setPortIfDefault(protocol) {
    const oldPort = protocol === "http" ? DEFAULT_PORT_HTTPS : DEFAULT_PORT_HTTP;
    const newPort = protocol === "http" ? DEFAULT_PORT_HTTP : DEFAULT_PORT_HTTPS;
    if (!portInput.value || portInput.value === oldPort.toString()) {
        portInput.value = newPort.toString();
    }
}

function onProtocolChange() {
    setPortIfDefault(protocolSelect.value);
}

/**
 * @param {ServerInfo} serverInfo
 * @returns {string}
 */
function generateRegionInfo(serverInfo) {
    const regions = [
        {
            $type: "StaticHttpRegionInfo, Assembly-CSharp",
            Name: serverInfo.name,
            PingServer: serverInfo.address,
            Servers: [
                {
                    Name: "http-1",
                    Ip: serverInfo.url,
                    Port: serverInfo.port,
                    UseDtls: false, // As no custom key can be specified, we need to disable DTLS on custom servers.
                },
            ],
            TranslateName: 1003, // StringNames.NoTranslation
        },
    ];

    const jsonServerData = {
        CurrentRegionIdx: 3,
        Regions: regions,
    };

    return JSON.stringify(jsonServerData, null, 4);
}

/**
 * @param {Blob} blob
 * @param {string} fileName
 */
function saveFile(blob, fileName) {
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    document.body.appendChild(a);
    a.style.display = "none";
    a.href = url;
    a.download = fileName;
    a.click();

    URL.revokeObjectURL(url);
}

function fillFromLocationHash() {
    const urlServerAddress = document.location.hash.substring(1).split(":");
    const serverAddress = urlServerAddress.length > 0 ? urlServerAddress[0] : null;
    const serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : null;
    const protocol = urlServerAddress.length > 2 ? urlServerAddress[2] : null;
    const serverName = urlServerAddress.length > 3 ? urlServerAddress[3] : null;

    if (serverAddress) {
        addressInput.value = serverAddress;
    }

    if (serverPort && !Number.isNaN(parseInt(serverPort))) {
        portInput.value = serverPort;
    }

    if (protocol === "http" || protocol === "https") {
        protocolSelect.value = protocol;
    }

    if (serverName) {
        nameInput.value = serverName;
    }
}

function setLocationHash() {
    const serverInfo = parseForm();
    document.location.hash = [serverInfo.address, serverInfo.port, serverInfo.protocol, serverInfo.name].join(":");
}

fillFromLocationHash();

if (["iPhone", "iPad", "iPod"].indexOf(window.navigator.platform) !== -1) {
    setPlatform("ios");
} else if (/Android/.test(window.navigator.userAgent)) {
    setPlatform("android");
} else {
    setPlatform("desktop");
}

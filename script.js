const DEFAULT_PORT_HTTP = "22000";
const DEFAULT_PORT_HTTPS = "443";

/**
 * @typedef {{ address: string, port: number, name: string, protocol: string, url: string }} ServerInfo
 * @returns {ServerInfo}
 */
function parseForm() {
    const address = document.getElementById("address").value.trim();
    const port = parseInt(document.getElementById("port").value) ?? DEFAULT_PORT_HTTP;
    const name = document.getElementById("name").value || "Impostor";
    const protocol = document.querySelector("input[name=serverProtocol]:checked").value || "http";

    return { address, port, name, protocol, url: `${protocol}://${address}` };
}

async function downloadAsync() {
    const serverInfo = parseForm();

    const json = generateRegionInfo(serverInfo);
    const blob = new Blob([json], { type: "text/plain" });
    saveFile(blob, "regionInfo.json");

    return false;
}

async function openApp() {
    const serverInfo = parseForm();

    const params = new URLSearchParams({
        servername: serverInfo.name,
        serverport: serverInfo.port,
        serverip: serverInfo.url,
        usedtls: false,
    });
    window.location = `amongus://init?${params.toString()}`;

    return false;
}

let currentPlatform;
let httpsSetExplicitly = false;

function setEnabled(platform, value) {
    for (const e of document.querySelectorAll(`.${platform}-support`)) {
        e.style.display = value ? "block" : "none";
    }
}

function setPlatform(platform) {
    if (currentPlatform === platform) {
        return;
    }

    if (currentPlatform) {
        setEnabled(currentPlatform, false);
        document.getElementById(currentPlatform).classList.remove("text-primary");
    }

    // HTTPS is mandatory on ios/android
    const httpRadio = document.getElementById("http");
    const httpsRadio = document.getElementById("https");
    if (platform === "android" || platform === "ios") {
        httpsSetExplicitly = httpsRadio.checked;
        httpsRadio.checked = true;
        httpRadio.disabled = true;
        setPortIfDefault("https");
    } else {
        httpRadio.disabled = false;
        if (!httpsSetExplicitly) {
            httpRadio.checked = true;
            setPortIfDefault("http");
        }
    }

    setEnabled(platform, true);
    document.getElementById(platform).classList.add("text-primary");

    currentPlatform = platform;
}

function setPortIfDefault(protocol) {
    const oldPort = protocol === "http" ? DEFAULT_PORT_HTTPS : DEFAULT_PORT_HTTP;
    const newPort = protocol === "http" ? DEFAULT_PORT_HTTP : DEFAULT_PORT_HTTPS;
    const portField = document.getElementById("port");
    if (portField.value === oldPort) {
        portField.value = newPort;
    }
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

    return JSON.stringify(jsonServerData);
}

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
    const serverAddress = urlServerAddress[0];
    const serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : DEFAULT_PORT_HTTP.toString();
    let protocol = urlServerAddress.length > 2 ? urlServerAddress[2] : "http";
    const serverName = urlServerAddress.length > 3 ? urlServerAddress[3] : "";

    if (serverAddress) {
        document.getElementById("address").value = serverAddress;
    }

    if (parseInt(serverPort) !== NaN) {
        document.getElementById("port").value = serverPort;
    }

    // Set the default protocol to http
    if (protocol !== "http" && protocol !== "https") {
        protocol = "http";
    }
    document.getElementById(protocol).checked = true;

    document.getElementById("name").value = serverName;
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

const DEFAULT_PORT = 22000;

const IP_REGEX = /(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}/;

async function parseAddressAsync(serverAddress) {
    if (serverAddress === "localhost") {
        serverAddress = "127.0.0.1";
    }

    if (IP_REGEX.test(serverAddress)) {
        // TODO: wait for update fixing StaticRegionInfo
        return [serverAddress, serverAddress];
    }

    /** @type {{ Status: number, Answer: { type: number, data: string }[] }} */
    const dns = await (await fetch("https://cloudflare-dns.com/dns-query?type=A&name=" + serverAddress, {
        headers: {
            "Accept": "application/dns-json"
        }
    })).json();

    if (dns && dns.Status === 0) {
        for (const record of dns.Answer) {
            if (record.type === 1 && IP_REGEX.test(record.data)) {
                return [record.data, serverAddress];
            }
        }
    }

    const message = "Failed DNS request for " + serverAddress;

    alert(message);
    throw Error(message);
}

function parseForm() {
    const serverAddress = document.getElementById("address").value.trim();
    const serverPort = parseInt(document.getElementById("port").value) ?? DEFAULT_PORT;
    const serverName = document.getElementById("name").value || "Impostor";
    const serverProtocol = document.querySelector("input[name=serverProtocol]:checked").value || "http";

    return [serverAddress, serverPort, serverName, serverProtocol];
}

async function downloadAsync() {
    const [serverAddress, serverPort, serverName, serverProtocol] = parseForm();

    const json = generateRegionInfo(
        serverName,
        serverAddress,
        serverPort,
        serverProtocol
    );
    const blob = new Blob([json], { type: "text/plain" });
    saveFile(blob, "regionInfo.json");

    return false;
}

async function openApp() {
    const [serveraddress, serverport, servername, serverprotocol] = parseForm();

    const serverip = serverprotocol + ":%2F%2F" + serveraddress

    const params = new URLSearchParams({
        servername,
        serverport,
        serverip,
        usedtls: false,
    });
    const url = `amongus://init?${params.toString()}`;
    window.location = url;

    return false;
}

let currentPlatform;
let httpsSetExplicitly;

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
    if ('android' == platform || 'ios' == platform) {
        httpsSetExplicitly = httpsRadio.checked;
        httpsRadio.checked = true;
        httpRadio.disabled = true;
        setPortIfDefault('https');
    } else {
        httpRadio.disabled = false;
        if (false == httpsSetExplicitly) {
            httpRadio.checked = true;
            setPortIfDefault('http');
        }
    }

    setEnabled(platform, true);
    document.getElementById(platform).classList.add("text-primary");

    currentPlatform = platform;
}

function setPortIfDefault(protocol) {
    const oldPort = protocol == 'http' ? '443' : '22000';
    const newPort = protocol == 'http' ? '22000' : '443';
    const portField = document.getElementById("port");
    if (portField.value == oldPort) {
        portField.value = newPort;
    }
}

function generateRegionInfo(name, fqdn, port, protocol) {
    const regions = [
        // Add default regions so they also show up in the menu
        {
            "$type": "StaticHttpRegionInfo, Assembly-CSharp",
            "Name": "North America",
            "PingServer": "matchmaker.among.us",
            "Servers": [
                {
                    "Name": "Http-1",
                    "Ip": "https://matchmaker.among.us",
                    "Port": 443,
                    "UseDtls": true,
                    "Players": 0,
                    "ConnectionFailures": 0
                }
            ],
            "TranslateName": 289
        },
        {
            "$type": "StaticHttpRegionInfo, Assembly-CSharp",
            "Name": "Europe",
            "PingServer": "matchmaker-eu.among.us",
            "Servers": [
                {
                    "Name": "Http-1",
                    "Ip": "https://matchmaker-eu.among.us",
                    "Port": 443,
                    "UseDtls": true,
                    "Players": 0,
                    "ConnectionFailures": 0
                }
            ],
            "TranslateName": 290
        },
        {
            "$type": "StaticHttpRegionInfo, Assembly-CSharp",
            "Name": "Asia",
            "PingServer": "matchmaker-as.among.us",
            "Servers": [
                {
                    "Name": "Http-1",
                    "Ip": "https://matchmaker-as.among.us",
                    "Port": 443,
                    "UseDtls": true,
                    "Players": 0,
                    "ConnectionFailures": 0
                }
            ],
            "TranslateName": 291
        },
        // Followed by the custom region
        {
            $type: "StaticHttpRegionInfo, Assembly-CSharp",
            Name: name,
            PingServer: fqdn,
            Servers: [
                {
                    Name: "http-1",
                    Ip: protocol + "://" + fqdn,
                    Port: port,
                    UseDtls: false, // As no custom key can be specified, we need to disable DTLS on custom servers.
                    Players: 0,
                    ConnectionFailures: 0
                }
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
    let url = URL.createObjectURL(blob);

    let a = document.createElement("a");
    document.body.appendChild(a);
    a.style.display = "none";
    a.href = url;
    a.download = fileName;
    a.click();

    URL.revokeObjectURL(url);
}

function fillFromLocationHash() {
    const urlServerAddress = document.location.hash.substr(1).split(":");
    const serverAddress = urlServerAddress[0];
    const serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : DEFAULT_PORT.toString();
    const protocol = urlServerAddress.length > 2 ? urlServerAddress[2] : 'http';
    const serverName = urlServerAddress.length > 3 ? urlServerAddress[3] : '';

    if (serverAddress) {
        document.getElementById("address").value = serverAddress;
    }

    if (new RegExp("^[0-9]+$", "g").test(serverPort)) {
        document.getElementById("port").value = serverPort;
    }

    // Set the default protocol to http
    if ('http' != protocol && 'https' != protocol) {
        protocol = 'http';
    }
    document.getElementById(protocol).checked = true;

    document.getElementById('name').value = serverName;
}

fillFromLocationHash();

if (["iPhone", "iPad", "iPod"].indexOf(window.navigator.platform) !== -1) {
    setPlatform("ios");
} else if (/Android/.test(window.navigator.userAgent)) {
    setPlatform("android");
} else {
    setPlatform("desktop");
}

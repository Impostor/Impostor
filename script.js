const DEFAULT_PORT = 22023;

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

async function downloadAsync() {
    const serverAddress = document.getElementById("address").value;
    const serverPort = document.getElementById("port").value ?? DEFAULT_PORT;
    const serverName = document.getElementById("name").value || "Impostor";

    const [serverIp, serverFqdn] = await parseAddressAsync(serverAddress);
    const json = generateRegionInfo(serverName, serverIp, serverFqdn, serverPort);
    const blob = new Blob([json], {type: "text/plain"});
    saveFile(blob, "regionInfo.json");

    return false;
}

let currentPlatform;

function setEnabled(platform, value) {
    document.querySelector(`.${platform}-support`).style.display = value ? "block" : "none";
}

function setPlatform(platform) {
    if (currentPlatform === platform) {
        return;
    }

    if (currentPlatform) {
        setEnabled(currentPlatform, false);
        document.getElementById(currentPlatform).classList.remove("text-primary");
    }

    setEnabled(platform, true);
    document.getElementById(platform).classList.add("text-primary");

    currentPlatform = platform;
}

function generateRegionInfo(name, ip, fqdn, port) {
    const regions = [
        // Add default regions so they also show up in the menu
        {
            $type: "DnsRegionInfo, Assembly-CSharp",
            Fqdn: "na.mm.among.us",
            DefaultIp: "50.116.1.42",
            Port: 22023,
            Name: "North America",
            TranslateName: 289,
        },
        {
            $type: "DnsRegionInfo, Assembly-CSharp",
            Fqdn: "eu.mm.among.us",
            DefaultIp: "172.105.251.170",
            Port: 22023,
            Name: "Europe",
            TranslateName: 290,
        },
        {
            $type: "DnsRegionInfo, Assembly-CSharp",
            Fqdn: "as.mm.among.us",
            DefaultIp: "139.162.111.196",
            Port: 22023,
            Name: "Asia",
            TranslateName: 291,
        },
        // Followed by the custom region
        {
            $type: "DnsRegionInfo, Assembly-CSharp",
            Fqdn: fqdn,
            DefaultIp: ip,
            Port: port,
            Name: name,
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

    if (serverAddress) {
        document.getElementById("address").value = serverAddress;
    }

    if (new RegExp("^[0-9]+$", "g").test(serverPort)) {
        document.getElementById("port").value = serverPort;
    }
}

fillFromLocationHash();

if (["iPhone", "iPad", "iPod"].indexOf(window.navigator.platform) !== -1) {
    setPlatform("ios");
} else if (/Android/.test(window.navigator.userAgent)) {
    setPlatform("android");
} else {
    setPlatform("desktop");
}

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

    const dns = await (await fetch("https://dns.google/resolve?type=A&name=" + serverAddress)).json();
    
    for (let i of dns?.Answer) {
         if (i.type === 1) {
            if (dns && dns.Status === 0 && i.length === 1 && IP_REGEX.test(dns.Answer[0].data)) {
                return [i.data, serverAddress];
            } else {
                const message = "Failed DNS request for " + serverAddress;

                alert(message);
                throw Error(message);
            }
         }
    } 

    if (dns && dns.Status === 0 && dns.Answer.length === 1 && IP_REGEX.test(dns.Answer[0].data)) {
        return [dns.Answer[0].data, serverAddress];
    } else {
        const message = "Failed DNS request for " + serverAddress;

        alert(message);
        throw Error(message);
    }
}

async function downloadAsync() {
    const serverAddress = document.getElementById("address").value;
    const serverPort = document.getElementById("port").value ?? DEFAULT_PORT;
    const serverName = document.getElementById("name").value || "Impostor";

    const [serverIp, serverFqdn] = await parseAddressAsync(serverAddress);
    const json = generateRegionInfo(serverName, serverIp, serverFqdn, serverPort);
    const blob = new Blob([json], { type: "text/plain" });
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
    const region = {
        "$type": "DnsRegionInfo, Assembly-CSharp",
        "Fqdn": fqdn,
        "DefaultIp": ip,
        "Port": port,
        "Name": name,
        "TranslateName": 1003 // StringNames.NoTranslation
    };

    const jsonServerData = {
        "CurrentRegionIdx": 0,
        "Regions": [region]
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

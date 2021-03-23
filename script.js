// const DEFAULT_PORT = 22023;

const MAPPINGS = {
    // TODO: once 2021.3.9 rolls out across all platforms make this selection affect only instructions
    "2021.3.5s": { CurrentRegionIdx: "NHKLLGFLCLM", Regions: "PBKMLNEHKHL" },
    "2021.3.9a": null,
    "2021.3.5o": { CurrentRegionIdx: "BBHHIMPDFMB", Regions: "GAOCJNFMKLA" },
    "2021.3.5i": { CurrentRegionIdx: "FMCHICBDPCE", Regions: "HACEEIOGMDC" },
    "2021.3.5e": { CurrentRegionIdx: "KCEOFOCILEP", Regions: "FHHFNNEMCJC" },
    "2021.2.21m": { CurrentRegionIdx: "BIPBMPFJBMI", Regions: "GECPHDGCFJM" }
};

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
    // const serverPort = document.getElementById("port").value ?? DEFAULT_PORT; TODO wait for update fixing ports in DnsRegionInfo
    const serverName = document.getElementById("name").value || "Impostor";
    const gamePlatform = document.getElementById("platform").value;

    const [serverIp, serverFqdn] = await parseAddressAsync(serverAddress);
    const json = generateRegionInfo(serverName, serverIp, serverFqdn, MAPPINGS[gamePlatform]);
    const blob = new Blob([json], { type: "text/plain" });
    saveFile(blob, "regionInfo.json");

    return false;
}

const platformTexts = {
    ".ios-support": ["o"],
    ".android-support": ["a"],
    ".desktop-support": ["s", "i", "e", "m"]
};

function updatePlatformText() {
    const gamePlatform = document.getElementById("platform").value;

    for (let className in platformTexts) {
        const isVisible = platformTexts[className].some(postfix => gamePlatform.endsWith(postfix));
        document.querySelectorAll(className).forEach(element => element.style.display = isVisible ? "block" : "none");
    }
}

function generateRegionInfo(name, ip, fqdn, mappings) {
    const region = {
        "$type": "DnsRegionInfo, Assembly-CSharp",
        "Fqdn": fqdn,
        "DefaultIp": ip,
        "Name": name,
        "TranslateName": 1003 // StringNames.NoTranslation
    };

    const jsonServerData = {
        [mappings?.CurrentRegionIdx ?? "CurrentRegionIdx"]: 0,
        [mappings?.Regions ?? "Regions"]: [region]
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
    // const serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : DEFAULT_PORT.toString();

    if (serverAddress) {
        document.getElementById("address").value = serverAddress;
    }

    // if (new RegExp("^[0-9]+$", "g").test(serverPort)) {
    //     document.getElementById("port").value = serverPort;
    // }
}

fillFromLocationHash();

if (["iPhone", "iPad", "iPod"].indexOf(window.navigator.platform) !== -1) {
    document.getElementById("platform").value = "2021.3.5o";
} else if (/Android/.test(window.navigator.userAgent)) {
    document.getElementById("platform").value = "2021.3.9a";
}

updatePlatformText();

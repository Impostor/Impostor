// const DEFAULT_PORT = 22023;

const MAPPINGS = {
    // TODO: once 2021.3.9 rolls out across all platforms remove this selection altogether
    "2021.3.5s": { CurrentRegionIdx: "NHKLLGFLCLM", Regions: "PBKMLNEHKHL" },
    "2021.3.9a": { CurrentRegionIdx: "CurrentRegionIdx", Regions: "Regions" },
    "2021.3.5o": { CurrentRegionIdx: "BBHHIMPDFMB", Regions: "GAOCJNFMKLA" },
    "2021.3.5i": { CurrentRegionIdx: "FMCHICBDPCE", Regions: "HACEEIOGMDC" },
    "2021.3.5e": { CurrentRegionIdx: "KCEOFOCILEP", Regions: "FHHFNNEMCJC" },
    "2021.2.21m": { CurrentRegionIdx: "BIPBMPFJBMI", Regions: "GECPHDGCFJM" },
};

function download() {
    const serverIp = document.getElementById("ip").value;
    // const serverPort = document.getElementById("port").value ?? DEFAULT_PORT; TODO wait for update fixing ports in DnsRegionInfo
    const serverFqdn = document.getElementById("fqdn").value;
    let serverName = document.getElementById("name").value;
    if (!serverName) {
        serverName = "Impostor";
    }

    const gamePlatform = document.getElementById("platform").value;

    const json = generateRegionInfo(serverName, serverIp, serverFqdn, MAPPINGS[gamePlatform]);
    const blob = new Blob([json], { type: "text/plain" });
    saveFile(blob, "regionInfo.json");

    return false;
}

const platformTexts = {
    ".ios-support": ["o"],
    ".android-support": ["a"],
    ".desktop-support": ["s", "i", "e", "m"],
}

function updatePlatformText() {
    const gamePlatform = document.getElementById("platform").value;

    for (let className in platformTexts) {
        const isVisible = platformTexts[className].some(postfix => gamePlatform.endsWith(postfix));
        document.querySelector(className).style.display = isVisible ? "block" : "none";
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
        [mappings.CurrentRegionIdx]: 0,
        [mappings.Regions]: [region]
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
    const serverIp = urlServerAddress[0];
    // const serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : DEFAULT_PORT.toString();

    const ipPattern = document.getElementById("ip").getAttribute("pattern");
    if (new RegExp(ipPattern).test(serverIp)) {
        document.getElementById("ip").value = serverIp;
    }

    // if (new RegExp("^[0-9]+$", "g").test(serverPort)) {
    //     document.getElementById("port").value = serverPort;
    // }
}

fillFromLocationHash();

if (['iPhone', 'iPad', 'iPod'].indexOf(window.navigator.platform) !== -1) {
    document.getElementById("platform").value = "2021.3.5o";
} else if (/Android/.test(window.navigator.userAgent)) {
    document.getElementById("platform").value = "2021.3.9a";
}

updatePlatformText();

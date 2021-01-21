var REGION_NAME = "Impostor"
var SERVER_PORT = 22023;

$(document).ready(function(){
    $("#default_server_port").text(SERVER_PORT);
    $("#default_server_name").text(REGION_NAME);
    fillIPAdressUsingLocationHash();
    
    showPlatformText();
    
    $("#serverFileForm").submit(function(e){
        e.preventDefault();
        let serverIp = $("#ip").val();
        let serverPort = $("#port").val();
        let serverName = $("#region_name").val();
        let serverFileBytes = generateServerFile((serverName.length > 0 && serverName.length <= 20) ? serverName : REGION_NAME, serverIp, serverPort);
        let blob = new Blob([serverFileBytes.buffer]);
        saveFile(blob, "regionInfo.dat");
    });

});

function fillIPAdressUsingLocationHash() {
    let urlServerAddress = document.location.hash.substr(1).split(":");
    let serverIp = urlServerAddress[0];
    let serverPort = urlServerAddress.length > 1 ? urlServerAddress[1] : SERVER_PORT.toString();
    const ipPattern = $("#ip").attr("pattern");
    
    if (new RegExp(ipPattern).test(serverIp)) {
        $("#ip").val(serverIp);
    }
    if (new RegExp("^[0-9]+$", "g").test(serverPort)) {
        $("#port").val(serverPort);
    }
}

function showPlatformText() {
    if (navigator.userAgent.match(/iPhone/i) || navigator.userAgent.match(/iPod/i) || navigator.userAgent.match(/iPad/i)) {
        $('.ios-support').show();
    } else if (navigator.userAgent.match(/android/i)) {
        $('.android-support').show();
    } else {
        $('.desktop-support').show();
    }
}

function generateServerFile(regionName, ip, port) {
    let bytesArray = int32(0);
    bytesArray.push(regionName.length);
    bytesArray = concatArrays(bytesArray, stringToBytes(regionName));
    bytesArray.push(ip.length);
    bytesArray = concatArrays(bytesArray, stringToBytes(ip));
    bytesArray = concatArrays(bytesArray, int32(1));
    
    let serverName = regionName + "-Master-1";

    bytesArray.push(serverName.length);
    bytesArray = concatArrays(bytesArray, stringToBytes(serverName));
    bytesArray = concatArrays(bytesArray, ipAddressToBytes(ip));
    bytesArray = concatArrays(bytesArray, int16(port));
    bytesArray = concatArrays(bytesArray, int32(0));
    return Uint8Array.from(bytesArray);
}

function saveFile(blob, fileName) {
    let url = URL.createObjectURL(blob);
    
    let a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";
    a.href = url;
    a.download = fileName;
    a.click();
    
    URL.revokeObjectURL(url);
}

function stringToBytes(str) {
    let bytes = [];
    for (let i = 0; i < str.length; i++) {
        bytes.push(str.charCodeAt(i));
    }
    return bytes;
}

function int16(int) {
    //Convert integer number to little-endian 16-bits int representation
    return [(int & 0xFF), 
            (int & 0xFF00) >> 8];
}

function int32(int) {
    //Convert integer number to little-endian 32-bits int representation
    return [(int & 0xFF), 
            (int & 0xFF00) >> 8, 
            (int & 0xFF0000) >> 16, 
            (int & 0xFF000000) >> 24];
}

function ipAddressToBytes(ipAddress) {
    let octets = ipAddress.split(".");
    let bytes = [];
    for(let i = 0; i < octets.length; i++) {
        bytes.push(parseInt(octets[i]));
    }
    return bytes;
}

function concatArrays(array1, array2) {
    let newArray = [];
    for (let i = 0; i < array1.length; i++) {
        newArray.push(array1[i]);
    }
    for (let i = 0; i < array2.length; i++) {
        newArray.push(array2[i]);
    }
    return newArray;
}

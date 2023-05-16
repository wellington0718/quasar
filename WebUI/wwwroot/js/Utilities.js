﻿function saveAsFile(filename, bytesBase64){

    const link = document.createElement("a");
    link.download = filename;
    link.href = "data:application/octet-stream;base64, " + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function ScrollToTop(selector) {
    body = document.querySelector("#radzenBody")
    body.scrollTo({ top: 0, behavior: 'smooth' })
}
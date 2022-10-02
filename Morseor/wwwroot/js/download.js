function downloadFile(mimeType, base64String, filename)
{
    fileDataUrl = "data:" + mimeType + ";base64," + base64String;
    fetch(fileDataUrl)
        .then(res => res.blob())
        .then(blob => {

            //The link
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: mimeType });
            link.download = filename;

            //click and remove
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

        });
}
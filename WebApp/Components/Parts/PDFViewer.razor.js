// PDFモーダル表示
export async function setSource(elementId, stream, title) {
    const arrayBuffer = stream.buffer;
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);

    const element = document.getElementById(elementId);
    const dialog = document.getElementById('PDFViewerDialog');
    element.setAttribute("src", `/pdfjs/web/viewer_modal.html?file=${url}#fileName=${title}&navpanes=0`);
    element.title = title;

    // モーダル表示
    dialog.showModal();

    // 閉じる：モーダルを閉じる
    const cancelButton = document.getElementById('PDFViewerCloseButton');
    cancelButton?.addEventListener('click', () => {
        URL.revokeObjectURL(url);
        element.setAttribute("src", "");
        dialog.close();
    });
}

// PDF別ウィンドウ表示(pdf.js使用) 
export async function OpenPDFWindow(elementId, stream, title) {

    const arrayBuffer = stream.buffer;
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);

    const windowFeatures = "popup,width=" + window.screen.width + ",height=" + window.screen.height
    window.open("/pdfjs/web/viewer.html?file=" + url + "&title=" + title, "_blank", windowFeatures);
}

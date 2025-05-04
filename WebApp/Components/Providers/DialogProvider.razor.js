export async function showAlert(message) {
    scrollLock(true);

    addMessageTag('alertMessage', message);
    const dialog = document.getElementById('alertDialog');
    dialog.showModal();

    // モーダルを閉じる
    const closeButton = document.getElementById('alertCloseButton');
    closeButton?.addEventListener('click', () => {
        document.getElementById('alertOKHiddenButton').click();
        dialog.close();

        scrollLock(false);
    });
}

export async function showConfirm(message) {
    scrollLock(true);

    addMessageTag('confirmMessage', message);
    const dialog = document.getElementById('confirmDialog');
    dialog.showModal();

    // YES:隠し送信ボタンクリック後、モーダルを閉じる
    const yesButton = document.getElementById('confirmYesButton');
    yesButton?.addEventListener('click', () => {
        document.getElementById('confirmYesHiddenButton').click();
        dialog.close();

        scrollLock(false);
    });

    // キャンセル：モーダルを閉じる
    const cancelButton = document.getElementById('confirmCancelButton');
    cancelButton?.addEventListener('click', () => {
        dialog.close();

        scrollLock(false);
    });
}

function addMessageTag(tagName, message) {
    const element = document.getElementById(tagName);
    element.innerHTML = "";
    const messages = message.split('\n');

    for (let msg of messages) {
        let div = document.createElement("div");
        div.textContent = msg;
        element.append(div);
    }
}

export async function showDialog(tagId) {
    scrollLock(true);

    const dialog = document.getElementById(tagId);
    dialog.showModal();
}
export async function closeDialog(tagId) {
    const dialog = document.getElementById(tagId);
    dialog.close();

    scrollLock(false);
}

function scrollLock(isLock)
{
    document.body.style[`overflow`] = isLock ? 'hidden' : '';
}
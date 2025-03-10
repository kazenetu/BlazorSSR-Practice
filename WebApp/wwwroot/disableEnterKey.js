window.disableEnterKey = async () => {
    const items = document.getElementsByTagName('input');
    for (var index in items) {
        // メソッドの場合は処理なし
        if (isNaN(parseInt(index))) continue;

        // inputタグにエンターキー無効化を実施
        items[index].removeEventListener('keydown', disableEnterKeyEvent);
        items[index].addEventListener('keydown', disableEnterKeyEvent);
    }
}
disableEnterKeyEvent = function (event) {
    console.log(`event.key=${event.key} code=${event.code}`);
    if (event.key === 'Enter') {
        event.preventDefault();
        return false;
    }
}

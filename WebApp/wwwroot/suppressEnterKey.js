window.suppressEnterKey = async () => {
    const items = document.getElementsByTagName('input');
    for (var index in items) {
        // メソッドの場合は処理なし
        if (isNaN(parseInt(index))) continue;

        // inoutタグにエンターキー無効化を実施
        items[index].addEventListener('keydown', function (event) {
            if (event.key === 'Enter') {
                event.preventDefault();
                return false;
            }
        });
    }
}

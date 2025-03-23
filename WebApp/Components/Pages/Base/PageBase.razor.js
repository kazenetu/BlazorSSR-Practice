let tokenizerInstance = undefined;

setTimeout(()=>{
    kuromoji.builder({ dicPath: "dict" }).build((err, tokenizer)=>{
        tokenizerInstance = tokenizer; 
    });
} ,100);

window.kanjiToFurigana = (kanjiID, furiganaID) => {
    const kanji = document.getElementById(kanjiID);
    const furigana = document.getElementById(furiganaID);

    let result = '';
    let items = tokenizerInstance.tokenize(kanji.value);
    for(let i=0;i<items.length;i++){
        if(items[i].reading == undefined) result += items[i].surface_form;
        else result += items[i].reading;
    }

    furigana.value = result;
};

window.getFurigana = (kanji) => {
    let result = '';
    const items = tokenizerInstance.tokenize(kanji);
    for (let i = 0; i < items.length; i++) {
        const item = items[i];

        if (item.pos_detail_1 === "数") {
            result += item.basic_form;
            continue;
        }

        if (item.pos === "記号") {
            result += item.basic_form;
            continue;
        }

        if (item.reading == undefined) result += item.surface_form;
        else result += item.reading;
    }

    return result;
};

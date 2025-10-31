/**　クエリに紐づく対象要素をクリックする */
export function clickQuery(query) {
    const element = document.querySelector(query);
    if (element !== null) element.click();
}

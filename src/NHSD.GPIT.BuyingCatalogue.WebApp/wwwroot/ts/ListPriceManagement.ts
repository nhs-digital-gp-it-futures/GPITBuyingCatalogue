window.onload = function () {
    const unpublishedPriceCheckboxContainer = "unpublished-price-checkbox";
    const unpublishedPriceCheckbox = "ShowUnpublishedPrices";

    // only run if there are actually unpublished items to hide
    if (document.getElementsByClassName("unpublished").length > 0) {
        document.getElementById(unpublishedPriceCheckboxContainer).style.display = 'block';

        ChangePriceDisplayStyle('none');

        const checkbox = document.getElementById(unpublishedPriceCheckbox);

        checkbox.addEventListener("click", ChangePriceDisplay)
    }
}

function ChangePriceDisplay() {
    this.checked ? ChangePriceDisplayStyle('table-row') : ChangePriceDisplayStyle('none');
}

function ChangePriceDisplayStyle(style) {
    const unpublishedPrice = document.getElementsByClassName("unpublished");
    for (let i = 0; i < unpublishedPrice.length; i++) {
        const input = unpublishedPrice.item(i) as HTMLInputElement;
        input.style.display = style;
    }
}

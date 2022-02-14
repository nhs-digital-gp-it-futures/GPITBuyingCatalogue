window.onload = function () {
    const inactiveItemsCheckboxContainer = "inactive-items-checkbox";
    const inactiveItemsCheckbox = "ShowInactiveItems";

    document.getElementById(inactiveItemsCheckboxContainer).style.display = 'block';

    ChangeItemsDisplayStyle('none');

    const checkbox = document.getElementById(inactiveItemsCheckbox);

    checkbox.addEventListener("click", ChangeItemsDisplay)
}

function ChangeItemsDisplay() {
    this.checked ? ChangeItemsDisplayStyle('table-row') : ChangeItemsDisplayStyle('none');
}

function ChangeItemsDisplayStyle(style) {
    const inactiveItemss = document.getElementsByClassName("inactive");
    for (let i = 0; i < inactiveItemss.length; i++) {
        const input = inactiveItemss.item(i) as HTMLInputElement;
        input.style.display = style;
    }
}

window.onload = function() {
    updateTotals();

    Array.from(document.querySelectorAll(`.nhsuk-checkboxes__input:checked`))
        .map(x => x.getAttribute('subgroup'))
        .filter((value, index, self) => self.indexOf(value) === index)
        .forEach(x => updateGroupCount(x));

    Array.from(document.querySelectorAll('.selection-counter'))
        .forEach(x => x.removeAttribute('style'));

    Array.from(document.querySelectorAll('.nhsuk-checkboxes__input'))
        .forEach(x => x.addEventListener('change', checkBoxClicked, false));
}

function checkBoxClicked(checkBox) {
    updateGroupCount(checkBox.target.getAttribute('subgroup'));
    updateTotals();
}

function updateGroupCount(subgroup) {
    const count = document.querySelectorAll(`.nhsuk-checkboxes__input[subgroup=${subgroup}]:checked`).length;
    const output = count > 0 ? ` - ${count} selected` : '';

    document.querySelector(`.counter-class[subgroup=${subgroup}]`).innerText = output;
}

function updateTotals() {
    const selected = document.querySelectorAll('.nhsuk-checkboxes__input:checked').length;
    const total = document.querySelectorAll('.nhsuk-checkboxes__input').length;

    Array.from(document.querySelectorAll('.selection-counter'))
        .forEach(x => x.innerText = `${selected} out of ${total} selected`);
}

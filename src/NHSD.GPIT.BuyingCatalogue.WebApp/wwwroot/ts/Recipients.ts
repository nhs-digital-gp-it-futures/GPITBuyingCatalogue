window.onload = async () => {
    const expanders = document.querySelectorAll('details.nhsuk-expander');
    expanders.forEach((expander) => expander.addEventListener('change', (event) => handleChange(event, expander), false));
}

function handleChange(event: Event, expander: Element) {
    let changed = event.target as HTMLInputElement;

    if (changed.name.startsWith('selectAll')) {
        var checkboxes = expander.querySelectorAll('input[type="checkbox"]:not([name^=selectAll])');
        if (changed.checked) {
            checkboxes.forEach((checkbox) => (checkbox as HTMLInputElement).checked = true);
        } else {
            checkboxes.forEach((checkbox) => (checkbox as HTMLInputElement).checked = false);
        }
    }
    
    var count = expander.querySelectorAll('input[type="checkbox"]:checked:not([name^=selectAll])').length;
    let summary = expander.querySelector('summary > span.nhsuk-details__summary-text_secondary > b') as HTMLElement;
    if (summary !== null) {
      summary.innerText = `${count} `;
    }
}


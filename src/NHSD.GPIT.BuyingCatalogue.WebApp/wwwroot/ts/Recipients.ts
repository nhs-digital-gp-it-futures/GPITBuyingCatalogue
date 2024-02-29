var updateRecipients = (function (): () => void {
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

    //Function called on submit modal
    return function updateRecipients() {
        var checkedboxes = document.querySelectorAll('.modal-checkbox:checked');
        checkedboxes.forEach(function (item) {

            // Find the checkbox with matching ods code
            var row = item.closest('tr');
            var query = "#sr-table #" + row.id + " input[type='checkbox'].main-srs";
            var checkbox = document.querySelector(query) as HTMLInputElement;

            // Check if the checkbox exists
            if (checkbox) {
                // Set the checkbox to checked
                checkbox.checked = true;
            }
        });
        var expanders = document.querySelectorAll('.nhsuk-expander');
        expanders.forEach(function (container) {
            var checks = container.querySelectorAll('.main-srs:checked:not(.sub-sr-selector)');
            var checkboxCount = checks.length;

            var countBox = container.querySelector('.nhsuk-details__summary-text_secondary b');
            countBox.innerHTML = checkboxCount + " ";
        });
    }
})();


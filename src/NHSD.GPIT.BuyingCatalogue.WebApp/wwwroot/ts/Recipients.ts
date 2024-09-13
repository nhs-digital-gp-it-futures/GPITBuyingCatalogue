var updateRecipients = (function (): () => void {
    window.onload = async () => {
        const expanders = document.querySelectorAll('details.nhsuk-expander');
        expanders.forEach((expander) => {
            updateSublocationSelectAll(expander);
            expander.addEventListener('change', (event) => handleChange(event, expander), false);
        });
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

        updateSublocationSelectAll(expander);
    }

    function updateSublocationSelectAll(expander: Element) {

        //Multiple select all checkboxes due to responsive table
        //TODO replace with single element when checkbox moved into own column
        let selectAllCheckboxes = expander.querySelectorAll('input[type="checkbox"][name^="selectAll"]');
        if (!selectAllCheckboxes) return;

        let checkboxes = expander.querySelectorAll('input[type="checkbox"]:not([name^=selectAll])');
        let totalCount = checkboxes.length;
        let selectedCount = Array.from(checkboxes).filter(cb => (cb as HTMLInputElement).checked).length;

        var isChecked = selectedCount === totalCount;
        var isIndeterminate = !isChecked && selectedCount > 0;

        selectAllCheckboxes.forEach((checkbox) => {
            (checkbox as HTMLInputElement).checked = isChecked;
            (checkbox as HTMLInputElement).indeterminate = isIndeterminate;
            isIndeterminate ? (checkbox as HTMLInputElement).classList.add("nhsuk-checkboxes__input_select-all") :
                (checkbox as HTMLInputElement).classList.remove("nhsuk-checkboxes__input_select-all");
        });
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
    }
})();


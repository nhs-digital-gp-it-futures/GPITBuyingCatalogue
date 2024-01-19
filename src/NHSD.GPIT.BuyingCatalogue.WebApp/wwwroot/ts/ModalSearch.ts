class modalSearchConfig {
    noRecordsFound: HTMLElement;
    dialog: HTMLDialogElement;
    showDialogButton: HTMLElement;
    searchInput: HTMLInputElement;
    applyCallback: string;
    constructor(dialogId, showDialogButtonId, applyCallback) {
        this.dialog = document.getElementById(dialogId) as HTMLDialogElement;
        this.showDialogButton = document.getElementById(showDialogButtonId);
        this.applyCallback = applyCallback;
        this.showDialogButton.addEventListener("click", () => { this.dialog.showModal(); });

        this.dialog.addEventListener('close', () => {
            if (this.dialog.returnValue === 'apply') { window[this.applyCallback]() }
        });

        this.searchInput = document.getElementById(dialogId + "-filter-term") as HTMLInputElement;
        this.noRecordsFound = document.getElementById(dialogId + "-no-records-found");

        this.searchInput.addEventListener("input", () => this.tableSearch(dialogId));
    }

    tableSearch(dialogId) {
        var searchTerm = this.searchInput.value.toLowerCase();
        var tableContainer = document.getElementById(dialogId + "-search-table");
        var table = tableContainer.getElementsByTagName("table")[0];
        var rows = table.getElementsByTagName("tbody")[0].getElementsByTagName("tr");

        let hasMatch = false;

        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];
            const columns = row.getElementsByTagName("td");
            let rowMatch = false;

            for (let j = 0; j < columns.length; j++) {
                const column = columns[j];
                if (column.textContent.toLowerCase().includes(searchTerm)) {
                    rowMatch = true;
                    break;
                }
            }

            if (rowMatch) {
                row.style.display = "";
                hasMatch = true;
            } else {
                row.style.display = "none";
            }
        }

        if (hasMatch) {
            this.noRecordsFound.style.display = "none";
        } else {
            this.noRecordsFound.style.display = "block";
        }
    }
}

class modalSearchConfig {
    noRecordsFound: HTMLElement;
    dialog: HTMLDialogElement;
    showDialogButton: HTMLElement;
    searchInput: HTMLInputElement;
    applyCallback: () => void;
    dialogId: string;
    constructor(dialogId: string,
        showDialogButtonId: string,
        applyCallback: () => void,
        shouldClearSearch: boolean,
        shouldClearSelection: boolean,
        tableContent: () => Promise<string>) {
        this.dialogId = dialogId;
        this.dialog = document.getElementById(dialogId) as HTMLDialogElement;
        this.showDialogButton = document.getElementById(showDialogButtonId);
        this.applyCallback = applyCallback;
        this.searchInput = document.getElementById(dialogId + "-filter-term") as HTMLInputElement;
        this.noRecordsFound = document.getElementById(dialogId + "-no-records-found");

        this.showDialogButton.addEventListener("click", async (event: Event) => {
            if (tableContent != null) {
                var content = await tableContent();
                var tableContainer = document.getElementById(this.dialogId + "-search-table");
                tableContainer.innerHTML = content;
                this.noRecordsFound.style.display = "none";
            }

            this.dialog.showModal();
        });

        this.dialog.addEventListener('close', () => {
            if (this.dialog.returnValue === 'apply') { applyCallback() }
            if (shouldClearSearch) { this.clearSearch() }
            if (shouldClearSelection) { this.clearSelection() }
        });

        this.searchInput.addEventListener("input", () => this.tableSearch());
    }

    tableSearch() {
        var searchTerm = this.searchInput.value.toLowerCase();
        var tableContainer = document.getElementById(this.dialogId + "-search-table");
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

    clearSearch() {
        this.searchInput.value = "";
        this.tableSearch();
    }

    clearSelection() {
        var checkedboxes = document.querySelectorAll('.modal-checkbox:checked');
        checkedboxes.forEach(function (item) {
            (item as HTMLInputElement).checked = false;
        });
    }
}

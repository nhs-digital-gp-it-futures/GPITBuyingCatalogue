class ModalSearchConfig {
    recordsFound: HTMLElement;
    notFoundText: string;
    dialog: HTMLDialogElement;
    showDialogButton: HTMLElement;
    searchInput: HTMLInputElement;
    applyCallback: () => void;
    dialogId: string;
    debounceTime: number = 300;

    constructor(dialogId: string,
        showDialogButtonId: string,
        applyCallback: () => void,
        shouldClearSearch: boolean,
        shouldClearSelection: boolean,
        tableContent: () => Promise<string>,
        notFoundText: string) {
        this.dialogId = dialogId;
        this.dialog = document.getElementById(dialogId) as HTMLDialogElement;
        this.showDialogButton = document.getElementById(showDialogButtonId);
        this.applyCallback = applyCallback;
        this.searchInput = document.getElementById(dialogId + "-filter-term") as HTMLInputElement;
        this.recordsFound = document.getElementById(dialogId + "-records-found");
        this.notFoundText = notFoundText;

        this.showDialogButton.addEventListener("click", async () => {
            if (tableContent != null) {
                const content = await tableContent();
                const tableContainer = document.getElementById(this.dialogId + "-search-table");
                tableContainer.innerHTML = content;
            }

            this.dialog.showModal();
        });

        document.addEventListener("keypress", (event) => this.enterKeyEventListener(event))

        this.dialog.addEventListener('close', () => {
            if (this.dialog.returnValue === 'apply') { applyCallback() }
            if (shouldClearSearch) { this.clearSearch() }
            if (shouldClearSelection) { this.clearSelection() }
            document.removeEventListener('keypress', (event) => this.enterKeyEventListener(event))
        });

        this.searchInput.addEventListener("input", this.debounce(this.tableSearch, this.debounceTime));
    }

    tableSearch() {
        const searchTerm = this.searchInput.value.toLowerCase();
        const tableContainer = document.getElementById(this.dialogId + "-search-table");
        const table = tableContainer.getElementsByTagName("table")[0];
        const rows = table.getElementsByTagName("tbody")[0].getElementsByTagName("tr");

        let matches = 0;

        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];

            let isHeader = row.getElementsByTagName("th").length !== 0;

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
                matches++;
            } else {
                if(!isHeader) {
                    row.style.display = "none";
                }
            }
        }

        this.recordsFound.style.display = "block";

        if (matches > 0) {
            this.recordsFound.innerText = `${matches} ${matches === 1 ? 'result' : 'results'} found`;
        } else {
            this.recordsFound.innerText = this.notFoundText;
        }

        this.hideEmptyHeaders();
    }

    hideEmptyHeaders = () => {
        let allSubGroupElements = document.querySelectorAll('[subgroup]')

        let subGroupSet: Array<string> = [];

        allSubGroupElements.forEach(element => {
            let elementSubgroup = element.getAttribute("subgroup");
            if (!subGroupSet.includes(elementSubgroup)) {
                subGroupSet.push(elementSubgroup);
            }
        })

        subGroupSet.forEach(subGroupName => {
                let spanElementsInSubgroup = document.querySelectorAll(`span[subgroup=${subGroupName}]`);

                let inputElementsInSubgroup = document.querySelectorAll(`input[subgroup=${subGroupName}]`);

                let visibleInputElementsInSubgroup = Array.from(inputElementsInSubgroup).filter((el) => {
                    return el.checkVisibility({checkVisibilityCSS: true});
                });

                let liveElement = document.getElementById(spanElementsInSubgroup[0].id)

                liveElement.parentElement.parentElement.style.display = visibleInputElementsInSubgroup.length === 0 ? "none" : "";
            }
        )
    }

    clearSearch() {
        this.searchInput.value = "";
        this.tableSearch();
    }

    clearSelection() {
        let checkedBoxes = document.querySelectorAll('.modal-checkbox:checked');
        checkedBoxes.forEach(function (item) {
            (item as HTMLInputElement).checked = false;
        });
    }

    enterKeyEventListener(event: KeyboardEvent) {
        if (event.key === "Enter") {
            event.preventDefault();
        }
    }

    debounce(func: () => void, delay: number) {
        let timeout: ReturnType<typeof setTimeout>;

        return () => {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this), delay);
        };
    }

}

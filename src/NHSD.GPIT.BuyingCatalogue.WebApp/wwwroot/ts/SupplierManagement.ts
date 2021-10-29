class SupplierManagement {

    // a variable of type boolean
    checked: boolean;

    SetSupplierEvents = () => {
        const inactiveSupplierCheckboxContainer: string = "inactive-supplier-checkbox";
        const inactiveSupplierCheckbox: string = "ShowInactiveSuppliers";

        document.getElementById(inactiveSupplierCheckboxContainer).style.display = "block";

        ChangeSupplierDisplayStyle("none");

        const checkbox: HTMLElement = document.getElementById(inactiveSupplierCheckbox);

        checkbox.addEventListener("click", this.ChangeSupplierDisplay);
    }

    ChangeSupplierDisplay(): void {

        this.checked ? ChangeSupplierDisplayStyle("table-row") : ChangeSupplierDisplayStyle("none");
    }
}

window.onload = function (): void {
    const supplierManagement: SupplierManagement = new SupplierManagement();
    supplierManagement.SetSupplierEvents();
};

function ChangeSupplierDisplayStyle(style: string): void {
    const inactiveSuppliers: HTMLCollectionOf<Element> = document.getElementsByClassName("inactive");
    for (let i: number = 0; i < inactiveSuppliers.length; i++) {
        const input: HTMLInputElement = inactiveSuppliers.item(i) as HTMLInputElement;
        input.style.display = style;
    }
}

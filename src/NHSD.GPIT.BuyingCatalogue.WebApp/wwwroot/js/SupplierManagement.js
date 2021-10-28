class SupplierManagement {
    constructor() {
        this.SetSupplierEvents = () => {
            const inactiveSupplierCheckboxContainer = "inactive-supplier-checkbox";
            const inactiveSupplierCheckbox = "ShowInactiveSuppliers";
            document.getElementById(inactiveSupplierCheckboxContainer).style.display = "block";
            ChangeSupplierDisplayStyle("none");
            const checkbox = document.getElementById(inactiveSupplierCheckbox);
            checkbox.addEventListener("click", this.ChangeSupplierDisplay);
        };
    }
    ChangeSupplierDisplay() {
        this.checked ? ChangeSupplierDisplayStyle("table-row") : ChangeSupplierDisplayStyle("none");
    }
}
window.onload = function () {
    const supplierManagement = new SupplierManagement();
    supplierManagement.SetSupplierEvents();
};
function ChangeSupplierDisplayStyle(style) {
    const inactiveSuppliers = document.getElementsByClassName("inactive");
    for (let i = 0; i < inactiveSuppliers.length; i++) {
        const input = inactiveSuppliers.item(i);
        input.style.display = style;
    }
}
//# sourceMappingURL=SupplierManagement.js.map
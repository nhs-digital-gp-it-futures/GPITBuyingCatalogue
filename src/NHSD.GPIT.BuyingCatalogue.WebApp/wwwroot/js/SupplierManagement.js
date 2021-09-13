window.onload = function () {
    const inactiveSupplierCheckboxContainer = "inactive-supplier-checkbox";
    const inactiveSupplierCheckbox = "ShowInactiveSuppliers";

    document.getElementById(inactiveSupplierCheckboxContainer).style.display = 'block';

    ChangeSupplierDisplayStyle('none');

    const checkbox = document.getElementById(inactiveSupplierCheckbox);

    checkbox.addEventListener("click", ChangeSupplierDisplay)
}

function ChangeSupplierDisplay() {
    this.checked ? ChangeSupplierDisplayStyle('table-row') : ChangeSupplierDisplayStyle('none');
}

function ChangeSupplierDisplayStyle(style) {
    const inactiveSuppliers = document.getElementsByClassName("inactive");
    for (let i = 0; i < inactiveSuppliers.length; i++) {
        inactiveSuppliers.item(i).style.display = style;
    }
}

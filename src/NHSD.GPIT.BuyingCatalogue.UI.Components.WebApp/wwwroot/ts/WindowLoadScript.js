window.onload = function () {
    var inactiveSupplierCheckboxContainer = "inactive-supplier-checkbox";
    var inactiveSupplierCheckbox = "ShowInactiveSuppliers";
    document.getElementById(inactiveSupplierCheckboxContainer).style.display = 'block';
    ChangeSupplierDisplayStyle('none');
    var checkbox = document.getElementById(inactiveSupplierCheckbox);
    checkbox.addEventListener("click", ChangeSupplierDisplay);
};
function ChangeSupplierDisplay() {
    this.checked ? ChangeSupplierDisplayStyle('table-row') : ChangeSupplierDisplayStyle('none');
}
function ChangeSupplierDisplayStyle(style) {
    var inactiveSuppliers = document.getElementsByClassName("inactive");
    for (var i = 0; i < inactiveSuppliers.length; i++) {
        var input = inactiveSuppliers.item(i);
        input.style.display = style;
    }
}
//# sourceMappingURL=SupplierManagement.js.map
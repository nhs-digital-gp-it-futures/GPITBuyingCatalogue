let targetForm = document.getElementById("sort-form") as HTMLFormElement;

let select = document.getElementById("SelectedSortOption") as HTMLSelectElement;

let button = document.getElementById("Submit") as HTMLButtonElement;

button.remove();

select.addEventListener('change', function () {
    targetForm.submit();
});



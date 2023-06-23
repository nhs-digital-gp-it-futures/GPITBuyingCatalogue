const SORT_BY_KEY = 'sortBy';

window.onload = async () => {
    const currentUrl = new URL(window.location.href);

    const solutionsSortUrl = new URL('/catalogue-solutions/solution-sort', currentUrl);
    const result: string = await fetch(solutionsSortUrl, {method: 'GET'})
        .then(response => {
            if (response.ok) return response.text();

            throw new Error('Response was not successful');
        })
        .catch(() => null);

    if (result == null) return;

    prepareSortOptions(result, currentUrl);
}

function prepareSortOptions(
    innerHtml: string,
    currentUrl: URL) {
    const solutionSortContainer = document.getElementById('solution-sort-container');
    solutionSortContainer.innerHTML = innerHtml;

    const sortOptions = document.getElementById('SelectedSortOption') as HTMLOptionElement;
    const sortByValue = currentUrl.searchParams.get(SORT_BY_KEY);

    if (sortByValue !== null) {
        sortOptions.value = sortByValue;
    }

    sortOptions.addEventListener('change', handleSortChange, true);
}

function handleSortChange(event: Event) {
    const dropdown = event.target as HTMLOptionElement;
    if (dropdown == null) return;

    const currentUrl = new URL(window.location.href);
    currentUrl.searchParams.set(SORT_BY_KEY, dropdown.value);

    window.location.href = currentUrl.toString();
}

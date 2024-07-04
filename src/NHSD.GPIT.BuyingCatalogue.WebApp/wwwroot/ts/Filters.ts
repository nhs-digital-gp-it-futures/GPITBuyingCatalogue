interface CapabilitiesAndEpics {
    capabilities: Map<Number, string[]>,
    updateCapabilities: () => void
}

var filters = (function (): CapabilitiesAndEpics {
    const CHECKBOX_CHECKED_SELECTOR = 'input[type="checkbox"]:checked';
    const FRAMEWORK_ID_PARAM_NAME = 'selectedFrameworkId';
    const APPLICATION_TYPES_PARAM_NAME = 'selectedApplicationTypeIds';
    const HOSTING_TYPES_PARAM_NAME = 'selectedHostingTypeIds';
    const INTEROPERABILITY_OPTIONS_PARAM_NAME = 'selectedIntegrations';
    const SELECTED_PARAM_NAME = 'selected';
    const SORT_BY_PARAM_NAME = 'sortBy';
    const GROUP_DELIMITER = '|';
    const DELIMITER = '.';
    const FRAMEWORK_FILTERS_ID = 'selected-framework-id';
    const APPLICATION_TYPE_FILTERS_ID = 'application-type-filters';
    const HOSTING_TYPE_FILTERS_ID = 'hosting-type-filters';
    const INTEROPERABILITY_FILTERS_ID = 'integration-options';


    class capabilitiesAndEpics implements CapabilitiesAndEpics {
        capabilities: Map<Number, string[]>;

        constructor() {
            const currentUrl = new URL(window.location.href);
            const initialSelected = currentUrl.searchParams.get(SELECTED_PARAM_NAME);
            const capabilityGroups = initialSelected?.split(GROUP_DELIMITER) ?? [];
            this.capabilities = new Map<Number, string[]>(capabilityGroups.map(v => v.split(DELIMITER))
                .filter(v => !isNaN(Number.parseInt(v[0])))
                .map(v => [Number.parseInt(v[0]), v.slice(1)]));
        }

        updateCapabilities(): void {
            this.capabilities = new Map(Array.from<HTMLInputElement>(document.querySelectorAll('#search-capabilities-modal .modal-checkbox:checked'))
                .map(v => document.getElementById(v.id.replace("__Selected", "__Id")) as HTMLInputElement)
                .map(v => Number.parseInt(v.value))
                .map(v => [v, this.capabilities.get(v) ?? []]));

            const capabilityCount = Array.from(this.capabilities.keys()).length;
            const epicsButton = document.getElementById("select-epics-button") as HTMLButtonElement;
            epicsButton.disabled = !(capabilityCount > 0)

            document.getElementById("selected-capabilities-count").textContent = capabilityCount.toString();
            document.getElementById("selected-epics-count").textContent = Array.from(this.capabilities.values())
                .map(v => v ?? [])
                .reduce((t, v) => t + v.length, 0).toString();

            const hiddenInput = document.getElementById("Selected") as HTMLInputElement;
            hiddenInput.value = getSelectedFilterString();

            invalidateResults();
        }

        updateEpics(): void {
            const changes = Array.from<HTMLInputElement>(document.querySelectorAll('#search-epics-modal .modal-checkbox:checked'))
                .map(v => document.getElementById(v.id.replace("__Selected", "__Id")) as HTMLInputElement)
                .map(v => ({
                    CapabilityId: Number.parseInt(v.value.split(",")[0]),
                    EpidId: v.value.split(",")[1],
                }))
                .reduce((map, v) => map.set(v.CapabilityId, [...map.get(v.CapabilityId) ?? [], ...[v.EpidId]]), new Map<Number, string[]>())

            this.capabilities = new Map(Array.from(this.capabilities.keys())
                .map(v => [v, changes.get(v) ?? []]));

            document.getElementById("selected-epics-count").textContent = Array.from(this.capabilities.values())
                .map(v => v ?? [])
                .reduce((t, v) => t + v.length, 0).toString();

            const hiddenInput = document.getElementById("Selected") as HTMLInputElement;
            hiddenInput.value = getSelectedFilterString();

            invalidateResults();
        }

        async getCapabilitiesModelContent(): Promise<string> {
            const currentUrl = new URL(window.location.href);
            const requestUrl = new URL('/catalogue-solutions/filter-capabilities-modal', currentUrl);
            requestUrl.searchParams.set(SELECTED_PARAM_NAME, getSelectedFilterString());
            const result: Response = await fetch(requestUrl, {method: 'GET'});
            if (result.ok) {
                return await result.text();
            } else {
                throw new Error('Request was not successful');
            }
        }

        async getEpicsModelContent(): Promise<string> {
            const currentUrl = new URL(window.location.href);
            const requestUrl = new URL('/catalogue-solutions/filter-epics-modal', currentUrl);
            requestUrl.searchParams.set(SELECTED_PARAM_NAME, getSelectedFilterString());
            const result: Response = await fetch(requestUrl, {method: 'GET'});
            if (result.ok) {
                return await result.text();
            } else {
                throw new Error('Request was not successful');
            }
        }
    }

    const capabilitiesAndEpicsInstance = new capabilitiesAndEpics();

    window.addEventListener('load', async () => {
        const currentUrl = new URL(window.location.href);
        const initialSortByValue = currentUrl.searchParams.get(SORT_BY_PARAM_NAME);
        renderSortBy(await getSortOrderContent(), initialSortByValue);
        const filters = document.querySelectorAll(`#${FRAMEWORK_FILTERS_ID}, #${APPLICATION_TYPE_FILTERS_ID}, #${HOSTING_TYPE_FILTERS_ID}, #${INTEROPERABILITY_FILTERS_ID}`);
        filters.forEach((filter) => filter.addEventListener('change', async (event) => handleFilterChange(event, filter), false));
    });

    async function getSortOrderContent(): Promise<string> {
        const currentUrl = new URL(window.location.href);

        const solutionsSortUrl = new URL('/catalogue-solutions/solution-sort', currentUrl);
        const result: Response = await fetch(solutionsSortUrl, {method: 'GET'});
        if (result.ok) {
            return await result.text();
        } else {
            throw new Error('Request was not successful');
        }
    }

    async function getResultsContent(searchResultsUrl: URL): Promise<string> {
        const result: Response = await fetch(searchResultsUrl, {method: 'GET'})

        if (result.ok) {
            return await result.text();
        } else {
            throw new Error('Request was not successful');
        }
    }

    function renderSortBy(
        innerHtml: string,
        sortByValue: string) {
        const solutionSortContainer = document.getElementById('solution-sort-container');
        if (solutionSortContainer == null)
            return;

        solutionSortContainer.innerHTML = innerHtml;

        const sortOptions = document.getElementById('SelectedSortOption') as HTMLOptionElement;

        if (sortByValue !== null) {
            sortOptions.value = sortByValue;
        }

        sortOptions.addEventListener('change', event => handleSortChange(event), true);
    }

    async function renderResults(
        innerHtml: string,
        currentSortBy: string) {
        const warning = document.getElementById('framework-expired-warning');
        if (warning != null) {
            warning.style.display = "none";
        }

        const textSearch = document.getElementById('marketing-suggestion-search') as HTMLInputElement;
        textSearch.value = "";

        const resultsContainer = document.getElementById('solutions-list');
        resultsContainer.innerHTML = innerHtml;
        var content = await getSortOrderContent();
        renderSortBy(content, currentSortBy);
    }

    function handleSortChange(event: Event) {
        invalidateResults();
    }

    async function handleFilterChange(event: Event, expander: Element) {
        clearHiddenCheckboxes(INTEROPERABILITY_FILTERS_ID);
        invalidateResults();
    }

    async function invalidateResults() {
        const currentUrl = new URL(window.location.href);

        const searchResultsUrl = new URL('/catalogue-solutions/search-results', currentUrl);
        searchResultsUrl.search = currentUrl.search;
        searchResultsUrl.searchParams.set("search", "");
        searchResultsUrl.searchParams.set(FRAMEWORK_ID_PARAM_NAME, getSelectedFramework());
        searchResultsUrl.searchParams.set(APPLICATION_TYPES_PARAM_NAME, getFilterString(APPLICATION_TYPE_FILTERS_ID));
        searchResultsUrl.searchParams.set(HOSTING_TYPES_PARAM_NAME, getFilterString(HOSTING_TYPE_FILTERS_ID));
        searchResultsUrl.searchParams.set(INTEROPERABILITY_OPTIONS_PARAM_NAME, getIntegrations(INTEROPERABILITY_FILTERS_ID));
        searchResultsUrl.searchParams.set(SELECTED_PARAM_NAME, getSelectedFilterString());

        const currentSortBy = getSortBy();
        if (currentSortBy)
            searchResultsUrl.searchParams.set(SORT_BY_PARAM_NAME, currentSortBy);

        await renderResults(await getResultsContent(searchResultsUrl), currentSortBy);
    }

    function getSortBy(): string {
        const sortOptions = document.getElementById('SelectedSortOption') as HTMLOptionElement;
        return sortOptions ? sortOptions.value : null;
    }

    function getSelectedFramework(): string {
        const frameworkFilter = document.getElementById(`${FRAMEWORK_FILTERS_ID}`);

        return Array.from<HTMLInputElement>(frameworkFilter.querySelectorAll('input[type="radio"]:checked'))
            .map(v => v.value)
            .find(v => true) ?? "";
    }

    function clearHiddenCheckboxes(parentElementId: string) {
        const element = document.getElementById(parentElementId);

        Array.from<HTMLInputElement>(element.querySelectorAll(`.nhsuk-checkboxes__conditional--hidden ${CHECKBOX_CHECKED_SELECTOR}`))
            .forEach(c => c.checked = false);
    }

    function getFilterString(parentElementId: string): string {
        const element = document.getElementById(parentElementId);
        let array = Array.from<HTMLInputElement>(element.querySelectorAll(CHECKBOX_CHECKED_SELECTOR));

        return array.map(v => (document.getElementById((v as HTMLInputElement).id.replace("__Selected", "__Value")) as HTMLInputElement).value)
            .reduce((r, i) => `${r}${i}${DELIMITER}`, "") ?? "";
    }

    function getIntegrations(parentElementId: string): string {
        const element = document.getElementById(parentElementId);
        if (!element || element.children.length < 2) throw new Error(`Could not find ${parentElementId} or had no children`);

        const checkboxContainer = element.children.item(1);
        if (!checkboxContainer || checkboxContainer.children.length === 0) throw new Error(`Could not find ${parentElementId} or had no children`);
        const checkboxes = Array.from(checkboxContainer.children);

        const checkboxPairs = checkboxes
            .slice(checkboxes.length / 2)
            .map((_, index) => checkboxes.slice(index *= 2, index + 2))
            .filter(item => item.length === 2);

        return checkboxPairs
            .filter(items => items[0].querySelector(CHECKBOX_CHECKED_SELECTOR))
            .map(items => {
                const parent = items[0].querySelector(CHECKBOX_CHECKED_SELECTOR);
                const nested = Array.from(items[1].querySelectorAll(CHECKBOX_CHECKED_SELECTOR));

                const parentId = items[0].querySelector(`input[id=${parent.id.replace('__Selected', '__Id')}]`) as HTMLInputElement;
                const nestedIds = Array.from<string>(nested.map(element => items[1].querySelector(`#${element.id.replace('__Selected', '__Value')}`)).map(element => (element as HTMLInputElement).value));

                return [parentId.value, ...nestedIds].join(DELIMITER);
            })
            .join(GROUP_DELIMITER);
    }

    function getSelectedFilterString(): string {
        return Array.from(capabilitiesAndEpicsInstance.capabilities.entries())
            .reduce((capabilities, c) => `${capabilities}${c[0]}${epicsFilterString(c[1])}${GROUP_DELIMITER}`, "");

        function epicsFilterString(c: string[]) {
            return c.length > 0
                ? c.reduce((epics, e) => `${epics}${e}${DELIMITER}`, DELIMITER).slice(0, -1)
                : "";
        }
    }

    return capabilitiesAndEpicsInstance;
})();

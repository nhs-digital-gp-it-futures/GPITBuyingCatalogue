interface CapabilitiesAndEpics {
    capabilities: Map<Number, string[]>,
    updateCapabilities: () => void
}

var filters = (function (): CapabilitiesAndEpics {
    const FRAMEWORK_ID_PARAM_NAME = 'selectedFrameworkId';
    const APPLICATION_TYPES_PARAM_NAME = 'selectedApplicationTypeIds';
    const HOSTING_TYPES_PARAM_NAME = 'selectedHostingTypeIds';
    const INTEROPERABILITY_OPTIONS_PARAM_NAME = 'selectedInteroperabilityOptions';
    const IM1_INTEGRATIONS_PARAM_NAME = 'selectedIM1Integrations';
    const GPCONNECT_INTEGRATIONS_PARAM_NAME = 'selectedGPConnectIntegrations';
    const SELECTED_PARAM_NAME = 'selected';
    const SORT_BY_PARAM_NAME = 'sortBy';
    const GROUP_DELIMITER = '|';
    const DELIMITER = '.';
    const FRAMEWORK_FILTERS_ID = 'selected-framework-id';
    const APPLICATION_TYPE_FILTERS_ID = 'application-type-filters';
    const HOSTING_TYPE_FILTERS_ID = 'hosting-type-filters';
    const INTEROPERABILITY_FILTERS_ID = 'interoperability-filters';


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
            const result: Response = await fetch(requestUrl, { method: 'GET' });
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
            const result: Response = await fetch(requestUrl, { method: 'GET' });
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
        const result: Response = await fetch(solutionsSortUrl, { method: 'GET' });
        if (result.ok) {
            return await result.text();
        } else {
            throw new Error('Request was not successful');
        }
    }

    async function getResultsContent(searchResultsUrl: URL): Promise<string> {
        const result: Response = await fetch(searchResultsUrl, { method: 'GET' })

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
        invalidateResults();
    }

    async function invalidateResults() {
        const currentUrl = new URL(window.location.href);

        const searchResultsUrl = new URL('/catalogue-solutions/search-results', currentUrl);
        searchResultsUrl.search = currentUrl.search;
        searchResultsUrl.searchParams.set("search", "");
        searchResultsUrl.searchParams.set(FRAMEWORK_ID_PARAM_NAME, getSelectedFamework());
        searchResultsUrl.searchParams.set(APPLICATION_TYPES_PARAM_NAME, getFilterString(APPLICATION_TYPE_FILTERS_ID));
        searchResultsUrl.searchParams.set(HOSTING_TYPES_PARAM_NAME, getFilterString(HOSTING_TYPE_FILTERS_ID));
        searchResultsUrl.searchParams.set(INTEROPERABILITY_OPTIONS_PARAM_NAME, getFilterString(INTEROPERABILITY_FILTERS_ID, 'InteroperabilityOptions'));
        searchResultsUrl.searchParams.set(IM1_INTEGRATIONS_PARAM_NAME, getFilterString(INTEROPERABILITY_FILTERS_ID, 'IM1IntegrationsOptions', true ));
        searchResultsUrl.searchParams.set(GPCONNECT_INTEGRATIONS_PARAM_NAME, getFilterString(INTEROPERABILITY_FILTERS_ID, 'GPConnectIntegrationsOptions', true));
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

    function getSelectedFamework(): string {
        var frameworkFilter = document.getElementById(`${FRAMEWORK_FILTERS_ID}`);
        return Array.from<HTMLInputElement>(frameworkFilter.querySelectorAll('input[type="radio"]:checked'))
            .map(v => v.value)
            .find(v => true) ?? "";
    }

    function getFilterString(parentElementId: string, checkboxIdStartsWith?: string, excludeHidden?: boolean): string {
        var element = document.getElementById(parentElementId);
        var array = Array.from<HTMLInputElement>(element.querySelectorAll('input[type="checkbox"]:checked'));

        if (checkboxIdStartsWith != null) {
            if (excludeHidden ?? false) {
                var hidden = Array.from<HTMLInputElement>(element.querySelectorAll('.nhsuk-checkboxes__conditional--hidden input[type="checkbox"]:checked'))
                    .map(v => v.id);
                array = array.filter(e => e.id.startsWith(checkboxIdStartsWith) && !hidden.includes(e.id));
            } else {
                array = array.filter(e => e.id.startsWith(checkboxIdStartsWith));
            }
        }

        return array.map(v => (document.getElementById((v as HTMLInputElement).id.replace("__Selected", "__Value")) as HTMLInputElement).value)
            .reduce((r, i) => `${r}${i}${DELIMITER}`, "") ?? "";
    }

    function getSelectedFilterString(): string {
        return Array.from(capabilitiesAndEpicsInstance.capabilities.entries())
            .reduce((capabilties, c) => `${capabilties}${c[0]}${epicsFilterString(c[1])}${GROUP_DELIMITER}`, "");

        function epicsFilterString(c: string[]) {
            return c.length > 0
                ? c.reduce((epics, e) => `${epics}${e}${DELIMITER}`, DELIMITER).slice(0, -1)
                : "";
        }
    }

    return capabilitiesAndEpicsInstance;
})();

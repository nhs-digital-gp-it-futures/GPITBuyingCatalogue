/**
 * https://github.com/alphagov/accessible-autocomplete?tab=readme-ov-file#api-documentation
 */

type SourceFunction = (
    query: string,
    populateResults: (values: string[]) => void
) => string[];

interface AccessibleAutocompleteOptions {
    element: HTMLElement;
    id: string;
    source: string[] | SourceFunction;
    name?: string;
    confirmOnBlur?: boolean;
    onConfirm?(confirmed: string): void;
    cssNamespace?: string;
    defaultValue?: string;
    minLength?: number;
    placeholder?: string;
    templates?: {
        inputValue(selectedSuggestion: string): string;
        suggestion(suggestion: string): string;
    }
}

declare function accessibleAutocomplete(options: AccessibleAutocompleteOptions) : null;

class suggestionSearchConfig {
    modelId: string;
    ajaxUrl: string;
    queryParameterName: string;
    titleText: string;
    placeholderText: string;
    currentPageUrl: string;
    form: HTMLFormElement;
    defaultInput: HTMLElement;
    formInputLabel: HTMLElement;

    constructor(modelId: string,
                ajaxUrl: string,
                queryParameterName: string,
                titleText: string,
                currentPageUrl: string,
                placeholderText: string) {
        this.modelId = modelId;
        this.ajaxUrl = ajaxUrl;
        this.queryParameterName = queryParameterName;
        this.titleText = titleText;
        this.placeholderText = placeholderText;
        this.currentPageUrl = currentPageUrl;
        let form = document.getElementById(modelId.concat("-search-form"));

        if (!(form instanceof HTMLFormElement))
            throw new Error("Expected an HTMLFormElement");

        this.form = form;
        this.defaultInput = document.getElementById(modelId.concat("-form-input"));
        this.formInputLabel = document.getElementById(modelId.concat("-form-input-label"));
    }

    Implement() {
        this.defaultInput.parentNode.removeChild(this.defaultInput);
        this.formInputLabel.removeAttribute("for");
        this.form.firstElementChild.removeAttribute("style");
        accessibleAutocomplete({
            element: document.getElementById(this.modelId.concat("-container")),
            id: this.modelId,
            source: this.source.bind(this),
            name: this.queryParameterName,
            confirmOnBlur: false,
            onConfirm: this.onConfirm.bind(this),
            defaultValue: this.defaultValue(),
            placeholder: this.placeholderText,
            minLength: 2,
            cssNamespace: "suggestion-search",
            templates: {
                inputValue: this.inputValue.bind(this),
                suggestion: this.suggestion.bind(this),
            },
        });
        if (this.form)
            this.addFormEvents();
    }

    source(query: string, populateResults: (values: string[]) => void) {
        const trimmedQuery = query.trim();
        if (trimmedQuery.length < 2) {
            populateResults([]);
            return;
        }

        const url = this.ajaxUrl.concat("?", this.queryParameterName, "=", trimmedQuery);
        const xhr = new XMLHttpRequest();
        xhr.open('GET', url);
        xhr.onload = () => {
            if (xhr.status === 200) {
                const json = JSON.parse(xhr.responseText);
                populateResults(json);
            }
        };
        xhr.send();
    }

    suggestion(result) {
        if (result.title && result.category) {
            const truncateLength = 100;
            const dots = result.title.length > truncateLength ? '...' : '';
            const resultTruncated = result.title.substring(0, truncateLength) + dots;
            return `<span class="suggestion-search__option-title">${resultTruncated}</span>${result.category ? `<span class="suggestion-search__option-category">${result.category}</span>` : ''}`;
        }
        return '';
    }

    inputValue(input) {
        if (input && input.title)
            return input.title.trim();
        return '';
    }

    onConfirm(result) {
        window.location.href = result.url;
    }

    addFormEvents() {
        this.form.addEventListener('keyup', function (keyboardEvent) {
            if (keyboardEvent.key === 'Enter' && document.activeElement.id === this.modelId)
                this.form.submit();
        });
    }

    defaultValue() {
        let searchParams = new URLSearchParams(window.location.search);
        if (searchParams.has(this.queryParameterName)) {
            return searchParams.get(this.queryParameterName);
        }
        return '';
    }
}

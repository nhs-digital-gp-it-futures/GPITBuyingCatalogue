class autocompleteConfig {
    constructor(modelId, ajaxUrl, queryParameterName, titleText, currentPageUrl) {
        this.modelId = modelId;
        this.ajaxUrl = ajaxUrl;
        this.queryParameterName = queryParameterName;
        this.titleText = titleText;
        this.currentPageUrl = currentPageUrl;
        this.form = document.getElementById(modelId.concat("-search-form"));
        this.defaultInput = document.getElementById(modelId.concat("-form-input"));
        this.formInputLabel = document.getElementById(modelId.concat("-form-input-label"));
    }
    Implement() {
        this.defaultInput.parentNode.removeChild(this.defaultInput);
        this.formInputLabel.removeAttribute("for");
        accessibleAutocomplete({
            element: document.getElementById(this.modelId.concat("-container")),
            id: this.modelId,
            source: this.source.bind(this),
            name: this.queryParameterName,
            confirmOnBlur: false,
            onConfirm: this.onConfirm.bind(this),
            defaultValue: this.defaultValue(),
            minLength: 2,
            templates: {
                inputValue: this.inputValue.bind(this),
                suggestion: this.suggestion.bind(this),
            },
        });
        if (this.form)
            this.addFormEvents();
    }
    source(query, populateResults) {
        const url = this.ajaxUrl.concat("?", this.queryParameterName, "=", query);
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
            return `<span class="autocomplete__option-title">${resultTruncated}</span>
						${result.category ? `<span class="autocomplete__option-category">${result.category}</span>` : ''}
					  `;
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
        this.form.addEventListener('keyup', ({ key }) => {
            if (key === 'Enter' && document.activeElement.id === this.modelId)
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

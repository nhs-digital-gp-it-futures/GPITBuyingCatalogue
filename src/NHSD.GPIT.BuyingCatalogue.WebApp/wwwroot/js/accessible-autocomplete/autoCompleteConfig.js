class autoCompleteConfig {
    constructor(modelId) {
        this.modelId = modelId;
    }
    Implement() {
        accessibleAutocomplete.enhanceSelectElement({
            defaultValue: "",
            selectElement: document.querySelector(this.modelId)
        });
    }
}

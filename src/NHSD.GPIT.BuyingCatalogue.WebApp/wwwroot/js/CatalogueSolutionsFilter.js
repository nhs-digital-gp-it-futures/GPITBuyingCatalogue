const filterUrl = "/filter";
const queryCapabilities = "capabilities";
const querySelectedFramework = "selectedframework";
const filterContainer = "filter-container";
const filterCapabilitiesDetails = "filter-capabilities-details";
const filterDetailsText = ".nhsuk-details__text";
const EpicSplitCharacter = "E";
const DFOCVCMarker = "D";
const SupplierSplitCharacter = "S";
const SupplierSolutionCharacter = "X";
const CapabilityMarker = "C";
const InputTypeHidden = "input[type='hidden']";
const NhsukCheckboxesInput = ".nhsuk-checkboxes__input";
const NhsukCheckboxesItem = ".nhsuk-checkboxes__item";
const NhsRadiosInput = ".nhsuk-radios__input";
const CapabilitiesDelimiter = "|";
class CatalogueSolutionsFilter {
    constructor() {
        this.ReplaceFilterAndAddBinders = (html) => {
            const filterForm = "filter-form";
            const SubmitButtonId = "Submit";
            const formContainer = document.getElementById(filterContainer);
            const form = document.getElementById(filterForm);
            form.parentNode.removeChild(form);
            formContainer.innerHTML = html;
            const submitButton = document.getElementById(SubmitButtonId);
            submitButton.removeAttribute("type");
            submitButton.addEventListener("click", this.generateQueryParam);
            document.querySelectorAll(NhsRadiosInput).forEach(item => {
                item.addEventListener("click", this.reload);
            });
            this.RefireDomContentLoadedEvent();
            this.reselectCapabilityAndEpicsFiltersAndFrameworkFilter();
        };
        this.generateQueryParam = () => {
            const queryPage = "page";
            let output = "";
            let query = "#" + filterContainer + " " + NhsukCheckboxesInput;
            document.querySelectorAll(query).forEach(checkbox => {
                const input = checkbox;
                if (input.checked) {
                    let value = null;
                    value = checkbox.parentNode.querySelector(InputTypeHidden).getAttribute("value");
                    if (value.includes(SupplierSplitCharacter)) {
                        output += this.EncodeSupplierDefinedEpic(value);
                    }
                    else if (value.includes(EpicSplitCharacter) && !value.includes(CapabilityMarker)) {
                        output += this.EncodeDFOCVCEpic(value);
                    }
                    else if (value.includes(EpicSplitCharacter) && value.includes(CapabilityMarker)) {
                        output += this.EncodeNormalEpic(value);
                    }
                    else if (output.length > 1) {
                        output += CapabilitiesDelimiter + value;
                    }
                    else {
                        output += value;
                    }
                }
            });
            const url = new URL(window.location.href);
            url.searchParams.delete(queryPage);
            url.searchParams.delete(queryCapabilities);
            url.searchParams.delete(querySelectedFramework);
            if (output.length > 0) {
                url.searchParams.append(queryCapabilities, output);
            }
            url.searchParams.append(querySelectedFramework, this.GetSelectedFramework());
            window.location.href = url.toString();
        };
        this.reload = () => {
            const selectedFramework = this.GetSelectedFramework();
            const targetUrl = new URL(window.location.href);
            targetUrl.pathname += filterUrl;
            targetUrl.searchParams.delete(querySelectedFramework);
            targetUrl.searchParams.append(querySelectedFramework, selectedFramework);
            const xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (xhttp.readyState === 4) {
                    if (xhttp.status === 200) {
                        refreshCapabilitiesAndEpics(xhttp.responseText);
                    }
                    else {
                        console.log("Error: " + xhttp.status);
                    }
                }
            };
        };
        this.reselectCapabilityAndEpicsFiltersAndFrameworkFilter = () => {
            const FoundationCapabilitiesId = "FC";
            const CapabilitiesSplitCharacter = "C";
            const url = new URLSearchParams(window.location.search);
            const capabilities = url.get(queryCapabilities);
            const framework = url.get(querySelectedFramework);
            if (capabilities && capabilities.length > 0) {
                const splitCapabilities = capabilities.split(CapabilitiesDelimiter);
                splitCapabilities.forEach(capability => {
                    if (capability === FoundationCapabilitiesId || !capability.includes(EpicSplitCharacter)) {
                        this.CheckCheckboxWithHiddenInputValue(capability);
                    }
                    else {
                        const epics = capability.split(SupplierSplitCharacter).map(s => s.split(EpicSplitCharacter)).concat.apply([]);
                        epics.forEach(epic => {
                            if (epic.startsWith(CapabilitiesSplitCharacter)) {
                                this.CheckCheckboxWithHiddenInputValue(epic);
                            }
                            else if (epic.includes(DFOCVCMarker)) {
                                this.CheckCheckboxWithHiddenInputValue(this.DecodeDFOCVCEpic(epic));
                            }
                            else if (epic.includes(SupplierSolutionCharacter)) {
                                this.CheckCheckboxWithHiddenInputValue(this.DecodeSupplierDefinedEpic(epic));
                            }
                            else {
                                this.CheckCheckboxWithHiddenInputValue(this.DecodeNormalEpic(epics[0], epic));
                            }
                        });
                    }
                });
            }
            if (framework && framework.length > 0) {
                const selector = NhsRadiosInput + "[value='" + framework + "']";
                const input = document.querySelector(selector);
                input.checked = true;
            }
        };
    }
    CheckCheckboxWithHiddenInputValue(value) {
        const selector = "#" + filterContainer + " " + NhsukCheckboxesItem + " " + InputTypeHidden + "[value='" + value + "']";
        const input = document.querySelector(selector).parentNode.querySelector(NhsukCheckboxesInput);
        input.click();
    }
    GetSelectedFramework() {
        const radioInputs = document.querySelectorAll(NhsRadiosInput);
        let selectedFramework = "All";
        for (let i = 0; i < radioInputs.length; i++) {
            const input = radioInputs[i];
            if (input.checked) {
                selectedFramework = input.value;
                break;
            }
        }
        return selectedFramework;
    }
    RefireDomContentLoadedEvent() {
        window.document.dispatchEvent(new Event("DOMContentLoaded", {
            bubbles: true,
            cancelable: true,
        }));
    }
    EncodeNormalEpic(rawValue) {
        return rawValue.substring(rawValue.indexOf(EpicSplitCharacter));
    }
    EncodeDFOCVCEpic(rawValue) {
        return EpicSplitCharacter + rawValue.substring(4) + DFOCVCMarker;
    }
    EncodeSupplierDefinedEpic(rawValue) {
        return rawValue.replace("E0", "_").replace("S0", "S").replace("X0", "X");
    }
    DecodeNormalEpic(capabilityId, encodedValue) {
        return capabilityId + EpicSplitCharacter + encodedValue;
    }
    DecodeDFOCVCEpic(encodedValue) {
        return "E000" + encodedValue.substring(0, 2);
    }
    DecodeSupplierDefinedEpic(encodedValue) {
        return ("S0" + encodedValue).replace("_", "E0").replace("X", "X0");
    }
}
window.onload = function () {
    var catalgoueSolutionsFilter = new CatalogueSolutionsFilter();
    const xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4) {
            if (xhttp.status === 200) {
                catalgoueSolutionsFilter.ReplaceFilterAndAddBinders(xhttp.responseText);
            }
            else {
                console.log("Error: " + xhttp.status);
            }
        }
    };
    const targetUrl = new URL(window.location.href);
    targetUrl.pathname += filterUrl;
    const url = new URLSearchParams(window.location.search);
    const framework = url.get(querySelectedFramework);
    if (framework && framework.length > 0) {
        targetUrl.searchParams.append(querySelectedFramework, framework);
    }
    xhttp.open("GET", targetUrl.toString(), true);
    xhttp.send();
};
function refreshCapabilitiesAndEpics(html) {
    const filterHtml = new DOMParser().parseFromString(html, "text/html");
    let newCapabilities = null;
    newCapabilities = filterHtml.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);
    let currentCapabilities = null;
    currentCapabilities = document.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);
    currentCapabilities.parentNode.removeChild(currentCapabilities);
    document.getElementById(filterCapabilitiesDetails).appendChild(newCapabilities);
    this.RefireDomContentLoadedEvent();
}
//# sourceMappingURL=CatalogueSolutionsFilter.js.map
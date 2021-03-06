const filterUrl = "/filter";
const queryCapabilities = "capabilities";
const querySelectedFramework = "selectedframework";
const filterContainer = "filter-container";
const filterCapabilitiesDetails = "filter-capabilities-details";
const filterDetailsText = ".nhsuk-details__text";
const EpicSplitCharacter = 'E';
const DFOCVCMarker = 'D';
const SupplierSplitCharacter = 'S';
const SupplierSolutionCharacter = 'X';
const SupplierNewFormatCharacter = 'N';
const CapabilityMarker = 'C';
const InputTypeHidden = "input[type='hidden']";
const NhsukCheckboxesInput = ".nhsuk-checkboxes__input";
const NhsukCheckboxesItem = ".nhsuk-checkboxes__item";
const NhsRadiosInput = ".nhsuk-radios__input";
const CapabilitiesDelimiter = '|';

window.onload = function () {
    const xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4) {
            if (xhttp.status === 200) {
                ReplaceFilterAndAddBinders(xhttp.responseText);
            }
            else {
                console.log('Error: ' + xhttp.status);
            }
        }
    }

    const targetUrl = new URL(window.location.href);

    targetUrl.pathname += filterUrl;

    const url = new URLSearchParams(window.location.search);

    const framework = url.get(querySelectedFramework);

    if (framework && framework.length > 0)
        targetUrl.searchParams.append(querySelectedFramework, framework);

    xhttp.open("GET", targetUrl.toString(), true);

    xhttp.send();
}

function ReplaceFilterAndAddBinders(html) {
    const filterForm = "filter-form";
    const SubmitButtonId = "Submit";

    const form = document.getElementById(filterForm);

    form.innerHTML = html;
    form.onsubmit = (e) => {
        e.preventDefault();
        return false;
    };

    const submitButton = document.getElementById(SubmitButtonId);

    submitButton.removeAttribute("type");
    submitButton.addEventListener('click', generateQueryParam);

    document.querySelectorAll(NhsRadiosInput).forEach(item => {
        item.addEventListener('click', reload);
    });

    RefireDomContentLoadedEvent();

    reselectCapabilityAndEpicsFiltersAndFrameworkFilter();
}

function generateQueryParam() {
    const queryPage = "page";
    let output = "";
    let query = "#" + filterContainer + " " + NhsukCheckboxesInput;

    document.querySelectorAll(query).forEach(checkbox => {
        const input = checkbox as HTMLInputElement;
        if (input.checked) {
            const value = checkbox.parentNode.querySelector(InputTypeHidden).getAttribute("value");

            if (value.includes(SupplierSplitCharacter) && !value.includes(SupplierSolutionCharacter))
                output += EncodeNewSupplierDefinedEpic(value);
            else if (value.includes(SupplierSplitCharacter))
                output += EncodeSupplierDefinedEpic(value);
            else if (value.includes(EpicSplitCharacter) && !value.includes(CapabilityMarker))
                output += EncodeDFOCVCEpic(value);
            else if (value.includes(EpicSplitCharacter) && value.includes(CapabilityMarker))
                output += EncodeNormalEpic(value)
            else
                if (output.length > 1)
                    output += CapabilitiesDelimiter + value;
                else
                    output += value;
        }
    });

    const url = new URL(window.location.href);

    url.searchParams.delete(queryPage);

    url.searchParams.delete(queryCapabilities);

    url.searchParams.delete(querySelectedFramework);

    if (output.length > 0) {
        url.searchParams.append(queryCapabilities, output);
    }

    url.searchParams.append(querySelectedFramework, GetSelectedFramework());

    window.location.href = url.toString();
}

function reload() {
    const selectedFramework = GetSelectedFramework();

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
                console.log('Error: ' + xhttp.status);
            }
        }
    }

    xhttp.open("GET", targetUrl.toString(), true);

    xhttp.send();
}

function refreshCapabilitiesAndEpics(html) {
    const filterHtml = new DOMParser().parseFromString(html, "text/html");

    const newCapabilities = filterHtml.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);

    const currentCapabilities = document.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);

    currentCapabilities.parentNode.removeChild(currentCapabilities);

    document.getElementById(filterCapabilitiesDetails).appendChild(newCapabilities);

    RefireDomContentLoadedEvent();
}

function reselectCapabilityAndEpicsFiltersAndFrameworkFilter() {
    const FoundationCapabilitiesId = "FC";
    const CapabilitiesSplitCharacter = "C";

    const url = new URLSearchParams(window.location.search);

    const capabilities = url.get(queryCapabilities);

    const framework = url.get(querySelectedFramework);

    if (capabilities && capabilities.length > 0) {
        // Rebuild the id's and then click the corresponding checkbox

        const splitCapabilities = capabilities.split(CapabilitiesDelimiter);

        splitCapabilities.forEach(capability => {
            if (capability === FoundationCapabilitiesId || (!capability.includes(EpicSplitCharacter) && !capability.includes(SupplierSplitCharacter))) {
                CheckCheckboxWithHiddenInputValue(capability);
            }
            else {
                const epics = capability.split(SupplierSplitCharacter).map(s => s.split(EpicSplitCharacter)).flat();

                epics.forEach(epic => {
                    let value = "";

                    if (epic.startsWith(CapabilitiesSplitCharacter))
                        value = epic;
                    else if (epic.includes(DFOCVCMarker))
                        value = DecodeDFOCVCEpic(epic);
                    else if (epic.includes(SupplierNewFormatCharacter))
                        value = DecodeNewSupplierDefinedEpic(epic);
                    else if (epic.includes(SupplierSolutionCharacter))
                        value = DecodeSupplierDefinedEpic(epic);
                    else
                        value = DecodeNormalEpic(epics[0], epic);

                    CheckCheckboxWithHiddenInputValue(value);
                });
            }
        });
    }

    if (framework && framework.length > 0) {
        const selector = NhsRadiosInput + "[value='" + framework + "']";
        const input = document.querySelector(selector) as HTMLInputElement;
        input.checked = true;
    }
}

function CheckCheckboxWithHiddenInputValue(value) {
    const selector = "#" + filterContainer + " " + NhsukCheckboxesItem + " " + InputTypeHidden + "[value='" + value + "']";

    const input = document
        .querySelector(selector)
        .parentNode
        .querySelector(NhsukCheckboxesInput) as HTMLInputElement;

    input.click();
}

function GetSelectedFramework() {
    const radioInputs = document.querySelectorAll(NhsRadiosInput);

    let selectedFramework = "All";

    for (let i = 0; i < radioInputs.length; i++) {
        const input = radioInputs[i] as HTMLInputElement;

        if (input.checked) {
            selectedFramework = input.value;
            break;
        }
    }

    return selectedFramework;
}

function RefireDomContentLoadedEvent() {
    window.document.dispatchEvent(new Event("DOMContentLoaded", {
        bubbles: true,
        cancelable: true,
    }));
}

// Epic Id's have 3 Patterns: Normal, DFOCVC and Supplier Defined
//
// Normal is the CapabilityId e.g  "C47" followed by its unique Epic Id e.g "C47E1".
//
// DFOCVC does not include the capability Id it's attached too, but instead starts with "E000",
// so you get key that look like "E00033" or "E00001" (the unique numbers are always 2 numbers e.g "01").
//
// Supplier Defined have two formats, the old and new format.
// The old format has the Supplier Id, the Solution Id then a Unique Id e.g "S042X02E01"
// where "S042" is the Supplier Id, "X01" is the Solution Id and "E01" is the unique key.
//
// The new format has the Supplier identifier and a Unique Id e.g "S00001".
// This format is supplier agnostic as these epics are available globally, throughout all solutions.
//
// Compression Logic for Each Type
//
// Normal just remove the Capability Id, so "C47E1" just becomes "E1", since they're tied to the capability in the URL e.g "C47E1E2E3E4".
//
// DFOCVC epics gain a D on the end of their Id e.g "E00047D" so that it's not recombined with the capability Id server side we then remove 3 of the zeros so "E00047D" would become "E47D".
//
// Supplier Defined (new)
// Leading zeros are stripped preserving just the unique ID, S00001 becomes S1 or S00100 becomes S100.
// An 'N' is appended to the end to denote this as a new Supplier defined Epic.
//
// Supplier Defined (old)
// We strip all prepending zeros from the numbers, so "S042X01E01" becomes "S42X1E1".
// next we change the E to an _ because E is used as a split character for other epics, so "S42X1E1" becomes "S42X1_1".

function EncodeNormalEpic(rawValue) {
    return rawValue.substring(rawValue.indexOf(EpicSplitCharacter));
}

function EncodeDFOCVCEpic(rawValue) {
    return EpicSplitCharacter + rawValue.substring(4) + DFOCVCMarker;
}

function EncodeSupplierDefinedEpic(rawValue) {
    return rawValue.replace("E0", '_').replace("S0", 'S').replace("X0", 'X');
}

function EncodeNewSupplierDefinedEpic(rawValue) {
    return rawValue.replace(/S[0]+/, 'S') + SupplierNewFormatCharacter;
}

function DecodeNormalEpic(capabilityId, encodedValue) {
    return capabilityId + EpicSplitCharacter + encodedValue;
}

function DecodeDFOCVCEpic(encodedValue) {
    return "E000" + encodedValue.substring(0, 2);
}

function DecodeSupplierDefinedEpic(encodedValue) {
    return ("S0" + encodedValue).replace('_', "E0").replace('X', "X0");
}

function DecodeNewSupplierDefinedEpic(encodedValue) {
    return "S" + encodedValue.replace('N', '').padStart(5, '0');
}

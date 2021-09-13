const filterUrl = "/filter";
const queryCapabilities = "capabilities";
const querySelectedFramework = "selectedframework";
const filterContainer = "filter-container";
const filterCapabilitiesDetails = "filter-capabilities-details";
const filterDetailsText = ".nhsuk-details__text";
const EpicSplitCharacter = "E";
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

    const formContainer = document.getElementById(filterContainer);
    const form = document.getElementById(filterForm);

    form.parentNode.removeChild(form);

    formContainer.innerHTML = html;

    const submitButton = document.getElementById(SubmitButtonId);

    submitButton.removeAttribute("type");
    submitButton.addEventListener('click', generateQueryParam);

    document.querySelectorAll(NhsRadiosInput).forEach(item => {
        item.addEventListener('click', reload)
    });

    RefireDomContentLoadedEvent();

    reselectCapabilityAndEpicsFiltersAndFrameworkFilter();
}

function generateQueryParam() {
    const queryPage = "page";
    let output = "";
    let query = "#" + filterContainer + " " + NhsukCheckboxesInput;

    document.querySelectorAll(query).forEach(checkbox => {
        if (checkbox.checked) {
            const value = checkbox.parentNode.querySelector(InputTypeHidden).getAttribute("value");

            if (value.includes(EpicSplitCharacter))
                output += value.substring(value.indexOf(EpicSplitCharacter));
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
            if (capability === FoundationCapabilitiesId || !capability.includes(EpicSplitCharacter)) {
                CheckCheckboxWithHiddenInputValue(capability);
            }
            else {
                const epics = capability.split(EpicSplitCharacter);

                epics.forEach(epic => {
                    if (epic.startsWith(CapabilitiesSplitCharacter))
                        CheckCheckboxWithHiddenInputValue(epic);
                    else
                        CheckCheckboxWithHiddenInputValue(epics[0] + EpicSplitCharacter + epic);
                });
            }
        });
    }

    if (framework && framework.length > 0) {
        const selector = NhsRadiosInput + "[value='" + framework + "']";
        document.querySelector(selector).checked = true;
    }
}

function CheckCheckboxWithHiddenInputValue(value) {
    const selector = "#" + filterContainer + " " + NhsukCheckboxesItem + " " + InputTypeHidden + "[value='" + value + "']";

    document
        .querySelector(selector)
        .parentNode
        .querySelector(NhsukCheckboxesInput).click();
}

function GetSelectedFramework() {
    const radioInputs = document.querySelectorAll(NhsRadiosInput);

    let selectedFramework = "All";

    for (let i = 0; i < radioInputs.length; i++) {
        if (radioInputs[i].checked) {
            selectedFramework = radioInputs[i].value;
            break;
        }
    }

    return selectedFramework;
}

function RefireDomContentLoadedEvent() {
    const DomContentLoaded = "DOMContentLoaded";
    let event; // The custom event that will be created
    if (document.createEvent) {
        event = document.createEvent("HTMLEvents");
        event.initEvent(DomContentLoaded, true, true);
        event.eventName = DomContentLoaded;
        document.dispatchEvent(event);
    } else {
        event = document.createEventObject();
        event.eventName = DomContentLoaded;
        event.eventType = DomContentLoaded;
        document.fireEvent("on" + event.eventType, event);
    }
}

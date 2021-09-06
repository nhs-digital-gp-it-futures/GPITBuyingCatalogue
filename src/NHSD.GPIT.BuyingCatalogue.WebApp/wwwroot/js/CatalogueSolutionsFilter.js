const filterUrl = "/filter";
const queryCapabilities = "capabilities";
const querySelectedFramework = "selectedframework";
const filterContainer = "filter-container";
const filterCapabilitiesDetails = "filter-capabilities-details";
const filterDetailsText = ".nhsuk-details__text";
const EpicSplitCharater = "E";
const InputTypeHidden = "input[type='hidden']";
const NhsukCheckboxesInput = ".nhsuk-checkboxes__input";
const NhsukCheckboxesItem = ".nhsuk-checkboxes__item";
const NhsRadiosInput = ".nhsuk-radios__input";
const CapabilitiesDelimiter = '|';

window.onload = function () {
    let xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {
        if (xhttp.readyState == 4) {
            if (xhttp.status == 200) {
                ReplaceFilterAndAddBinders(xhttp.responseText);
            }
            else {
                console.log('Error: ' + xhttp.status);
            }
        }
    }

    var targetUrl = new URL(window.location.href);

    targetUrl.pathname += filterUrl;

    let url = new URLSearchParams(window.location.search);

    let framework = url.get(querySelectedFramework);

    if (framework != null && framework.length > 0)
        targetUrl.searchParams.append(querySelectedFramework, framework);

    xhttp.open("GET", targetUrl.toString(), true);

    xhttp.send();
};

function ReplaceFilterAndAddBinders(html) {
    const filterForm = "filter-form";
    const SubmitButtonId = "Submit";

    let formContainer = document.getElementById(filterContainer);
    let form = document.getElementById(filterForm);

    form.parentNode.removeChild(form);

    formContainer.innerHTML = html;

    let submitButton = document.getElementById(SubmitButtonId);

    submitButton.removeAttribute("type");
    submitButton.addEventListener('click', generateQueryParam);

    document.querySelectorAll(NhsRadiosInput).forEach(item => {
        item.addEventListener('click', reload)
    });

    RefireDomContentLoadedEvent();

    reselectCapabilityAndEpicsFiltersAndFrameworkFilter();
};

function generateQueryParam() {
    const queryPage = "page";
    let output = "";
    let query = "#" + filterContainer + " " + NhsukCheckboxesInput;

    document.querySelectorAll(query).forEach(checkbox => {
        if (checkbox.checked) {
            let value = checkbox.parentNode.querySelector(InputTypeHidden).getAttribute("value");

            if (value.includes(EpicSplitCharater))
                output += value.substring(value.indexOf(EpicSplitCharater));
            else
                if (output.length > 1)
                    output += CapabilitiesDelimiter + value;
                else
                    output += value;
        }
    });

    let url = new URL(window.location.href);

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
    let selectedFramework = GetSelectedFramework();

    let targetUrl = new URL(window.location.href);

    targetUrl.pathname += filterUrl;

    targetUrl.searchParams.delete(querySelectedFramework);

    targetUrl.searchParams.append(querySelectedFramework, selectedFramework);

    let xhttp = new XMLHttpRequest();

    xhttp.onreadystatechange = function () {
        if (xhttp.readyState == 4) {
            if (xhttp.status == 200) {
                refreshCapabilitesAndEpics(xhttp.responseText);
            }
            else {
                console.log('Error: ' + xhttp.status);
            }
        }
    }

    xhttp.open("GET", targetUrl.toString(), true);

    xhttp.send();
};

function refreshCapabilitesAndEpics(html) {
    let filterHtml = new DOMParser().parseFromString(html, "text/html");

    let newCapabilites = filterHtml.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);

    let currentCapabilites = document.getElementById(filterCapabilitiesDetails).querySelector(filterDetailsText);

    currentCapabilites.parentNode.removeChild(currentCapabilites);

    document.getElementById(filterCapabilitiesDetails).appendChild(newCapabilites);

    RefireDomContentLoadedEvent();
};

function reselectCapabilityAndEpicsFiltersAndFrameworkFilter() {
    const FoundationCapabilitiesId = "FC";
    const CapabilitiesSplitCharacter = "C";

    let url = new URLSearchParams(window.location.search);

    let capabilities = url.get(queryCapabilities);

    let framework = url.get(querySelectedFramework);

    if (capabilities != null && capabilities.length > 0) {
        //rebuild the id's and then click the corresponding checkbox

        var splitCapabilities = capabilities.split(CapabilitiesDelimiter);

        splitCapabilities.forEach(capability => {
            if (capability == FoundationCapabilitiesId || !capability.includes(EpicSplitCharater)) {
                CheckCheckboxWithHiddenInputValue(capability);
            }
            else {
                let epics = capability.split(EpicSplitCharater);

                epics.forEach(epic => {
                    if (epic.startsWith(CapabilitiesSplitCharacter))
                        CheckCheckboxWithHiddenInputValue(epic);
                    else
                        CheckCheckboxWithHiddenInputValue(epics[0] + EpicSplitCharater + epic);
                });
            }
        });
    }

    if (framework != null && framework.length > 0) {
        let selector = NhsRadiosInput + "[value='" + framework + "']";
        document.querySelector(selector).checked = true;
    }
}

function CheckCheckboxWithHiddenInputValue(value) {
    let selector = "#" + filterContainer + " " + NhsukCheckboxesItem + " " + InputTypeHidden + "[value='" + value + "']";

    document
        .querySelector(selector)
        .parentNode
        .querySelector(NhsukCheckboxesInput).click();
}

function GetSelectedFramework() {
    let radioInputs = document.querySelectorAll(NhsRadiosInput);

    let selectedFramework = "All";

    for (var i = 0; i < radioInputs.length; i++) {
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
};

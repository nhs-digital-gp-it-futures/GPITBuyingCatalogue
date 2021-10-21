const filterUrl: string = "/filter";
const queryCapabilities: string = "capabilities";
const querySelectedFramework: string = "selectedframework";
const filterContainer: string = "filter-container";
const filterCapabilitiesDetails: string = "filter-capabilities-details";
const filterDetailsText: string = ".nhsuk-details__text";
const EpicSplitCharacter: string = "E";
const DFOCVCMarker: string = "D";
const SupplierSplitCharacter: string = "S";
const SupplierSolutionCharacter: string = "X";
const CapabilityMarker: string = "C";
const InputTypeHidden: string = "input[type='hidden']";
const NhsukCheckboxesInput: string = ".nhsuk-checkboxes__input";
const NhsukCheckboxesItem: string = ".nhsuk-checkboxes__item";
const NhsRadiosInput: string = ".nhsuk-radios__input";
const CapabilitiesDelimiter: string = "|";

class CatalogueSolutionsFilter {

    // a variable of type HTMLElement
    checked: boolean;

    ReplaceFilterAndAddBinders = (html: string) => {
        const filterForm: string = "filter-form";
        const SubmitButtonId: string = "Submit";

        const formContainer: HTMLElement = document.getElementById(filterContainer)!;
        const form: HTMLElement = document.getElementById(filterForm)!;

        form.parentNode!.removeChild(form);

        formContainer.innerHTML = html;

        const submitButton: HTMLElement = document.getElementById(SubmitButtonId)!;

        submitButton.removeAttribute("type");
        submitButton.addEventListener("click", this.generateQueryParam);

        document.querySelectorAll(NhsRadiosInput).forEach(item => {
            item.addEventListener("click", this.reload);
        });

        this.RefireDomContentLoadedEvent();

        this.reselectCapabilityAndEpicsFiltersAndFrameworkFilter();
    }

    generateQueryParam=() => {
        const queryPage: string = "page";
        let output: string = "";
        let query: string = "#" + filterContainer + " " + NhsukCheckboxesInput;

        document.querySelectorAll(query).forEach(checkbox => {
            const input: HTMLInputElement = checkbox as HTMLInputElement;
            if (input.checked) {

                let value: string = null;
                value = checkbox.parentNode!.querySelector(InputTypeHidden)!.getAttribute("value");

                if (value.includes(SupplierSplitCharacter)) {
                    output += this.EncodeSupplierDefinedEpic(value);
                } else if (value.includes(EpicSplitCharacter) && !value.includes(CapabilityMarker)) {
                    output += this.EncodeDFOCVCEpic(value);
                } else if (value.includes(EpicSplitCharacter) && value.includes(CapabilityMarker)) {
                    output += this.EncodeNormalEpic(value);
                } else if (output.length > 1) {
                        output += CapabilitiesDelimiter + value;
                } else {
                    output += value;
                }
            }
        });

        const url: URL = new URL(window.location.href);

        url.searchParams.delete(queryPage);

        url.searchParams.delete(queryCapabilities);

        url.searchParams.delete(querySelectedFramework);

        if (output.length > 0) {
            url.searchParams.append(queryCapabilities, output);
        }

        url.searchParams.append(querySelectedFramework, this.GetSelectedFramework());

        window.location.href = url.toString();
    }

    reload=()=> {
        const selectedFramework: string = this.GetSelectedFramework();

        const targetUrl: URL = new URL(window.location.href);

        targetUrl.pathname += filterUrl;

        targetUrl.searchParams.delete(querySelectedFramework);

        targetUrl.searchParams.append(querySelectedFramework, selectedFramework);

        const xhttp: XMLHttpRequest = new XMLHttpRequest();

        xhttp.onreadystatechange = function (): void {
            if (xhttp.readyState === 4) {
                if (xhttp.status === 200) {
                    refreshCapabilitiesAndEpics(xhttp.responseText);
                } else {
                    console.log("Error: " + xhttp.status);
                }
            }
        };
    }

    reselectCapabilityAndEpicsFiltersAndFrameworkFilter= () => {
        const FoundationCapabilitiesId: string = "FC";
        const CapabilitiesSplitCharacter: string = "C";

        const url: URLSearchParams = new URLSearchParams(window.location.search);

        const capabilities: string = url.get(queryCapabilities)!;

        const framework: string = url.get(querySelectedFramework)!;

        if (capabilities && capabilities.length > 0) {
            // rebuild the id's and then click the corresponding checkbox

            const splitCapabilities: string[] = capabilities.split(CapabilitiesDelimiter);

            splitCapabilities.forEach(capability => {
                if (capability === FoundationCapabilitiesId || !capability.includes(EpicSplitCharacter)) {
                    this.CheckCheckboxWithHiddenInputValue(capability);
                } else {
                    const epics: string[] = capability.split(SupplierSplitCharacter).map(s => s.split(EpicSplitCharacter)).concat.apply([]);

                    epics.forEach(epic => {
                        if (epic.startsWith(CapabilitiesSplitCharacter)) {
                            this.CheckCheckboxWithHiddenInputValue(epic);
                        } else if (epic.includes(DFOCVCMarker)) {
                            this.CheckCheckboxWithHiddenInputValue(this.DecodeDFOCVCEpic(epic));
                        } else if (epic.includes(SupplierSolutionCharacter)) {
                            this.CheckCheckboxWithHiddenInputValue(this.DecodeSupplierDefinedEpic(epic));
                        } else {
                            this.CheckCheckboxWithHiddenInputValue(this.DecodeNormalEpic(epics[0], epic));
                        }
                    });
                }
            });
        }

        if (framework && framework.length > 0) {
            const selector: string = NhsRadiosInput + "[value='" + framework + "']";
            const input: HTMLInputElement = document.querySelector(selector) as HTMLInputElement;
            input.checked = true;
        }
    }

    CheckCheckboxWithHiddenInputValue(value: string): void {
        const selector: string = "#" + filterContainer + " " + NhsukCheckboxesItem + " " + InputTypeHidden + "[value='" + value + "']";

        const input: HTMLInputElement=document.querySelector(selector)!.parentNode!.querySelector(NhsukCheckboxesInput) as HTMLInputElement;

        input.click();
    }

    GetSelectedFramework(): string {
        const radioInputs: NodeListOf<Element> = document.querySelectorAll(NhsRadiosInput);

        let selectedFramework : string = "All";

        for (let i : number = 0; i < radioInputs.length; i++) {

            const input: HTMLInputElement = radioInputs[i] as HTMLInputElement;

            if (input.checked) {
                selectedFramework = input.value;
                break;
            }
        }

        return selectedFramework;
    }

    RefireDomContentLoadedEvent(): void {
        window.document.dispatchEvent(new Event("DOMContentLoaded", {
            bubbles: true,
            cancelable: true,
        }));
    }


    // Epic Id's have 3 Patterns: Normal, DFOCVC and Supplier Defined
    // Normal is the CapabilityId e.g  "C47" followed by its unique Epic Id e.g "C47E1".
    // DFOCVC does not include the capability Id it's attached too, but instead starts with "E000",
    // so you get key that look like "E00033" or "E00001" (the unique numbers are always 2 numbers e.g "01").
    // Supplier Defined have the Supplier Id, the Solution Id then a Unique Id e.g "S042X02E01"
    // where "S042" is the Supplier Id, "X01" is the Solution Id and "E01" is the unique key.
    /// Compression Logic for Each Type ///
    /// Normal
    // Normal just remove the Capability Id, so "C47E1" just becomes "E1", since they're tied to the capability in the URL e.g "C47E1E2E3E4".
    /// DFOCVC
    // DFOCVC epics gain a D on the end of their Id e.g "E00047D" so that it's not recombined with the capability Id server side
    // we then remove 3 of the zeros so "E00047D" would become "E47D".
    /// Supplier Defined
    // we strip all prepending zeros from the numbers, so "S042X01E01" becomes "S42X1E1".
    // next we change the E to an _ because E is used as a split character for other epics, so "S42X1E1" becomes "S42X1_1".

    EncodeNormalEpic(rawValue: string): string {
        return rawValue.substring(rawValue.indexOf(EpicSplitCharacter));
    }

    EncodeDFOCVCEpic(rawValue: string): string  {
        return EpicSplitCharacter + rawValue.substring(4) + DFOCVCMarker;
    }

    EncodeSupplierDefinedEpic(rawValue: string): string  {
        return rawValue.replace("E0", "_").replace("S0", "S").replace("X0", "X");
    }

    DecodeNormalEpic(capabilityId: string, encodedValue: string): string {
        return capabilityId + EpicSplitCharacter + encodedValue;
    }

    DecodeDFOCVCEpic(encodedValue: string): string {
        return "E000" + encodedValue.substring(0, 2);
    }

    DecodeSupplierDefinedEpic(encodedValue: string): string {
        return ("S0" + encodedValue).replace("_", "E0").replace("X", "X0");
    }
}

window.onload = function (): void {

    var catalgoueSolutionsFilter: CatalogueSolutionsFilter = new CatalogueSolutionsFilter();

    const xhttp: XMLHttpRequest = new XMLHttpRequest();

    xhttp.onreadystatechange = function (): void {
        if (xhttp.readyState === 4) {
            if (xhttp.status === 200) {
                catalgoueSolutionsFilter.ReplaceFilterAndAddBinders(xhttp.responseText);
            } else {
                console.log("Error: " + xhttp.status);
            }
        }
    };

    const targetUrl: URL = new URL(window.location.href);

    targetUrl.pathname += filterUrl;

    const url: URLSearchParams = new URLSearchParams(window.location.search);

    const framework: string = url.get(querySelectedFramework)!;

    if (framework && framework.length > 0) {
        targetUrl.searchParams.append(querySelectedFramework, framework);
    }

    xhttp.open("GET", targetUrl.toString(), true);

    xhttp.send();
};

function refreshCapabilitiesAndEpics(html: string): void {
    const filterHtml: Document = new DOMParser().parseFromString(html, "text/html");

    let newCapabilities: HTMLElement = null;
    newCapabilities = filterHtml.getElementById(filterCapabilitiesDetails)!.querySelector(filterDetailsText);

    let currentCapabilities: HTMLElement = null;
    currentCapabilities = document.getElementById(filterCapabilitiesDetails)!.querySelector(filterDetailsText);

    currentCapabilities.parentNode!.removeChild(currentCapabilities);

    document.getElementById(filterCapabilitiesDetails)!.appendChild(newCapabilities);

    this.RefireDomContentLoadedEvent();
}

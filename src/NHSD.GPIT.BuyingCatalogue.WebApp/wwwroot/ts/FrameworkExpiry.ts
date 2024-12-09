window.addEventListener('load', function () {
    const warningCallout: HTMLElement = document.getElementById('framework-expired-warning');
    if (warningCallout === null) return;

    const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
    container.style.display = 'none';

    const button: HTMLButtonElement = document.createElement('button');
    button.setAttribute('style', 'unset');
    button.setAttribute('class', 'disabled');
    button.textContent = 'Show more';
    button.setAttribute('type', "button");

    button.addEventListener('click', _ => {
        const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
        const isHidden: boolean = container.style.display === 'none';

        if (isHidden) {
            button.textContent = 'Show less';
            container.style.display = null;
        } else {
            container.focus();
            button.textContent = 'Show more';
            container.style.display = 'none';
        }
    });

    warningCallout.appendChild(button);
});

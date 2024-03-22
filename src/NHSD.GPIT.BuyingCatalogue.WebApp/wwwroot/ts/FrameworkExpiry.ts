window.addEventListener('load', function () {
    const warningCallout: HTMLElement = document.getElementById('framework-expired-warning');
    if (warningCallout === null) return;

    const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
    container.style.display = 'none';

    const link: HTMLAnchorElement = document.createElement('a');
    link.text = 'Show more';
    link.href = '#';
    link.style.color = '#000';

    link.addEventListener('click', _ => {
        const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
        const isHidden: boolean = container.style.display === 'none';

        if (isHidden) {
            link.text = 'Show less';
            container.style.display = null;
        } else {
            link.text = 'Show more';
            container.style.display = 'none';
        }
    });

    warningCallout.appendChild(link);
});

window.onload = function () {
    const warningCallout: HTMLElement = document.getElementById('framework-expired-warning');
    const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
    container.style.display = 'none';

    const link = document.createElement('a');
    link.text = 'Show more';
    link.href = '#';
    link.style.color = '#000';

    link.addEventListener('click', _ => {
        const container: HTMLElement = warningCallout.querySelector('div[class="container"]');
        const isHidden = container.style.display === 'none';

        if (isHidden) {
            link.text = 'Show less';
            container.style.display = null;
        } else {
            link.text = 'Show more';
            container.style.display = 'none';
        }
    });

    warningCallout.appendChild(link);
}

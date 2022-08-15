window.onload = function() {
    const button = document.getElementById('Submit');
    const form = document.getElementsByTagName("form")[0];

    if (button && form) {
        button.onclick = function() {
            button.disabled = true;
            form.submit();
        }
    }
};

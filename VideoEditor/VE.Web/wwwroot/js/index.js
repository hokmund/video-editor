$(document).ready(function () {
    $("body").tooltip(
        {
            selector: "[data-toggle=tooltip]",
            placement: "bottom",
            viewport: { selector: "body", padding: 0 }
        }
    );
});
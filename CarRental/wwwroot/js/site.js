// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    let phoneInput = document.getElementById("PhoneNumber");
    let countryCodeSelect = document.getElementById("CountryCode");

    function updatePhoneNumber() {
        if (phoneInput.value.startsWith("+")) {
            phoneInput.value = phoneInput.value.replace(/^\+\d+\s*/, "");
        }
        phoneInput.value = countryCodeSelect.value + " " + phoneInput.value;
    }

    countryCodeSelect.addEventListener("change", updatePhoneNumber);
});

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Added an over simplified example of dynamic form validation via Javascript using first name and last name as candidates
// The name fields are evaluated upon change, which are used to infer the disabled state of the submit button
// In this simple case, if both name fields contain zero length text then the submit button is enabled
// In reality all fields would need to be evaluated with varying levels of constraints being applied
// The date field should always be a valid date with the pre-requisite formatting applied
// The project type should be a known type or at least a non zero length value
// The hours worked should be 24 or less and non negative value

const date = document.querySelector("#date");
const firstName = document.querySelector("#firstName");
const lastName = document.querySelector("#lastName");
const project = document.querySelector("#project");
const hoursWorked = document.querySelector("#hoursWorked");
const submitButton = document.querySelector('#submitTimesheet');

function validateForm() {
    submitButton.disabled = (firstName.value.length == 0) || (lastName.value.length == 0);
}

firstName.addEventListener("input", onFirstNameChange);

function onFirstNameChange() {
    validateForm();
}

lastName.addEventListener("input", function (event) {
    validateForm();
});

function resetForm() {
    date.value = '';
    firstName.value = '';
    lastName.value = '';
    project.value = '';
    hoursWorked.value = '';
    submitButton.disabled = true;
}

window.onload = function (event) {
    resetForm();
};

// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const deleteButtons = document.querySelectorAll('.btn-danger[data-bs-toggle="modal"]');

deleteButtons.forEach(button => {
    button.addEventListener('click', function () {
        const userId = this.getAttribute('data-id');
        const userName = this.getAttribute('data-name');

        document.getElementById('userName').textContent = userName;


        document.getElementById('confirmDelete').onclick = function () {
            window.location.href = `/User/Delete/${userId}`; 
        };
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const overlay = document.querySelector(".overlay-div");
    const registerBtn = overlay.querySelector(".move-button");

    registerBtn.addEventListener("click", function () {
        overlay.classList.toggle("active");

        registerBtn.style.opacity = "0";
        setTimeout(() => {
            registerBtn.innerText = overlay.classList.contains("active") ? "Login" : "Register";
            registerBtn.style.opacity = "1";
        }, 500);
    });
});



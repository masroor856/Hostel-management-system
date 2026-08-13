window.addEventListener("scroll", function () {

    const navbar = document.querySelector(".account-navbar");

    if (!navbar) return;

    if (window.scrollY > 50) {

        navbar.classList.add("scrolled");

    } else {

        navbar.classList.remove("scrolled");

    }

});
/*=================
ACCOUNT MOBILE MENU
==============================*/

const menuToggle = document.getElementById("menuToggle");

const accountMenu = document.getElementById("accountMenu");

if (menuToggle && accountMenu) {

    menuToggle.addEventListener("click", function () {

        accountMenu.classList.toggle("active");

    });

}
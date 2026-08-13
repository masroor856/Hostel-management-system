/*====================================================
HOSTEL MANAGEMENT SYSTEM
SITE.JS
====================================================*/

/*==============================
FAQ ACCORDION
==============================*/

document.querySelectorAll(".faq-question").forEach(question => {

    question.addEventListener("click", function () {

        const item = this.parentElement;

        document.querySelectorAll(".faq-item").forEach(faq => {

            if (faq !== item) {

                faq.classList.remove("active");

            }

        });

        item.classList.toggle("active");

    });

});


/*==============================
SCROLL REVEAL
==============================*/

const revealElements = document.querySelectorAll(
    ".reveal,.reveal-left,.reveal-right,.reveal-scale,reveal-up,.reveal-down"
);

function revealOnScroll() {

    const trigger = window.innerHeight - 120;

    revealElements.forEach(el => {

        if (el.getBoundingClientRect().top < trigger) {

            el.classList.add("active");

        }

    });

}

window.addEventListener("scroll", revealOnScroll);

revealOnScroll();


/*==============================
COUNTER ANIMATION
==============================*/

let counterStarted = false;

function animateCounters() {

    if (counterStarted) return;

    const section = document.querySelector(".statistics");

    if (!section) return;

    if (section.getBoundingClientRect().top < window.innerHeight - 100) {

        counterStarted = true;

        document.querySelectorAll(".counter").forEach(counter => {

            const target = parseInt(counter.dataset.target);

            let value = 0;

            const speed = target / 80;

            function updateCounter() {

                value += speed;

                if (value < target) {

                    counter.innerHTML = Math.ceil(value);

                    requestAnimationFrame(updateCounter);

                } else {

                    if (target == 98) {

                        counter.innerHTML = "98%";

                    }

                    else if (target == 24) {

                        counter.innerHTML = "24/7";

                    }

                    else {

                        counter.innerHTML = target + "+";

                    }

                }

            }

            updateCounter();

        });

    }

}

window.addEventListener("scroll", animateCounters);

animateCounters();


/*==============================
BACK TO TOP
==============================*/

const backTop = document.getElementById("backToTop");

if (backTop) {

    window.addEventListener("scroll", function () {

        if (window.scrollY > 500) {

            backTop.classList.add("show");

        }

        else {

            backTop.classList.remove("show");

        }

    });

    backTop.addEventListener("click", function () {

        window.scrollTo({

            top: 0,

            behavior: "smooth"

        });

    });

}


/*==============================
PARALLAX HERO
==============================*/

const hero = document.querySelector(".hero");

if (hero) {

    window.addEventListener("scroll", function () {

        hero.style.backgroundPositionY =

            -(window.pageYOffset * 0.25) + "px";

    });

}


/*==============================
SMOOTH SCROLL
==============================*/

document.querySelectorAll('a[href^="#"]').forEach(anchor => {

    anchor.addEventListener("click", function (e) {

        const target = document.querySelector(this.getAttribute("href"));

        if (target) {

            e.preventDefault();

            target.scrollIntoView({

                behavior: "smooth"

            });

        }

    });

});


/*==============================
NAVBAR SHRINK
==============================*/

const navbar = document.querySelector(".navbar");

if (navbar) {

    window.addEventListener("scroll", function () {

        if (window.scrollY > 80) {

            navbar.classList.add("scrolled");

        }

        else {

            navbar.classList.remove("scrolled");

        }

    });

}


/*=========================================
PREMIUM PAGE LOADER
=========================================*/

(function () {

    const loader =
        document.getElementById("page-loader");

    if (!loader) return;

    const message =
        document.getElementById("loader-message");

    const messages = [

        "Preparing Hostel Management System...",

        "Loading Student Records...",

        "Checking Hostel Availability...",

        "Connecting Secure Services...",

        "Loading Dashboard...",

        "Loading Resources...",

        "Almost Ready..."

    ];

    let messageIndex = 0;

    /*
     * Change loader messages
     */
    if (message) {

        message.textContent =
            messages[0];

        const messageTimer =
            setInterval(function () {

                messageIndex++;

                if (
                    messageIndex >=
                    messages.length
                ) {

                    clearInterval(messageTimer);

                    return;

                }

                message.textContent =
                    messages[messageIndex];

            }, 300);

    }

    /*
     * ALWAYS hide the loader.
     */
    function hideLoader() {

        loader.classList.add("hide");

        /*
         * Completely remove it after
         * the fade animation.
         */
        setTimeout(function () {

            loader.style.display = "none";

        }, 900);

    }

    /*
     * Normal page load
     */
    window.addEventListener(
        "load",
        function () {

            setTimeout(
                hideLoader,
                1800
            );

        }
    );

    /*
     * Safety fallback.
     *
     * If something prevents load from
     * firing, the page will still become
     * visible.
     */
    setTimeout(
        hideLoader,
        4000
    );

})();


/*=========================================
PAGE TRANSITION
=========================================*/

const transition = document.getElementById("page-transition");

if (transition) {

    document.querySelectorAll("a").forEach(link => {

        link.addEventListener("click", function (e) {

            const href = this.getAttribute("href");

            /*
             * Ignore empty links
             */
            if (!href) return;

            /*
             * Ignore hash/section links
             */
            if (href.startsWith("#")) return;

            /*
             * Ignore javascript links
             */
            if (href.startsWith("javascript:")) return;

            /*
             * Ignore new-tab links
             */
            if (this.target === "_blank") return;

            /*
             * Ignore download links
             */
            if (this.hasAttribute("download")) return;

            /*
             * Same-page navigation with a fragment
             * should NOT activate the page transition.
             */
            const currentPath =
                window.location.pathname;

            const url =
                new URL(href, window.location.origin);

            if (
                url.pathname === currentPath &&
                url.hash
            ) {

                return;

            }

            /*
             * Show transition briefly
             */
            transition.classList.add("active");

        });

    });

}
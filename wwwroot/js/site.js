let menuBtn = document.querySelector('.customNavMobileMenuBtn');
menuBtn.addEventListener('click', () => {

    menuBtn.classList.toggle('customNavMobileMenuBtnChangeShapeToClose');
    let header = document.querySelector('header');
    header.classList.toggle('toggleMobileMenu');
    header.style.display = "unset";
});
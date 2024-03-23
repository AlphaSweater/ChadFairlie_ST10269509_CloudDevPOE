
/* Reference: https://codepen.io/GoostCreative/pen/jOawZbZ */
$(document).ready(function () {
    $('.sidebar').hover(
        function () {
            $(this).removeClass('close');
            $('body').addClass('shift');
        },
        function () {
            $(this).addClass('close');
            $('body').removeClass('shift');
        }
    );

    $('.nav-links li').click(function () {
        $(this).find('i.arrow').toggleClass('rotate');
        $(this).children('.sub-menu').slideToggle();
    });
});
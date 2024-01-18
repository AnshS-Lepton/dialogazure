
$(function () {

    $(document).on('click', "#dvNav > ul > li > a", function (e) {

        $(this).find('.addon').toggleClass('icon-Hide_Menu').toggleClass('icon-Open_Menu');
        //$(this).closest('li').find('span.hasDrop').toggleClass('icomoon-icon-arrow-down-2').toggleClass('icomoon-icon-arrow-up-2');
        $(this).closest('li').find('.sidenav-second-level').slideToggle();
    });

    $('#sidenavToggler').on('click', function () {
        if ($('.sidenav').width() == 250) {
            $('.sidenav').animate({ 'width': '53px' }, 'slow', function () { $('.content-wrapper').css('width', '96.6%'), 'slow' });
            $('.sidenav-toggler').animate({ 'margin-left': '35px' }, 'slow');
            $('#sidenavToggler').addClass('fa-arrow-right');
            $('#sidenavToggler').removeClass('fa-arrow-left');
            $('.navbar-sidenav > li > a > span.nav-link-text').hide();
            $('#dvNav > ul > li > ul.sidenav-second-level').hide();
        }
        else {

            $('.sidenav').animate({ 'width': '250px' }, 'slow', function () {
                $('.content-wrapper').css('width', 'calc(100% - 250px)'), 'slow';
                $('.sidenav').show();
            });
            $('.sidenav-toggler').animate({ 'margin-left': '235px' }, 'slow');
            $('#sidenavToggler').addClass('fa-arrow-left');
            $('#sidenavToggler').removeClass('fa-arrow-right');
            $('.navbar-sidenav  > li > a > span.nav-link-text').show();
        }
    });

});

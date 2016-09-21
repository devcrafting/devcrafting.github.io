$(function() {
    var url = window.location;
    $('.nav li a').each(function(){
        if (url.href.indexOf(this.href) == 0)
        {
            $(this).parent().addClass('active');
        }
    });
});
/*
 * Site-wide JS, with utilities, global definitions and Layout specific code
 */

$(document).ready(function () {

    // Toggle the sidebar state on sidebar-toggle click
    $('.sidebar-toggle').on('click', toggleSidebar);

    // To toggle the state of the sidebar on mouse enter or leave, but it doesn't work well enough on mobile to be acceptable
    //$('#sidebar').on('mouseenter', toggleSidebar);
    //$('#sidebar').on('mouseleave', toggleSidebar);
});


/** Toggle the status of the Sidebar between Open and Closed */
function toggleSidebar() {
    let sd = $('#sidebar');

    if (sd.hasClass('closed')) {
        // without this it looks weird
        sd.addClass('pre-open');

        setTimeout(function () {
            sd.toggleClass('pre-open');
            sd.toggleClass('closed');
            updateSdStatus();
        }, 120);

    }
    else {
        sd.toggleClass('closed');
        updateSdStatus();
    }
}

/** Update the cookie that store the status of the sidebar */
function updateSdStatus() {
    var status = $('#sidebar').hasClass('closed') ? 'closed' : 'open';
    //Cookies.set('sdStatus', status);
}

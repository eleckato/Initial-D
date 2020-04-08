$(document).ready(function () {

    // Toggle the sidebar state on sidebar-toggle click
    $('.sidebar-toggle').on('click', toggleSidebar);

    // To toggle the state of the sidebar on mouse enter or leave, but it doesn't work well enough on mobile to be acceptable
    //$('#sidebar').on('mouseenter', toggleSidebar);
    //$('#sidebar').on('mouseleave', toggleSidebar);
});


/** Togle the status of the Sidebar between Open and Closed */
function toggleSidebar() {
    let sd = $('#sidebar');

    if (sd.hasClass('closed')) {
        // whithout this it looks weird
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
    Cookies.set('sdStatus', status);
}


/** Open an alert with a message and a color palette. Optionally, it can automatically hide in an amount of seconds.
 * @param {string} message The message to show
 * @param {string} palette_class The color scheme to use, all bootstrap main alert-xxxx are cool
 * @param {int} duration Optional int that define the duration in seconds */
function ShowAlert(message, palette_class, duration) {
    // Transform duration in s to ms
    duration = duration * 1000;

    // Make the base element to hold the alert html
    const div = document.createElement('div');
    
    // Not the best thing ever but ok
    var alert_html =
        '<div class="alert alert-dismissible fade show ' + palette_class + '" role="alert"> ' +
        message +
        '<button type="button" class="close" data-dismiss="alert" aria-label="Close"> ' +
        '<span aria-hidden="true">&times;</span> ' +
        '</button> ' +
        '</div> ';

    // Put the alert inside the div container
    div.innerHTML = alert_html;

    // Append that div to the #alerts dom element
    $('#alerts').append(div);

    // Automatically hides the alert in [duration] seconds. This is optional.
    if (duration != null && duration > 0) {
        $(div).fadeTo(duration, 1).slideUp(500, function () {
            $(div).slideUp(500);
        });
    }
}
/** When the user clicks on a cell inside a .clickable-row, it redirect them to the data-url of said row.
 * Use .not-clickable on a cell to exclude it from the event */
$(document).on('click', '.clickable-row td:not(.not-clickable)', function () {
    // Get the row
    let row = $(this).closest('.clickable-row');
    // Assemble the URL
    let url = row.data('url');
    // Redirect the window
    window.location = url;
});
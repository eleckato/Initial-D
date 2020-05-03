WaitForJquery(function () {
    // Format the exposed textbox at load time
    $(document).ready(function () {
        $('.money-editor').each(function (i, editor) {

            let ogEditorId = $(editor).data('og-editor');
            let ogEditor = $('#' + ogEditorId);

            let val = ogEditor.val();

            let final = new Intl.NumberFormat('es-CL', { style: 'currency', currency: 'CLP' }).format(val);

            $(editor).val(final);
        });
    });

    // Format the exposed textbox when the user un-focus it
    $(document).on('focusout', '.money-editor', function () {
        let ogEditorId = $(this).data('og-editor');
        let ogEditor = $('#' + ogEditorId);

        let val = $(this).val();
        let final = new Intl.NumberFormat('es-CL', { style: 'currency', currency: 'CLP' }).format(val);

        $(this).val(final);
        ogEditor.val(val);
    });

    // Un-format the exposed textbox when the user focus it
    $(document).on('focusin', '.money-editor', function () {
        let ogEditorId = $(this).data('og-editor');
        let ogEditor = $('#' + ogEditorId);

        let val = ogEditor.val();
        $(this).val(val);
    });

    // Delete all non-number characters on user input
    $(document).on('input', '.money-editor', function () {
        let oldVal = $(this).val();

        oldVal = oldVal.replace(/\D/g, '');

        $(this).val(oldVal);
    });
});
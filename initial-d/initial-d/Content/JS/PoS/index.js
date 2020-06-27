/* JS para Point Of Sale Module - Vista Index*/

WaitForJquery(function () {

    // En Modal:add-element Selecciona una fila al hacerle click
    function selectRow(inRow) {
        let row = $(inRow);
        let modal = row.closest('.modal-content');

        let otherRows = row.closest('tbody').find('tr');
        $.each(otherRows, function (i, row) {
            $(row).removeClass('row-active');
        });

        row.addClass('row-active');

        let submitBnt = modal.find('button[type="submit"]');
        submitBnt.prop('disabled', false)

        modal.find('input[name="sell-quantity"]').val("1");
        modal.find('input[name="sell-quantity"]').prop("disabled", false);

        let stock = row.data('stock');
        modal.find('input[name="sell-quantity"]').data("stock", stock);
    }
    $(document).on('click', '.modal-add-item-row', function () { selectRow(this) })


    // Resetea el Modal:add-prod y Modal:add-serv
    function resetItemModal(inModal) {
        let modal = $(inModal);

        modal.find('tbody tr').removeClass('row-active');
        modal.find('button[type="submit"]').prop('disabled', true);
        modal.find('input[name="sell-quantity"]').val("");
        modal.find('input[name="sell-quantity"]').prop("disabled", true);
    }
    $(document).on('show.bs.modal', '#add-prod', function () { resetItemModal(this) });
    $(document).on('show.bs.modal', '#add-serv', function () { resetItemModal(this) });
    $(document).on('show.bs.modal', '#add-user', function () { resetItemModal(this) });

    // Llena la lista de productos sacando los items en el listado y preguntando a la API
    $(document).on('show.bs.modal', '#add-prod', function () {
        console.log('> Add Product Clicked')


        let cont = $('#add-prod .modal-body');
        let saleId = $('#sale-id').val();

        console.log({ "cont": cont, "saleId": saleId });

        let url = URL_GetAllProdList;
        var data = {
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = '<div class="px-5"><h4>Error comunicándose con el servidor</h4></div>';

        // Load HTML into the #item-list
        var res = cont.load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    });
    // Llena la lista de servicios sacando los items en el listado y preguntando a la API
    $(document).on('show.bs.modal', '#add-serv', function () {
        console.log('> Add Service Clicked')


        let cont = $('#add-serv .modal-body');
        let saleId = $('#sale-id').val();

        console.log({ "cont": cont, "saleId": saleId });

        let url = URL_GetAllServList;
        var data = {
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = '<div class="px-5"><h4>Error comunicándose con el servidor</h4></div>';

        // Load HTML into the #item-list
        var res = cont.load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    });


    // Agrega el producto seleccionado en modal #add-prod a la venta en Session y actualiza la vista
    $(document).on('click', '#add-prod button[type=submit]', function () {
        // Get modal
        let modal = $(this).closest('#add-prod');
        // Get selected row in the Modal
        let selectedRow = modal.find('tr.row-active');

        // Get data needed for request
        let quantity = modal.find('input[name=sell-quantity]').val();
        let prodId = selectedRow.data('id');
        let saleId = $('#sale-id').val();

        console.log("REQUEST AddProd")
        console.log({ prodId, quantity, saleId });

        // Check for data integrity
        if (!prodId || !quantity || !saleId) {
            console.log('-------- ERROR WITH REQUEST DATA! (ↀДↀ) --------');
            console.log('One or more data- fields are falseys')

            return;
        }

        // Get the URL and define the data for the POST
        var url = URL_AddProd;
        var data = {
            "prodId": prodId,
            "quantity": quantity,
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();

        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

        // Hide the #add-prod modal
        $('#add-prod').modal('hide');

    });

    // Agrega el producto seleccionado en modal #add-prod a la venta en Session y actualiza la vista
    $(document).on('click', '#add-serv button[type=submit]', function () {
        console.log("REQUEST AddServ");

        // Get modal
        let modal = $(this).closest('#add-serv');
        // Get selected row in the Modal
        let selectedRow = modal.find('tr.row-active');

        // Get data needed for request
        let servId = selectedRow.data('id');
        let saleId = $('#sale-id').val();

        console.log({ servId, saleId });

        // Check for data integrity
        if (!servId || !saleId) {
            console.log('-------- ERROR WITH REQUEST DATA! (ↀДↀ) --------');
            console.log('One or more data- fields are falseys')

            return;
        }

        // Get the URL and define the data for the POST
        var url = URL_AddServ;
        var data = {
            "servId": servId,
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();

        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

        // Hide the #add-prod modal
        $('#add-serv').modal('hide');

    });

    // Cambia la cantidad de un producto en Session y actualiza la vista
    $(document).on('click', '.amount-btn', function () {
        console.log("REQUEST Change amount");

        // Get row
        let row = $(this).closest('tr');

        let isPlus = $(this).data('plus');
        let itemId = row.data('id');
        let saleId = $('#sale-id').val();

        console.log({ isPlus, itemId, saleId });

        // Check for data integrity
        if (!itemId || !saleId) {
            console.log('-------- ERROR WITH REQUEST DATA! (ↀДↀ) --------');
            console.log('One or more data- fields are falseys')

            return;
        }

        // Get the URL and define the data for the POST
        var url = URL_ChangeAmount;
        var data = {
            "isPlus": isPlus,
            "itemId": itemId,
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();


        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    })


    // Cambia el Id del elemento a borrar por el modal #delete-item-modal
    $(document).on('show.bs.modal', '#delete-item-modal', function (event) {
        console.log('> Delete item clicked')

        // Button that triggered the modal
        var button = $(event.relatedTarget)
        // Get the itemId from the button
        let itemId = button.data('id');

        console.log({ "button": button, "itemId": itemId });

        // Change the itemId in the confirm delete button
        $('#modal-delete-btn').data('id', itemId);

        console.log($('#modal-delete-btn').data('id'));
    });

    // Elimina un item de la venta en Session y actualiza la vista
    $(document).on('click', '#modal-delete-btn', function () {
        console.log('> Confirm delete item clicked')


        let itemId = $(this).data('id');
        let saleId = $('#sale-id').val();


        console.log({ "itemId": itemId, "saleId": saleId });

        // Check for data integrity
        if (!itemId || !saleId) {
            console.log('-------- ERROR WITH REQUEST DATA! (ↀДↀ) --------');
            console.log('One or more data- fields are falseys')

            return;
        }

        // Get the URL and define the data for the POST
        var url = URL_DeleteItem;
        var data = {
            "itemId": itemId,
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();

        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    });


    // Cambia el contenido del modal #details-item-modal con los datos específicos del item seleccionado
    $(document).on('show.bs.modal', '#details-item-modal', function (event) {
        console.log('> Details item clicked')

        // Button that triggered the modal
        var button = $(event.relatedTarget);
        // Get the itemId from the button
        let itemId = button.data('id');
        let saleId = $('#sale-id').val();

        console.log({ "button": button, "itemId": itemId });

        let body = $(this).find('.modal-body');

        let url = URL_GetDetailHtml;
        var data = {
            "itemId": itemId,
            "saleId": saleId
        }

        console.log("DATA");
        console.log(data);

        // #item-list HTML backup
        let backup_item_list = '<div class="px-5"><h4>Error comunicándose con el servidor</h4></div>';

        // Load HTML into the #item-list
        var res = body.load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    });


    // Llena la lista de servicios sacando los items en el listado y preguntando a la API
    $(document).on('show.bs.modal', '#add-user', function () {
        console.log('> Add User Clicked')


        let cont = $('#add-user .modal-body');
        let saleId = $('#sale-id').val();

        console.log({ "cont": cont, "saleId": saleId });

        let url = URL_GetAllUserList;
        var data = {
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = '<div class="px-5"><h4>Error comunicándose con el servidor</h4></div>';

        // Load HTML into the #item-list
        var res = cont.load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

    });

    // Agrega el producto seleccionado en modal #add-prod a la venta en Session y actualiza la vista
    $(document).on('click', '#add-user button[type=submit]', function () {
        console.log("REQUEST Add User");

        // Get modal
        let modal = $(this).closest('#add-user');
        // Get selected row in the Modal
        let selectedRow = modal.find('tr.row-active');

        // Get data needed for request
        let userId = selectedRow.data('id');
        let saleId = $('#sale-id').val();

        console.log({ userId, saleId });

        // Check for data integrity
        if (!userId || !saleId) {
            console.log('-------- ERROR WITH REQUEST DATA! (ↀДↀ) --------');
            console.log('One or more data- fields are falseys')

            return;
        }

        // Get the URL and define the data for the POST
        var url = URL_AddUser;
        var data = {
            "userId": userId,
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();

        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

        // Hide the #add-user modal
        $('#add-user').modal('hide');
    });

    // Elimina el producto
    $(document).on('click', '#remove-user-modal button[type=submit]', function () {
        console.log("REQUEST Remove User");

        // Get data needed for request
        let saleId = $('#sale-id').val();

        // Get the URL and define the data for the POST
        var url = URL_RemoveUser;
        var data = {
            "saleId": saleId
        }

        // #item-list HTML backup
        let backup_item_list = $('#item-list').html();

        // Load HTML into the #item-list
        var res = $('#item-list').load(url, data, function (responseText, textStatus, jqXHR) {
            checkHtmlLoad(responseText, textStatus, res, backup_item_list);
        });

        // Hide the #add-user modal
        $('#remove-user-modal').modal('hide');
    });

    // Cuando cambia la textbox para seleccionar la cantidad del producto en el modal, no permite números menores a 1
    $(document).on('input', '#select-prod-sell-quantity', function () {
        console.log('> select-prod-sell-quantity CHANGED');

        let input = $(this);

        let val = input.val();

        if (val != "" && val <= 0) {
            input.val(1);
        }

        let stock = input.data('stock');

        console.log({ 'stock': stock });

        if (val > stock) {
            input.val(stock);
        }
    });


    $(document).on('click', '#prod-modal-search-submit', searchProd);
    $(document).on('keypress', '#prod-modal-search-txtbox' , function (e) {
        if (e.which == 13) {
            searchProd();
        }
    });

    // Filtra la lista de productos para que sean solo 
    function searchProd() {
        let search = $('#prod-modal-search-txtbox').val();
        var regexObj = new RegExp(search, 'i');

        let allRows = $('#prod-data-list').find('table tbody tr');

        $.each(allRows, function (i, row) {
            let r = $(row);

            let name = r.data('name');

            if (!name.match(regexObj)) {
                r.hide();
            }
            else {
                r.show();
            }
        });
    }


    $(document).on('click', '#serv-modal-search-submit', searchServ);
    $(document).on('keypress', '#serv-modal-search-txtbox', function (e) {
        if (e.which == 13) {
            searchServ();
        }
    });

    function searchServ() {
        let search = $('#serv-modal-search-txtbox').val();
        var regexObj = new RegExp(search, 'i');

        let allRows = $('#serv-data-list').find('table tbody tr');

        $.each(allRows, function (i, row) {
            let r = $(row);

            let name = r.data('name');

            if (!name.match(regexObj)) {
                r.hide();
            }
            else {
                r.show();
            }
        });
    }



    $(document).on('click', '#user-modal-search-submit', searchServ);
    $(document).on('keypress', '#user-modal-search-txtbox', function (e) {
        if (e.which == 13) {
            searchServ();
        }
    });

    function searchServ() {
        let search = $('#user-modal-search-txtbox').val();
        var regexObj = new RegExp(search, 'i');

        let allRows = $('#user-data-list').find('table tbody tr');

        $.each(allRows, function (i, row) {
            let r = $(row);

            let name = r.data('name');

            if (!name.match(regexObj)) {
                r.hide();
            }
            else {
                r.show();
            }
        });
    }


    // Check if the AJAX HTML load of an element is successful and take some actions
    function checkHtmlLoad(responseText, textStatus, res, backup_item_list) {

        // if the request fails
        if (textStatus == "error") {
            console.log("> ERROR: There was an error getting the data from the server");
            ShowMessage("Internal Error", [responseText]);
            $('#item-list').html(backup_item_list);
        }
        else {

            // if something fail on server
            if (responseText.startsWith('ERROR:')) {
                console.log("> ERROR: Internal Error / " + responseText);

                ShowMessage("Internal Error", [responseText])
                $(res).html(backup_item_list);

                // if everything goes cool and dandy
            } else {
                console.log("> Data loaded into");
                console.log(res);
            }
        }

    }

});
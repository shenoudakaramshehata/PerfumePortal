function AddToFavorite(id, e) {
    $.ajax({
        type: "POST",
        url: `/api/Integration/addItemToFavourite/${id}`,
        success: function (data) {

            if (data["Status"] == true) {
                $(e).parent().html(` <i class="fa fa-heart text-danger" onclick="RemoveFromFavorite(${id},this)"></i>`)

                $(e).remove();

                $.notify({
                    icon: 'fa fa-check',
                    message: lang == "ar" ? "تمت إضافة المنتج الي الفمضلة" : 'Item Successfully added to your favourite'
                }, {
                    element: 'body',
                    position: null,
                    type: "success",
                    allow_dismiss: true,
                    newest_on_top: false,
                    showProgressbar: true,
                    placement: {
                        from: "top",
                        align: "right"
                    },
                    offset: 20,
                    spacing: 10,
                    z_index: 1031,
                    delay: 5000,
                    animate: {
                        enter: 'animated fadeInDown',
                        exit: 'animated fadeOutUp'
                    },
                    icon_type: 'class',
                    template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                        '<button type="button" aria-hidden="true" class="btn-close" data-notify="dismiss"></button>' +
                        '<span data-notify="icon"></span> ' +
                        '<span data-notify="title">{1}</span> ' +
                        '<span data-notify="message">{2}</span>' +
                        '<div class="progress" data-notify="progressbar">' +
                        '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                        '</div>' +
                        '<a href="{3}" target="{4}" data-notify="url"></a>' +
                        '</div>'
                });
            } else {
                window.location.href = "/Login"
            }
            
        },
    });
}
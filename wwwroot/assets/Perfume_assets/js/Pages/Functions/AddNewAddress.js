function addNewAddress(e) {
    e.preventDefault()
    $.ajax({
        type: "POST",
        url: `/api/Integration/AddCustomerAddress`,
        data: {
            Address: $("#Address").val(),
            CountryId: parseInt($("#country_address").val()),
            CityName: $("#CityName").val(),
            AreaName: $("#AreaName").val(),
            BuildingNo: $("#BuildingNo").val(),
            Mobile: $("#Mobile").val(),
        },
        success: function (data) {
            if (data["Status"] == true) {
                setTimeout(function () {
                    window.location.reload()
                },500)
                $.notify({
                    icon: 'fa fa-check',
                    message: lang == "ar" ? "تمت إضافة العنوان بنجاح" : 'address added successfully'
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
            }
        },
    });
}

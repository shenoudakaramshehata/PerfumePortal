$.ajax({
    type: "GET",
    url: `/api/Integration/GetAllCustomersAddress`,
    success: function (data) {
        var location = window.location.pathname
        if (location == "/Perfume/Checkout") {
            if (data['CustomerAddressList'].length <= 0) {
                window.location.href = "/Perfume/Cart"
            }
        }
        if (data['Status'] == true) {
            data["CustomerAddressList"].forEach((address, index) => {
                if (document.querySelector(".customer-address")) {
                    $(".customer-address")[0].innerHTML += `
            <div class="mb-2 fs-6 p-2 d-flex justify-content-between mt-3">
                <div>
                    ${index += 1} - ${address['Address']}, ${address['AreaName']}, ${address['BuildingNo']}, ${address['CityName']}, ${address['Mobile']}
                </div>
                ${location == "/Perfume/Cart" ?
                            `
                    <div class="text-center">
                        <button class="bg-transparent border-0" onclick="deleteAddress(${address.CustomerAddressId})"><i class="fa fa-trash text-danger"></i></button>
                        <button class="bg-transparent border-0" data-bs-toggle="modal" data-bs-target="#editAddressModal" onclick="GetCustomerAddressById(${address.CustomerAddressId})"><i class="fa fa-edit text-primary"></i></button>
                    </div>
                `
                            : `
                   <input type="radio" class="radio_address" name="radio" value="${address['CustomerAddressId']}"/>
                `}

            </div>
            `

                }
            })
            $($(".radio_address")[0]).attr('checked', 'checked');

        }
    },
});

function GetCustomerAddressById(id) {
    $.ajax({
        type: "GET",
        url: `/api/Integration/GetCustomerAddressById/${id}`,
        success: function (data) {
            var address = data['customerAddress']

            $("#Address_edit").val(address["Address"])
            $("#CityName_edit").val(address["CityName"])
            $("#AreaName_edit").val(address["AreaName"])
            $("#BuildingNo_edit").val(address["BuildingNo"])
            $("#Mobile_edit").val(address["Mobile"])
            $("#country_address_edit").val(address["CountryId"])

            $('#form_edit_address').attr("onsubmit", `EditAddress(${address['CustomerAddressId']},event)`);
        },
    });
}

function deleteAddress(CustomerAddressId) {
    Swal.fire({
        title: lang == 'en' ? "Are you sure?" : "هل انت متأكد ؟",
        text: lang == 'en' ? "You won't be able to revert this!" : "لن تكن قادراً علي إستعادة هذا",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: lang == 'en' ? "Delete" : "حذف",
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire(
                lang == 'en' ? "Deleted!" : "تم الحذف",
                lang == 'en' ? "Your address has been deleted successfuly!" : "تم حذف العنوان بنجاح",
                'success'
            )
            $.ajax({
                type: "DELETE",
                url: `/api/Integration/DeleteCustomerAddress/${CustomerAddressId}`,
                success: function (data) {
                    if (data['Status'] == true) {
                        window.location.reload()
                    }
                },
            });
        }
    })

}

function EditAddress(CustomerAddressId,e) {
    e.preventDefault()
    var obj = {
        CustomerAddressId: CustomerAddressId,
        Address: $("#Address_edit").val(),
        CountryId: parseInt($("#country_address_edit").val()),
        CityName: $("#CityName_edit").val(),
        AreaName: $("#AreaName_edit").val(),
        BuildingNo: $("#BuildingNo_edit").val(),
        Mobile: $("#Mobile_edit").val(),
    }
    $.ajax({
        type: "PUT",
        url: `/api/Integration/EditCustomerAddress`,
        data: {
            CustomerAddressId: CustomerAddressId,
            Address: $("#Address_edit").val(),
            CountryId: parseInt($("#country_address_edit").val()),
            CityName: $("#CityName_edit").val(),
            AreaName: $("#AreaName_edit").val(),
            BuildingNo: $("#BuildingNo_edit").val(),
            Mobile: $("#Mobile_edit").val(),
        },
        success: function (data) {
            if (data["Status"] == true) {
                $.notify({
                    icon: 'fa fa-check',
                    message: lang == "ar" ? "تمت تعديل العنوان بنجاح" : 'address edited successfully'
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
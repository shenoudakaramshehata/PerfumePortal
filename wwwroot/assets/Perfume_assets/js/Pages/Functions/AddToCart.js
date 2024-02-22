function addToCartByDefault(id, e) {
    $($(e).children().first()[0]).addClass("fa fa-spinner fa-spin").removeClass("ti-shopping-cart");
    
    $.ajax({
        type: "POST",
        url: `/api/Integration/addItemToCart/${id}/1`,
        success: function (data) {
            if (data["Status"] == true) {
                if ($(".shopping_Cart_empty")) {
                    $(".shopping_Cart_empty").remove()
                }
                if (document.querySelectorAll(".cart_view_buttons").length == 0) {
                    $(".shoppingCartIconContent")[0].innerHTML += `
                    <li class="cart_view_buttons">
                        <div class="total">
                            <h5>${lang == 'en' ? 'Subtotal' : 'الإجمالي'} : <span class="totalAmount">$299.00</span></h5>
                        </div>
                    </li>
                    <li class="cart_view_buttons">
                        <div class="buttons"><a href="/Perfume/Cart" class="view-cart">${lang == 'en' ? 'Cart' : 'عربة التسوق'}</a> <a href="/Perfume/Checkout" class="checkout">${lang == 'en' ? 'Checkout' : 'الدفع'}</a></div>
                    </li>
                `
                }
                $(".item_shopping_cart").each(function () {
                   $(this).remove()
                })
                cartView()
                $($(e).children().first()[0]).addClass("fa fa-check").removeClass("fa-spinner fa-spin");
                setTimeout(function () {
                    $($(e).children().first()[0]).addClass("ti-shopping-cart").removeClass("fa fa-check");
                }, 1000);
                $.notify({
                    icon: 'fa fa-check',
                    message: lang == "ar" ? "تمت إضافة المنتج الي عربة التسوق بنجاح" : 'Item Successfully added to your cart'
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

function addToCartByQuantity(id, e) {
    $(e).html(`<i class="fa fa-spinner fa-spin text-light"></i>`);
    var textSuccess = lang == "ar" ? "تمت إضافة المنتج الي عربة التسوق بنجاح" : 'Item Successfully added to your cart'
    var input_val = $(".input-number").val();
    $.ajax({
        type: "POST",
        url: `/api/Integration/addItemToCart/${id}/${input_val}`,
        success: function (data) {
            if (data["Status"] == true) {
                if ($(".shopping_Cart_empty")) {
                    $(".shopping_Cart_empty").remove()
                }
                $(".item_shopping_cart").each(function () {
                    $(this).remove()
                })
                cartView()
                $(e).html(`<i class="fa fa-check text-light"></i>`);
                setTimeout(function () {
                    $(e).html(`<span class="text-light">إضافة الي السلة</span>`)
                }, 1000);
                $.notify({
                    icon: 'fa fa-check',
                    message: textSuccess,
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
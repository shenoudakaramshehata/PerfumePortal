if (document.querySelector(".wishlist-section")) {
    $.ajax({
        type: "GET",
        url: `/api/Integration/ShowFavouriteItems`,
        success: function (data) {
            var FavoriteItems = data.Cart;
            if (data["Status"] == true) {
                FavoriteItems.forEach(product => {
                    var ItemImage = product.ItemImageAndTitle[0]["ItemImage"];
                    var ItemTitleAr = product.ItemImageAndTitle[0]["ItemTitleAr"];
                    var ItemTitleEn = product.ItemImageAndTitle[0]["ItemTitleEn"];
                    var ItemPrice = product.ItemImageAndTitle[0].ItemPrice.length > 0 ? product.ItemImageAndTitle[0].ItemPrice[0].Price : 0
                    if (document.getElementById("table_wishlist")) {
                        $("#table_wishlist")[0].innerHTML += `
                        <tr>
                            <td>
                                <a href="#"><img src="/${ItemImage}" alt=""></a>
                            </td>
                            <td>
                                <a href="#">${lang == 'en' ? ItemTitleEn : ItemTitleAr}</a>
                            </td>
                            <td>
                                <h2>${ItemPrice}</h2>
                            </td>
                            <td>
                                <i class="fa fa-trash text-danger cursron-pointer" onclick="removeFromWishList(${product.ItemId},this)"></i>
                            </td>
                        </tr>
                        `
                    }
                })

            } else {
                $("#table_wishlist")[0].innerHTML = ` <tr> <td colspan="6">Your favorite is empty</td></tr>`
                //if (window.location.pathname == "/Perfume/Wishlist") {
                //    window.location.href = "/Perfume/Home"
                //}
            }
            //if (data["Status"] == true) {
            //    $(".cart_qty_cls").html(CartItems.length)
            //    var totalPrice = 0;
            //    CartItems.forEach(product => {
            //        var ItemImage = product.ItemImageAndTitle[0]["ItemImage"];
            //        var ItemTitleAr = product.ItemImageAndTitle[0]["ItemTitleAr"];
            //        var ItemTitleEn = product.ItemImageAndTitle[0]["ItemTitleEn"];
            //        console.log(product.ItemPrice)
            //        totalPrice += parseInt(product.ItemTotal);
            //        $(
            //            `<li class="item_shopping_cart">
            //        <div class="media">
            //            <a href="#">
            //                <img alt="" class="me-3"
            //                     src="/${ItemImage}">
            //            </a>
            //            <div class="media-body">
            //                <a href="#">
            //                    <h4>${lang == 'en' ? ItemTitleEn : ItemTitleAr}</h4>
            //                </a>
            //                <h4><span>${product.ItemQty} x $ ${product.ItemPrice}</span></h4>
            //            </div>
            //        </div>
            //        <div class="close-circle">
            //          <i class="fa fa-times" onclick="removeFromCart(${product.ItemId})" aria-hidden="true"></i>
            //        </div>
            //    </li>`
            //        ).insertBefore(".insert");
            //        if (document.getElementById("cart_shopping")) {
            //            $("#cart_shopping tbody")[0].innerHTML += `
            //            <tr class="item_shopping_cart">
            //                <td>
            //                    <a href="#"><img src="/${ItemImage}" width="50" height="50" alt=""></a>
            //                </td>
            //                <td>
            //                    <a href="#">${lang == 'en' ? ItemTitleEn : ItemTitleAr}</a>
            //                    <div class="mobile-cart-content row">
            //                        <div class="col">
            //                            <div class="qty-box">
            //                                <div class="input-group">
            //                                    <input type="text" name="quantity" class="form-control input-number"
            //                                            value="${product.ItemQty}">
            //                                </div>
            //                            </div>
            //                        </div>
            //                        <div class="col">
            //                            <h2 class="td-color">$ ${product.ItemTotal}</h2>
            //                        </div>
            //                        <div class="col">
            //                            <h2 class="td-color">
            //                                <i class="ti-close" onclick="removeFromCart(${product.ItemId})"></i>
            //                            </h2>
            //                        </div>
            //                    </div>
            //                </td>
            //                <td>
            //                    <h2>$${product.ItemTotal}</h2>
            //                </td>
            //                <td>
            //                    <div class="qty-box">
            //                        <div class="input-group">
            //                            <input type="number" name="quantity" min="1" data-intial="${product.ItemQty}" onblur="UpdateCartQuantity(${product.ItemId},this)" class="form-control input-number"
            //                                    value="${product.ItemQty}">
            //                        </div>
            //                    </div>
            //                </td>
            //                <td>
            //                    <h2 class="td-color">$${product.ItemTotal}</h2>
            //                </td>
            //                <td><i class="fa fa-trash text-danger cursron-pointer" onclick="removeFromCart(${product.ItemId})"></i></td>
            //            </tr>
            //            `
            //        }
            //    })
            //    $(".cart_total").html(totalPrice + "$")
            //    $(".totalAmount").html(totalPrice + "$")
            //} else {
            //    if (window.location.pathname == "/Perfume/Cart") {
            //        window.location.href = "/Perfume/Home"
            //    }
            //    $(".cart_qty_cls").remove()
            //    $(
            //        `<li class="shopping_Cart_empty text-center m-0 fs-6">Your cart is empty</li>`
            //    ).insertBefore(".insert")
            //    if (document.getElementById("cart_shopping")) {
            //        $("#cart_shopping tbody")[0].innerHTML = `
            //            <tr> <td colspan="6">Your cart is empty</td></tr>
            //        `
            //    }

            //}
        },
    });
}

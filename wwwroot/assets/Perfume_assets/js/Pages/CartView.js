
function cartView() {
    $.ajax({
        type: "GET",
        url: `/api/Integration/ShowCartList`,
        success: function (data) {
            console.log(data)
            var CartItems = data.Cart;
            if (data["Status"] == true) {
                $(".cart_qty_cls").html(CartItems.length)
                var totalPrice = 0;
                CartItems.forEach(product => {

                    var ItemImage = product.ItemImageAndTitle[0]["ItemImage"];
                    var ItemTitleAr = product.ItemImageAndTitle[0]["ItemTitleAr"];
                    var ItemTitleEn = product.ItemImageAndTitle[0]["ItemTitleEn"];
                    totalPrice += parseInt(product.ItemTotal);
                    if (document.querySelector(".order-box .qty")) {
                        $(".order-box .qty")[0].innerHTML += `
                        <li>${lang == 'en' ? ItemTitleEn : ItemTitleAr} x ${product.ItemQty}  <span>${product.ItemPrice}</span></li>
                    `

                    }
                    $(
                        `<li class="item_shopping_cart">
                <div class="media">
                    <a href="#">
                        <img alt="" class="me-3"
                             src="/${ItemImage}">
                    </a>
                    <div class="media-body">
                        <a href="#">
                            <h4>${lang == 'en' ? ItemTitleEn : ItemTitleAr}</h4>
                        </a>
                        <h4><span>${product.ItemQty} x $ ${product.ItemPrice}</span></h4>
                    </div>
                </div>
                <div class="close-circle">
                  <i class="fa fa-times" onclick="removeFromCart(${product.ItemId})" aria-hidden="true"></i>
                </div>
            </li>`
                    ).insertBefore(".insert");
                    if (document.getElementById("cart_shopping")) {
                        $("#cart_shopping tbody")[0].innerHTML += `
                    <tr class="item_shopping_cart">
                        <td>
                            <a href="#"><img src="/${ItemImage}" width="50" height="50" alt=""></a>
                        </td>
                        <td>
                            <a href="#">${lang == 'en' ? ItemTitleEn : ItemTitleAr}</a>
                        </td>
                        <td>
                            <h2>${product.ItemPrice}</h2>
                        </td>
                        <td>
                            <div class="qty-box">
                                <div class="input-group">
                                    <input type="number" name="quantity" min="1" data-intial="${product.ItemQty}" onblur="UpdateCartQuantity(${product.ItemId},this)" class="form-control input-number"
                                            value="${product.ItemQty}">
                                </div>
                            </div>
                        </td>
                        <td>
                            <h2 class="td-color">${product.ItemTotal}</h2>
                        </td>
                        <td><i class="fa fa-trash text-danger cursron-pointer" onclick="removeFromCart(${product.ItemId})"></i></td>
                    </tr>
                    `
                    }
                })

                $(".cart_total").html(totalPrice)
                $(".totalAmount").html(totalPrice)
                if (document.querySelector(".sub-total")) {
                    $(".sub-total")[0].innerHTML +=
                        `
                        <li>${lang == 'en' ? 'Shipping Cost' : 'تكلفة الشحن'} <span class="count">${localStorage.getItem("ShappingCost")}</span></li>
                        <li>${lang == 'en' ? 'Subtotal' : 'المبلغ الإجمالي'} <span class="count">${totalPrice + parseInt(localStorage.getItem("ShappingCost")) }</span></li>
                        `
                    $(".order-box .total")[0].innerHTML +=
                        `
                            <li>${lang == 'en' ? 'Total' : 'الإجمالي'} <span class="count">${totalPrice + parseInt(localStorage.getItem("ShappingCost")) }</span></li>                        
                            `
                }
            } else {
                $(".cart_view_buttons").each(function () {
                    $(this).remove()
                })
                if (window.location.pathname == "/Perfume/Cart" || window.location.pathname == "/Perfume/Checkout") {
                    window.location.href = "/Perfume/Home"
                }
                $(".cart_qty_cls").remove()
                $(
                    `<li class="shopping_Cart_empty text-center m-0 fs-6">Your cart is empty</li>`
                ).insertBefore(".insert")
                if (document.getElementById("cart_shopping")) { 
                    $("#cart_shopping tbody")[0].innerHTML = `
                    <tr> <td colspan="6">Your cart is empty</td></tr>
                `
                }
                
            }
        },
    });
}
cartView()
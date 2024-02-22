function removeFromCart(id) {
    $("body").addClass("wait-cursor");
    $(".loader").fadeIn();
    $.ajax({
        type: "DELETE",
        url: `/api/Integration/DeleteItemFromShoppingCart/${id}`,
        success: function (data) {
            $(".item_shopping_cart").each(function () {
                $(this).remove()
            })
            $(".loader").fadeOut();
            $("body").removeClass("wait-cursor");

            cartView()
        },
    });
}
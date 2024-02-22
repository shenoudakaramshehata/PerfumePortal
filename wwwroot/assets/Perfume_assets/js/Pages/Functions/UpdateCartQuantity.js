function UpdateCartQuantity(id, element) {
    if ($(element).val() != $(element).data('intial')) {
        var quantity = $(element).val();
        console.log(parseInt(quantity))
        $("body").addClass("wait-cursor");
        $(".loader").fadeIn();
        $.ajax({
            type: "PUT",
            url: `/api/Integration/UpdateItemQuantityInCart/${id}/${parseInt(quantity)}`,
            success: function (data) {
                console.log(data)
                $(".loader").fadeOut();
                $(".item_shopping_cart").each(function () {
                    $(this).remove()
                })
                $("body").removeClass("wait-cursor");
                cartView()
            },
        });
    }
    else {
        return false;
    }
  
}
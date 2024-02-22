function productView(id) {
    $('.addToCart_preview').attr("onclick", `addToCartByQuantity(${id},this)`);

    $.ajax({
        type: "GET",
        url: `/api/Integration/GetItemById/${id}`,
        success: function (data) {
            var product = data.data[0];

            if (product.OutOfStock) {
                $('.addToCart_preview').attr("disabled", 'disabled');
            }
            image_product_view.src = `/${product.ItemImage}`
            title_product_view.innerHTML = `${lang == "en" ? product.ItemTitleEn : product.ItemTitleAr}`
            itme_price.innerHTML = `${product['ItemPrice'][0]['Price']}`

            description_product_view.innerHTML = `${lang == "en" ? product.ItemDescriptionEn : product.ItemDescriptionAr}`
        },
    });
}
function closeProductView() {

    $(".input-number").val(1)
    counter = 1;
}
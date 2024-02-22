if (document.getElementById("product_details")) {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const productID = urlParams.get('id')
    $.ajax({
        type: "GET",
        url: `/api/Integration/GetItemById/${productID}`,
        success: function (data) {
            var product = data.data[0];
 
            var msgOut;
            var msgIn;
            if (lang == 'en') {
                msgOut = 'Out of stock'
                msgIn = 'in stock'

            } else {
                msgOut = 'نفذ من المخزون'
                msgIn = 'متوفر في المخزون'

            }
            $(".itme_price")[0].innerHTML = `${product['ItemPrice'][0]['Price']} <span class="fs-6">( ${product['OutOfStock'] ? msgOut :  msgIn} )</span>`
            $('#cartEffect').attr("onclick", `addToCartByQuantity(${product.ItemId},this)`);
            if (product.OutOfStock) {
                $('#cartEffect').attr("disabled", 'disabled');
            }
            $(".product-slick")[0].innerHTML += `
                    <div>
                        <img src="/${product.ItemImage}" alt=""
                                class="img-fluid blur-up lazyload image_zoom_cls-0">
                    </div>
                    `
            $(".slider-nav")[0].innerHTML += `
                        <div>
                            <img src="/${product.ItemImage}" alt=""
                                    class="img-fluid blur-up lazyload image_zoom_cls-0">
                        </div>
                    `
            if (product.ItemImageNavigation.length > 0) {
                product.ItemImageNavigation.forEach(image => {
                    $(".product-slick")[0].innerHTML += `
                    <div>
                        <img src="/${image.ImageName}" alt=""
                                class="img-fluid blur-up lazyload image_zoom_cls-0">
                    </div>
                    `
                    $(".slider-nav")[0].innerHTML += `
                        <div>
                            <img src="/${image.ImageName}" alt=""
                                    class="img-fluid blur-up lazyload image_zoom_cls-0">
                        </div>
                    `
                })

            } else {
                $(".product-slick")[0].innerHTML += `
                    <div>
                        <img src="/${product.ItemImage}" alt=""
                                class="img-fluid blur-up lazyload image_zoom_cls-0">
                    </div>
                    `
            }
            $('.product-slick').slick({
                slidesToShow: 1,
                slidesToScroll: 1,
                arrows: true,
                fade: true,
                asNavFor: '.slider-nav'
            });
            $('.slider-nav').slick({
                vertical: false,
                slidesToShow: 3,
                slidesToScroll: 1,
                asNavFor: '.product-slick',
                arrows: false,
                dots: false,
                focusOnSelect: true
            });
            title_product_detail_view.innerHTML = `${lang == "en" ? product.ItemTitleEn : product.ItemTitleAr}`
            $(".description_product_detail_view").each(function () {
   
                this.innerHTML = `${lang == "en" ? product.ItemDescriptionEn : product.ItemDescriptionAr}`
            })
        },
    });
}
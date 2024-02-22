if (document.getElementById("home")) {
    var lang = localStorage.getItem("lang");
    if (document.getElementById("home")) {
        $.ajax({
            type: "GET",
            url: "/api/Integration/GetAllItemsByLastest",
            data: {
               countryId: localStorage.getItem("countryId") ? localStorage.getItem("countryId") : null
            },
            success: function (products) {
                $("#product_latest_drops")[0].innerHTML = `<div class="product-latest product-m no-arrow" id="products_latest"></div>`
                products.Items.forEach(product => {
                    console.log(product.OutOfStock)
                    var image;
                    if (product['ItemImageList'].length > 0) {
                        image = product['ItemImageList'][0]['ImageName']
                    } else {
                        image = product.ItemImage
                    }
                    $("#products_latest")[0].innerHTML += `
                <div class="product-box">
                <div class="img-wrapper">
                    <div class="front">
                        <a href="/Perfume/ProductDetails?id=${product.ItemId}">
                            <img src="/${product.ItemImage}" class="img-fluid blur-up lazyload bg-img" alt="">
                        </a>
                    </div>
                    <div class="back">
                        <a href="/Perfume/ProductDetails?id=${product.ItemId}">
                            <img src="/${image}" class="img-fluid blur-up lazyload bg-img" alt="">
                        </a>
                    </div>
                    <div class="cart-info cart-wrap">
                        <button onclick="addToCartByDefault(${product.ItemId},this)"
                            title="Add to cart" ${product.OutOfStock ? 'disabled' : ''}>
                            <i class="ti-shopping-cart ${product.OutOfStock ? 'outofstock' : ''}"></i>
                        </button>

                        <a href="javascript:void(0)" title="Add to Wishlist">
                           ${product.IsFavorate ? ` <i class="fa fa-heart text-danger" onclick="RemoveFromFavorite(${product.ItemId},this)"></i>` : `<i class="ti-heart" onclick="AddToFavorite(${product.ItemId},this)" aria-hidden="true"></i>` } 
                        </a> 

                        <a href="#"
                            data-bs-toggle="modal" onclick="productView(${product.ItemId})" data-bs-target="#quick-view"
                            title="Quick View"><i class="ti-search" aria-hidden="true"></i></a>
                    </div>
                </div>
                <div class="product-detail">
                    <a href="/Perfume/ProductDetails?id=${product.ItemId}">
                        <h6>${lang == "en" ? product.ItemTitleEn : product.ItemTitleAr}</h6>
                    </a>
                    <h4>${product.ItemPrice[0]["Price"]}</h4>
                </div>
            </div>
        `
                })
                $('.product-latest').slick({
                    infinite: true,
                    speed: 300,
                    slidesToShow: 4,
                    slidesToScroll: 4,
                    autoplay: true,
                    autoplaySpeed: 3000,
                    responsive: [{
                        breakpoint: 1200,
                        settings: {
                            slidesToShow: 3,
                            slidesToScroll: 3
                        }
                    },
                    {
                        breakpoint: 991,
                        settings: {
                            slidesToShow: 2,
                            slidesToScroll: 2
                        }
                    }
                    ]
                });
            },
        });
    }



}

var lang = localStorage.getItem("lang");
if (document.getElementById("home")) {
    $.ajax({
        type: "GET",
        url: "/api/Integration/GetAllCategoryWithItems",
        data: {
            countryId: localStorage.getItem("countryId") ? localStorage.getItem("countryId") : null
        },
        success: function (data) {
            var categories = data.CategoryWithItems;
            categories.forEach(category => {
                $(".nav_tab")[0].innerHTML += `
            <button class="nav-link bg-transparent border-0 fs-6 nav-category-${category.CategoryId}" id="nav-category-${category.CategoryId}-tab" data-bs-toggle="tab" data-bs-target="#nav-category-${category.CategoryId}" type="button" role="tab" aria-controls="nav-category-${category.CategoryId}" aria-selected="true">${lang == "en" ? category.CategoryTlen : category.CategoryTlar}</button>
        `
                $(".tab-content")[0].innerHTML += `
            <div class="tab-pane fade" id="nav-category-${category.CategoryId}" role="tabpanel" aria-labelledby="nav-category-${category.CategoryId}-tab">
            <div class="no-slider row"></div>
            </div>
        `
                category['Item'].forEach(product => {
                    console.log(product)
                    var image;
                    if (product['ItemImageNavigation'].length > 0) {
                        image = product['ItemImageNavigation'][0]['ImageName']
                    } else {
                        image = product.ItemImage
                    }
                    $(`#nav-category-${product.CategoryId} .no-slider`)[0].innerHTML += `
            <div class="product-box">
                <div class="img-wrapper">
                    <div class="front">
                        <a href="/Perfume/ProductDetails?id=${product.itemId}">
                            <img src="/${product.ItemImage}" width="570" height="684"
                                class="img-fluid blur-up lazyload bg-img" alt="">
                        </a>
                    </div>
                    <div class="back">
                        <a href="/Perfume/ProductDetails?id=${product.itemId}">
                            <img src="/${image}" width="570" height="684"
                                class="img-fluid blur-up lazyload bg-img" alt="">
                        </a>
                    </div>
                    <div class="cart-info cart-wrap">
                        <button onclick="addToCartByDefault(${product.itemId},this)"
                            title="Add to cart" ${product.OutOfStock ? 'disabled' : ''}>
                            <i class="ti-shopping-cart ${product.OutOfStock ? 'outofstock' : ''}"></i>
                        </button>
                        <a href="javascript:void(0)" title="Add to Wishlist">
                            ${product.IsFavorate ? ` <i class="fa fa-heart text-danger" onclick="RemoveFromFavorite(${product.itemId},this)"></i>` : `<i class="ti-heart" onclick="AddToFavorite(${product.itemId},this)" aria-hidden="true"></i>` } 
                        </a> <a href="#"
                            data-bs-toggle="modal" onclick="productView(${product.itemId})" data-bs-target="#quick-view"
                            title="Quick View"><i class="ti-search" aria-hidden="true"></i></a>
                    </div>
                </div>
                <div class="product-detail">
                    <a href="/Perfume/ProductDetails?id=${product.itemId}">
                        <h6>${lang == "en" ? product.ItemTitleEn : product.ItemTitleAr}</h6>
                    </a>
                    <h4>${product.ItemPrice[0]["Price"]}</h4>
                </div>
            </div>                    
            `
                })
            })
            $(".nav_tab").children().first().addClass("active");
            $(".tab-content").children().first().addClass("show active");
        },
    });
}
$.ajax({
    type: "GET",
    url: `/api/Integration/GetAllCountries`,
    success: function (data) {

        if (data["Status"] == true) {
            if (!localStorage.getItem("countryId") && !localStorage.getItem("ShappingCost")) {
                localStorage.setItem("countryId", data["countryList"][0].CountryId);
                localStorage.setItem("ShappingCost", data["countryList"][0].ShippingCost);
            }

            data["countryList"].forEach(country => {
                if (document.getElementById("country_address")) {
                    $("#country_address")[0].innerHTML += `<option value="${country.CountryId}">${lang == "en" ? country.CountryTlen : country.CountryTlen}</option>`
                    $("#country_address_edit")[0].innerHTML += `<option value="${country.CountryId}">${lang == "en" ? country.CountryTlen : country.CountryTlen}</option>`

                }
                $(".country_dropdown")[0].innerHTML += `
                    <li>
                    <a href="#" onclick="ChangeCountry(${country.CountryId},${country.ShippingCost})">
                        <img src="/${country.Pic}" alt="country-img" class="me-2 rounded" height="18" width="25">
                        <span class="align-middle">${lang == "en" ? country.CountryTlen : country.CountryTlen}</span>
                    </a>
                    </li>
                `
            })
        }
    },
});


function ChangeCountry(CountryId, ShippingCost) {
    localStorage.setItem("countryId", CountryId);
    localStorage.setItem("ShappingCost", ShippingCost);
    if (document.querySelector(".mobile-cart")) {
        $.ajax({
            type: "DELETE",
            url: `/api/Integration/RemoveAllItemsFromShoppingCart`,
            success: function (data) {
 
                if (data["Status"] == true) {
                    window.location.reload()
                }
            },
        });
    } else {
        window.location.reload()
    }

}


$.ajax({
    type: "GET",
    url: `/api/Integration/GetCountryById/${localStorage.getItem("countryId") }`,
    success: function (data) {
        $("#country_picture")[0].src = "/" + data['country']['Pic']
        $("#country_currency")[0].innerHTML = `${lang == "en" ? data['country']['CurrencyTlen'] : data['country']['CurrencyTlar']}`
    },
});
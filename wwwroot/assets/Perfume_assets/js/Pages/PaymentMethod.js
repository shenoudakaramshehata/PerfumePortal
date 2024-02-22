$.ajax({
    type: "GET",
    url: `/api/Integration/GetAllPaymentMethods`,
    success: function (data) {
        if (data['Status'] == true) {
            data['paymentMehodsList'].forEach((payment, index) => {

                if (document.querySelector(".payment-options ul")) {
                    $(".payment-options ul")[0].innerHTML += `
                    <li>
                        <div class="radio-option">
                            <input type="radio" class="payment_method" name="payment-group" value="${payment['PaymentMethodId']}" id="payment-${index}">
                            <label for="payment-${index}">
                                ${lang == 'en' ? payment['PaymentMethodEN'] : payment['PaymentMethodAR']}
                            </label>
                        </div>
                    </li>
                    `
                }
            })
            $($(".payment_method")[0]).attr('checked', 'checked');
        } else {
            window.location.href = "/"
        }
    },
});
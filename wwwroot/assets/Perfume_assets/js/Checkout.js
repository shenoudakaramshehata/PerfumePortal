$.ajax({
    type: "GET",
    url: `/api/Integration/DeleteCustomerAddress/${CustomerAddressId}`,
    success: function (data) {
        if (data['Status'] == true) {
            window.location.reload()
        }
    },
});
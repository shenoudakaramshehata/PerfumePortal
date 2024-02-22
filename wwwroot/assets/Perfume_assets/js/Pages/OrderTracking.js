$.ajax({
    type: "GET",
    url: `/api/Integration/GetOrderTraking`,
    success: function (data) {
        console.log(data)
        if (data['Status'] == true) {
            if (data['orderTrakingList'].length > 0) {
                data['orderTrakingList'].forEach((orderTracking, index) => {
                    $("#order_tracking")[0].innerHTML += `
                    <tr class="item_shopping_cart">
                        <td class="fw-bold">
                            ${index += 1}
                        </td>
                        <td class="fw-bold">
                           ${orderTracking['CustomerName']}
                        </td>
                        <td class="fw-bold">
                            <span clas="td-color">${orderTracking['Status']}</span>
                        </td>
                        <td class="fw-bold">
                            ${orderTracking['Address']} ,${orderTracking['Country']}
                        </td>
                        <td class="fw-bold">
                            ${orderTracking['ActionDate']}
                        </td>
                        <td class="fw-bold">
                            ${orderTracking['ActionTime']}
                        </td>
                        <td class="fw-bold">${orderTracking['Remarks']}</td>
                    </tr>
`

                })
            } else {
                $("#order_tracking")[0].innerHTML = ` <tr> <td colspan="6">Your order tracking is empty</td></tr>`
            }
            
        } else {
            if (!data['isLogin'] && window.location.pathname == '/Perfume/OrderTracking') {
                window.location.href = '/Login'
            }
        }
    },
});

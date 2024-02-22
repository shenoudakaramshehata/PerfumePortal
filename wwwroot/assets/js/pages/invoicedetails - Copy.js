function tConvert(time) {
    var d = new Date(time);
    time_s = (d.getHours() + ':' + d.getMinutes());
    var t = time_s.split(":");
    var hours = t[0];
    var minutes = t[1];
    var newformat = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    return (hours + ':' + minutes + ' ' + newformat);
}

var str_dt = function formatDate(date) {
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var d = new Date(date),
        month = '' + monthNames[(d.getMonth())],
        day = '' + d.getDate(),
        year = d.getFullYear();
    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;
    return [day + " " + month, year].join(', ');
};

if ((localStorage.getItem("invoices-list") !== null) && (localStorage.getItem("option") !== null) && (localStorage.getItem("invoice_no") !== null)) {

    var invoices_list = localStorage.getItem("invoices-list");
    var options = localStorage.getItem("option");
    var invoice_no = localStorage.getItem("invoice_no");
    var invoices = JSON.parse(invoices_list);

    let viewobj = invoices.find(o => o.invoice_no === invoice_no);

    if ((viewobj != '') && (options == "view-invoice")) {
        let badge;
        switch (viewobj.status) {
            case 'Paid':
                badge = "success";
                break;
            case 'Refund':
                badge = "primary";
                break;
            case 'Unpaid':
                badge = "warning";
                break;
            case 'Cancel':
                badge = "danger";
        };

        document.getElementById("Shipping-Label").src = "/" + viewobj.invoice_ShippingLabel

        //document.getElementById("address-details").innerHTML = viewobj.company_details.address;
        //document.getElementById("zip-code").innerHTML = viewobj.company_details.zip_code;

        //document.getElementById("invoice-no").innerHTML = viewobj.invoice_no;
        //document.getElementById("invoice-date").innerHTML = str_dt(viewobj.date);
        //document.getElementById("invoice-time").innerHTML = tConvert(viewobj.date);
        //document.getElementById("payment-status").innerHTML = viewobj.status;
        //document.getElementById("payment-status").classList.replace("badge-soft-success", 'badge-soft-' + badge);
        //document.getElementById("total-amount").innerHTML = viewobj.invoice_amount;
        //document.getElementById("address-details").innerHTML = viewobj.invoice_amount;

        //document.getElementById("billing-name").innerHTML = viewobj.billing_address.full_name;
        //document.getElementById("billing-address-line-1").innerHTML = viewobj.billing_address.address;
        //document.getElementById("billing-phone-no").innerHTML = viewobj.billing_address.phone;
        //document.getElementById("billing-tax-no").innerHTML = viewobj.billing_address.tax;


        
      
    }
}
function freezePage() {
    $("#loading").addClass('modalWindow');
    $("#loading img").css('display', 'inline-block');
}

function unFreezePage() {
    $("#loading").removeClass('modalWindow');
    $("#loading img").css('display', 'none');
}

function loadProductList(products) {
    var item = "";
    for (var i = 0; i < products.length; i++) {
        item = item + getProductItem(products[i].Id, products[i].Title, products[i].ImageUrl, products[i].Amount);
    }
    $('#product-list').html(item);
}

function getProductItem(id, title, image, amount) {

    var item = "<div class='align-items-center'><div class='col-md-12'><div><img class='img-responsive' style='width: 100px; margin: 0 auto;' src='" +
        image +
        "'/></div><div style= 'display:block; text-align:center;'><div style='font-size:14px; font-weight:bold; margin-bottom:20px;'>" +
        title +
        "</div><div style='font-size:14px; font-weight:bold; margin-bottom:20px;'>" +
        amount +
        "</div><div style='font-size:14px; margin-bottom:20px;'><label>Quantity</label><input class='form-control' type='text' value='1' id='quantity'></div>" +
  
        "<div style='font-size:14px; margin-bottom:20px;'><label>Description</label><textarea class='form-control' type='textArea' value='' id='description'></textarea></div>" +
        "<div style='font-size:14px; margin-bottom:20px;'> <input type='button' value='Select' class='btn btn-primary' style='margin-top:5px;' onclick=addToBasket('" +
        id + "'); />" +
        "</div></div></div></div>";
    return item;
}

function addToBasket(id) {
    freezePage();

    addProductToCookie(id, false);
    $.ajax({
        type: "GET",
        url: "/Pos/GetBasketInfoByCookie",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {

            var item = "";
            for (var i = 0; i < data.Products.length; i++) {
                item = item +
                    loadBasket(data.Products[i].Title,
                        data.Products[i].Quantity,
                        /*                            "<div><input type='text' id= 'discount' value= " + data.Products[i].Discount + " style = 'margin-top:5px;'</div > ",*/
                    /*    data.Products[i].Discount,*/
                        data.Products[i].Amount,
                        data.Products[i].RowAmount,
                        data.Products[i].Id,
                        data.Products[i].Description);

            }

            $('#factor tbody').html(item);
            $('#total').html(data.Total);
            $('#totalAmount').html(data.Total);
            $('#remainAmount').html(data.Total);
            //$('#totalAmount').val(data.Total); 
            /*      changeTotal();*/
            changeTotalOrder();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    unFreezePage();
}

function addProductToCookie(id, isDiscount) {

    var currentBasket = '';
    var currentCookie = getCookie("basket");
  /*  var discount = $('#discount').val();*/
    var qty = $('#quantity').val();

    var description = $('#description').val();


    if (currentCookie.includes(id)) {

        var cookieItems = currentCookie.split('/');

        var newcookie = '';

        var isSetNewPro = false;

        for (var i = 0; i < cookieItems.length - 1; i++) {
            if (cookieItems[i].includes(id)) {
                var finalcookieItem = cookieItems[i].split('^');


                cookieItems[i] = id + "^" + qty + "^" + 0 + "^" + description;
                isSetNewPro = true;
                newcookie = newcookie + cookieItems[i] + "/";
                //break;
                alert(id);

            } else {
                newcookie = newcookie + cookieItems[i] + "/";
            }
        }

        if (!isSetNewPro) {
            newcookie = currentCookie + id + "^" + qty + "^" + 0 + "^" + description + "./";
        }

        currentBasket = newcookie;

    } else {
        currentBasket = currentCookie + id + "^" + qty + "^" + 0 + "^" + description + "/";
    }
    setCookie("basket", currentBasket);
}

function addProductToCookieWithDiscount(discountid) {
    var currentBasket = '';
    var currentCookie = getCookie("basket");

    id = discountid.substring(8, 44);

    if (currentCookie.includes(id)) {

        var cookieItems = currentCookie.split('/');

        var newcookie = '';

        var isSetNewPro = false;

        for (var i = 0; i < cookieItems.length - 1; i++) {
            if (cookieItems[i].includes(id)) {
                var finalcookieItem = cookieItems[i].split('^');


                var qty = parseInt(finalcookieItem[1]);
                var discount = $('#' + discountid).val();
                cookieItems[i] = id + "^" + qty + "^" + discount;
                isSetNewPro = true;
                newcookie = newcookie + cookieItems[i] + "/";
                //break;


            } else {
                newcookie = newcookie + cookieItems[i] + "/";
            }
        }

        if (!isSetNewPro) {
            newcookie = currentCookie + id + "^1" + " ^" + discount + "./";
        }

        currentBasket = newcookie;

    } else {
        currentBasket = currentCookie + id + "^1/";
    }
    setCookie("basket", currentBasket);
}


function setCookie(name, value) {
    document.cookie = name + "=" + (value || "") + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return "";
}

function loadBasket(Title, quantity, amount, rowAmount, id, description) {
  
    //var ddl = loadClorDropDown(childProducts, id);

    // var ddlMattress = loadMattressDropDown(hasMattress, matresses, id);
    var item = "<tr>" +
        "<td>" +
        Title +
        "</td>" +
        "<td id='qty" + id + "' class='qtytable'>" + quantity +
        //"<td class='qtytable'><input class='qty' type='text' value=" + quantity + " id='qty" + id + "' onKeyUp='return changeRowTotal(this.id,3)'  />" +
        "</td>" +
    /*    "<td id='discount" + id + "'>" + discount + "</td > " +*/
        "<td id='description" + id + "'>" + description + "</td > " +
        "<td id='amount" + id + "' class='amounttable'>" + amount +
        //"<td class='amounttable'><input type='text' value=" + amount + " id='amount" + id + "' onKeyUp='return changeRowTotal(this.id,6)'/>" +
        //"<td>" + amount +
        "</td>" +

        "<td id='rowAmount" + id + "'>" +
        rowAmount +
        "</td> " +
        "<td><i class='fa fa-remove' onclick=removeRow('" + id + "','PrintFactor'); />" +
        "</td>" +
        "<td ><input type='hidden' value=" + amount + " id='amountHidden" + id + "' />" +
        "</tr>";

    return item;
}

function removeRow(id, viewName) {
    freezePage();
    $.ajax({
        type: "GET",
        url: "/Pos/RemoveFromBasket",
        contentType: "application/json; charset=utf-8",
        data: { "id": id },
        datatype: "json",
        success: function (data) {
            console.log(data.Products);
            var item = "";
            for (var i = 0; i < data.Products.length; i++) {
                if (viewName === "PrintFactor") {
                    item = item + loadBasket(data.Products[i].Title,
                        data.Products[i].Quantity, data.Products[i].Amount,
                        data.Products[i].RowAmount, data.Products[i].Id);
                }
                else if (viewName === "AddOrder") {
                    item = item + loadBasketWithoutAmont(data.Products[i].Title,
                        data.Products[i].Quantity,

                        data.Products[i].Id);
                }
            }


            $('#factor tbody').html(item);
            $('#total').html("sum total: " + data.Total);
            var totalAmount = (data.Total).replace(" toman", "").replace(",", "");
            //  $('#totalAmount').html(totalAmount);
            document.getElementById('totalAmount').innerHTML = totalAmount;
            //  $('#remainAmount').html(totalAmount);
            document.getElementById('remainAmount').innerHTML = totalAmount;
            //$('#totalAmount').val(data.Total); 

            //  changeTotalOrder();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    unFreezePage();
}

function loadBasketWithoutAmont(Title, quantity, id) {
    //var ddl = loadClorDropDown(childProducts, id);

    // var ddlMattress = loadMattressDropDown(hasMattress, matresses, id);

    var item = "<tr>" +
        "<td>" +
        Title +
        "</td>" +
        "<td class='qtytable'>" + quantity +
        //"<td class='qtytable'><input class='qty' type='text' value=" + quantity + " id='qty" + id + "' onKeyUp='return changeRowTotal(this.id,3)'  />" +
        "</td>" +

        //"<td class='amounttable'><input type='text' value=" + amount + " id='amount" + id + "' onKeyUp='return changeRowTotal(this.id,6)'/>" +
        //"<td>" + amount +

        "<td><i class='fa fa-remove' onclick=removeRow('" + id + "','AddOrder'); />" +
        "</td>" + "</tr>";

    return item;
}

function addToBasketWithoutAmont(id) {
    freezePage();

    addProductToCookie(id, false);//my change add new parameter type

    $.ajax({
        type: "GET",
        url: "/Pos/GetBasketInfoByCookie",
        contentType: "application/json; charset=utf-8",
        data: {},
        datatype: "json",
        success: function (data) {

            var item = "";
            for (var i = 0; i < data.Products.length; i++) {
                item = item +
                    loadBasketWithoutAmont(data.Products[i].ParentTitle,
                        data.Products[i].ChildTitle,
                        data.Products[i].Quantity,

                        data.Products[i].Id);
            }

            $('#factor tbody').html(item);
            $('#total').html(data.Total);
            $('#totalAmount').html(data.Total);
            $('#remainAmount').html(data.Total);
            //$('#totalAmount').val(data.Total); 
            /*      changeTotal();*/
            changeTotalOrder();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    unFreezePage();
}

function changeTotalOrder() {


    $('#totalAmount').val('');
    var total = $('#total').html();
    var addedAmount = $('#addedAmount').val();
    var decreasedAmount = $('#decreasedAmount').val();
    var decreasedAmountPercent = 0;
    var decreasedAmountPercentHidden = $('#decreasedAmountPercentHidden').val();


    var payment = $('#payment').val();
    if ($('#addedAmount').val().length === 0)
        addedAmount = 0;
    if ($('#decreasedAmount').val().length === 0)
        decreasedAmount = 0;
    if ($('#payment').val().length === 0)
        payment = 0;
   

    total = clearAmount(total);
    total = clearAmount(total);
    total = clearAmount(total);
    addedAmount = clearAmount(addedAmount);
    addedAmount = clearAmount(addedAmount);
    addedAmount = clearAmount(addedAmount);

    decreasedAmount = clearAmount(decreasedAmount);
    decreasedAmount = clearAmount(decreasedAmount);
    decreasedAmount = clearAmount(decreasedAmount);

    payment = clearAmount(payment);
    payment = clearAmount(payment);
    payment = clearAmount(payment);


    //if (decreasedAmountPercentHidden != decreasedAmountPercent) {
    //    if ($('#decreasedAmountPercent').val().length > 0)
    //        decreasedAmount = (total * decreasedAmountPercent) / 100;
    //    $('#decreasedAmountPercentHidden').val(decreasedAmountPercent);
    //}


    //if (decreasedAmountPercentHidden == 0 && decreasedAmountPercent != 0) {
    //    if ($('#decreasedAmountPercent').val().length > 0)
    //        decreasedAmount = (total * decreasedAmountPercent) / 100;
    //    $('#decreasedAmountPercentHidden').val(decreasedAmountPercent);
    //}

    var tot = parseInt(total) + parseInt(addedAmount) - parseInt(decreasedAmount);
    var remain = parseInt(tot) - parseInt(payment);
    $('#payment').val(commafy(payment));
    $('#decreasedAmount').val(commafy(decreasedAmount));
    $('#addedAmount').val(commafy(addedAmount));
    $('#totalAmount').val(commafy(tot));
    $('#remainAmount').val(commafy(remain));
}

function clearAmount(amount) {
    if (amount.includes('تومان'))
        amount = amount.replace('تومان', '');

    if (amount.includes(','))
        amount = amount.replace(',', '');

    return amount;
}

function commafy(num) {
    var str = num.toString().split('.');
    if (str[0].length >= 5) {
        str[0] = str[0].replace(/(\d)(?=(\d{3})+$)/g, '$1,');
    }
    if (str[1] && str[1].length >= 5) {
        str[1] = str[1].replace(/(\d{3})/g, '$1 ');
    }
    return str.join('.');
}

function clearForm() {
    $('#Order_DeliverCellNumber').val('');
    $('#Order_DeliverFullName').val('');
    $('#Order_Address').val('');
    $('#addedAmount').val('0');
    $('#decreasedAmount').val('0');
    $('#payment').val('0');
    $('#remainAmount').val('0');
    $('#totalAmount').val('0');
    $('#PaymentTypeId').val('');
    $('#CustomerTypeId').val('');
    $('#ProductGroupId').val('');
    $('#ProductId').val('');

    $('#factor tbody').html('');
    $('#total').html('0');
    setCookie("basket", '');

    $('.panel-body input').css('border-color', '#d9d9d9');
}

function finalizeOrder() {

    freezePage();
    var cookie = getCookie('basket');

    if (cookie) {

        var orderDate = $('#Order_OrderDate').val();
        var cellNumber = $('#Order_DeliverCellNumber').val();
        var fullName = $('#Order_DeliverFullName').val();
        var address = $('#Order_AddressLine1').val();
        var addedAmount = $('#addedAmount').val();
        var decreasedAmount = $('#decreasedAmount').val();
        var desc = $('#desc').val();
        var paymentTypeId = $('#PaymentTypeId').val();
        var paymentAmount = $('#payment').val();
        var subtotalAmount = $('#total').val();
        var totalAmount = $('#totalAmount').val();
        var customerTypeId = $('#CustomerTypeId').val();
        var paymentTypeIsRequired = null;

        if (paymentAmount === '0')
            paymentTypeIsRequired = "true";

        else if (paymentAmount !== '0' && paymentTypeId)
            paymentTypeIsRequired = "true";

        var img = getCookie('image');

        if (cellNumber && fullName && paymentTypeIsRequired && paymentTypeId && customerTypeId) {
            $.ajax({
                type: "Post",
                url: "/Pos/PostFinalize",
                data: {

                    "orderDate": orderDate,
                    "cellNumber": cellNumber,
                    "fullName": fullName,
                    "address": address,
                    "addedAmount": addedAmount,
                    "decreasedAmount": decreasedAmount,
                    "desc": desc,
                    "paymentAmount": paymentAmount, "paymentTypeId": paymentTypeId, "customerTypeId": customerTypeId,
                    "subtotalAmount": subtotalAmount,
                    "totalAmount": totalAmount
                },
                success: function (data) {
                    if (data.includes("true")) {
                        var orderCode = data.split('-')[1];
                        var orderId = data.split('*')[1];
                        $('#submit-succes').css('display', 'block');
                        $('#submit-succes').html('فاکتور شماره ' + orderCode + ' با موفقیت ثبت گردید.');
                        $('#submit-error').css('display', 'none');
                        clearForm();
                        $('#print-order').css('display', 'block');
                        $("#print-order").attr("href", "/Report/InvoiceRedirect/" + orderCode);
                        $('#factor-order').css('display', 'block');
                        $("#factor-order").attr("href", "/Orders/CustomerOrder/" + orderId);
                        //window.location = "/orders/list";
                    } else {
                        $('#submit-succes').css('display', 'none');
                        $('#submit-error').css('display', 'block');
                        $('#submit-error').html('خطایی رخ داده است. لطفا دوباره تلاش کنید');

                    }

                },
                error: function () {
                    $('#submit-succes').css('display', 'none');
                    $('#submit-error').css('display', 'block');
                    $('#submit-error').html('خطایی رخ داده است. لطفا دوباره تلاش کنید');
                }
            });


        } else {
            $('#submit-succes').css('display', 'none');
            $('#submit-error').css('display', 'block');
            $('#submit-error').html('فیلدهای ستاره دار را تکمیل نمایید.');
            if (!paymentTypeIsRequired)
                $('#submit-error').html('نوع پرداخت را مشخص کنید.');

            if (cellNumber === '') {
                $('#CellNumber').css('border-color', 'red');
            }
            if (fullName === '') {
                $('#fullName').css('border-color', 'red');
            }
            if (paymentTypeId === '') {
                $('#PaymentTypeId').css('border-color', 'red');
            }
            if (customerTypeId === '') {
                $('#CustomerTypeId').css('border-color', 'red');
            }
        }
    } else {
        $('#submit-succes').css('display', 'none');
        $('#submit-error').css('display', 'block');
        $('#submit-error').html('محصولی انتخاب نشده است.');
    }
    unFreezePage();
}

$("#addedAmount").change(function () {
});


function changeRowTotal(id, type) {
    var discount = $('#' + id).val();
    var productId = getProductIdByElementId(id, type);
    freezePage();


    addProductToCookie(productId, true);

    $.ajax({
        type: "GET",
        url: "/Pos/GetBasketInfoByCookie",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: {

            "discount": discount,
            "productId": productId,
        },
        success: function (data) {

            var item = "";
            for (var i = 0; i < data.Products.length; i++) {
                item = item +
                    loadBasket(data.Products[i].Title,
                        data.Products[i].Quantity,
                        /*                            "<div><input type='text' id= 'discount' value= " + data.Products[i].Discount + " style = 'margin-top:5px;'</div > ",*/
                        data.Products[i].Discount,
                        data.Products[i].Amount,
                        data.Products[i].RowAmount,
                        data.Products[i].Id);
            }

            $('#factor tbody').html(item);
            $('#total').html(data.Total);
            $('#totalAmount').html(data.Total);
            $('#remainAmount').html(data.Total);
            //$('#totalAmount').val(data.Total); 
            /*      changeTotal();*/
            changeTotalOrder();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    unFreezePage();
}

function getProductIdByElementId(id, type) {
    var startIndex = parseInt(type);
    var finishIndex = parseInt(type) + 36;
    var productId = id.substring(startIndex, finishIndex);

    return productId;
}


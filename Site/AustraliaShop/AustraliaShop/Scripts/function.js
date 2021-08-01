function SubmitComment(id) {
    var reviewVal = getCookie('review');

    var review = reviewVal.split('_');
    if (review[1] === id) {
        reviewVal = review[0];
    } else {
        reviewVal = 0;
    }
    var nameVal = $("#name").val();
    var emailVal = $("#email").val();
    var bodyVal = $("#message").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/ProductComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, id: id, reviewVal: reviewVal },
                type: "GET"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDiv").css('display', 'none');
                    $("#SuccessDiv").css('display', 'block');
                    localStorage.setItem("id", "");
                }
                else if (result === "InvalidEmail") {
                    $("#errorDiv").html('your email is invald.');
                    $("#errorDiv").css('display', 'block');
                    $("#SuccessDiv").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDiv").html('please complete all field.');
        $("#errorDiv").css('display', 'block');
        $("#SuccessDiv").css('display', 'none');

    }
}


function addDiscountCode() {
    var coupon = $("#coupon").val();
    $('#errorDiv-discount').css('display', 'none');
    if (coupon !== "") {
        $.ajax(
            {
                url: "/Basket/DiscountRequestPost",
                data: { coupon: coupon },
                type: "GET"
            }).done(function (result) {
                if (result !== "Invald" && result !== "Used" && result !== "Expired") {
                    location.reload();
                }
                else if (result !== true) {
                    $('#errorDiv-discount').css('display', 'block');
                    if (result.toLowerCase() === "used") {
                        $('#errorDiv-discount').html("coupon used previously");
                    }
                    else if (result.toLowerCase() === "expired") {
                        $('#errorDiv-discount').html("coupon is expire.");
                    }
                    else if (result.toLowerCase() === "invald") {
                        $('#errorDiv-discount').html("coupon is invalid.");
                    }
                    else if (result.toLowerCase() === "true") {
                        $('#SuccessDiv-discount').css('display', 'block');
                        $('#errorDiv-discount').css('display', 'none');
                    }
                }
            });

    } else {
        $('#SuccessDiv-discount').css('display', 'none');
        $('#errorDiv-discount').html('please enter coupon.');
        $('#errorDiv-discount').css('display', 'block');
    }
}

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function addToBasket(code, qty) {
    var sizeCookie = getCookie('sizecookie');

    if (sizeCookie === null) {
        alert('please choose size');
    } else {
        

        if (qty === 'detail') {
            qty = $('#basketQty').val();
        }

        $.ajax(
            {
                url: "/cart",
                data: { code: code, qty: qty },
                type: "Post"
            }).done(function(result) {
            if (result !== true) {
                window.location = "/basket";
            }
        });
    }
}

function DisappearButton() {
    $('#btnPayment').css('display', 'none');
}

function AppearButton() {
    $('#btnPayment').css('display', 'block');
}

function finalizeOrder() {
    DisappearButton();
    var fullname = $('#fullname').val();
    var country = $('#ddlCountry option:selected').val();
    var address = $('#billing_address').val();
    var address2 = $('#billing_address2').val();
    var cityTown = $('#city').val();
    var state = $('#state').val();
    var zipcode = $('#zipcode').val();
    var cellnumber = $('#cellnumber').val();
    var email = $('#email').val();
    var createAccount = $('#createaccount').is(':checked');
    var password = $('#password').val();
    var orderNotes = $('#note').val();

    if (createAccount === true && password === '') {
        $('#error-box').css('display', 'block');
        $('#error-box').html('if you want create account, please complete password box');
        $('#success-checkout-box').css('display', 'none');
    } else {
        if (country === undefined || country === '0' || country === 0) {
            country = "";
        }

        if (fullname !== "" && email !== "" && address !== "" && country !== "") {
            $.ajax(
                {
                    url: "/Basket/Finalize",
                    data: {
                        email: email,
                        notes: orderNotes,
                        cellnumber: cellnumber,
                        address: address,
                        country: country,
                        fullname: fullname,
                        address2: address2,
                        cityTown: cityTown,
                        state: state,
                        zipcode: zipcode,
                        createAccount: createAccount,
                        password: password
                    },
                    type: "GET"
                }).done(function(result) {
                if (result.includes('nonstock')) {
                    var products = result.split('|');
                    var productNames = "";
                    for (var i = 1; i < products.length - 1; i++) {
                        productNames = productNames + "\"" + products[i] + "\"";
                    }

                    $('#error-box').css('display', 'block');
                    $('#error-box').html('کاربر گرامی موجودی کالای ' +
                        productNames +
                        'از موجودی کالا انتخابی شما کمتر می باشد. لطفا به ' +
                        '<a href="/basket">سبد خرید </a>' +
                        ' خود مراجعه کنید و موجودی را تغییر دهید.');
                    AppearButton();

                } else if (result === 'duplicateEmail') {

                    $('#error-box').css('display', 'block');
                    $('#error-box').html('Email address registered is site before, please login with your password');
                    $('#success-checkout-box').css('display', 'none');

                } else if (result !== "false") {
                    deleteCookie("basket-babakshop");
                    deleteCookie("sizecookie");
                    deleteCookie("discount");
                    $("form").unbind('submit').submit();


                    window.location = result;
                } else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('an error occurred while submit order, please try again');
                    $('#success-checkout-box').css('display', 'none');

                    AppearButton();

                }
            });
        } else {
            $('#error-box').css('display', 'block');
            $('#error-box').html('please complete all required fields');
            $('#success-checkout-box').css('display', 'none');
            AppearButton();
        }
    }

}

 
function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function updateBasket() {
    $('.loading-fuulpage').css('display', 'block');
    var cookie = getCookie('basket-babakshop');

    var orderDetails = cookie.split('/');
    var newOrderDetails = '';

    for (var i = 0; i < orderDetails.length - 1; i++) {
        var orderDetail = orderDetails[i].split('^');

        var txtQtyVal = $('#txt-qty-' + orderDetail[0]).val();

        if (txtQtyVal !== orderDetail[1]) {
            orderDetails[i] = orderDetail[0] + '^' + txtQtyVal + "^" + orderDetail[2];
        }
        newOrderDetails = newOrderDetails + orderDetails[i] + '/';
    }

    deleteCookie('basket-babakshop');
    setCookie('basket-babakshop', newOrderDetails, 100);
    location.reload();
}

function search() {
    var searchTerm = $('#TextSearch').val();

    //window.location = "/product?searchterm=" + searchTerm;

    //window.location.replace("/product?searchterm=" + searchTerm);
    //return false;


    window.onload = function () {
        document.getElementById("search-form").onsubmit = function () {
            window.location.replace("/product?searchterm=" + searchTerm);
            return false;
        }
    }
}

function postNewsletter() {
    var newsletter = $('#txtNewsletter').val();
    if (newsletter === '') {
        $('#danger-alert-nl').css('display', 'block');
        $('#success-alert-nl').css('display', 'none');
        $('#danger-alert-nl').html('ایمیل خود را وارد نمایید');
    } else {
        $.ajax(
            {
                url: "/Newsletters/EmailPost",
                data: { email: newsletter },
                type: "Post"
            }).done(function (result) {
                if (result === "true") {
                    $('#danger-alert-nl').css('display', 'none');
                    $('#success-alert-nl').css('display', 'block');
                } else if (result == "invalidemail") {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('ایمیل وارد شده صحیح نمی باشد');
                } else {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('خطایی رخ داده است. لطفا دقایقی دیگر مجدادا تلاش کنید');
                }
            });
    }
}


function LoginInCheckout() {
    var loginPassword = $('#loginPassword').val();
    var loginEmail = $('#loginEmail').val();
    if (loginPassword === '' || loginEmail==='') {
        $('#danger-alert-login').css('display', 'block');
        $('#danger-alert-login').html('please complete login form');
    } else {
        $.ajax(
            {
                url: "/basket/CheckoutLogin",
                data: { loginPassword: loginPassword, loginEmail: loginEmail },
                type: "Post",
                success: function(data) {
                    if (data === "true") {
                        $('#danger-alert-nl').css('display', 'none');
                        location.reload();
                    } else {
                        $('#danger-alert-login').css('display', 'block');
                        $('#danger-alert-login').html('Invalid email or password');
                    }
                }
            });
    }
}


function SubmitBlogComment(id) {

    //var url = window.location.pathname;
    //var id = url.substring(url.lastIndexOf('/') + 1);

    var nameVal = $("#name").val();
    var emailVal = $("#email").val();
    var bodyVal = $("#message").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/BlogComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, code: id },
                type: "POST"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDivBlog").css('display', 'none');
                    $("#SuccessDivBlog").css('display', 'block');
                    localStorage.setItem("id", "");
                }
                else if (result === "InvalidEmail") {
                    $("#errorDivBlog").html('Invalid email.');
                    $("#errorDivBlog").css('display', 'block');
                    $("#SuccessDivBlog").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDivBlog").html('please complete all field.');
        $("#errorDivBlog").css('display', 'block');
        $("#SuccessDivBlog").css('display', 'none');

    }
}

function SubmitContactUs() {

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/ContactUsForms/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal },
                type: "POST"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDivContact").css('display', 'none');
                    $("#SuccessDivContact").css('display', 'block');
                  
                }
                else if (result === "InvalidEmail") {
                    $("#errorDivContact").html('Invalid Email');
                    $("#errorDivContact").css('display', 'block');
                    $("#SuccessDivContact").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDivContact").html('Please complete all field.');
        $("#errorDivContact").css('display', 'block');
        $("#SuccessDivContact").css('display', 'none');

    }
}


function searchResult() {
    var searchQuery = $('#search_input').val();
    location.href = "/result?searchquery=" + searchQuery;
}
function submitSearchResult() {
    var searchQuery = $('#txtsearch').val();
    location.href = "/result?searchquery=" + decodeURIComponent(searchQuery);
}
function submitSearchForSearchPageResult() {
    var searchQuery = $('#txtsearchpage').val();
    location.href = "/result?searchquery=" + decodeURIComponent(searchQuery);
}


function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}


function getItem(code, title, amount, imageUrl, stock, discountAmount) {
    return "		<div class='col-md-3 col-6 newclass'><div class='product'><div>" +
        "<a href='/product/" + code + "'><img src='" + imageUrl + "' alt='" + title + "'></a></div>" +
        getNoStockTitle(stock) +
        "<div class='product_info'><h6 class='product_title'><a href='/product/" + code + "'>" + title + "</a></h6>" +
        "<div class='product_price'>" + getAmount(amount, discountAmount) + " </div>" +
        getAddToBasketButton(code, stock) +
        "</div></div></div>";
}
function getAmount(amount, discountAmout) {
    if (discountAmout === '') {
        return "<span class='price'>" + amount + "</span><del></del>";
    } else {
        return "<span class='price'>" + discountAmout + "</span><del>" + amount + "</del>";
    }

}
function getAddToBasketButton(code, stock) {
    var stockInt = parseInt(stock);

    if (stockInt > 0) {
        return "<div class='add-to-cart'><button class='btn btn-fill-out btn-addtocart' onclick='addToBasket('" +
            code +
            "', '1');'><i class='icon-basket-loaded'></i>خرید</button></div>";
    } else {
        return "<div class='add-to-cart'><button class='btn btn-addtocart btn-disable' disabled='disabled' onclick='addToBasket('" +
            code +
            "', '1');'><i class='icon-basket-loaded'></i>خرید</button></div>";
    }
}
function getNoStockTitle(stock) {
    var stockInt = parseInt(stock);

    if (stockInt === 0) {
        return " <span class='pr_flash bg-danger'>ناموجود</span>";
    } else {
        return "";
    }
}



function removeCookie(cookieName) {
    document.cookie = cookieName + "= ; expires = Thu, 01 Jan 1970 00:00:00 GMT";
}
 
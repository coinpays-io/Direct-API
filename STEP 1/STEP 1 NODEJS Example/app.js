var express = require('express');
var ejsLayouts = require('express-ejs-layouts');
var microtime = require('microtime');
var crypto = require('crypto');
var app = express();
var nodeBase64 = require('nodejs-base64-converter');
var request = require('request');

app.set('view engine', 'ejs');
app.use(ejsLayouts);
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

var merchant_id = 'XXXXXX';
var merchant_key = 'YYYYYYYYYYYYYY';
var merchant_salt = 'ZZZZZZZZZZZZZZ';
var basket = JSON.stringify([
    ['Example Product 1', '18.00', 1],
    ['Example Product 2', '33.25', 2],
    ['Example Product 3', '45.42', 1]
]);
var user_basket = nodeBase64.encode(basket);
var merchant_oid = "IN" + microtime.now(); // Order number: Must be unique for every transaction!! This information is sent back in the notification to your notification page.
var user_ip = '';
var email = 'XXXXXXXX'; // Your customer's email address registered on your site or received via the form.
var payment_amount = 100; // Amount to be collected. For 9.99, 9.99 * 100 = 999 should be sent.

// Here you can choose in which currency you would like to display your cart. Below are sample values. To access the last updated values
// Visit (https://app.coinpays.io/shared/currencies)
var currency = 'TRY';//USD-EUR-TRY-GBP-RUB-CNY-KRW

var test_mode = '0'; // It can be sent as 1 for testing when the store is in live mode.
var user_name = ''; // Your customer's name and surname information registered on your site or obtained through the form
var user_address = ''; // Your customer's address information registered on your site or received through the form
var user_phone = '05555555555'; // Your customer's phone number registered on your site or received via the form

// The page your customer will be directed to after the payment waiting page
// This page is not the page where you will confirm the order! This is the page where you will only inform your customer!
var merchant_pending_url = 'http://www.siteniz.com/odeme_basarili.php';

//Here you can choose which language you would like to display your payment page in. Below are sample values. To access the last updated values
//Visit (https://app.coinpays.io/shared/languages)
var lang = 'tr'; //tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

// Here you can choose which cryptocurrency you would like to receive your payment with. Below are sample values. To access the last updated values
// Visit (https://app.coinpays.io/shared/coins?merchant_key={merchant_key}&merchant_id={$merchant_id}&merchant_salt={merchant_salt}). The value will be "code".
var coin = "ETH";

// Here you can choose which network to receive the cryptocurrency you chose above.
var network = "BEP-20";

app.get("/", function (req, res) {


    var hashSTR = `${merchant_id}${user_ip}${merchant_oid}${email}${payment_amount}${user_basket}`;

    var coinpays_token = hashSTR + merchant_salt;

    var token = crypto.createHmac('sha256', merchant_key).update(coinpays_token).digest('base64');


    var options = {
        method: 'POST',
        url: 'https://app.coinpays.io/api/create-payment',
        headers:
            { 'content-type': 'application/x-www-form-urlencoded' },
        formData: {
            merchant_id: merchant_id,
            user_ip: user_ip,
            lang: lang,
            currency: currency,
            merchant_oid: merchant_oid,
            email: email,
            payment_amount: payment_amount,
            coinpays_token: token,
            user_basket: user_basket,
            coin: coin,
            network: network,
            user_name: user_name,
            user_address: user_address,
            user_phone: user_phone,
            merchant_pending_url: merchant_pending_url,
            test_mode: test_mode
        }
    };

    request(options, function (error, response, body) {
        if (error) throw new Error(error);
        var res_data = JSON.parse(body);

        if (res_data.status == 'success') {
            // You can find the returned data in the "res_data" object
        } else {

            res.end(body);
        }


    });


});


var port = 3000;
app.listen(port, function () {
    console.log("Server is running. Port:" + port);
});

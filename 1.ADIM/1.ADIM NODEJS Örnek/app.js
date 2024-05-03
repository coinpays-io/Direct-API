var express = require('express');
var ejsLayouts = require('express-ejs-layouts');
var microtime = require('microtime');
var crypto = require('crypto');
var app = express();
var nodeBase64 = require('nodejs-base64-converter');
var request = require('request');
var path = require('path');


app.set('views', path.join(__dirname, '/app_server/views'));

app.set('view engine', 'ejs');
app.use(ejsLayouts);
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

var merchant_id = 'XXXXXX';
var merchant_key = 'YYYYYYYYYYYYYY';
var merchant_salt = 'ZZZZZZZZZZZZZZ';
var basket = JSON.stringify([
    ['Örnek Ürün 1', '18.00', 1],
    ['Örnek Ürün 2', '33.25', 2],
    ['Örnek Ürün 3', '45.42', 1]
]);
var user_basket = nodeBase64.encode(basket);
var merchant_oid = "IN" + microtime.now(); // Sipariş numarası: Her işlemde benzersiz olmalıdır!! Bu bilgi bildirim sayfanıza yapılacak bildirimde geri gönderilir.
var user_ip = '';
var email = 'XXXXXXXX'; // Müşterinizin sitenizde kayıtlı veya form vasıtasıyla aldığınız eposta adresi.
var payment_amount = 100; // Tahsil edilecek tutar. 9.99 için 9.99 * 100 = 999 gönderilmelidir.

// Burada sepetinizin hangi para biriminde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
// (https://app.coinpays.io/shared/currencies) adresini ziyaret edin
var currency = 'TRY';//USD-EUR-TRY-GBP-RUB-CNY-KRW

var test_mode = '0'; // Mağaza canlı modda iken test işlem yapmak için 1 olarak gönderilebilir.
var user_name = ''; // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız ad ve soyad bilgisi
var user_address = ''; // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız adres bilgisi
var user_phone = '05555555555'; // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız telefon bilgisi

// Ödeme bekleniyor sayfası sonrası müşterinizin yönlendirileceği sayfa
// Bu sayfa siparişi onaylayacağınız sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
var merchant_pending_url = 'http://www.siteniz.com/odeme_basarili.php';

//Burada ödeme sayfanızın hangi dilde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
//(https://app.coinpays.io/shared/languages) adresini ziyaret edin
var lang = 'tr'; //tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

// Burada ödemenizi hangi kripto para birimi ile almak istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
// (https://app.coinpays.io/shared/coins) adresini ziyaret edin. Değer olarak "code" gelecektir.
var coin = "ETH";

// Burada yukarıda seçtiğiniz kripto para biriminin hangi ağda alınacağını seçebilirsiniz.
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
            res.render('layout', { iframetoken: res_data.token });
        } else {

            res.end(body);
        }


    });


});


var port = 3000;
app.listen(port, function () {
    console.log("Server is running. Port:" + port);
});

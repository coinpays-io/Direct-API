var express = require('express');
var ejsLayouts = require('express-ejs-layouts');
var crypto = require('crypto');
var app = express();
var path = require('path');


app.set('views', path.join(__dirname, '/app_server/views'));

app.set('view engine', 'ejs');
app.use(ejsLayouts);
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

var merchant_key = 'YYYYYYYYYYYYYY';
var merchant_salt = 'ZZZZZZZZZZZZZZ';


app.post("/callback", function (req, res) {

    // ÖNEMLİ UYARILAR!
    // 1) Bu sayfaya oturum (SESSION) ile veri taşıyamazsınız. Çünkü bu sayfa müşterilerin yönlendirildiği bir sayfa değildir.
    // 2) Entegrasyonun 1. ADIM'ında gönderdiğniz merchant_oid değeri bu sayfaya POST ile gelir. Bu değeri kullanarak
    // veri tabanınızdan ilgili siparişi tespit edip onaylamalı veya iptal etmelisiniz.
    // 3) Aynı sipariş için birden fazla bildirim ulaşabilir (Ağ bağlantı sorunları vb. nedeniyle). Bu nedenle öncelikle
    // siparişin durumunu veri tabanınızdan kontrol edin, eğer onaylandıysa tekrar işlem yapmayın. Örneği aşağıda bulunmaktadır.

    var callback = req.body;

    // POST değerleri ile hash oluştur.
    coinpays_token = callback.merchant_oid + merchant_salt + callback.status + callback.total_amount;
    var token = crypto.createHmac('sha256', merchant_key).update(coinpays_token).digest('base64');

    // Oluşturulan hash'i, CoinPays'dan gelen post içindeki hash ile karşılaştır (isteğin CoinPays'dan geldiğine ve değişmediğine emin olmak için)
    // Bu işlemi yapmazsanız maddi zarara uğramanız olasıdır.

    if (token != callback.hash) {
        throw new Error("COINPAYS notification failed: bad hash");
    }

    if (callback.status == 'success') {
        //basarili
    } else {
        //basarisiz
    }

    res.send('OK');

});


var port = 3000;
app.listen(port, function () {
    console.log("Server is running. Port:" + port);
});
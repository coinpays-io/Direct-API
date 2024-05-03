var express = require('express');
var ejsLayouts = require('express-ejs-layouts');
var crypto = require('crypto');
var app = express();

app.set('view engine', 'ejs');
app.use(ejsLayouts);
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

var merchant_key = 'YYYYYYYYYYYYYY';
var merchant_salt = 'ZZZZZZZZZZZZZZ';


app.post("/callback", function (req, res) {

// IMPORTANT WARNINGS!
    // 1) You cannot move data to this page via session (SESSION). Because this page is not a page to which customers are directed.
    // 2) The merchant_oid value you sent in STEP 1 of the integration comes to this page via POST. Using this value
    // You must identify the relevant order from your database and confirm or cancel it.
    // 3) More than one notification may arrive for the same order (due to network connection problems, etc.). Therefore first of all
    // check the status of the order in your database, if confirmed, do not process again. An example is below.

    var callback = req.body;

// Create hash with POST values.
    coinpays_token = callback.merchant_oid + merchant_salt + callback.status + callback.total_amount;
    var token = crypto.createHmac('sha256', merchant_key).update(coinpays_token).digest('base64');

// Compare the generated hash with the hash in the post from CoinPays (to ensure that the request came from CoinPays and has not changed)
    // If you do not do this, you may incur financial loss.

    if (token != callback.hash) {
        throw new Error("COINPAYS notification failed: bad hash");
    }

    if (callback.status == 'success') {
        //success
    } else {
        //error
    }

    res.send('OK');

});


var port = 3000;
app.listen(port, function () {
    console.log("Server is running. Port:" + port);
});
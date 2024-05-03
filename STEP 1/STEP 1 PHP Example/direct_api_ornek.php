<?php

## Sample codes for STEP 1 ##

####################### REQUIRED FIELDS #######################
#
## API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
$merchant_id 	= '';
$merchant_key 	= '';
$merchant_salt	= '';
#
## Your customer's email address registered on your site or received via the form
$email = "";
#
## Amount to be collected.
$payment_amount	= ""; //For 9.99, 9.99 * 100 = 999 should be sent.
#
## Order number: Must be unique for every transaction!! This information is sent back in the notification to your notification page.
$merchant_oid = "";
#
## Your customer's name and surname information registered on your site or obtained through the form
$user_name = "";
#
## Your customer's address information registered on your site or received through the form
$user_address = "";
#
## Your customer's phone number registered on your site or received via the form
$user_phone = "";
#
## The page your customer will be directed to after the payment waiting page
## !!! This page is not the page where you will confirm the order! This is the page where you will only inform your customer!
## !!! The page where we will confirm the order is the "Notification URL" page (See: STEP 2 Folder).
$merchant_pending_url = "http://www.siteniz.com/odeme_onaylaniyor.php";
#
## Müşterinin sepet/sipariş içeriği
$user_basket = "";
#
/* EXAMPLE Creating $user_basket - You can multiply arrays according to the number of products
$user_basket = base64_encode(json_encode(array(
    array("Example Product 1", "18.00", 1), // 1. Product (Product Name - per price - quantity )
    array("Example Product 2", "33.25", 2), // 2. Product (Product Name - per price - quantity )
    array("Example Product 3", "45.42", 1)  // 3. Product (Product Name - per price - quantity )
)));
*/
############################################################################################

## Kullanıcının IP adresi
if( isset( $_SERVER["HTTP_CLIENT_IP"] ) ) {
    $ip = $_SERVER["HTTP_CLIENT_IP"];
} elseif( isset( $_SERVER["HTTP_X_FORWARDED_FOR"] ) ) {
    $ip = $_SERVER["HTTP_X_FORWARDED_FOR"];
} else {
    $ip = $_SERVER["REMOTE_ADDR"];
}

## !!! If you are running this sample code on your local machine, not on the server
## You should write your external IP address (https://www.whatismyip.com/) here. Otherwise you will get an invalid coinpays_token error.
$user_ip=$ip;
##

## It can be sent as 1 for testing when the store is in live mode.
$test_mode = 0;

## Here you can choose which language you would like to display your payment page in. Below are sample values. To access the last updated values
## Visit (https://app.coinpays.io/shared/languages)
$lang=  "tr"; //tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

## Here you can choose in which currency you would like to display your cart. Below are sample values. To access the last updated values
## Visit (https://app.coinpays.io/shared/currencies)
$currency = "TRY";//USD-EUR-TRY-GBP-RUB-CNY-KRW

## Here you can choose which cryptocurrency you would like to receive your payment with. Below are sample values. To access the last updated values
## Visit (https://app.coinpays.io/shared/coins?merchant_key={merchant_key}&merchant_id={$merchant_id}&merchant_salt={merchant_salt}). The value will be "code".
$coin = "ETH";

## Here you can choose which network to receive the cryptocurrency you chose above.
$network = "BEP-20";

####### You do not need to make any changes in this section.. #######
$hash_str = $merchant_id .$user_ip .$merchant_oid .$email .$payment_amount .$user_basket;
$coinpays_token=base64_encode(hash_hmac('sha256',$hash_str.$merchant_salt,$merchant_key,true));
$post_vals=array(
    'merchant_id'=>$merchant_id,
    'user_ip'=>$user_ip,
    'lang'=>$lang,
    'currency' => $currency,
    'merchant_oid'=>$merchant_oid,
    'email'=>$email,
    'payment_amount'=>$payment_amount,
    'coinpays_token'=>$coinpays_token,
    'network' => $network,
    'coin' => $coin,
    'user_basket'=>$user_basket,
    'user_name'=>$user_name,
    'user_address'=>$user_address,
    'user_phone'=>$user_phone,
    'merchant_pending_url'=>$merchant_pending_url,
    'test_mode' => $test_mode
);

$ch=curl_init();
curl_setopt($ch, CURLOPT_URL, "https://app.coinpays.io/api/create-payment");
curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
curl_setopt($ch, CURLOPT_POST, 1) ;
curl_setopt($ch, CURLOPT_POSTFIELDS, $post_vals);
curl_setopt($ch, CURLOPT_FRESH_CONNECT, true);
curl_setopt($ch, CURLOPT_TIMEOUT, 20);

// XXX: ATTENTION: If you receive the "SSL certificate problem: unable to get local issuer certificate" warning on your local machine,
// You can open and try the code below. HOWEVER, it is very important that this code remains closed on your server (your real environment) for security reasons!
//curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, 0);

$result = @curl_exec($ch);

if(curl_errno($ch))
    die("COINPAYS DIRECT API connection error. err:".curl_error($ch));

curl_close($ch);

$result=json_decode($result,1);

if($result['status']=='success'){
    echo "<pre>";
    print_r($result);
    echo "</pre>";
}else{
    die("COINPAYS DIRECT API failed. reason:".$result['reason']);
}
#########################################################################

?>
# Python 3.6+
# Sample codes for STEP 1

import base64
import hmac
import hashlib
import requests
import json

# API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
merchant_id = 'XXXXXX'
merchant_key = b'YYYYYYYYYYYYYY'
merchant_salt = b'ZZZZZZZZZZZZZZ'

# Your customer's email address registered on your site or received via the form
email = 'XXXXXXXX'

# Amount to be collected.
payment_amount = '' # For 9.99, 9.99 * 100 = 999 should be sent.

# Order number: Must be unique for every transaction!! This information is sent back in the notification to your notification page.
merchant_oid = ''

# Your customer's name and surname information registered on your site or obtained through the form
user_name = ''

# Your customer's address information registered on your site or received through the form
user_address = ''

# Your customer's phone number registered on your site or received via the form
user_phone = ''

# The page your customer will be directed to after the payment waiting page
# !!! This page is not the page where you will confirm the order! This is the page where you will only inform your customer!
# !!! The page where we will confirm the order is the "Notification URL" page (See: STEP 2 Folder).
merchant_pending_url = 'http://www.siteniz.com/odeme_basarili.php'

# Customer's cart/order content
user_basket = ''

# EXAMPLE Creating $user_basket - You can multiply arrays according to the number of products
"""
user_basket = base64.b64encode(json.dumps([['Example product 1', '18.00', 1],
               ['Example product 2', '33.25', 2],
               ['Example product 3', '45.42', 1]]).encode())
"""

# !!! If you are running this sample code on your local machine, not on the server
# You should write your external IP address (https://www.whatismyip.com/) here. Otherwise you will get an invalid coinpays_token error.
user_ip = ''

# Here you can choose which language you would like to display your payment page in. Below are sample values. To access the last updated values
# Visit (https://app.coinpays.io/shared/languages)
lang = "tr" #tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

# It can be sent as 1 for testing when the store is in live mode.
test_mode = '1'

# Here you can choose in which currency you would like to display your cart. Below are sample values. To access the last updated values
# Visit (https://app.coinpays.io/shared/currencies)
#USD-EUR-TRY-GBP-RUB-CNY-KRW
currency = 'TRY'

# Here you can choose which cryptocurrency you would like to receive your payment with. Below are sample values. To access the last updated values
# Visit (https://app.coinpays.io/shared/coins?merchant_key={merchant_key}&merchant_id={$merchant_id}&merchant_salt={merchant_salt}). The value will be "code".
coin = "ETH"

# Here you can choose which network to receive the cryptocurrency you chose above.
network = "BEP-20"

# You do not need to make any changes in this section.
hash_str = merchant_id + user_ip + merchant_oid + email + payment_amount + user_basket
coinpays_token = base64.b64encode(hmac.new(merchant_key, hash_str.encode() + merchant_salt, hashlib.sha256).digest())

params = {
        'merchant_id'=>merchant_id,
		'user_ip'=>user_ip,
		'lang'=>lang,
		'currency' => currency,
		'merchant_oid'=>merchant_oid,
		'email'=>email,
		'payment_amount'=>payment_amount,
		'coinpays_token'=>coinpays_token,
		'user_basket'=>user_basket,
		'user_name'=>user_name,
		'coin'=>coin,
		'network'=>network,
		'user_address'=>user_address,
		'user_phone'=>user_phone,
		'merchant_pending_url'=>merchant_pending_url,
		'test_mode' => test_mode
}

result = requests.post('https://app.coinpays.io/api/create-payment', params)
res = json.loads(result.text)

if res['status'] == 'success':
    print(res)
else:
    print(result.text)
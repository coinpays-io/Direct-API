# Python 3.6+
# 1. ADIM için örnek kodlar

import base64
import hmac
import hashlib
import requests
import json

# API Entegrasyon Bilgileri - Mağaza paneline giriş yaparak BİLGİ sayfasından alabilirsiniz.
merchant_id = 'XXXXXX'
merchant_key = b'YYYYYYYYYYYYYY'
merchant_salt = b'ZZZZZZZZZZZZZZ'

# Müşterinizin sitenizde kayıtlı veya form vasıtasıyla aldığınız eposta adresi
email = 'XXXXXXXX'

# Tahsil edilecek tutar.
payment_amount = '' # 9.99 için 9.99 * 100 = 999 gönderilmelidir.

# Sipariş numarası: Her işlemde benzersiz olmalıdır!! Bu bilgi bildirim sayfanıza yapılacak bildirimde geri gönderilir.
merchant_oid = ''

# Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız ad ve soyad bilgisi
user_name = ''

# Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız adres bilgisi
user_address = ''

# Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız telefon bilgisi
user_phone = ''

# Başarılı ödeme sonrası müşterinizin yönlendirileceği sayfa
# !!! Bu sayfa siparişi onaylayacağınız sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
# !!! Siparişi onaylayacağız sayfa "Bildirim URL" sayfasıdır (Bakınız: 2.ADIM Klasörü).
merchant_ok_url = 'http://www.siteniz.com/odeme_basarili.php'

# Ödeme sürecinde beklenmedik bir hata oluşması durumunda müşterinizin yönlendirileceği sayfa
# !!! Bu sayfa siparişi iptal edeceğiniz sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
# !!! Siparişi iptal edeceğiniz sayfa "Bildirim URL" sayfasıdır (Bakınız: 2.ADIM Klasörü).
merchant_fail_url = 'http://www.siteniz.com/odeme_hata.php'

# Müşterinin sepet/sipariş içeriği
user_basket = ''

# ÖRNEK $user_basket oluşturma - Ürün adedine göre array'leri çoğaltabilirsiniz
"""
user_basket = base64.b64encode(json.dumps([['Örnek ürün 1', '18.00', 1],
               ['Örnek ürün 2', '33.25', 2],
               ['Örnek ürün 3', '45.42', 1]]).encode())
"""

# !!! Eğer bu örnek kodu sunucuda değil local makinanızda çalıştırıyorsanız
# buraya dış ip adresinizi (https://www.whatismyip.com/) yazmalısınız. Aksi halde geçersiz coinpays_token hatası alırsınız.
user_ip = ''

# Burada ödeme sayfanızın hangi dilde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
# (https://app.coinpays.io/shared/languages) adresini ziyaret edin
lang = "tr"; #tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

# Mağaza canlı modda iken test işlem yapmak için 1 olarak gönderilebilir.
test_mode = '1'

# Burada sepetinizin hangi para biriminde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
# (https://app.coinpays.io/shared/currencies) adresini ziyaret edin
#USD-EUR-TRY-GBP-RUB-CNY-KRW
currency = 'TRY'

# Burada ödemenizi hangi kripto para birimi ile almak istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
# (https://app.coinpays.io/shared/coins) adresini ziyaret edin. Değer olarak "code" gelecektir.
coin = "ETH"

# Burada yukarıda seçtiğiniz kripto para biriminin hangi ağda alınacağını seçebilirsiniz.
network = "BEP-20"


# Bu kısımda herhangi bir değişiklik yapmanıza gerek yoktur.
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
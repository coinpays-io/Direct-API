// 1. ADIM için örnek kodlar

using Newtonsoft.Json.Linq; // Bu satırda hata alırsanız, site dosyalarınızın olduğu bölümde bin isimli bir klasör oluşturup içerisine Newtonsoft.Json.dll adlı DLL dosyasını kopyalayın.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApplication3
{
    public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        // ####################### DÜZENLEMESİ ZORUNLU ALANLAR #######################
        //
        // API Entegrasyon Bilgileri - Mağaza paneline giriş yaparak BİLGİ sayfasından alabilirsiniz.
        string merchant_id = "XXXXXX";
        string merchant_key = "YYYYYYYYYYYYYY";
        string merchant_salt = "ZZZZZZZZZZZZZZ";
        //
        // Müşterinizin sitenizde kayıtlı veya form vasıtasıyla aldığınız eposta adresi
        string emailstr = "ZZZZZZZZZZZZZZ";
        //
        // Tahsil edilecek tutar. 9.99 için 9.99 * 100 = 999 gönderilmelidir.
        int payment_amountstr = ;
        //
        // Sipariş numarası: Her işlemde benzersiz olmalıdır!! Bu bilgi bildirim sayfanıza yapılacak bildirimde geri gönderilir.
        string merchant_oid = "";
        //
        // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız ad ve soyad bilgisi
        string user_namestr = "";
        //
        // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız adres bilgisi
        string user_addressstr = "";
        //
        // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız telefon bilgisi
        string user_phonestr = "";
        //
        // Ödeme bekleniyor sayfası sonrası müşterinizin yönlendirileceği sayfa
        // !!! Bu sayfa siparişi onaylayacağınız sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
        // !!! Siparişi onaylayacağız sayfa "Bildirim URL" sayfasıdır (Bakınız: 2.ADIM Klasörü).
        string merchant_pending_url = "http://www.siteniz.com/basarili";
        //        
        // !!! Eğer bu örnek kodu sunucuda değil local makinanızda çalıştırıyorsanız
        // buraya dış ip adresinizi (https://www.whatismyip.com/) yazmalısınız. Aksi halde geçersiz coinpays_token hatası alırsınız.
        string user_ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (user_ip == "" || user_ip == null)
        {
            user_ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        //
        // ÖRNEK user_basket oluşturma - Ürün adedine göre object'leri çoğaltabilirsiniz
        object[][] user_basket = {
            new object[] {"Örnek ürün 1", "18.00", 1}, // 1. ürün (Ürün Ad - Birim Fiyat - Adet)
            new object[] {"Örnek ürün 2", "33.25", 2}, // 2. ürün (Ürün Ad - Birim Fiyat - Adet)
            new object[] {"Örnek ürün 3", "45.42", 1}, // 3. ürün (Ürün Ad - Birim Fiyat - Adet)
            };
        /* ############################################################################################ */

        //
        // Mağaza canlı modda iken test işlem yapmak için 1 olarak gönderilebilir.
        string test_mode = "1";
        //
 	    // Burada sepetinizin hangi para biriminde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için

	    // (https://app.coinpays.io/shared/currencies) adresini ziyaret edin
	    //USD-EUR-TRY-GBP-RUB-CNY-KRW
        string currency = "TRY";

        //
        // Burada ödeme sayfanızın hangi dilde görüntülemek istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
	    // (https://app.coinpays.io/shared/languages) adresini ziyaret edin
        string lang = "en";//tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

        // Burada ödemenizi hangi kripto para birimi ile almak istediğinizi seçebilirsiniz. Aşağıda örnek değerler mevcut. Son güncel değerlere erişmek için
        // (https://app.coinpays.io/shared/coins) adresini ziyaret edin. Değer olarak "code" gelecektir.
        string coin = "ETH";

        // Burada yukarıda seçtiğiniz kripto para biriminin hangi ağda alınacağını seçebilirsiniz.
        string network = "BEP-20";

        // Gönderilecek veriler oluşturuluyor
        NameValueCollection data = new NameValueCollection();
        data["merchant_id"] = merchant_id;
        data["user_ip"] = user_ip;
        data["merchant_oid"] = merchant_oid;
        data["email"] = emailstr;
        data["payment_amount"] = payment_amountstr.ToString();
        //
        // Sepet içerği oluşturma fonksiyonu, değiştirilmeden kullanılabilir.
        JavaScriptSerializer ser = new JavaScriptSerializer();
        string user_basket_json = ser.Serialize(user_basket);
        string user_basketstr = Convert.ToBase64String(Encoding.UTF8.GetBytes(user_basket_json));
        data["user_basket"] = user_basketstr;
        //
        // Token oluşturma fonksiyonu, değiştirilmeden kullanılmalıdır.
        string Birlestir = string.Concat(merchant_id, user_ip, merchant_oid, emailstr, payment_amountstr.ToString(), user_basketstr);
        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
        byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
        data["coinpays_token"] = Convert.ToBase64String(b);
        //
        data["test_mode"] = test_mode;
        data["user_name"] = user_namestr;
        data["network"] = network;
        data["coin"] = coin;
        data["user_address"] = user_addressstr;
        data["user_phone"] = user_phonestr;
        data["merchant_pending_url"] = merchant_pending_url;
        data["currency"] = currency;
        data["lang"] = lang;

        using (WebClient client = new WebClient())
        {
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] result = client.UploadValues("https://app.coinpays.io/api/create-payment", "POST", data);
            string ResultAuthTicket = Encoding.UTF8.GetString(result);
            dynamic json = JValue.Parse(ResultAuthTicket);

            if (json.status == "success")
            { 
                // "json" objesi içerisinde dönen datayı bulabilirsiniz
            }
            else
            {
                Response.Write("COINPAYS DIRECT API failed. reason:" + json.reason + "");
            }
        }
    }
}

}



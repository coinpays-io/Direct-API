// Sample codes for STEP 1

using Newtonsoft.Json.Linq; // If you receive an error on this line, create a folder named bin in the section where your site files are located and copy the DLL file named Newtonsoft.Json.dll into it.
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

        // ####################### REQUIRED FIELDS #######################
        //
        // API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
        string merchant_id = "XXXXXX";
        string merchant_key = "YYYYYYYYYYYYYY";
        string merchant_salt = "ZZZZZZZZZZZZZZ";
        //
        // Your customer's email address registered on your site or received via the form
        string emailstr = "ZZZZZZZZZZZZZZ";
        //
        // Amount to be collected. For 9.99, 9.99 * 100 = 999 should be sent.
        int payment_amountstr = ;
        //
        // Order number: Must be unique for every transaction!! This information is sent back in the notification to your notification page.
        string merchant_oid = "";
        //
        // Your customer's name and surname information registered on your site or obtained through the form
        string user_namestr = "";
        //
        // Your customer's address information registered on your site or received through the form
        string user_addressstr = "";
        //
        // Your customer's phone number registered on your site or received via the form
        string user_phonestr = "";
        //
        // The page your customer will be directed to after the payment waiting page
        // !!! This page is not the page where you will confirm the order! This is the page where you will only inform your customer!
        // !!! The page where we will confirm the order is the "Notification URL" page (See: STEP 2 Folder).
        string merchant_pending_url = "http://www.siteniz.com/basarili";
        //        
        // !!! If you are running this sample code on your local machine, not on the server
        // You should write your external IP address (https://www.whatismyip.com/) here. Otherwise you will get an invalid coinpays_token error.
        string user_ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (user_ip == "" || user_ip == null)
        {
            user_ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        //
        // EXAMPLE Creating user_basket - You can multiply objects according to the number of products
        object[][] user_basket = {
            new object[] {"Example product 1", "18.00", 1}, // 1. product (product name - per price - quantity)
            new object[] {"Example product 2", "33.25", 2}, // 2. product (product name - per price - quantity)
            new object[] {"Example product 3", "45.42", 1}, // 3. product (product name - per price - quantity)
            };
        /* ############################################################################################ */

        //
        // It can be sent as 1 for testing when the store is in live mode.
        string test_mode = "1";
        //
 	    // Here you can choose in which currency you would like to display your cart. Below are sample values. To access the last updated values

	    // Visit (https://app.coinpays.io/shared/currencies)
	    //USD-EUR-TRY-GBP-RUB-CNY-KRW
        string currency = "TRY";

        //
        // Here you can choose which language you would like to display your payment page in. Below are sample values. To access the last updated values
	    // Visit (https://app.coinpays.io/shared/languages)
        string lang = "en";//tr-en-de-fr-es-kr-jp-ar-ru-cn-id-ua

        // Here you can choose which cryptocurrency you would like to receive your payment with. Below are sample values. To access the last updated values
        // Visit (https://app.coinpays.io/shared/coins?merchant_key={merchant_key}&merchant_id={$merchant_id}&merchant_salt={merchant_salt}). The value will be "code".
        string coin = "ETH";

        // Here you can choose which network to receive the cryptocurrency you chose above.
        string network = "BEP-20";

        // Data to be sent is being created
        NameValueCollection data = new NameValueCollection();
        data["merchant_id"] = merchant_id;
        data["user_ip"] = user_ip;
        data["merchant_oid"] = merchant_oid;
        data["email"] = emailstr;
        data["payment_amount"] = payment_amountstr.ToString();
        //
        // The cart content creation function can be used without modification.
        JavaScriptSerializer ser = new JavaScriptSerializer();
        string user_basket_json = ser.Serialize(user_basket);
        string user_basketstr = Convert.ToBase64String(Encoding.UTF8.GetBytes(user_basket_json));
        data["user_basket"] = user_basketstr;
        //
        // The token creation function must be used without modification.
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
                // You can find the returned data in the "json" object
            }
            else
            {
                Response.Write("COINPAYS DIRECT API failed. reason:" + json.reason + "");
            }
        }
    }
}

}



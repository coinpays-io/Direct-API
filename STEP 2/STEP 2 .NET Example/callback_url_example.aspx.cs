// Sample codes for STEP 2

// IMPORTANT WARNINGS!
// 1) You cannot move data to this page via session (SESSION). Because this page is not a page to which customers are directed.
// 2) The merchant_oid value you sent in STEP 1 of the integration comes to this page via POST. Using this value
// You must identify the relevant order from your database and confirm or cancel it.
// 3) More than one notification may arrive for the same order (due to network connection problems, etc.). Therefore first of all
// check the status of the order in your database, if confirmed, do not process again. An example is below.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class bildirim_url_ornek : System.Web.UI.Page {

    // ####################### REQUIRED FIELDS #######################
    //
    // API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
    string merchant_key     = "YYYYYYYYYYYYYY";
    string merchant_salt    = "ZZZZZZZZZZZZZZ";
    // ###########################################################################

    protected void Page_Load(object sender, EventArgs e) {

        // ####### You do not need to make any changes in this section. #######
        // 
        // Generate hash with POST values.
        string merchant_oid = Request.Form["merchant_oid"];
        string status = Request.Form["status"];
        string total_amount = Request.Form["total_amount"];
        string hash = Request.Form["hash"];

        string Birlestir = string.Concat(merchant_oid, merchant_salt, status, total_amount);
        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
        byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
        string token = Convert.ToBase64String(b);

        //
        // Compare the generated hash with the hash in the post from CoinPays (to ensure that the request came from CoinPays and has not changed)
         // If you do not do this, you may incur financial loss.
        if (hash.ToString() != token) {
            Response.Write("COINPAYS notification failed: bad hash");
            return;
            }

        //###########################################################################
        
        // WHAT TO DO HERE
         // 1) Query the status of the order from your database using $post['merchant_oid'] value.
         // 2) If the order has already been confirmed or canceled echo "OK"; exit; Finish by doing .

        if (status == "success") { //Payment Confirmed

            // Notify the CoinPays system that the notification was received.
            Response.Write("OK");
            // WHAT NEEDS TO BE DONE HERE ARE THE APPROVAL PROCEDURES.
             // 1) Confirm the order.
             // 2) During the iframe call step, you can lose merchant_oid and other information to your database and compare them at this stage, retrieve the information, if any, and perform automatic order completion.
             // 2) If you want to inform your customer via message / SMS / e-mail, you can do so at this stage. In this process, you can record the merchant_oid information in the iframe call step and access the data by querying it at this stage.
             // 3) payment_amount order amount sent in STEP 1, in case of shopping in installments
             //may change. You can use the current amount in your accounting transactions by taking it from the Request.Form['total_amount'] value.
            } else { //Payment Not Approved
        
            // Notify the CoinPays system that the notification has been received.
            Response.Write("OK");
            // WHAT TO DO HERE
            // 1) Cancel the order.
            // 2) If you are going to record the reason why the payment is not approved, you can use the values below.
            // $post['failed_reason_code'] - failed error code
            // $post['failed_reason_msg'] - failed error message
            }          
    }
}
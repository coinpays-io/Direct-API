<?php

## Sample codes for STEP 2 ##

## IMPORTANT WARNINGS ##
## 1) You cannot move data to this page via session (SESSION). Because this page is not a page to which customers are directed.
## 2) The merchant_oid value you sent in STEP 1 of the integration is sent to this page via POST. Using this value
## You must identify the relevant order from your database and confirm or cancel it.
## 3) More than one notification may arrive for the same order (due to network connection problems, etc.). Therefore first of all
## check the status of the order in your database, if confirmed, do not process again. An example is below.

	$post = $_POST;

	####################### REQUIRED FIELDS #######################
	#
	## API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
	$merchant_key 	= 'YYYYYYYYYYYYYY';
	$merchant_salt	= 'ZZZZZZZZZZZZZZ';
	###########################################################################

	####### You do not need to make any changes in this section. #######
	#
	## Generate hash with POST values.
	$hash = base64_encode( hash_hmac('sha256', $post['merchant_oid'].$merchant_salt.$post['status'].$post['total_amount'], $merchant_key, true) );
	#
## Compare the generated hash with the hash in the post from CoinPays (to ensure that the request came from CoinPays and has not changed)
## If you do not do this, you may incur financial damage.
	if( $hash != $post['hash'] )
		die('COINPAYS notification failed: bad hash');
	###########################################################################

## WHAT TO DO HERE
## 1) Query the status of the order from your database using $post['merchant_oid'] value.
## 2) If the order has already been confirmed or canceled echo "OK"; exit; Finish by doing .

/* Order status query example
        $state = SQL
        if($state == "approval" || $state == "cancel"){
            echo "OK";
            exit;
        }
*/

	if( $post['status'] == 'success' ) { ## Ödeme Onaylandı

## WHAT TO DO HERE
## 1) Confirm the order.
## 2) If you are going to inform your customer such as message / SMS / e-mail, you should do so at this stage.
## 3) payment_amount order amount sent in STEP 1, in case of shopping in installments
## subject to change. You can use the current amount in your accounting transactions by taking the $post['total_amount'] value.

	} else { ## Ödemeye Onay Verilmedi

## WHAT TO DO HERE
## 1) Cancel the order.
## 2) If you are going to record the reason why the payment is not approved, you can use the values below.
## $post['failed_reason_code'] - failed error code
## $post['failed_reason_msg'] - failed error message

	}

	## Notify the CoinPays system that the notification has been received.
	echo "OK";
	exit;
?>
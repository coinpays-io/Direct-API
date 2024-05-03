# Python 3.6+
# Prepared with reference to Django Web Framework
# Sample codes for STEP 2
"""
IMPORTANT WARNINGS
1) You cannot transfer data to this page via session (SESSION). Because this page is not a page to which customers are directed.
2) The merchant_oid value you sent in STEP 1 of the integration is sent to this page via POST. Using this value, you must identify the relevant order from your database and confirm or cancel it.
3) More than one notification may arrive for the same order (due to network connection problems, etc.). Therefore, first check the status of the order in your database, if it is confirmed, do not process it again. An example is below.
"""

import base64
import hashlib
import hmac

from django.shortcuts import render, HttpResponse
from django.views.decorators.csrf import csrf_exempt

@csrf_exempt
def callback(request):

    if request.method != 'POST':
        return HttpResponse(str(''))

    post = request.POST

    # API Integration Information - You can get it from the INFORMATION page by logging into the store panel.
    merchant_key = b'YYYYYYYYYYYYYY'
    merchant_salt = 'ZZZZZZZZZZZZZZ'

    # You do not need to make any changes in this section.
     # Create hash with POST values.
    hash_str = post['merchant_oid'] + merchant_salt + post['status'] + post['total_amount']
    hash = base64.b64encode(hmac.new(merchant_key, hash_str.encode(), hashlib.sha256).digest())

    # Compare the generated hash with the hash in the post from CoinPays
     # (to ensure that the request came from CoinPays and has not changed)
     # If you do not do this, you may incur financial damage.
    if hash != post['hash']:
        return HttpResponse(str('COINPAYS notification failed: bad hash'))

    # WHAT TO DO HERE
     #1) Query the status of the order from your database using the post['merchant_oid'] value.
     #2) If the order has already been confirmed or canceled, finalize it by clicking "OK".

    if post['status'] == 'success':  # Payment Confirmed
        """
         THINGS TO DO HERE
         1) Confirm the order.
         2) If you are going to inform your customer such as message / SMS / e-mail, you should do so at this stage.
         3) The payment_amount order amount sent in STEP 1 may change if shopping in installments.
         You can use the current amount in your accounting transactions by taking the post['total_amount'] value.
         """
        print(request)
    else:  # Payment Not Approved
        """
         THINGS TO DO HERE
         1) Cancel the order.
         2) If you are going to record the reason why the payment is not approved, you can use the values below.
         post['failed_reason_code'] - failed error code
         post['failed_reason_msg'] - failed error message
         """
        print(request)

    # Notify the CoinPays system that the notification has been received.
    return HttpResponse(str('OK'))

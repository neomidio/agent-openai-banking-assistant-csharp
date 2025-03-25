using Microsoft.SemanticKernel;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Transactions;
public class PaymentMockPlugin
{
    [KernelFunction("payBill")]
    [Description("Submits a payment for a bill with the amount, documentId and recipientname.")]
    public string SubmitBillPayment(string recipientName, string documentId, string amount)
    {
        Console.WriteLine($"--> Bill payment executed for recipient: {recipientName} with documentId: {documentId} and amount: {amount}");
        return "Payment Successful";
    }
}


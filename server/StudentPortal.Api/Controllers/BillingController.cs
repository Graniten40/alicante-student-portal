using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/billing")]
public class BillingController : ControllerBase
{
    [HttpPost("payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent()
    {
        var service = new PaymentIntentService();

        var intent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = 1000, // 10.00 USD (test)
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        });

        return Ok(new
        {
            clientSecret = intent.ClientSecret
        });
    }
}

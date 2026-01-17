using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StudentPortal.Api.Controllers;

[ApiController]
[Route("api/billing")]
public class BillingController : ControllerBase
{
    public record CreatePaymentIntentRequest(string PackageId);

    [HttpPost("payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentRequest request)
    {
        // 🔐 Backend bestämmer alltid priset
        var packages = new Dictionary<string, long>
        {
            // Stripe använder "minor units"
            // USD → cents
            ["landing_week"] = 19900,   // $199.00
            ["full_support"] = 49900,   // $499.00
            ["deposit"]      = 5000     // $50.00
        };

        if (!packages.TryGetValue(request.PackageId, out var amount))
            return BadRequest("Invalid package");

        var service = new PaymentIntentService();

        var intent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount,
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

import { Component, OnInit, AfterViewInit } from '@angular/core';
import { loadStripe, Stripe, StripeElements } from '@stripe/stripe-js';
import { BillingService } from '../../services/billing.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-checkout',
  standalone: true,
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent implements OnInit, AfterViewInit {
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;

  clientSecret = '';
  status = '';

  // ✅ Product.Slug från din seed
  packageId = 'landing-week';

  private viewReady = false;
  private mounted = false;

  constructor(private billing: BillingService) {}

  async ngOnInit() {
    console.log('Sending packageId:', this.packageId); // ✅ rätt plats

    this.status = 'Loading payment...';

    this.stripe = await loadStripe(environment.stripePk, { locale: 'en' });
    if (!this.stripe) {
      this.status = 'Stripe could not be loaded. Check stripePk in environment.';
      return;
    }
    console.log('packageId being sent:', this.packageId);

    this.billing.createPaymentIntent(this.packageId).subscribe({
      next: (res) => {
        this.clientSecret = res.clientSecret;
        this.status = '';
        this.tryMount();
      },
      error: (err) => {
        console.error('createPaymentIntent failed', err);

        const msg =
          err?.error && typeof err.error === 'string'
            ? err.error
            : 'Could not start payment (backend error).';

        this.status = msg;
      }
    });
  }

  ngAfterViewInit() {
    this.viewReady = true;
    this.tryMount();
  }

  private tryMount() {
    if (this.mounted) return;
    if (!this.viewReady) return;
    if (!this.stripe) return;
    if (!this.clientSecret) return;

    this.elements = this.stripe.elements({ clientSecret: this.clientSecret });

    const paymentElement = this.elements.create('payment');
    paymentElement.mount('#payment-element');

    this.mounted = true;
  }

  async pay() {
    if (!this.stripe || !this.elements) {
      this.status = 'Payment form not ready yet.';
      return;
    }

    this.status = 'Processing payment...';

    const result = await this.stripe.confirmPayment({
      elements: this.elements,
      confirmParams: {
        return_url: window.location.origin + '/payment-result'
      }
    });

    if (result.error) {
      this.status = result.error.message ?? 'Payment failed';
    }
  }
}

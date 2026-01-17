import { Component, OnInit } from '@angular/core';
import { loadStripe, Stripe } from '@stripe/stripe-js';
import { BillingService } from '../../services/billing.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-checkout',
  standalone: true,
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent implements OnInit {
  stripe!: Stripe;
  clientSecret = '';
  status = '';

  constructor(private billing: BillingService) {}

  async ngOnInit() {
    this.stripe = await loadStripe(environment.stripePk, { locale: 'en' }) as Stripe;
    this.billing.createPaymentIntent().subscribe(res => {
      this.clientSecret = res.clientSecret;
      this.mountElement();
    });
  }

  async mountElement() {
    const elements = this.stripe.elements({
      clientSecret: this.clientSecret
    });

    const paymentElement = elements.create('payment');
    paymentElement.mount('#payment-element');
  }

  async pay() {
    this.status = 'Processing payment...';

    const result = await this.stripe.confirmPayment({
      elements: this.stripe.elements(),
      confirmParams: {
        return_url: window.location.origin + '/payment-result'
      }
    });

    if (result.error) {
      this.status = result.error.message ?? 'Payment failed';
    }
  }
}

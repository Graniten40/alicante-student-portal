import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-payment-result',
  standalone: true,
  template: `
    <div style="padding:24px">
      <h1>Payment result</h1>

      <p>
        Status:
        <strong>{{ status }}</strong>
      </p>

      <p *ngIf="paymentIntent">
        PaymentIntent: {{ paymentIntent }}
      </p>

      <p *ngIf="clientSecret">
        Client secret (short): {{ clientSecretPreview }}
      </p>

      <p *ngIf="!status || status === 'unknown'">
        Thanks! Checking payment status...
      </p>
    </div>
  `
})
export class PaymentResultComponent {
  status = 'unknown';
  paymentIntent: string | null = null;
  clientSecret: string | null = null;

  constructor(route: ActivatedRoute) {
    route.queryParamMap.subscribe(params => {
      this.status = params.get('redirect_status') ?? 'unknown';
      this.paymentIntent = params.get('payment_intent');
      this.clientSecret = params.get('payment_intent_client_secret');
    });
  }

  get clientSecretPreview(): string {
    if (!this.clientSecret) return '';
    // visa bara början/slutet så du inte råkar logga hela
    const s = this.clientSecret;
    return `${s.slice(0, 10)}...${s.slice(-6)}`;
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BillingService {

  constructor(private http: HttpClient) {}

  createPaymentIntent(packageId: string) {
    return this.http.post<{ clientSecret: string }>(
      `${environment.apiBaseUrl}/api/billing/payment-intent`,
      { packageId }
    );
  }
}

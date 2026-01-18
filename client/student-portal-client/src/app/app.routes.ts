import { Routes } from '@angular/router';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { PaymentResultComponent } from './pages/payment-result/payment-result.component';

export const routes: Routes = [
  { path: '', redirectTo: 'checkout', pathMatch: 'full' },

  { path: 'checkout', component: CheckoutComponent },

  // ✅ Stripe redirect landar här
  { path: 'payment-result', component: PaymentResultComponent },

  // (valfritt) fallback
  { path: '**', redirectTo: 'checkout' }
];

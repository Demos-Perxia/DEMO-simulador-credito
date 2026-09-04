import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';

export type SimulationMode = 'AMOUNT' | 'INSTALLMENT_CAPACITY';

export interface Activity {
  id: string;
  name: string;
}

export interface Agreement {
  id: string;
  name: string;
}

export interface SimulationAlternative {
  termInMonths: number;
  amountCop: number;
  monthlyInstallmentCop: number;
  totalEstimatedCop: number;
  estimatedInterestCop: number;
}

export interface SimulationResponse {
  mode: SimulationMode;
  effectiveAnnualRate: number;
  effectiveMonthlyRate: number;
  maximumPaymentCapacityCop: number;
  alternatives: SimulationAlternative[];
  disclaimer: string;
}

export interface SimulationRequest {
  activityId: string;
  agreementId: string;
  mode: SimulationMode;
  monthlyIncomeCop: number;
  payrollDeductionsCop: number;
  requestedAmountCop: number | null;
  maximumInstallmentCop: number | null;
}

@Injectable({ providedIn: 'root' })
export class CreditApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;
  private readonly activities$ = this.http.get<Activity[]>(`${this.baseUrl}/catalogs/activities`).pipe(shareReplay({ bufferSize: 1, refCount: true }));

  getActivities(): Observable<Activity[]> {
    return this.activities$;
  }

  getAgreements(activityId: string): Observable<Agreement[]> {
    return this.http.get<Agreement[]>(`${this.baseUrl}/catalogs/activities/${activityId}/agreements`);
  }

  simulate(request: SimulationRequest): Observable<SimulationResponse> {
    return this.http.post<SimulationResponse>(`${this.baseUrl}/credit-simulations`, request);
  }
}

import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { App } from './app';

const apiBase = 'https://localhost:7040/api/v1';
const activities = [{ id: 'teacher', name: 'Docente' }, { id: 'pensioner', name: 'Pensionado' }];
const agreements = [{ id: 'teacher-public', name: 'Docentes sector público' }];
const response = {
  mode: 'AMOUNT' as const,
  effectiveAnnualRate: 0.18,
  effectiveMonthlyRate: 0.013888,
  maximumPaymentCapacityCop: 1_200_000,
  disclaimer: 'Resultado informativo',
  alternatives: [
    { termInMonths: 60, amountCop: 20_000_000, monthlyInstallmentCop: 500_000, totalEstimatedCop: 30_000_000, estimatedInterestCop: 10_000_000 },
    { termInMonths: 120, amountCop: 20_000_000, monthlyInstallmentCop: 300_000, totalEstimatedCop: 36_000_000, estimatedInterestCop: 16_000_000 }
  ]
};

describe('App', () => {
  let fixture: ComponentFixture<App>;
  let component: App;
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne(`${apiBase}/catalogs/activities`).flush(activities);
  });

  afterEach(() => http.verify());

  it('shows the configured terms with zero values before a calculation', () => {
    expect(component.displayedAlternatives().map(item => item.termInMonths)).toEqual([60, 72, 96, 108, 120]);
    expect(component.displayedAlternatives().every(item => item.monthlyInstallmentCop === 0)).toBeTrue();
  });

  it('formats currency inputs with Colombian thousands separators', () => {
    const input = document.createElement('input');
    input.value = '3500000';

    component.updateCurrency('monthlyIncomeCop', { target: input } as unknown as Event);

    expect(component.form.controls.monthlyIncomeCop.value).toBe(3_500_000);
    expect(input.value).toBe('3.500.000');
  });

  it('loads activities and reloads compatible agreements when activity changes', () => {
    component.form.controls.activityId.setValue('teacher');
    const request = http.expectOne(`${apiBase}/catalogs/activities/teacher/agreements`);
    expect(request.request.method).toBe('GET');
    request.flush(agreements);

    expect(component.agreements()).toEqual(agreements);
    expect(component.form.controls.agreementId.enabled).toBeTrue();
  });

  it('switches mode, clears the obsolete target, and invalidates current results', () => {
    component.result.set(response);
    component.selectedTerm.set(60);
    component.form.controls.requestedAmountCop.setValue(20_000_000);

    component.selectMode('INSTALLMENT_CAPACITY');

    expect(component.mode()).toBe('INSTALLMENT_CAPACITY');
    expect(component.form.controls.requestedAmountCop.value).toBeNull();
    expect(component.result()).toBeNull();
    expect(component.planValueLabel()).toBe('Capacidad máxima');
  });

  it('submits an amount simulation and selects an alternative from the API response', () => {
    component.form.setValue({
      mode: 'AMOUNT',
      activityId: 'teacher',
      agreementId: 'teacher-public',
      monthlyIncomeCop: 4_000_000,
      payrollDeductionsCop: 300_000,
      requestedAmountCop: 20_000_000,
      maximumInstallmentCop: null
    }, { emitEvent: false });

    component.simulate();
    const request = http.expectOne(`${apiBase}/credit-simulations`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body.mode).toBe('AMOUNT');
    request.flush(response);

    expect(component.result()).toEqual(response);
    expect(component.selectedAlternative().termInMonths).toBe(120);
    component.selectAlternative(response.alternatives[0]);
    expect(component.selectedAlternative().termInMonths).toBe(60);
  });

  it('surfaces API validation errors without clearing the form', () => {
    component.form.setValue({
      mode: 'AMOUNT',
      activityId: 'teacher',
      agreementId: 'teacher-public',
      monthlyIncomeCop: 4_000_000,
      payrollDeductionsCop: 300_000,
      requestedAmountCop: 20_000_000,
      maximumInstallmentCop: null
    }, { emitEvent: false });

    component.simulate();
    http.expectOne(`${apiBase}/credit-simulations`).flush(
      { detail: 'Requested amount is outside the configured range.' },
      { status: 400, statusText: 'Bad Request' }
    );

    expect(component.error()).toContain('Requested amount');
    expect(component.form.controls.requestedAmountCop.value).toBe(20_000_000);
  });
});

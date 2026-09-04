import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, distinctUntilChanged, filter, of, startWith, switchMap, tap } from 'rxjs';
import {
  Activity,
  Agreement,
  CreditApiService,
  SimulationAlternative,
  SimulationMode,
  SimulationResponse
} from './core/credit-api.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly api = inject(CreditApiService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  readonly activities = signal<Activity[]>([]);
  readonly agreements = signal<Agreement[]>([]);
  readonly loadingAgreements = signal(false);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly result = signal<SimulationResponse | null>(null);
  readonly selectedTerm = signal<number | null>(null);

  readonly form = this.fb.nonNullable.group({
    mode: this.fb.nonNullable.control<SimulationMode>('AMOUNT'),
    activityId: this.fb.nonNullable.control('', Validators.required),
    agreementId: this.fb.nonNullable.control({ value: '', disabled: true }, Validators.required),
    monthlyIncomeCop: this.fb.nonNullable.control<number | null>(null, [Validators.required, Validators.min(1)]),
    payrollDeductionsCop: this.fb.nonNullable.control<number | null>(0, [Validators.required, Validators.min(0)]),
    requestedAmountCop: this.fb.nonNullable.control<number | null>(null),
    maximumInstallmentCop: this.fb.nonNullable.control<number | null>(null)
  });

  readonly mode = signal<SimulationMode>('AMOUNT');
  readonly termsInMonths = [120, 108, 96, 72, 60];
  readonly displayedAlternatives = computed(() => this.result()?.alternatives ?? this.termsInMonths.map(termInMonths => ({
    termInMonths,
    amountCop: 0,
    monthlyInstallmentCop: 0,
    totalEstimatedCop: 0,
    estimatedInterestCop: 0
  })));
  readonly targetLabel = computed(() => this.mode() === 'AMOUNT' ? 'Monto solicitado' : 'Cuota máxima');
  readonly planValueLabel = computed(() => this.mode() === 'AMOUNT' ? 'Cuota máxima' : 'Monto estimado');
  readonly alternativeValue = computed(() => (alternative: SimulationAlternative) => this.mode() === 'AMOUNT' ? alternative.monthlyInstallmentCop : alternative.amountCop);
  readonly selectedAlternative = computed(() => {
    const alternatives = this.result()?.alternatives ?? [];
    return alternatives.find(item => item.termInMonths === this.selectedTerm()) ?? alternatives[0] ?? null;
  });

  constructor() {
    this.loadActivities();
    this.form.controls.activityId.valueChanges.pipe(
      distinctUntilChanged(),
      tap(() => {
        this.form.controls.agreementId.reset({ value: '', disabled: true });
        this.agreements.set([]);
        this.invalidateResult();
      }),
      filter(Boolean),
      tap(() => this.loadingAgreements.set(true)),
      switchMap(activityId => this.api.getAgreements(activityId).pipe(catchError(() => of([] as Agreement[])))),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(agreements => {
      this.agreements.set(agreements);
      this.form.controls.agreementId.enable();
      this.loadingAgreements.set(false);
    });

    this.form.controls.mode.valueChanges.pipe(startWith('AMOUNT' as SimulationMode), takeUntilDestroyed(this.destroyRef)).subscribe(mode => {
      this.mode.set(mode);
      this.updateTargetValidation(mode);
      this.invalidateResult();
    });

    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.invalidateResult());
    this.updateTargetValidation('AMOUNT');
  }

  selectMode(mode: SimulationMode): void {
    this.form.controls.mode.setValue(mode);
  }

  selectAlternative(alternative: SimulationAlternative): void {
    this.selectedTerm.set(alternative.termInMonths);
  }

  formatCurrency(value: number | null): string {
    return value === null ? '' : new Intl.NumberFormat('es-CO').format(value);
  }

  updateCurrency(controlName: 'monthlyIncomeCop' | 'payrollDeductionsCop' | 'requestedAmountCop' | 'maximumInstallmentCop', event: Event): void {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '');
    const value = digits === '' ? null : Number(digits);
    this.form.controls[controlName].setValue(value);
    input.value = this.formatCurrency(value);
  }

  simulate(): void {
    this.error.set('');
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.error.set('Completa actividad, convenio, ingresos y el valor a simular antes de calcular las alternativas.');
      return;
    }

    const value = this.form.getRawValue();
    this.loading.set(true);
    this.api.simulate({
      activityId: value.activityId,
      agreementId: value.agreementId,
      mode: value.mode,
      monthlyIncomeCop: value.monthlyIncomeCop ?? 0,
      payrollDeductionsCop: value.payrollDeductionsCop ?? 0,
      requestedAmountCop: value.mode === 'AMOUNT' ? value.requestedAmountCop : null,
      maximumInstallmentCop: value.mode === 'INSTALLMENT_CAPACITY' ? value.maximumInstallmentCop : null
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: result => {
        this.result.set(result);
        this.selectedTerm.set(result.alternatives.at(-1)?.termInMonths ?? null);
        this.loading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.error.set(error.error?.detail ?? 'No fue posible realizar la simulación. Verifica que la API esté disponible.');
      }
    });
  }

  retryActivities(): void {
    this.loadActivities();
  }

  private loadActivities(): void {
    this.api.getActivities().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: activities => this.activities.set(activities),
      error: () => this.error.set('No fue posible cargar las actividades. Intenta nuevamente.')
    });
  }

  private updateTargetValidation(mode: SimulationMode): void {
    const target = mode === 'AMOUNT' ? this.form.controls.requestedAmountCop : this.form.controls.maximumInstallmentCop;
    const other = mode === 'AMOUNT' ? this.form.controls.maximumInstallmentCop : this.form.controls.requestedAmountCop;
    target.setValidators([Validators.required, Validators.min(1)]);
    other.clearValidators();
    other.setValue(null, { emitEvent: false });
    target.updateValueAndValidity({ emitEvent: false });
    other.updateValueAndValidity({ emitEvent: false });
  }

  private invalidateResult(): void {
    if (this.result()) {
      this.result.set(null);
      this.selectedTerm.set(null);
    }
  }
}

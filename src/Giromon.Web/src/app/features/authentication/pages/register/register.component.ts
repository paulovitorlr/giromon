import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { BrandComponent } from '../../../../shared/components/brand/brand.component';

@Component({
  selector: 'app-register', imports: [ReactiveFormsModule, RouterLink, BrandComponent],
  templateUrl: './register.component.html', styleUrl: '../auth-page.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly showPassword = signal(false);
  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true); this.errorMessage.set('');
    this.authService.register(this.form.getRawValue()).pipe(finalize(() => this.isLoading.set(false))).subscribe({
      next: () => void this.router.navigate(['/entrar'], { state: { registered: true } }),
      error: (error: HttpErrorResponse) => this.errorMessage.set(error.error?.message ?? 'Não foi possível criar sua conta.')
    });
  }
}

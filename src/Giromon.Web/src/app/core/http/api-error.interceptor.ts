import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthStore } from '../auth/auth.store';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const store = inject(AuthStore);
  const router = inject(Router);
  return next(request).pipe(catchError((error: HttpErrorResponse) => {
    if (error.status === 401 && !request.url.endsWith('/api/auth/login')) {
      store.clearSession();
      void router.navigate(['/entrar']);
    }
    return throwError(() => error);
  }));
};

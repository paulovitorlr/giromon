import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthStore } from './auth.store';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthStore).session()?.accessToken;
  return token ? next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })) : next(request);
};

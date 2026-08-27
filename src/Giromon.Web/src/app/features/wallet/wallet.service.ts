import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DepositResponse, Wallet, WalletTransaction } from './models/wallet.models';

@Injectable({ providedIn: 'root' })
export class WalletService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/wallet`;
  getWallet(): Observable<Wallet> { return this.http.get<Wallet>(this.url); }
  deposit(amount: number): Observable<DepositResponse> { return this.http.post<DepositResponse>(`${this.url}/deposits`, { amount }); }
  getTransactions(): Observable<WalletTransaction[]> { return this.http.get<WalletTransaction[]>(`${this.url}/transactions`); }
}

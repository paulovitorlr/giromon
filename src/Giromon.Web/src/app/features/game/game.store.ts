import { computed, inject, Injectable, signal } from '@angular/core';
import { forkJoin, finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { GameService } from './game.service';
import { PlaySlotResponse, SlotSymbol } from './models/game.models';
import { WalletService } from '../wallet/wallet.service';
import { WalletTransaction } from '../wallet/models/wallet.models';

@Injectable({ providedIn: 'root' })
export class GameStore {
  private readonly gameService = inject(GameService);
  private readonly walletService = inject(WalletService);
  private readonly balanceState = signal(0);
  private readonly resultState = signal<PlaySlotResponse | null>(null);
  private readonly transactionsState = signal<WalletTransaction[]>([]);
  readonly balance = this.balanceState.asReadonly();
  readonly result = this.resultState.asReadonly();
  readonly transactions = this.transactionsState.asReadonly();
  readonly isLoading = signal(false);
  readonly isSpinning = signal(false);
  readonly isDepositing = signal(false);
  readonly message = signal('');
  readonly hasWon = computed(() => (this.resultState()?.prizeAmount ?? 0) > 0);

  load(): void {
    this.isLoading.set(true); this.message.set('');
    forkJoin({ wallet: this.walletService.getWallet(), transactions: this.walletService.getTransactions() })
      .pipe(finalize(() => this.isLoading.set(false))).subscribe({
        next: data => { this.balanceState.set(data.wallet.balance); this.transactionsState.set(data.transactions); },
        error: error => this.message.set(this.getMessage(error, 'Não foi possível carregar sua carteira.'))
      });
  }

  play(betAmount: number): void {
    if (this.isSpinning() || betAmount <= 0) return;
    this.isSpinning.set(true); this.message.set(''); this.resultState.set(null);
    const startedAt = Date.now();
    this.gameService.play(betAmount).subscribe({
      next: result => setTimeout(() => {
        this.resultState.set(result); this.balanceState.set(result.balance); this.isSpinning.set(false); this.refreshTransactions();
      }, Math.max(0, 1100 - (Date.now() - startedAt))),
      error: error => { this.message.set(this.getMessage(error, 'O giro falhou. Tente novamente.')); this.isSpinning.set(false); }
    });
  }

  deposit(amount: number): void {
    if (this.isDepositing()) return;
    this.isDepositing.set(true); this.message.set('');
    this.walletService.deposit(amount).pipe(finalize(() => this.isDepositing.set(false))).subscribe({
      next: response => { this.balanceState.set(response.balance); this.message.set(`${amount} créditos adicionados!`); this.refreshTransactions(); },
      error: error => this.message.set(this.getMessage(error, 'Não foi possível adicionar créditos.'))
    });
  }

  previewSymbols(): SlotSymbol[] { return ['Leaf', 'Water', 'Fire']; }
  clearMessage(): void { this.message.set(''); }
  private refreshTransactions(): void { this.walletService.getTransactions().subscribe(items => this.transactionsState.set(items)); }
  private getMessage(error: HttpErrorResponse, fallback: string): string { return error.error?.message ?? fallback; }
}

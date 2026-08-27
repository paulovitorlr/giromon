import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe, DatePipe, KeyValuePipe } from '@angular/common';
import { Router } from '@angular/router';
import { AuthStore } from '../../../../core/auth/auth.store';
import { BrandComponent } from '../../../../shared/components/brand/brand.component';
import { GameStore } from '../../game.store';
import { SlotReelsComponent } from '../../components/slot-reels/slot-reels.component';
import { SlotSymbol, SYMBOLS } from '../../models/game.models';

@Component({
  selector: 'app-slot-machine',
  imports: [BrandComponent, SlotReelsComponent, CurrencyPipe, DatePipe, KeyValuePipe],
  templateUrl: './slot-machine.component.html', styleUrl: './slot-machine.component.scss'
})
export class SlotMachineComponent implements OnInit {
  readonly authStore = inject(AuthStore);
  readonly gameStore = inject(GameStore);
  private readonly router = inject(Router);
  readonly bet = signal(10);
  readonly showDeposit = signal(false);
  readonly showHistory = signal(false);
  readonly betOptions = [5, 10, 25, 50];
  protected readonly symbolMap = SYMBOLS;
  ngOnInit(): void { this.gameStore.load(); }
  currentSymbols(): SlotSymbol[] {
    const result = this.gameStore.result();
    return result ? [result.firstSymbol, result.secondSymbol, result.thirdSymbol] : this.gameStore.previewSymbols();
  }
  play(): void { this.gameStore.play(this.bet()); }
  chooseBet(value: number): void { if (!this.gameStore.isSpinning()) this.bet.set(value); }
  deposit(amount: number): void { this.gameStore.deposit(amount); this.showDeposit.set(false); }
  logout(): void { this.authStore.clearSession(); void this.router.navigate(['/entrar']); }
  transactionLabel(type: string): string { return ({ Deposit:'Depósito', Bet:'Aposta', Prize:'Prêmio' } as Record<string,string>)[type] ?? type; }
}

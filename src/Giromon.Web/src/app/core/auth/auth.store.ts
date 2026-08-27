import { computed, Injectable, signal } from '@angular/core';
import { AuthSession, LoginResponse } from '../../features/authentication/models/auth.models';

const SESSION_KEY = 'giromon.session';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly sessionState = signal<AuthSession | null>(this.restoreSession());
  readonly session = this.sessionState.asReadonly();
  readonly isAuthenticated = computed(() => Boolean(this.sessionState()?.accessToken));
  readonly playerName = computed(() => this.sessionState()?.name ?? 'Treinador');

  setSession(response: LoginResponse): void {
    const session: AuthSession = { ...response };
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    this.sessionState.set(session);
  }
  clearSession(): void {
    localStorage.removeItem(SESSION_KEY);
    this.sessionState.set(null);
  }
  private restoreSession(): AuthSession | null {
    const stored = localStorage.getItem(SESSION_KEY);
    if (!stored) return null;
    try { return JSON.parse(stored) as AuthSession; }
    catch { localStorage.removeItem(SESSION_KEY); return null; }
  }
}

import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PlaySlotResponse } from './models/game.models';

@Injectable({ providedIn: 'root' })
export class GameService {
  private readonly http = inject(HttpClient);
  play(betAmount: number): Observable<PlaySlotResponse> {
    return this.http.post<PlaySlotResponse>(`${environment.apiUrl}/api/games/slot/play`, { betAmount });
  }
}

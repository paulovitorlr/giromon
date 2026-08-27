import { Component, input } from '@angular/core';
import { SlotSymbol, SYMBOLS } from '../../models/game.models';

@Component({
  selector: 'app-slot-reels', templateUrl: './slot-reels.component.html', styleUrl: './slot-reels.component.scss'
})
export class SlotReelsComponent {
  readonly symbols = input.required<SlotSymbol[]>();
  readonly spinning = input(false);
  protected readonly symbolMap = SYMBOLS;
}

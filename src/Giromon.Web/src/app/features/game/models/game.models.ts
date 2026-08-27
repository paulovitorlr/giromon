export type SlotSymbol = 'Leaf' | 'Water' | 'Fire' | 'Lightning' | 'Master';
export interface PlaySlotRequest { betAmount: number; }
export interface PlaySlotResponse { roundId: string; firstSymbol: SlotSymbol; secondSymbol: SlotSymbol; thirdSymbol: SlotSymbol; betAmount: number; prizeAmount: number; balance: number; createdAt: string; }
export interface SymbolView { name: SlotSymbol; icon: string; label: string; multiplier: string; className: string; }
export const SYMBOLS: Record<SlotSymbol, SymbolView> = {
  Leaf: { name:'Leaf', icon:'☘', label:'Folha', multiplier:'2×', className:'leaf' },
  Water: { name:'Water', icon:'◆', label:'Água', multiplier:'3×', className:'water' },
  Fire: { name:'Fire', icon:'♨', label:'Fogo', multiplier:'5×', className:'fire' },
  Lightning: { name:'Lightning', icon:'ϟ', label:'Raio', multiplier:'10×', className:'lightning' },
  Master: { name:'Master', icon:'★', label:'Mestre', multiplier:'20×', className:'master' }
};

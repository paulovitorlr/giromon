export type TransactionType = 'Deposit' | 'Bet' | 'Prize';
export interface Wallet { id: string; balance: number; createdAt: string; }
export interface DepositRequest { amount: number; }
export interface DepositResponse { transactionId: string; type: TransactionType; amount: number; balance: number; createdAt: string; }
export interface WalletTransaction { id: string; type: TransactionType; amount: number; createdAt: string; }

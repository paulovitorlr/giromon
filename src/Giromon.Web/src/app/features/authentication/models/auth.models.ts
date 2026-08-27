export interface LoginRequest { email: string; password: string; }
export interface LoginResponse { userId: string; name: string; email: string; accessToken: string; }
export interface RegisterRequest extends LoginRequest { name: string; }
export interface RegisterResponse { id: string; name: string; email: string; createdAt: string; }
export interface AuthSession { userId: string; name: string; email: string; accessToken: string; }

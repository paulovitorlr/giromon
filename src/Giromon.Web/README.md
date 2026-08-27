# Giromon Web

Front-end SPA do Giromon, desenvolvido em Angular 20 e integrado à API .NET do projeto.

## Funcionalidades

- Cadastro de usuário com validação de formulário.
- Login e persistência da sessão JWT.
- Interceptor que adiciona o token às requisições autenticadas.
- Guards para rotas públicas e protegidas.
- Consulta do saldo da carteira.
- Depósito de créditos fictícios.
- Consulta do histórico de transações.
- Escolha do valor da aposta.
- Giro do caça-níquel com animação e resultado retornado pela API.
- Exibição do prêmio e atualização imediata do saldo.
- Layout responsivo para desktop, tablet e celular.

## Tecnologias

- Angular 20.3
- TypeScript 5.9
- Angular Standalone Components
- Signals para estado local e compartilhado
- Reactive Forms
- HttpClient e interceptors funcionais
- SCSS

Não foi adicionada uma biblioteca externa de gerenciamento de estado. O escopo atual é pequeno e Signals resolve os estados de sessão, carteira e partida com menos complexidade.

## Requisitos

- Node.js 20 ou superior
- npm 10 ou superior
- API do Giromon em execução
- PostgreSQL configurado para a API

## Executar localmente

Na raiz do repositório, inicie primeiro a API:

```powershell
dotnet run --project src/Giromon.Api
```

Em outro terminal, execute o front-end:

```powershell
cd src/Giromon.Web
npm install
npm start
```

Acesse `http://localhost:4200`.

O ambiente de desenvolvimento aponta para a API em `http://localhost:5080`, conforme definido em `src/environments/environment.ts`.

## Configuração da API

Os endereços estão nos arquivos:

```text
src/environments/environment.ts
src/environments/environment.production.ts
```

Antes do deploy, substitua o endereço de exemplo do arquivo de produção:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.seu-dominio.com'
};
```

A API deve aceitar a origem do front em `Cors:AllowedOrigins`.

## Rotas

| Rota | Acesso | Componente | Finalidade |
| --- | --- | --- | --- |
| `/entrar` | Público | `LoginComponent` | Autenticar o usuário |
| `/criar-conta` | Público | `RegisterComponent` | Cadastrar um usuário |
| `/jogar` | Protegido | `SlotMachineComponent` | Carteira e caça-níquel |

Usuários não autenticados são redirecionados para `/entrar`. Usuários autenticados que tentarem abrir as páginas de acesso são enviados para `/jogar`.

## Endpoints utilizados

| Método | Endpoint | Uso |
| --- | --- | --- |
| `POST` | `/api/users/register` | Cadastro |
| `POST` | `/api/auth/login` | Login e obtenção do JWT |
| `GET` | `/api/wallet` | Saldo atual |
| `POST` | `/api/wallet/deposits` | Créditos fictícios |
| `GET` | `/api/wallet/transactions` | Histórico |
| `POST` | `/api/games/slot/play` | Realizar giro |

## Arquitetura

O projeto usa organização **feature-first**:

```text
src/app/
├── core/
│   ├── auth/                 # sessão, guards e interceptor JWT
│   └── http/                 # tratamento global de erros HTTP
├── features/
│   ├── authentication/       # cadastro e login
│   ├── game/                 # máquina, partida e estado do jogo
│   └── wallet/               # saldo, depósito e transações
├── shared/
│   └── components/           # componentes reutilizáveis
├── app.config.ts
└── app.routes.ts
```

Essa separação mantém juntos os arquivos que mudam pela mesma razão. Por exemplo, modelos, serviço, store e componentes do jogo ficam em `features/game`, em vez de serem distribuídos por pastas genéricas.

### Fluxo de autenticação

1. O formulário envia e-mail e senha para a API.
2. A resposta é salva pelo `AuthStore` no `localStorage`.
3. O `authInterceptor` adiciona `Authorization: Bearer <token>` às requisições.
4. O `authGuard` protege a arena.
5. Uma resposta `401` limpa a sessão e redireciona para o login.

O armazenamento local é adequado ao protótipo. Em um produto real, recomenda-se avaliar cookies `HttpOnly`, proteção contra XSS e renovação de sessão.

### Estado do jogo

O `GameStore` centraliza:

- saldo atual;
- último resultado;
- transações;
- carregamento inicial;
- estado da animação de giro;
- depósito em andamento;
- mensagens da interface.

O resultado e o saldo sempre vêm da API. O navegador não calcula prêmios e não sorteia o resultado real.

## Símbolos e pagamentos

| Símbolo da API | Nome visual | Multiplicador |
| --- | --- | --- |
| `Leaf` | Folha | 2× |
| `Water` | Água | 3× |
| `Fire` | Fogo | 5× |
| `Lightning` | Raio | 10× |
| `Master` | Mestre | 20× |

O multiplicador é apresentado apenas como informação. O cálculo oficial continua no domínio da API.

## Build de produção

```powershell
cd src/Giromon.Web
npm run build
```

Os arquivos são gerados em:

```text
dist/giromon-web/browser
```

O servidor de hospedagem deve redirecionar rotas desconhecidas para `index.html`, permitindo que o Angular Router processe URLs como `/jogar`.

## Verificação realizada

O comando `npm run build` foi executado com sucesso. O bundle inicial ficou abaixo do limite configurado no `angular.json`.

Os testes do back-end não foram executados neste ambiente porque o SDK .NET não está disponível nele. Nenhum arquivo do back-end foi alterado.

## Próximas melhorias possíveis

- Testes unitários dos stores, guards e serviços HTTP.
- Feedback sonoro opcional no giro.
- Animações distintas para cada prêmio.
- Página de perfil.
- Histórico de rodadas, caso a API exponha esse endpoint.
- Configuração automatizada de ambientes no pipeline de deploy.

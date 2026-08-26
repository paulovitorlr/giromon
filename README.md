# GiroMon

MVP de caça-níquel com API ASP.NET Core, front-end Angular e PostgreSQL.

## Pré-requisitos

- .NET SDK 8
- Node.js 20 ou superior
- Docker Desktop

## Banco de dados

Na raiz do projeto:

```bash
docker compose up -d
```

As credenciais locais possuem valores padrão no `compose.yaml`. Para alterá-las, copie `.env.example` para `.env` e ajuste os valores. Ajuste também `ConnectionStrings__Postgres` ao iniciar a API.

## API

```bash
dotnet restore
dotnet run --project src/Giromon.Api
```

A API inicia em `http://localhost:5080`. Teste:

```bash
curl http://localhost:5080/health
```

Variáveis suportadas pelo ASP.NET Core:

```text
ConnectionStrings__Postgres=Host=localhost;Port=5432;Database=giromon;Username=giromon;Password=giromon_dev
Cors__AllowedOrigins__0=http://localhost:4200
```

## Front-end

```bash
cd src/Giromon.Web
npm install
npm start
```

Abra `http://localhost:4200`.

# webapi-gestao-petshop

API REST para gestão de um petshop: clientes, pets, colaboradores, produtos e estoque.

## Stack

- C# / ASP.NET Core (.NET 6)
- Entity Framework Core 7 com SQL Server
- FluentValidation
- Swagger (Swashbuckle)
- xUnit, `WebApplicationFactory`, EF Core InMemory e FluentAssertions nos testes

## Destaques técnicos

- **Endpoints assíncronos** de ponta a ponta, com `CancellationToken`, `ToListAsync`/`FirstOrDefaultAsync`/`SaveChangesAsync` e `AsNoTracking` nas consultas somente leitura.
- **Soft delete com query filter global**: `HasQueryFilter(p => !p.IsDeleted)` no `DbContext` garante que produtos deletados não aparecem em nenhuma consulta.
- **DTOs separados das entidades**: requests e responses próprios por recurso, sem expor entidades do EF nas respostas de criação e atualização.
- **Validação com FluentValidation** (incluindo regra assíncrona de e-mail único) e erros de validação retornados como **422 com `ProblemDetails`**.
- **Respostas HTTP padronizadas**: `201` com header `Location` na criação, `204` na exclusão e `404` para recursos inexistentes, todas documentadas com `ProducesResponseType`.
- **Testes de integração** cobrindo todos os endpoints, com banco InMemory isolado por teste.

## Endpoints

| Recurso | Método | Rota | Descrição |
|---|---|---|---|
| Clientes | GET | `/api/cliente` | Lista os clientes |
| Clientes | GET | `/api/cliente/{id}` | Busca um cliente pelo Id |
| Clientes | POST | `/api/cliente` | Cadastra um cliente |
| Clientes | PUT | `/api/cliente` | Atualiza nome e e-mail de um cliente |
| Clientes | DELETE | `/api/cliente/{id}` | Remove um cliente |
| Pets | GET | `/api/pets/{id}` | Busca um pet pelo Id |
| Colaboradores | GET | `/api/employees` | Lista os colaboradores |
| Colaboradores | GET | `/api/employees/{id}` | Busca um colaborador pelo Id |
| Colaboradores | POST | `/api/employees` | Cadastra um colaborador (e-mail único) |
| Colaboradores | PUT | `/api/employees` | Atualiza um colaborador |
| Colaboradores | DELETE | `/api/employees/{id}` | Remove um colaborador |
| Produtos | GET | `/api/produtos` | Lista os produtos não deletados |
| Produtos | GET | `/api/produtos/{id}` | Busca um produto pelo Id |
| Produtos | POST | `/api/produtos` | Cadastra um produto |
| Produtos | DELETE | `/api/produtos/{id}` | Marca um produto como deletado (soft delete) |
| Estoque | GET | `/api/estoque` | Lista os itens do estoque |

## Como rodar

Pré-requisitos: .NET 6 SDK (ou superior com o runtime 6 instalado), Docker e a ferramenta `dotnet-ef`.

1. Suba o SQL Server:

   ```bash
   docker run -d --name petshop-sql -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=PetShop@2026" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
   ```

2. Configure os segredos do projeto:

   ```bash
   cd PetShop/PetShop.Api
   dotnet user-secrets set "DatabaseServerName" "localhost,1433"
   dotnet user-secrets set "DatabasePassword" "PetShop@2026"
   ```

3. Aplique as migrations:

   ```bash
   dotnet tool install --global dotnet-ef --version 7.0.11
   dotnet ef database update
   ```

4. Execute a API:

   ```bash
   dotnet run
   ```

5. Acesse o Swagger em `https://localhost:5001/swagger`.

## Testes

```bash
cd PetShop
dotnet test
```

Os testes sobem a API em memória com `WebApplicationFactory` e substituem o SQL Server por um banco InMemory isolado por teste, sem depender do Docker.

## Próximos passos

- Tornar `Price` e `Quantity` tipos numéricos (`decimal` e `int`).
- Extrair a regra de negócio dos controllers para uma camada de serviços.
- Atualizar o projeto para .NET 8.

## Autoria

Desenvolvido por **Marianna Corrêa** — [LinkedIn](https://linkedin.com/in/marianna-corr%C3%AAa-da-silva-valente)

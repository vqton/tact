# Code Quality & Code Skill Rules for Accounting WebApp Project
**Version:** 1.0 | **Effective Date:** March 2026  
**Applies to:** All C# / .NET 9 code in Core, Application, Infrastructure, Web, Tests projects  
**Goal:** Produce readable, maintainable, performant, secure, and testable code that supports long-term compliance (VAS/Circular 99), easy onboarding, and minimal technical debt.

These rules complement VASCompliance.md — prioritize both.

## 1. General Principles (Always Apply)
- Follow **SOLID** + **DRY** principles
- Favor **technical responsibility grouping** (Controllers, Services, Models, Views)
  - Example: `src/Controllers/TransactionsController.cs`, `src/Services/JournalService.cs`, `src/Models/Journal.cs`, `src/Views/Transactions/Post.cshtml`
  - Keep related code co-located by function, not by feature
- Use modern C# 13 / .NET 9 features aggressively (primary constructors, collection expressions, default lambda parameters, alias any type, etc.) when they improve clarity.
- Code must be **self-documenting** — prefer meaningful names over comments (comments explain **why**, not **what**).
- **One class, one responsibility**: Services handle business logic; controllers handle HTTP; models represent domain.

## 2. Naming Conventions (Microsoft + Project-Specific)
- **Classes / Records / Interfaces**: PascalCase, noun/noun-phrase (e.g., JournalEntry, PostJournalEntryCommand)
- **Interfaces**: Prefix with `I` only if truly needed (e.g., IRepository<T>); prefer non-prefixed for application services (e.g., IJournalService)
- **Methods / Properties**: PascalCase
- **Local variables / parameters**: camelCase
- **Constants / readonly fields**: PascalCase with `const` or `static readonly`
- **Private fields**: `_camelCase` (underscore prefix)
- **Async methods**: End with `Async` (e.g., GetLedgerAsync)
- **Commands / Queries**: Noun + Verb (e.g., CreateAccountCommand, GetBalanceSheetQuery)
- **DTOs / ViewModels**: Suffix with `Dto`, `Vm`, or `Response` when needed (e.g., BalanceSheetRowDto)
- **Tests**: `[Feature]Tests` class, `[MethodUnderTest]_[Scenario]_[Expected]` method names

## 3. Formatting & Style (Enforce via .editorconfig + Roslyn Analyzers)
- Use **.editorconfig** at solution root (see Microsoft defaults + project tweaks)
- Indentation: 4 spaces (no tabs)
- Line length: Soft limit 120 chars, hard 140
- Braces: Always use (even for single-line if/else/for)
- `var` usage: Prefer `var` when type is obvious from right-hand side; explicit type otherwise
- Null checks: Prefer modern patterns (`is null`, `??`, `?.`, null-forgiving `!` sparingly)
- String interpolation over `string.Format` / concatenation
- Prefer collection expressions: `int[] numbers = [1, 2, 3];`
- Organize usings: System.* first, then others, remove unused (dotnet format enforces)

## 4. Architecture & Design Rules
- **Dependency Direction**: Outer layers (Controllers, Views) depend on inner (Services, Models, DbContext)
- **Service pattern**: Controllers inject services (`IAccountService`, `IJournalService`), call methods directly
  - Example: `var account = await _accountService.CreateAccountAsync(dto);`
- **No MediatR / CQRS**: Direct method calls on services; simpler and faster
- **DbContext usage**: Services inject DbContext (scoped lifetime), use it directly with async queries
- **Domain entities**: Rich models with validation methods, invariants enforced in setters or dedicated methods
- **No business logic in controllers**: Keep controllers thin—HTTP handling only. All domain logic → services
- **Repositories**: Not needed; services access DbContext.Entities directly with projections/filters
- **Error handling**: Throw domain exceptions (e.g., `UnbalancedJournalException`); controllers catch and return appropriate HTTP response
- **Async everywhere** I/O-bound: Controllers, services, EF queries — use async/await consistently

## 5. Performance & Reliability Best Practices
- Understand **hot paths** — cache aggressively (IMemoryCache / HybridCache in .NET 9+)
- Avoid blocking calls (no .Result / .Wait(); use async)
- Paginate large collections (trial balance, ledger views)
- Use `AsNoTracking()` for read-only queries
- Profile with dotnet-trace / dotnet-counters when needed
- Avoid large object allocations in hot paths

## 6. Security Rules (Financial App Mandatory)
- Enforce HTTPS (UseHsts, UseHttpsRedirection)
- Secure cookies: HttpOnly, Secure, SameSite=Strict
- Input validation: FluentValidation + [ValidateAntiForgeryToken]
- Role-based + policy-based authorization (Accountant can post, Auditor can view only)
- Never log sensitive data (mask account numbers, amounts in Serilog enrichers)
- Use parameterized queries (EF Core does this by default)

## 7. Testing Standards (Aim 80%+ Coverage)
- Unit tests: xUnit, test behavior (not implementation)
- Test names: Descriptive (e.g., PostJournalEntry_WithUnbalancedAmounts_ThrowsDomainException)
- Use Respawn for integration test database reset
- Mock external dependencies (Moq for IMediator in controller tests if needed)
- Test edge cases: Invalid VAS rules, zero/negative amounts, currency mismatches
- Arrange-Act-Assert pattern strictly

## 8. Logging & Observability
- Use Serilog structured logging (context enrichers: UserId, RequestId)
- Log levels: Information for normal flow, Warning/Error for exceptions
- Never log full exceptions in production (use Exception.ToString() sparingly)

## 9. Tooling & Automation
- Enforce with: `dotnet format`, Roslyn analyzers (Microsoft.CodeAnalysis.* packages)
- .editorconfig + Directory.Build.props for consistent warnings/errors
- Pre-commit hooks: format + build + test (optional husky or git hooks)
- VS Code tasks: build, watch, test, format

## 10. Review Checklist (Use in PRs / Amazon Q Generations)
- [ ] Follows naming & formatting rules
- [ ] Business logic in Services, not controllers
- [ ] Controllers call services, services access DbContext
- [ ] Async all the way
- [ ] Validated against VASCompliance.md
- [ ] Tests cover happy + edge paths
- [ ] No magic strings/numbers (use constants)
- [ ] Readable: one responsibility per method/class
- [ ] Secure & performant considerations applied

**Enforcement:**  
- Amazon Q: Apply these rules + VASCompliance.md in every code generation.  
- Violations: Add // CODE-QUALITY-FIX or block PR.  
- Goal: Code that any senior .NET developer can understand in <5 minutes per slice.

Last updated: March 09, 2026 – Software Architect review
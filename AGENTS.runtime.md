# AGENTS.runtime.md
**Strict Runtime Rules (Daily Use)**

Use these rules as default behavior for all implementation tasks in this repo.

## 1) Non-Negotiables
- .NET 9 pure ASP.NET Core MVC monolith only (no SPA, no separate API by default).
- Keep flow: Controller -> Service -> DbContext.
- Put business logic in Services, not Controllers.
- Enforce VAS compliance first (Circular 99/2025/TT-BTC).
- Use Vietnamese labels and VND formatting in UI/report outputs.

## 2) Code Standards
- C# 13 style, 4-space indent, braces always.
- Naming: `PascalCase` for types/methods, `camelCase` for locals/params, `_camelCase` for private fields.
- Async all I/O (`await`, `ToListAsync`, `SaveChangesAsync`); no `.Result` / `.Wait()`.
- No magic strings/numbers; prefer constants/value objects.
- Keep methods/classes single-responsibility and readable.

## 3) EF Core Rules
- EF Core 9 async-first.
- Use DbContext directly in Services (no Repository layer).
- Use `AsNoTracking()` for read-only queries.
- Project to DTOs for list/report screens.
- Ensure indexes for frequent filter/sort columns.

## 4) Frontend Rules
- Razor + Bootstrap 5.3+ + HTMX 2.x + Alpine.js 3.x.
- Server-first rendering with progressive enhancement.
- Use HTMX for partial updates where it reduces full reloads.
- Alpine only for small local state (modal/toggle).
- No inline JS and no heavy jQuery patterns.

## 5) Security Baseline
- HTTPS enforced.
- Anti-forgery on state-changing forms.
- Validate all input server-side.
- Role/authorization checks on protected actions.
- Never log secrets or sensitive personal/financial data.

## 6) VAS Accounting Constraints
- Every journal entry must satisfy Debits = Credits.
- Default currency is VND.
- Fiscal year is January-December.
- Seed and protect core Chart of Accounts per Appendix II.
- Do not use removed/eliminated accounts from Circular 200.

## 7) Delivery Rules
- Add/update tests for happy path + edge cases.
- Run build/tests before finishing when feasible.
- Commit frequently with clear messages:
  - `Task X: ...`
  - `Fix: ...`
  - `Refactor: ...`
  - `Style: ...`

## 8) Secrets & Config
- Never hard-code connection strings/keys/passwords.
- Use user-secrets in dev and environment variables in production.
- Do not commit production secrets or `*.db` artifacts.

## 9) Done Checklist
- [ ] Builds cleanly
- [ ] Async/architecture rules followed
- [ ] VAS constraints satisfied
- [ ] Security checks applied
- [ ] UI uses Vietnamese labels + VND formatting
- [ ] Tests updated for changed behavior

Reference full guidance: `AGENTS.md`.


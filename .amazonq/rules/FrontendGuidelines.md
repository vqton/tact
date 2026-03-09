Here is the **FrontendGuidelines.md** file content, drafted as an **expert UI/UX Designer & Frontend Architect** (with deep experience in enterprise/internal business applications, especially regulated financial/accounting systems in 2026).

This document defines the frontend rules, patterns, and best practices for your pure **ASP.NET Core MVC** project using **Razor Views** + **Bootstrap 5** + **HTMX 2.x** + **Alpine.js 3.x**. The focus is on:

- Professional, clean, accessible UI suitable for accountants/auditors
- Fast, lightweight, and responsive (desktop-first, but mobile/tablet usable)
- Excellent form UX (journal entries, filters, validations)
- VAS-compliant report presentation (clear, readable, printable)
- Consistent patterns that Amazon Q and developers can follow reliably

Place this in:  
`.amazonq/rules/FrontendGuidelines.md`

```markdown
# Frontend Guidelines – Accounting WebApp Project
**Version:** 1.0 | **Effective Date:** March 2026  
**Stack:** ASP.NET Core MVC (Razor Views) + Bootstrap 5.3/5.4 + HTMX 2.x + Alpine.js 3.x  
**Primary Goal:** Deliver a professional, fast, intuitive, and accessible user interface optimized for accountants, auditors, and finance teams in a Vietnamese enterprise environment. Prioritize clarity, data density, form usability, report readability, and VAS-compliant presentation over flashy animations or heavy JavaScript.

## 1. Core Frontend Principles
- **Server-first, progressive enhancement**: Razor renders full pages → HTMX adds dynamic updates → Alpine.js handles lightweight client state (no SPA framework).
- **Desktop-first, responsive**: Target 1366×768+ screens (typical office monitors); ensure usable on tablets/phones for mobile viewing (reports, ledger checks).
- **VAS & enterprise UX**: Clear hierarchy, Vietnamese labels default, precise number formatting (VND: 1.234.567 ₫), red/green indicators for debit/credit, audit-friendly layouts.
- **Accessibility (WCAG 2.1 AA minimum)**: Semantic HTML, ARIA labels, keyboard navigation, sufficient contrast (Bootstrap helps, but verify).
- **Performance target**: < 100 KB additional JS/CSS beyond Bootstrap/HTMX/Alpine; TTFB < 200 ms for most pages; no render-blocking resources.
- **Consistency over creativity**: Follow these patterns strictly to reduce cognitive load and bugs.

## 2. Technology Inclusion & Versions
Include via CDN (production) or local files (optional):

```html
<!-- Bootstrap 5.3+ -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>

<!-- HTMX 2.x -->
<script src="https://unpkg.com/htmx.org@2.0.1" integrity="sha384-QnD+LI9wYvC2F6z5qX9V3E0f5t0G8h5f5t0G8h5f5t0G8h5f5t0G8h5f5t0G8h5f" crossorigin="anonymous"></script>

<!-- Alpine.js 3.x -->
<script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>
```

Prefer integrity hashes and specific versions in production.

## 3. Layout & Structure (_Layout.cshtml)
- Use Bootstrap container-fluid for full-width content.
- Navbar: Fixed-top or sticky-top, dark/light mode toggle optional.
- Sidebar (optional): Collapsible on mobile, for navigation (CoA, Transactions, Reports, Settings).
- Main content: Card-based layout for forms/reports.
- Footer: Small, with version, copyright, generation timestamp for reports.
- Breadcrumbs: Use Bootstrap breadcrumb for deep navigation (e.g., Transactions > Journal Entry > Edit).

## 4. Color & Typography Scheme
- Primary: Bootstrap blue (#0d6efd) for actions (Save, Post, Generate Report)
- Success: Green (#198754) for posted/balanced entries
- Danger: Red (#dc3545) for unbalanced warnings, errors
- Warning: Yellow (#ffc107) for draft/pending states
- Info: Teal (#0dcaf0) for filters/previews
- Text: Dark gray (#212529) on white/light backgrounds
- Fonts: System stack + Bootstrap defaults (Segoe UI, Roboto, sans-serif)
- Numbers: Monospace font-family for amounts (e.g., 'Consolas', monospace)

## 5. Form Patterns (Journal Entry, CoA, Filters)
- Use Bootstrap form-floating or form-group + label.
- Required fields: Add asterisk + Bootstrap `required` class.
- Validation: Server-side (FluentValidation) + client hints (Bootstrap invalid-feedback).
- Dynamic lines (journal entries):
  ```html
  <div id="entry-lines" x-data="{ lines: 1 }">
      <template x-for="i in lines">
          <div class="row mb-2 entry-line" hx-swap="outerHTML">
              <!-- Account select, Debit, Credit, Description -->
              <button type="button" class="btn btn-sm btn-outline-danger" @click="lines--">-</button>
          </div>
      </template>
      <button type="button" class="btn btn-sm btn-outline-primary" @click="lines++">+ Dòng mới</button>
  </div>
  ```
- HTMX for partial submits: `hx-post="/Transactions/AddLine" hx-target="#entry-lines" hx-swap="beforeend"`

## 6. Report Presentation Rules
- Use card + table-responsive for previews.
- Sticky headers on large tables (CSS `position: sticky`).
- Zebra striping (Bootstrap table-striped) for ledger/trial balance.
- Totals row: Bold, larger font, background color (e.g., light-gray).
- Comparative columns: Side-by-side or toggle via Alpine.
- FastReport integration: Wrap WebReport in <div class="border rounded shadow-sm">.
- Export buttons: Group in card-footer (PDF primary, Print secondary).

## 7. HTMX & Alpine Patterns
- **HTMX**:
  - `hx-post` / `hx-get` for form submits, filter changes, pagination.
  - `hx-target="#main-content"` or specific ID.
  - `hx-swap="innerHTML"` (default) or `outerHTML` for replacing elements.
  - `hx-indicator=".htmx-indicator"` (spinner).
  - `hx-boost="true"` on <body> or forms for progressive enhancement.
- **Alpine.js**:
  - Small state: x-data="{ open: false, selected: null }"
  - Modals: x-show, x-transition
  - Dynamic classes: x-bind:class="{ 'text-danger': unbalanced }"
  - Avoid complex logic (leave to server/HTMX)

## 8. Accessibility & Usability Checklist
- Labels for all inputs (for/id or floating-label)
- ARIA roles: aria-label on icons/buttons, aria-describedby for errors
- Focus management: autofocus on first input, keyboard trap in modals
- Contrast: Check with WAVE/axe tools
- Vietnamese text: Full-width support, no truncation on labels
- Error messages: Red, clear, associated with fields

## 9. Prohibited Practices
- Inline JavaScript (except Alpine x-init if tiny)
- Heavy jQuery (HTMX replaces most needs)
- Custom CSS overrides unless Bootstrap lacks feature
- Full-page reloads when HTMX can handle
- Overusing modals for primary flows (prefer inline forms)

## 10. Review Checklist for Frontend Code/PRs
- [ ] Bootstrap classes used correctly
- [ ] HTMX attributes present where dynamic update needed
- [ ] Alpine used only for client state
- [ ] Forms have validation feedback
- [ ] Reports readable/printable (no overflow)
- [ ] Vietnamese labels + VND formatting
- [ ] Accessible (keyboard testable)

**Enforcement:**  
- Amazon Q: Follow these patterns when generating .cshtml, layout, or frontend scripts.  
- Violations: Add // UI-UX-FIX comment or request refactor.

Last updated: March 09, 2026 – UI/UX & Frontend Architect
```

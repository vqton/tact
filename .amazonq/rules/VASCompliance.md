# VAS Compliance Rules for Accounting WebApp Project
**Version:** 1.1 | **Effective Date in Project:** March 2026  
**Primary Legal Reference:** Circular 99/2025/TT-BTC (issued 27 Oct 2025, effective 1 Jan 2026 for fiscal years beginning on/after 1 Jan 2026)  
**Replaces:** Circular 200/2014/TT-BTC (and related prior circulars)  
**Overarching Laws:** Accounting Law 2015 (as amended); Pillar Two / Global Minimum Tax rules (reflected via account 82112 in Circular 99); Ministry of Finance IFRS roadmap (VAS mandatory for statutory filings; IFRS voluntary for group reporting)

**Goal:** Enforce strict statutory compliance with Circular 99 in all code (models, validators, handlers, reports, UI). Prioritize VAS; allow optional IFRS-like disclosures where permitted.

## 1. Core Principles (Always Enforce in Code)
- Double-entry invariant: Debits MUST equal credits per journal entry (domain guard + FluentValidation).
- Functional currency: Default & mandatory = VND. Support currency change only per Article 5 (new fiscal year start, specific exchange rate rules).
- Economic substance over legal form; principle-based judgment encouraged.
- Voucher/documentation + approval workflow required for postings.
- No prohibited negative balances (e.g., assets/equity).
- Fiscal year: Default Jan–Dec; respect Vietnamese statutory periods.

## 2. Chart of Accounts (CoA) – Highest Risk Area
**Reference:** Appendix II of Circular 99

- **Autonomy rule:** Enterprises may supplement/rename Level 2+ accounts or adjust contents for internal needs → allow UI customization below Level 1.
- **Preserve Level 1 substance** for statutory reporting (e.g., assets 1xx, liabilities 3xx).
- **Mandatory new / key accounts (seed & validate):**
  - 215 – Biological Assets (separate from 211 Tangible Fixed Assets)
  - 2295 – Provision for Impairment of Biological Assets (differences → 632 COGS)
  - 1383 – Special Consumption Tax on Imported Goods
  - 332 – Dividends Payable / Profit Payable
  - 82112 – Additional Corporate Income Tax Expense (GloBE 15% top-up tax)
  - 419 – Treasury shares (renamed)
- **Eliminated accounts (block usage):** 161, 441, 611, 631, 1562 (purchasing costs often direct to 632).
- **Implementation:** Hierarchical entity (Code, Name, Type enum, Level, ParentId). Seed Appendix II defaults in migration. Prevent breaking changes to core Level 1/2 codes. Log customizations in audit trail.

## 3. Financial Statements & Reporting (FastReport)
**Reference:** VAS 01–03 formats + appendices in Circular 99; enhanced Notes disclosures

- Formats MUST follow Circular 99 structure (Balance Sheet: current/non-current; Income Statement: multi-step with functional classification option; Cash Flow: indirect method default).
- **Mandatory enhanced disclosures (Notes):** Cash restrictions, biological assets changes, BCCs, dividends/bonds, items >10% of totals, valuation bases/assumptions/uncertainties, going concern (Board assessment), top-up tax.
- **Implementation rules:**
  - Use .frx templates in wwwroot/Reports/ matching exact appendix layouts.
  - Bind aggregated DTOs via RegisterData (group by type/level).
  - Support comparative periods, VND formatting (1.234.567 ₫).
  - PDF export priority; ensure integrity/accuracy per software rules.

## 4. Key Business Rules & Validators (FluentValidation)
- Revenue: Recognize at transfer of control/performance obligation fulfillment.
- Impairment/provisions: Reliable evidence required (e.g., 2295, 2293, 2294).
- Deferred tax: Guidance for timing differences (IAS 12 approximation where applicable).
- Journal: Enforce balanced entries + description/voucher ref.
- Audit trail: Log all changes (user, timestamp, before/after) via Serilog/entity.

## 5. Prohibited / High-Risk Practices (Reject or Flag)
- Usage of removed accounts from Circular 200.
- Hard-coded outdated CoA.
- Ignoring VND default or improper currency translation.
- Unbalanced entries (throw DomainException).
- Reports missing comparative figures or required Notes disclosures.
- Missing internal governance controls documentation.

## 6. References (Official & Reliable)
- Full Circular 99: https://thuvienphapluat.vn/van-ban/Doanh-nghiep/Thong-tu-99-2025-TT-BTC-huong-dan-Che-do-ke-toan-doanh-nghiep-565484.aspx
- KPMG Summary (Nov 2025): https://kpmg.com/vn/en/home/insights/2025/11/key-changes-in-vietnamese-accounting-system-for-enterprises.html
- Vietnam Briefing: https://www.vietnam-briefing.com/news/circular-99-vietnam-accounting-regime-means-for-ifrs-alignment.html
- **Disclaimer:** This is internal guidance only — not legal/tax advice. Always cross-reference official MOF documents and consult external auditors for statutory filings.

**Enforcement:**  
- All code changes/PRs MUST reference this file.  
- Amazon Q agent: Apply these rules as priority context in every generation.  
- Violations: Block or add // VAS-COMPLIANCE-FIX comment.

Last updated: March 09, 2026 – CFO review
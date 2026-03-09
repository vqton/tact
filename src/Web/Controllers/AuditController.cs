namespace Web.Controllers;

using AccountingVAS.Web.Data;
using AccountingVAS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin,Auditor")]
public class AuditController(AccountingDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string? userId,
        string? entityType,
        string? action,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            (fromDate, toDate) = (toDate, fromDate);
        }

        var query = context.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var normalizedUserId = userId.Trim();
            query = query.Where(x => x.UserId == normalizedUserId);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var normalizedEntityType = entityType.Trim();
            query = query.Where(x => x.EntityType == normalizedEntityType);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            var normalizedAction = action.Trim();
            query = query.Where(x => x.Action == normalizedAction);
        }

        if (fromDate.HasValue)
        {
            var start = fromDate.Value.Date;
            query = query.Where(x => x.Timestamp >= start);
        }

        if (toDate.HasValue)
        {
            var endExclusive = toDate.Value.Date.AddDays(1);
            query = query.Where(x => x.Timestamp < endExclusive);
        }

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Take(500)
            .ToListAsync();

        var model = new AuditIndexVm
        {
            UserId = userId?.Trim(),
            EntityType = entityType?.Trim(),
            Action = action?.Trim(),
            FromDate = fromDate?.Date,
            ToDate = toDate?.Date,
            Items = items
        };

        if (IsHtmxRequest())
        {
            return PartialView("_AuditTable", model);
        }

        return View(model);
    }

    private bool IsHtmxRequest()
    {
        return string.Equals(Request.Headers["HX-Request"], "true", StringComparison.OrdinalIgnoreCase);
    }
}

public class AuditIndexVm
{
    public string? UserId { get; init; }
    public string? EntityType { get; init; }
    public string? Action { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public IReadOnlyList<AuditLogEntry> Items { get; init; } = [];
}

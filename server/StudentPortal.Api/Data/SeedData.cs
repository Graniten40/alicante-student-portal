using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Models;
using StudentPortal.Api.Data;
using StudentPortal.Api.Domain;

namespace StudentPortal.Api.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        // -------------------------
        // 0) MARKETS
        // -------------------------
        if (!await db.Markets.AnyAsync())
        {
            db.Markets.Add(new Market
            {
                Code = "TH-CNX",
                Name = "Chiang Mai",
                Currency = "THB",
                TimeZone = "Asia/Bangkok",
                IsActive = true
            });

            await db.SaveChangesAsync();
        }

        var cm = await db.Markets.FirstAsync(m => m.Code == "TH-CNX");

        // -------------------------
        // 1) PRODUCTS (Core + Add-ons)
        // -------------------------

        // Helper: Upsert product by (MarketId + Slug)
        async Task<Product> UpsertProductAsync(Product p)
        {
            var existing = await db.Products
                .FirstOrDefaultAsync(x => x.MarketId == p.MarketId && x.Slug == p.Slug);

            if (existing is null)
            {
                db.Products.Add(p);
                await db.SaveChangesAsync();
                return p;
            }

            // Update fields (if you change text later, seed updates it)
            existing.TitleEn = p.TitleEn;
            existing.TitleZh = p.TitleZh;
            existing.DescriptionEn = p.DescriptionEn;
            existing.DescriptionZh = p.DescriptionZh;
            existing.PriceCents = p.PriceCents;
            existing.IsFree = p.IsFree;
            existing.IsActive = p.IsActive;

            await db.SaveChangesAsync();
            return existing;
        }

        // --- Core packages
        var landingWeek = await UpsertProductAsync(new Product
        {
            MarketId = cm.Id,
            Slug = "landing-week",
            TitleEn = "Landing Week – Chiang Mai",
            TitleZh = "清迈落地周服务",
            DescriptionEn = "A safe landing week: housing shortlist, viewings, screening, contract check, move-in support.",
            DescriptionZh = "清迈安全落地一周：房源筛选、看房协调、房屋检查、合同要点、入住支持。",
            PriceCents = 0,   // TODO: set THB in cents-equivalent if you want, or store THB as cents anyway
            IsFree = false,
            IsActive = true
        });

        var landingMonth = await UpsertProductAsync(new Product
        {
            MarketId = cm.Id,
            Slug = "landing-month",
            TitleEn = "Landing Month – Chiang Mai",
            TitleZh = "清迈落地月服务",
            DescriptionEn = "Everything in Landing Week + first-month support and setup coordination.",
            DescriptionZh = "包含落地周全部内容 + 首月支持与生活设置协调。",
            PriceCents = 0,
            IsFree = false,
            IsActive = true
        });

        var vip = await UpsertProductAsync(new Product
        {
            MarketId = cm.Id,
            Slug = "vip-white-glove",
            TitleEn = "VIP White-Glove – Chiang Mai",
            TitleZh = "清迈VIP尊享落地服务",
            DescriptionEn = "Priority support, airport transfer, welcome pack, extra viewings, and a rescue process.",
            DescriptionZh = "优先支持、接机、欢迎包、更多看房选项与应急流程。",
            PriceCents = 0,
            IsFree = false,
            IsActive = true
        });

        // --- Add-ons (slugs start with addon-)
        var addons = new List<Product>
        {
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-airport-transfer",
                TitleEn = "Add-on: Airport Transfer",
                TitleZh = "加购：接机服务",
                DescriptionEn = "Partner transfer from airport to your accommodation.",
                DescriptionZh = "合作伙伴提供从机场到住所的接送服务。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-welcome-pack-basic",
                TitleEn = "Add-on: Welcome Pack (Basic)",
                TitleZh = "加购：欢迎包（基础）",
                DescriptionEn = "Water + basics on arrival (coordinated via partner).",
                DescriptionZh = "到达当日基础欢迎物资（合作伙伴协调）。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-welcome-pack-comfort",
                TitleEn = "Add-on: Welcome Pack (Comfort)",
                TitleZh = "加购：欢迎包（舒适）",
                DescriptionEn = "Water, fruit, coffee/tea, and small comforts (partner).",
                DescriptionZh = "水、水果、咖啡/茶等舒适物资（合作伙伴）。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-cleaning-subscription-biweekly",
                TitleEn = "Add-on: Cleaning Subscription (bi-weekly)",
                TitleZh = "加购：清洁订阅（每两周）",
                DescriptionEn = "Partner cleaning service every two weeks.",
                DescriptionZh = "合作伙伴每两周一次清洁服务。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-laundry-linen-service",
                TitleEn = "Add-on: Laundry/Linen Service",
                TitleZh = "加购：洗衣/床品服务",
                DescriptionEn = "Pickup and delivery laundry/linen service (partner).",
                DescriptionZh = "上门取送洗衣/床品服务（合作伙伴）。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-handyman-ac-coordination",
                TitleEn = "Add-on: Handyman / AC Service Coordination",
                TitleZh = "加购：维修/空调服务协调",
                DescriptionEn = "We coordinate a trusted technician visit.",
                DescriptionZh = "我们协调可信的维修/空调技师上门。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            },
            new()
            {
                MarketId = cm.Id,
                Slug = "addon-orientation-tour",
                TitleEn = "Add-on: Orientation Tour (2–3 hours)",
                TitleZh = "加购：城市导览（2–3小时）",
                DescriptionEn = "A short tour to learn how Chiang Mai works.",
                DescriptionZh = "快速了解清迈生活方式与区域的导览。",
                PriceCents = 0,
                IsFree = false,
                IsActive = true
            }
        };

        foreach (var a in addons)
            await UpsertProductAsync(a);

        // -------------------------
        // 2) PACK CONTENT (JSON skeleton) for core packages only
        // -------------------------
        async Task EnsurePackContentAsync(Product product, string jsonV1)
        {
            var exists = await db.PackContents
                .AnyAsync(c => c.ProductId == product.Id && c.Version == 1);

            if (!exists)
            {
                db.PackContents.Add(new PackContent
                {
                    ProductId = product.Id,
                    Version = 1,
                    JsonContent = jsonV1
                });

                await db.SaveChangesAsync();
            }
        }

        var landingWeekJson = """
{
  "title": { "en": "Landing Week – Chiang Mai", "zh": "清迈落地周服务" },
  "goal":  { "en": "A safe, stress-free landing week with housing and move-in support.", "zh": "通过住房筛选与入住支持，实现安全无压力落地。" },
  "steps": [
    { "key": "onboarding", "title": { "en": "Onboarding & needs assessment", "zh": "需求沟通与目标确认" }, "tasks": [] },
    { "key": "areas", "title": { "en": "Area match & housing shortlist", "zh": "区域匹配与房源清单" }, "tasks": [] },
    { "key": "viewings", "title": { "en": "Coordinated viewings", "zh": "看房协调" }, "tasks": [] },
    { "key": "screening", "title": { "en": "Verified Home Screening", "zh": "房屋检查清单" }, "tasks": [] },
    { "key": "contract", "title": { "en": "Contract check (practical)", "zh": "合同要点检查（非法律建议）" }, "tasks": [] },
    { "key": "move-in", "title": { "en": "Move-in support & checklist", "zh": "入住支持与清单" }, "tasks": [] }
  ],
  "templates": [],
  "pitfalls": []
}
""";

        var landingMonthJson = """
{
  "title": { "en": "Landing Month – Chiang Mai", "zh": "清迈落地月服务" },
  "goal":  { "en": "Full first-month support: setup, coordination, and a smooth routine.", "zh": "首月全程支持：生活设置、协调安排与稳定节奏。" },
  "steps": [
    { "key": "included", "title": { "en": "Everything in Landing Week", "zh": "包含落地周全部内容" }, "tasks": [] },
    { "key": "support", "title": { "en": "First-month support (office hours)", "zh": "首月支持（工作时间）" }, "tasks": [] },
    { "key": "setup", "title": { "en": "SIM, internet booking, utilities guidance", "zh": "SIM卡、网络预约与水电指导" }, "tasks": [] },
    { "key": "cleaning", "title": { "en": "Move-in deep cleaning (partner)", "zh": "入住深度清洁（合作伙伴）" }, "tasks": [] },
    { "key": "errands", "title": { "en": "1–2 support errands (coordination)", "zh": "1–2项事务协助（协调）" }, "tasks": [] }
  ],
  "templates": [],
  "pitfalls": []
}
""";

        var vipJson = """
{
  "title": { "en": "VIP White-Glove – Chiang Mai", "zh": "清迈VIP尊享落地服务" },
  "goal":  { "en": "Priority support with extra options and a rescue process if something fails.", "zh": "优先支持，更多选项，并在出现问题时按流程救援处理。" },
  "steps": [
    { "key": "included", "title": { "en": "Everything in Landing Month", "zh": "包含落地月全部内容" }, "tasks": [] },
    { "key": "priority", "title": { "en": "Priority support policy", "zh": "优先支持规则" }, "tasks": [] },
    { "key": "transfer", "title": { "en": "Airport transfer (partner)", "zh": "接机服务（合作伙伴）" }, "tasks": [] },
    { "key": "welcome", "title": { "en": "Welcome pack", "zh": "欢迎包" }, "tasks": [] },
    { "key": "extra", "title": { "en": "Extra viewings & extra areas", "zh": "更多看房与更多区域" }, "tasks": [] },
    { "key": "rescue", "title": { "en": "Rescue process (not a guarantee)", "zh": "应急处理流程（非保证）" }, "tasks": [] }
  ],
  "templates": [],
  "pitfalls": []
}
""";

        await EnsurePackContentAsync(landingWeek, landingWeekJson);
        await EnsurePackContentAsync(landingMonth, landingMonthJson);
        await EnsurePackContentAsync(vip, vipJson);
    }
}

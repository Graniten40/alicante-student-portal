using Microsoft.EntityFrameworkCore;
using StudentPortal.Api.Models;

namespace StudentPortal.Api.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Se till att DB är up-to-date
        await db.Database.MigrateAsync();

        // 1) PRODUCTS
        if (!await db.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new()
                {
                    Slug = "basic-arrival",
                    TitleEn = "Basic Arrival Pack",
                    TitleZh = "基础到达套餐",
                    DescriptionEn = "Everything you must do in the first 30 days in Spain.",
                    DescriptionZh = "抵达西班牙后前30天必须完成的事项清单。",
                    PriceCents = 0,
                    IsFree = true
                },
                new()
                {
                    Slug = "student-visa",
                    TitleEn = "Student Visa Pack (Spain)",
                    TitleZh = "学生签证套餐（西班牙）",
                    DescriptionEn = "Visa requirements, documents, timeline, and common rejection reasons.",
                    DescriptionZh = "签证要求、材料清单、时间线与常见拒签原因。",
                    PriceCents = 4900,
                    IsFree = false
                },
                new()
                {
                    Slug = "housing",
                    TitleEn = "Housing Pack (Alicante)",
                    TitleZh = "住房套餐（阿利坎特）",
                    DescriptionEn = "Find safe housing, avoid scams, and understand Spanish contracts.",
                    DescriptionZh = "安全找房、防诈骗、看懂西班牙租房合同。",
                    PriceCents = 5900,
                    IsFree = false
                }
            };

            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }

        // 2) PACK CONTENT (Basic Arrival)
        var basic = await db.Products.FirstAsync(p => p.Slug == "basic-arrival");

        var hasContent = await db.PackContents.AnyAsync(c => c.ProductId == basic.Id);
        if (!hasContent)
        {
            var json = """
{
  "title": { "en": "Basic Arrival Pack", "zh": "基础到达套餐" },
  "goal":  { "en": "Get settled during your first 30 days in Spain.", "zh": "帮助你在抵达西班牙后的30天内顺利安顿。" },
  "steps": [
    {
      "key": "day-1-3",
      "title": { "en": "Day 1–3: Essentials", "zh": "第1–3天：必备事项" },
      "tasks": [
        { "key": "sim", "text": { "en": "Get a SIM card + mobile data plan", "zh": "办理电话卡与流量套餐" } },
        { "key": "transport", "text": { "en": "Learn transport options in Alicante (TRAM/bus)", "zh": "了解阿利坎特交通（电车/公交）" } },
        { "key": "apps", "text": { "en": "Install essential apps (maps, banking, translation)", "zh": "安装必备应用（地图/银行/翻译）" } }
      ],
      "pitfalls": [
        { "en": "Avoid paying for housing before viewing or signing a contract.", "zh": "看房或签合同前不要付款。" }
      ],
      "templates": [
        {
          "key": "tpl-landlord",
          "title": { "en": "Message to landlord", "zh": "给房东的消息模板" },
          "text": {
            "en": "Hi! I’m a student and I’m interested in the apartment. Can we schedule a viewing this week? Thank you!",
            "zh": "你好！我是学生，对这套房子很感兴趣。本周可以安排看房吗？谢谢！"
          }
        }
      ]
    },
    {
      "key": "week-1",
      "title": { "en": "Week 1: Paperwork basics", "zh": "第1周：基础手续" },
      "tasks": [
        { "key": "nie", "text": { "en": "Understand NIE: what it is and why you need it", "zh": "了解NIE：是什么、为什么需要" } },
        { "key": "padron", "text": { "en": "Empadronamiento (register your address)", "zh": "办理住址登记（Empadronamiento）" } }
      ],
      "pitfalls": [
        { "en": "Bring originals + copies for appointments.", "zh": "预约办理时带原件和复印件。" }
      ]
    }
  ]
}
""";

            db.PackContents.Add(new PackContent
            {
                ProductId = basic.Id,
                Version = 1,
                JsonContent = json
            });

            await db.SaveChangesAsync();
        }
    }
}

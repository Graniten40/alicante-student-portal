using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketsAndProductMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Drop old global unique index on Slug
            migrationBuilder.DropIndex(
                name: "IX_Products_Slug",
                table: "Products");

            // 2) Create Markets table first
            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            // 3) Seed Chiang Mai market with a known Id = 1
            migrationBuilder.InsertData(
                table: "Markets",
                columns: new[] { "Id", "Code", "Name", "Currency", "TimeZone", "IsActive" },
                values: new object[] { 1, "TH-CNX", "Chiang Mai", "THB", "Asia/Bangkok", true }
            );

            // 4) Add MarketId to Products (default 0 is fine temporarily)
            migrationBuilder.AddColumn<int>(
                name: "MarketId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 5) Backfill existing rows to the seeded market (so FK won't fail)
            migrationBuilder.Sql("UPDATE [Products] SET [MarketId] = 1 WHERE [MarketId] = 0;");

            // 6) New unique index per market
            migrationBuilder.CreateIndex(
                name: "IX_Products_MarketId_Slug",
                table: "Products",
                columns: new[] { "MarketId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markets_Code",
                table: "Markets",
                column: "Code",
                unique: true);

            // 7) Add FK now that data is valid
            migrationBuilder.AddForeignKey(
                name: "FK_Products_Markets_MarketId",
                table: "Products",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Markets_MarketId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Markets");

            migrationBuilder.DropIndex(
                name: "IX_Products_MarketId_Slug",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class categoryConcernChannelPivot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category_concern_channels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    category_concern_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_concern_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_concern_channels_category_concerns_category_concern_id",
                        column: x => x.category_concern_id,
                        principalTable: "category_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_concern_channels_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_concern_channels_category_concern_id",
                table: "category_concern_channels",
                column: "category_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_category_concern_channels_channel_id",
                table: "category_concern_channels",
                column: "channel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_concern_channels");
        }
    }
}

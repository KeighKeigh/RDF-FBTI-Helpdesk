using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ContractorAndContractorChannelPivot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contractors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contractor_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_added = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contractors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contractor_channels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    contractor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contractor_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_contractor_channels_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contractor_channels_contractors_contractor_id",
                        column: x => x.contractor_id,
                        principalTable: "contractors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contractor_channels_channel_id",
                table: "contractor_channels",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_contractor_channels_contractor_id",
                table: "contractor_channels",
                column: "contractor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contractor_channels");

            migrationBuilder.DropTable(
                name: "contractors");
        }
    }
}

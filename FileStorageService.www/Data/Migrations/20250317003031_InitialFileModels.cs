using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorageService.www.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialFileModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileHandles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileHandles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlockNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", maxLength: 1024, nullable: false),
                    FileHandleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileBlocks_FileHandles_FileHandleId",
                        column: x => x.FileHandleId,
                        principalTable: "FileHandles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileBlocks_FileHandleId",
                table: "FileBlocks",
                column: "FileHandleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileBlocks");

            migrationBuilder.DropTable(
                name: "FileHandles");
        }
    }
}

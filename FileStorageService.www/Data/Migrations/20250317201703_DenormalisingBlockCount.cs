using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorageService.www.Data.Migrations
{
    /// <inheritdoc />
    public partial class DenormalisingBlockCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileBlockCount",
                table: "FileHandles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE FileHandles
                SET FileBlockCount = (
                    SELECT COUNT(*) 
                    FROM FileBlocks 
                    WHERE FileBlocks.FileHandleId = FileHandles.Id
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileBlockCount",
                table: "FileHandles");
        }
    }
}

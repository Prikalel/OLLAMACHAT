using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Context.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnqueuedCompletionJobId",
                table: "UserChats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnqueuedCompletionJobId",
                table: "UserChats",
                type: "TEXT",
                nullable: true);
        }
    }
}

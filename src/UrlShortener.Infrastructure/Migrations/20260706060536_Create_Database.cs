using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Create_Database : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "auth");

        migrationBuilder.EnsureSchema(
            name: "outbox");

        migrationBuilder.EnsureSchema(
            name: "shorten");

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            schema: "outbox",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "text", nullable: false),
                payload = table.Column<string>(type: "text", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                error = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_outbox_messages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                last_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                pending_email = table.Column<string>(type: "text", nullable: true),
                profile_image_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "email_verification_tokens",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_email_verification_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_email_verification_tokens_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "password_reset_tokens",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                token_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                used_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_password_reset_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_password_reset_tokens_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "refresh_tokens",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                token_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expired_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                revoked_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                replaced_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                replaced_by_token_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_refresh_tokens_refresh_tokens_replaced_by_token_id",
                    column: x => x.replaced_by_token_id,
                    principalSchema: "auth",
                    principalTable: "refresh_tokens",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_refresh_tokens_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "shorten_urls",
            schema: "shorten",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                long_url = table.Column<string>(type: "text", nullable: false),
                code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_shorten_urls", x => x.id);
                table.ForeignKey(
                    name: "fk_shorten_urls_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_email_verification_tokens_id_user_id",
            schema: "auth",
            table: "email_verification_tokens",
            columns: new[] { "id", "user_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_email_verification_tokens_user_id",
            schema: "auth",
            table: "email_verification_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_processed_on_utc_retry_count_created_on_utc",
            schema: "outbox",
            table: "outbox_messages",
            columns: new[] { "processed_on_utc", "retry_count", "created_on_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_password_reset_tokens_id_user_id",
            schema: "auth",
            table: "password_reset_tokens",
            columns: new[] { "id", "user_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_password_reset_tokens_token_hash",
            schema: "auth",
            table: "password_reset_tokens",
            column: "token_hash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_password_reset_tokens_user_id",
            schema: "auth",
            table: "password_reset_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_replaced_by_token_id",
            schema: "auth",
            table: "refresh_tokens",
            column: "replaced_by_token_id");

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_token_hash",
            schema: "auth",
            table: "refresh_tokens",
            column: "token_hash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_user_id",
            schema: "auth",
            table: "refresh_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_shorten_urls_code",
            schema: "shorten",
            table: "shorten_urls",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_shorten_urls_long_url",
            schema: "shorten",
            table: "shorten_urls",
            column: "long_url");

        migrationBuilder.CreateIndex(
            name: "ix_shorten_urls_user_id_enabled_created_on_utc",
            schema: "shorten",
            table: "shorten_urls",
            columns: new[] { "user_id", "enabled", "created_on_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            schema: "auth",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_first_name_last_name",
            schema: "auth",
            table: "users",
            columns: new[] { "first_name", "last_name" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "email_verification_tokens",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "outbox_messages",
            schema: "outbox");

        migrationBuilder.DropTable(
            name: "password_reset_tokens",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "refresh_tokens",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "shorten_urls",
            schema: "shorten");

        migrationBuilder.DropTable(
            name: "users",
            schema: "auth");
    }
}

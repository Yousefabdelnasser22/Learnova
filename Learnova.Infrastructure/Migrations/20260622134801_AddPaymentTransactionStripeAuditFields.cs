using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTransactionStripeAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessedWebhookEvents_Provider_EventId",
                table: "ProcessedWebhookEvents");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "ProcessedWebhookEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "ProcessedWebhookEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisputeClosedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputeId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputeReason",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisputeStatus",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DisputedAmount",
                table: "PaymentTransactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisputedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastWebhookEventId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastWebhookEventType",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWebhookReceivedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundedAmount",
                table: "PaymentTransactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeChargeId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputeClosedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputeId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputeReason",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputeStatus",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputedAmount",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "DisputedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "LastWebhookEventId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "LastWebhookEventType",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "LastWebhookReceivedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RefundId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RefundedAmount",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "StripeChargeId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "StripeCheckoutSessionId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "ProcessedWebhookEvents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventId",
                table: "ProcessedWebhookEvents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedWebhookEvents_Provider_EventId",
                table: "ProcessedWebhookEvents",
                columns: new[] { "Provider", "EventId" },
                unique: true);
        }
    }
}

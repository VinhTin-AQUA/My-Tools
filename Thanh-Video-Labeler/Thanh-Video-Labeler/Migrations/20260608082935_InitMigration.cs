using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thanh_Video_Labeler.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExceFileName = table.Column<string>(type: "TEXT", nullable: false),
                    SheetName = table.Column<string>(type: "TEXT", nullable: false),
                    SheetCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SheetName = table.Column<string>(type: "TEXT", nullable: false),
                    SheetCode = table.Column<string>(type: "TEXT", nullable: false),
                    SheetStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoAwsConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    TotalVideoAws = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVideoAwsDownloaded = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoAwsConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoAwsInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Case = table.Column<string>(type: "TEXT", nullable: false),
                    StoreLocation = table.Column<string>(type: "TEXT", nullable: false),
                    ServerID = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedBy = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedDate = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfIncident = table.Column<string>(type: "TEXT", nullable: false),
                    SessionName = table.Column<string>(type: "TEXT", nullable: false),
                    ChannelName = table.Column<string>(type: "TEXT", nullable: false),
                    BackupTime = table.Column<string>(type: "TEXT", nullable: false),
                    Size = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Tester = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    DateDownloadFile = table.Column<string>(type: "TEXT", nullable: false),
                    AWSlink = table.Column<string>(type: "TEXT", nullable: false),
                    VideoStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoAwsInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransID = table.Column<string>(type: "TEXT", nullable: false),
                    TransNB = table.Column<string>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<string>(type: "TEXT", nullable: false),
                    DVRStart = table.Column<string>(type: "TEXT", nullable: false),
                    Channel = table.Column<string>(type: "TEXT", nullable: false),
                    StartShift = table.Column<string>(type: "TEXT", nullable: false),
                    EndShift = table.Column<string>(type: "TEXT", nullable: false),
                    EmployeeID = table.Column<string>(type: "TEXT", nullable: false),
                    RegisterName = table.Column<string>(type: "TEXT", nullable: false),
                    LoyaltyCard = table.Column<string>(type: "TEXT", nullable: false),
                    SiteName = table.Column<string>(type: "TEXT", nullable: false),
                    SiteEmployee = table.Column<string>(type: "TEXT", nullable: false),
                    ExceptionAmount = table.Column<string>(type: "TEXT", nullable: false),
                    TOperatorID = table.Column<string>(type: "TEXT", nullable: false),
                    TPACID = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    RegionName = table.Column<string>(type: "TEXT", nullable: false),
                    Division = table.Column<string>(type: "TEXT", nullable: false),
                    ExceptionType = table.Column<string>(type: "TEXT", nullable: false),
                    Payment = table.Column<string>(type: "TEXT", nullable: false),
                    CardNo = table.Column<string>(type: "TEXT", nullable: false),
                    Desc = table.Column<string>(type: "TEXT", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    TransDate = table.Column<string>(type: "TEXT", nullable: false),
                    Assign = table.Column<string>(type: "TEXT", nullable: false),
                    HighRisk = table.Column<string>(type: "TEXT", nullable: false),
                    PreTransID = table.Column<string>(type: "TEXT", nullable: false),
                    PreTransDate = table.Column<string>(type: "TEXT", nullable: false),
                    PreTotalAmount = table.Column<string>(type: "TEXT", nullable: false),
                    HasRedeemLater = table.Column<string>(type: "TEXT", nullable: false),
                    YPredProba = table.Column<string>(type: "TEXT", nullable: false),
                    Bucket = table.Column<string>(type: "TEXT", nullable: false),
                    BucketSmall = table.Column<string>(type: "TEXT", nullable: false),
                    TransDateWeekdayMask = table.Column<string>(type: "TEXT", nullable: false),
                    TransDateFormatted = table.Column<string>(type: "TEXT", nullable: false),
                    VideoStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "Sheet");

            migrationBuilder.DropTable(
                name: "VideoAwsConfig");

            migrationBuilder.DropTable(
                name: "VideoAwsInfo");

            migrationBuilder.DropTable(
                name: "VideoInfo");
        }
    }
}

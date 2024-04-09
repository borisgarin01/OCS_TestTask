using FluentMigrator;

namespace Data.Migrations
{
    [Migration(1, "InitialMigration_09_04_2024_00_03")]
    public class InitialMigration : Migration
    {
        public override void Down()
        {
            Delete.Table("Activities");
        }

        public override void Up()
        {
            Create.Table("Activities")
                .WithColumn("ActivityType")
                .AsInt32().PrimaryKey()
                .NotNullable()
                .WithColumn("ActivityName")
                .AsString(63)
                .NotNullable()
                .WithColumn("Description")
                .AsString(63)
                .NotNullable();

            Create.Table("ApplicationsForComitteeConsideration")
                .WithColumn("Id")
                .AsGuid().PrimaryKey()
                .NotNullable()
                .WithColumn("ApplicationId")
                .AsGuid().NotNullable()
                .Unique()
                .WithColumn("SubmittingTimestamp")
                .AsDateTime();

            Create.Table("Applications")
                .WithColumn("Id")
                .AsGuid().PrimaryKey()
                .WithColumn("AuthorId")
                .AsGuid()
                .WithColumn("Activity")
                .AsInt32()
                .NotNullable()
                .WithColumn("Name")
                .AsString(99)
                .NotNullable()
                .WithColumn("Description")
                .AsString(299)
                .Nullable()
                .WithColumn("Outline")
                .AsString(999)
                .NotNullable()
                .WithColumn("CreationTimeStamp")
                .AsDateTime().NotNullable();
        }
    }
}

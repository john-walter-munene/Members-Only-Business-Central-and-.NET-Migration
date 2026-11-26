codeunit 50211 "Membership Management"
{
    // Create a new member in the Pending state. Never grant membership here.
    procedure RegisterMember(var ClubMember: Record "Club Member")
    begin
        // Require the Identity link and an email before inserting.
        ClubMember.TestField("Member No.");
        ClubMember.TestField("Identity User ID");
        ClubMember.TestField(Email);

        // Business rules: every new member starts as Pending and is not an admin.
        ClubMember."Membership Status" := ClubMember."Membership Status"::Pending;
        ClubMember."Is Administrator" := false;
        ClubMember."Registration Date" := CurrentDateTime();
        ClubMember."Last Modified Date" := CurrentDateTime();

        // Insert the record. The trigger that called this procedure skips its own insert.
        ClubMember.Insert(true);
    end;
}

page 50210 "Club Members API"
{
    PageType = API;
    Caption = 'Club Members API';
    APIPublisher = 'membersonly';
    APIGroup = 'club';
    APIVersion = 'v1.0';
    EntityName = 'clubMember';
    EntitySetName = 'clubMembers';
    SourceTable = "Club Member";

    // Let the trigger perform the insert so AL can apply the business rules.
    DelayedInsert = true;
    ODataKeyFields = SystemId;
    InsertAllowed = true;
    ModifyAllowed = false;
    DeleteAllowed = false;

    layout
    {
        area(Content)
        {
            repeater(Group)
            {
                field(id; Rec.SystemId) { Caption = 'Id'; Editable = false; }
                field(memberNo; Rec."Member No.") { Caption = 'Member No.'; }
                field(identityUserId; Rec."Identity User ID") { Caption = 'Identity User ID'; }
                field(firstName; Rec."First Name") { Caption = 'First Name'; }
                field(lastName; Rec."Last Name") { Caption = 'Last Name'; }
                field(email; Rec.Email) { Caption = 'Email'; }

                // AL sets this on insert, so the API client can't choose it.
                field(membershipStatus; Rec."Membership Status") { Caption = 'Membership Status'; Editable = false; }
                field(isAdministrator; Rec."Is Administrator") { Caption = 'Is Administrator'; Editable = false; }
            }
        }
    }

    trigger OnInsertRecord(BelowxRec: Boolean): Boolean
    var
        MembershipMgt: Codeunit "Membership Management";
    begin
        // Delegate the insert to the codeunit, which owns the rules.
        MembershipMgt.RegisterMember(Rec);

        // Return false because the codeunit already inserted the record.
        exit(false);
    end;
}
table 50201 "Club Member"
{
    Caption = 'Club Member';
    DataClassification = CustomerContent;

    fields
    {
        // Business key for the member, supplied by the caller.
        field(1; "Member No."; Code[20])
        {
            Caption = 'Member No.';
            DataClassification = CustomerContent;
        }

        // Links this member to the ASP.NET Core Identity user.
        field(2; "Identity User ID"; Text[450])
        {
            Caption = 'Identity User ID';
            DataClassification = SystemMetadata;
        }

        // Member's first name.
        field(3; "First Name"; Text[100])
        {
            Caption = 'First Name';
            DataClassification = CustomerContent;
        }

        // Member's last name.
        field(4; "Last Name"; Text[100])
        {
            Caption = 'Last Name';
            DataClassification = CustomerContent;
        }

        // Member's email address.
        field(5; Email; Text[250])
        {
            Caption = 'Email';
            DataClassification = CustomerContent;
        }

        // Current membership state. Set by AL, not by the client.
        field(6; "Membership Status"; Enum "Club Membership Status")
        {
            Caption = 'Membership Status';
            DataClassification = CustomerContent;
        }

        // Admin flag. Always false on registration.
        field(7; "Is Administrator"; Boolean)
        {
            Caption = 'Is Administrator';
            DataClassification = CustomerContent;
        }

        // Date and time the member registered.
        field(8; "Registration Date"; DateTime)
        {
            Caption = 'Registration Date';
            DataClassification = CustomerContent;
        }

        // Date and time the record was last changed.
        field(9; "Last Modified Date"; DateTime)
        {
            Caption = 'Last Modified Date';
            DataClassification = CustomerContent;
        }
    }

    keys
    {
        // Primary key on the member number.
        key(PK; "Member No.")
        {
            Clustered = true;
        }
    }
}
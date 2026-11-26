permissionset 50220 "Members Integration"
{
    Caption = 'Members Integration';
    Assignable = true;

    Permissions =
        tabledata "Club Member" = RIMD,
        page "Club Members API" = X;
}
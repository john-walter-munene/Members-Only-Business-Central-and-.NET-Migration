enum 50200 "Club Membership Status"
{
    Extensible = true;

    // Registered, but has not entered the passcode yet.
    value(0; Pending) { Caption = 'Pending'; }

    // Entered the correct passcode.
    value(1; Member) { Caption = 'Member'; }

    // Blocked from posting (used in Step 9 tests).
    value(2; Suspended) { Caption = 'Suspended'; }
}

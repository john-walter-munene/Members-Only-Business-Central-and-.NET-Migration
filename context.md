This makes the project considerably more interesting. You're not just porting Members-Only into Business Central; you're translating the entire Odin Project learning assignment into a different application architecture.

I would preserve the original ten learning objectives, but reinterpret each one using:

- **Business Central:** The business application, data model, pages, security and business logic.
- **AL:** The native Business Central development language.
- **C#/.NET:** The external application layer for functionality that belongs outside AL, including a possible public-facing interface.
- **SQL Server:** Business Central's native persistence layer, rather than PostgreSQL.
- **Business Central APIs:** Communication between the external .NET application and Business Central.

One architectural clarification: C# does not replace AL inside Business Central. AL is still responsible for custom tables, pages, codeunits, permission sets and extensions. C#/.NET is a complementary application layer.

I would also preserve the original assignment's distinction between authentication, membership and administrator privileges. Those are three different concerns, and keeping them separate will make this a much better learning exercise.

# **Members-Only: Business Central + .NET Migration Blueprint**

## **1. Target architecture**

I'd use a three-layer architecture.

**Presentation layer**

C# / .NET

ASP.NET Core MVC / Razor Pages

- Public clubhouse
- Registration and login
- Message forms
- Membership interface
- Admin interface

Business Central APIs / authenticated integration

**Business application layer**

AL

Microsoft Dynamics 365 Business Central

- Custom tables
- Pages
- Codeunits
- Enums
- Business rules
- Permission sets
- API pages

**Persistence layer**

SQL Server managed by Business Central

- Members
- Messages
- Membership configuration
- Business data and relationships

### **Division of responsibilities**

| Responsibility    | Technology                         |
| ----------------- | ---------------------------------- |
| User interface    | ASP.NET Core                       |
| Application logic | C#                                 |
| Authentication    | ASP.NET Core Identity              |
| Business data     | BC custom tables                   |
| Business rules    | AL codeunits                       |
| Authorization     | .NET policies + BC permission sets |
| Integration       | BC APIs                            |
| Persistence       | BC-managed SQL Server              |

A particularly important decision: don't store user passwords in your Business Central Member table. ASP.NET Core Identity should manage credentials and password hashing. Business Central should store the member's business information and membership status.

You also don't need to create a separate SQL database for the application initially.

# **2. The ten-step migration**

These are deliberately mapped one-to-one against the Odin assignment.

## **Step 01 — Design the database models**

Original assignment: Database modelling

**Odin objective:** Design users and messages with relationships, membership status and author tracking.

**Business Central implementation:**

Design the custom AL tables before building the interface.

I'd start with three tables.

**A. Member table**

```
Table: Club Member

Fields:
- Member No. (Primary Key)
- Identity User ID
- First Name
- Last Name
- Email
- Membership Status
- Is Administrator
- Registration Date
- Last Modified Date
```

`Identity User ID` is the identifier linking the BC member record to the ASP.NET Core Identity account.

**B. Message table**

```
Table: Club Message

Fields:
- Message No. (Primary Key)
- Title
- Message Text
- Author Member No.
- Created At
- Last Modified At
```

Create a table relation between `Author Member No.` and `Club Member`.

**C. Membership Setup table**

```
Table: Club Membership Setup

Fields:
- Primary Key
- Membership Passcode
- Membership Enabled
```

For production, consider storing a securely derived passcode hash rather than plaintext.

**Research independently:**

- AL table objects.
- Table fields and data types.
- Primary keys and secondary keys.
- TableRelation property.
- Enum data types.
- System audit fields.
- Database normalization.
- Business Central table extensions versus new tables.

**Completion criteria:** You can explain the relationships between members, messages and membership configuration before writing any pages.

## **Step 02 — Set up the application environment and project skeleton**

Original assignment: PostgreSQL and project skeleton

**Odin objective:** Set up PostgreSQL, generate the project structure and create database models.

**Business Central implementation:**

Instead of PostgreSQL, create a Business Central AL extension with SQL Server persistence managed by BC.

Your solution will have two projects:

```
MembersOnly/
│
├── MembersOnly.AL/
│   ├── app.json
│   ├── src/
│   │   ├── Tables/
│   │   ├── Pages/
│   │   ├── Codeunits/
│   │   ├── Enums/
│   │   ├── API/
│   │   └── PermissionSets/
│   └── tests/
│
└── MembersOnly.Web/
    ├── Controllers/
    ├── Models/
    ├── Views/
    ├── Services/
    ├── Data/
    ├── Identity/
    ├── Authorization/
    └── Program.cs
```

**Research independently:**

Business Central:

- AL Language extension.
- `app.json`.
- AL project structure.
- Object ID ranges.
- Launch configuration.
- Publishing extensions.
- Symbols and dependencies.

.NET:

- ASP.NET Core project structure.
- Dependency injection.
- Configuration and environment variables.
- Entity modelling for ASP.NET Core Identity.
- HttpClient and service registration.

**Completion criteria:** Both projects build independently, and the .NET application can communicate with a Business Central sandbox through an authenticated API.

## **Step 03 — Build user registration**

Original assignment: Signup, validation and bcrypt

**Odin objective:** Create users, validate registration details, confirm passwords and securely hash credentials.

**Business Central + .NET implementation:**

Use ASP.NET Core Identity for registration.

The registration process should be:

1. User opens registration form.
2. Enters first name, last name, email and password.
3. .NET validates the fields.
4. ASP.NET Core Identity hashes and stores the password.
5. .NET communicates with Business Central.
6. Business Central creates a corresponding Club Member record.
7. Member receives `Pending` membership status.

The important part is that registration must not automatically grant membership.

**Research independently:**

- ASP.NET Core Identity.
- IdentityUser and IdentityManager.
- Password hashing.
- Model validation.
- Data annotations.
- Custom validation attributes.
- Password confirmation.
- Business Central API pages.
- C# HttpClient.
- Transaction consistency across external systems.

**Completion criteria:** A user can register, authenticate and receive a corresponding Business Central member record without becoming an active member.

## **Step 04 — Implement the membership activation process**

Original assignment: Join the club using a secret passcode

**Odin objective:** A registered user enters a secret passcode to become a member.

**Business Central implementation:**

This is a particularly good opportunity to learn how Business Central handles configuration and business rules.

Create:

- Membership Setup table.
- Membership Setup page.
- Membership Management codeunit.
- Membership activation API operation.

Suggested business process:

```
Registered user
      |
      v
Pending membership
      |
      v
Enter membership passcode
      |
      v
.NET sends activation request
      |
      v
BC Membership Management Codeunit
      |
      v
Validate passcode against setup
      |
      +---- Invalid --> Reject
      |
      +---- Valid ----> Activate membership
```

The codeunit should own the activation rules rather than allowing the external application to directly change membership status.

**Research independently:**

- Business Central setup tables.
- Card pages.
- Single-instance versus ordinary codeunits.
- AL procedures.
- Codeunit event patterns.
- API actions and API pages.
- Secure secret management.
- Enum transitions.
- Error handling.

**Completion criteria:** A pending member can activate membership through a valid passcode, and an invalid passcode cannot change their status.

## **Step 05 — Implement authentication**

Original assignment: Passport.js login

**Odin objective:** Authenticate users using Passport.js.

**.NET implementation:**

Replace Passport.js with ASP.NET Core Identity authentication.

Your login process:

1. User enters email and password.
2. ASP.NET Core Identity verifies credentials.
3. Authentication cookie is issued.
4. User accesses protected routes.
5. Authorization policies determine which functionality is available.

Business Central does not need to authenticate every clubhouse member as a BC user.

In this architecture, the .NET application authenticates its users, while its integration identity communicates with Business Central.

If AppKings wants you to demonstrate actual BC user administration too, create separate BC users for the internal administrators and assign appropriate permission sets.

**Research independently:**

- ASP.NET Core authentication.
- Cookie authentication.
- Claims and claims principals.
- Authorization middleware.
- Role-based authorization.
- Policy-based authorization.
- Business Central authentication and OAuth 2.0.
- Microsoft Entra ID integration.

**Completion criteria:** Users can log in and access authenticated application functionality, with their membership status remaining independent of authentication.

## **Step 06 — Implement message creation**

Original assignment: Create a new message

**Odin objective:** Allow authenticated users to create messages.

**Business Central implementation:**

Create a message management workflow using:

- Club Message table.
- Message List page.
- Message Card page.
- Message Management codeunit.
- Message API page.

The workflow:

```
Authenticated user
       |
       v
Create message form (.NET)
       |
       v
Message API request
       |
       v
Business Central
       |
       v
Validate author and membership
       |
       v
Insert Club Message
```

Business Central should determine the author from the authenticated integration context and validated member association, not blindly trust an author ID submitted by the browser.

**Research independently:**

- AL page objects.
- List pages.
- Card pages.
- Page actions.
- Page triggers.
- Record data type.
- Insert and Modify methods.
- OnInsert trigger.
- Codeunit business logic.
- API pages.
- API entity sets.

**Completion criteria:** Authenticated users can create messages, and every message has a valid author relationship and timestamp.

## **Step 07 — Implement public and members-only visibility**

Original assignment: Different message visibility

**Odin objective:** Everyone can read messages, but only members can see the author's identity and date.

This is one of the most interesting parts of the migration.

I'd implement visibility as a server-side business rule.

| Visitor               | Message title | Message text | Author  | Date    |
| --------------------- | ------------- | ------------ | ------- | ------- |
| Anonymous             | Visible       | Visible      | Hidden  | Hidden  |
| Registered non-member | Visible       | Visible      | Hidden  | Hidden  |
| Active member         | Visible       | Visible      | Visible | Visible |
| Administrator         | Visible       | Visible      | Visible | Visible |

The .NET application can request messages from Business Central, then return different response DTOs depending on the authenticated user's membership.

For example:

```
public class PublicMessageDto
{
    public string Title { get; set; }
    public string MessageText { get; set; }
}
```

And:

```
public class MemberMessageDto
{
    public string Title { get; set; }
    public string MessageText { get; set; }
    public string AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

Do not simply hide the author column in the frontend. The public API response must not expose the protected information.

**Research independently:**

- ASP.NET Core authorization policies.
- DTOs.
- Response shaping.
- Data transfer objects.
- Business Central API queries.
- Filtering records.
- API security.
- Object-level authorization.

**Completion criteria:** Anonymous visitors cannot retrieve author information, while active members and administrators can.

## **Step 08 — Implement administrator functionality**

Original assignment: Admin flag and delete messages

**Odin objective:** Administrators can delete messages, while ordinary users cannot.

**Business Central implementation:**

Here I'd deliberately make the security model stronger than the original assignment.

Create an administrator role in your application and a corresponding BC permission set.

Suggested structure:

| Permission             | Member | Administrator |
| ---------------------- | ------ | ------------- |
| Read messages          | Yes    | Yes           |
| Create messages        | Yes    | Yes           |
| Read member identities | Yes    | Yes           |
| Delete messages        | No     | Yes           |
| Manage members         | No     | Yes           |
| Configure membership   | No     | Yes           |

Use the `Is Administrator` field as a business-role attribute, but don't rely on that field alone to authorize deletion.

You should also have:

- .NET administrator authorization policy.
- Business Central permission set.
- Server-side delete validation.
- Audit information for administrative operations.

Never allow someone to become an administrator by checking a registration form checkbox.

**Research independently:**

- AL permission sets.
- TableData permissions.
- Page permissions.
- Permission inheritance.
- ASP.NET Core role authorization.
- Authorization policies.
- Secure API endpoints.
- Audit logging.
- Record deletion.

**Completion criteria:** An ordinary member cannot delete messages even if they manually construct an API request. An authorized administrator can.

## **Step 09 — Complete the entire application**

Original assignment: Verify all functionality

**Odin objective:** Ensure the complete application satisfies all requirements.

**Business Central implementation:**

This becomes your integration and acceptance-testing milestone.

Build a test matrix:

| Test                             | Expected outcome        |
| -------------------------------- | ----------------------- |
| Register account                 | Pending member created  |
| Login                            | Authentication succeeds |
| Wrong password                   | Authentication rejected |
| Wrong passcode                   | Membership unchanged    |
| Correct passcode                 | Membership activated    |
| Create message                   | Message persisted in BC |
| Guest views messages             | Author and date hidden  |
| Member views messages            | Author and date visible |
| Member deletes message           | Rejected                |
| Administrator deletes message    | Successful              |
| Suspended member creates message | Rejected                |

I'd also add Business Central AL automated tests.

This is important because you want to prove both sides of the application work correctly:

- .NET application behavior.
- Business Central business rules and data integrity.

**Research independently:**

- AL test codeunits.
- Test runner codeunits.
- Test isolation.
- Automated integration testing.
- ASP.NET Core integration tests.
- API contract testing.
- Error handling.
- Logging and monitoring.

**Completion criteria:** All core acceptance tests pass, including security tests.

## **Step 10 — Deployment**

Original assignment: Deploy to a PaaS

**Odin objective:** Deploy the application and share it.

**Business Central implementation:**

Your deployment now has two distinct deliverables.

**A. Business Central extension**

- Publish AL extension.
- Install extension.
- Configure membership setup.
- Create permission sets.
- Assign permissions.
- Configure API integration.

**B. .NET application**

- Publish ASP.NET Core application.
- Configure environment variables.
- Configure Business Central API credentials.
- Configure Identity persistence.
- Configure HTTPS.
- Deploy the application.
- Test the live integration.

Your final demonstration should show someone visiting the public clubhouse, registering, activating membership, logging in, posting a message and seeing the correct visibility rules.

**Research independently:**

- Business Central extension deployment.
- Runtime versions.
- Extension publishing.
- ASP.NET Core publishing.
- Production configuration.
- Microsoft Entra ID.
- OAuth 2.0 client credentials.
- Secret management.
- Application logging.
- Deployment pipelines.

**Completion criteria:** The live .NET application communicates with your deployed Business Central environment and completes the entire membership workflow.

# **3. Final application object inventory**

By the time you finish, your Business Central extension should contain approximately these objects:

| Object type     | Objects                                                                  |
| --------------- | ------------------------------------------------------------------------ |
| Tables          | Club Member, Club Message, Club Membership Setup                         |
| Pages           | Members List, Member Card, Messages List, Message Card, Membership Setup |
| Codeunits       | Membership Management, Message Management                                |
| Enums           | Membership Status                                                        |
| API Pages       | Members API, Messages API                                                |
| Permission Sets | Members User, Members Administrator                                      |
| Profile         | Members Administrator Role Center                                        |
| Test Codeunits  | Membership Tests, Message Tests, Security Tests                          |

The .NET project will have its own controllers, services, Identity infrastructure, models, DTOs and authorization policies.

# **4. Important architectural decisions**

There are four things I'd keep in mind throughout the exercise.

1. **Business Central owns the business data.** Don't connect your C# application directly to BC's SQL Server database. Use supported APIs.
2. **.NET owns application authentication.** Don't store passwords in custom BC tables.
3. **AL owns business rules.** For example, membership activation and message deletion should be validated by BC, not just the frontend.
4. **Authorization happens at multiple levels.** Hiding a button is not security. Protect the actual API operations.

There is also a useful distinction between the two deployment scenarios:

- With Business Central on-premises, you can run the BC Server and its SQL Server infrastructure in your own environment.
- With Business Central online, Microsoft hosts that infrastructure, and your integration communicates through supported APIs.

Both can support this project.

# **5. Your independent research checklist**

Since you want to research and execute each step yourself, I'd use this as your master checklist.

**Migration progress**

# **0/30**

**Step 01 — Data modelling**

AL tables and fields

TableRelation and keys

Enums and relationships

**Step 02 — Environment**

AL development environment

ASP.NET Core project

BC API connectivity

**Step 03 — Registration**

ASP.NET Core Identity

Validation and password hashing

Member record creation

**Step 04 — Membership**

Setup tables and pages

Codeunits and business rules

Membership activation API

**Step 05 — Authentication**

Cookie authentication

Claims and authorization

BC OAuth integration

**Step 06 — Messages**

List and Card pages

CRUD operations

Message API

**Step 07 — Visibility**

DTOs and response shaping

Membership authorization

Public API security

**Step 08 — Administration**

AL permission sets

Administrator authorization

Audit and deletion

**Step 09 — Testing**

AL test codeunits

.NET integration tests

Security and acceptance tests

**Step 10 — Deployment**

Publish AL extension

Deploy ASP.NET Core

Configure production integration

Reset checklist

# **6. What I want you to get out of this project**

I wouldn't measure success by how quickly you finish it.

I'd measure it by whether, after completing the project, you can comfortably explain:

- How Business Central tables relate to one another.
- How AL codeunits implement business processes.
- How to create and configure Business Central users.
- How permission sets control access.
- How API pages expose business data.
- How external .NET applications integrate with BC.
- How to test and deploy an AL extension.

Those are transferable skills you can use on actual Business Central implementation work.

And one final strategic suggestion: **keep the original Odin Project implementation intact.** Don't replace it. Treat the BC + .NET implementation as a second edition of the same application.

You'll then have an interesting portfolio comparison: one business problem implemented using Node.js, Express and PostgreSQL, and another using AL, Business Central, C# and .NET.

That is a credible demonstration of your ability to move between application architectures.

For your AppKings objective, I'd make this your first serious Business Central pilot, document each milestone, and eventually present the finished application as a technical demonstration of what you can deliver.
# PROJECT PROPOSAL

|  |  |
| --- | --- |
| **NAME** | **RUSSEL M. LABIAGA** |
| **PROJECT TITLE  (specific)** | **Green Basket****: A Web-Based Disaster Recovery Planning and Readiness Management System with RTO/RPO Tracking** |
| **SUBJECT:** **CODE:** **TIME:** | **System Administration and Maintenance – IT23/L** **3493** **7:30-9:30 PM** |
| **PROJECT NAME** | **#37 Disaster Recovery Planner** |
| **Company/Institution** | **Hospital** |
| **Technologies and Framework:** | **Programming Language** -C# - JavaScript **Backend Technologies** - ASP.NET Core MVC -ASP.NET Core Identity - Entity Framework Core **Frontend Technologies** -Razor Views (.cshtml) - HTML5/CSS3 - JavaScript **Database** - Microsoft SQL Server |
| **Core Techniques** | **Asset prioritization** classify systems by criticality to decide restoration order. **RTO/RPO tracking** Define the maximum downtime and data loss allowed for each asset. **Dependency mapping** Record which backups or other assets each system needs to be recovered. **Recovery testing and comparison** Compare recovery time and data loss from drills against the targets. **Task tracking** Assign and monitor recovery actions with status and due dates. **Lessons-learned documentation** Record findings from tests and incidents to improve the plan. |
| **Development Tools** | **Visual Studio Code** Development environment **Git/GitHub** Version control and collaboration **SQL Server Management Studio (SSMS)** Database design and management |
| **Monitoring and Alert Features** | Secure login with three user roles Asset registry with priority level, RTO/RPO, and backup dependency Recovery procedure management (steps per asset) Emergency contact directory Recovery test logging with RTO/RPO comparison and lessons learned Recovery task tracking (assignee, status, due date) |
| **Target Users** | System Administrators IT Security Officers / Cybersecurity Analysts Hospital Management |
| **User Roles** | **Administrator** Full access. Manages user accounts and roles, and can create, view, edit, and delete all records. **DR Coordinator** Maintains the recovery plan. Can create, view, and edit assets, procedures, contacts, tests, and tasks, but cannot delete records or manage users. **Staff** Read-only. Can view assets, procedures, contacts, tests, and tasks, but cannot modify any data. |
| **LOGO/ICON** |  |
| **Project Objectives:** | Develop a centralized web-based system for managing disaster recovery plans, replacing scattered spreadsheets and documents. Maintain a prioritized registry of IT assets with RTO/RPO targets, backup dependencies, recovery procedures, and contacts. Record recovery tests and tasks, and document lessons learned to improve the plan. Provide role-based access for Administrators, DR Coordinators, and Staff. |
| **Project Description:** | The Disaster Recovery Planner is a web-based information system that helps a hospital prepare for IT-related incidents such as ransomware, destructive malware, system failure, or data loss. It does not perform backups or restores itself. It is the central place where the recovery plan is documented, so IT administrators and disaster recovery coordinators know which systems to restore first, which steps to follow, who to contact, and how fast recovery must happen. Staff members have read-only access. During an incident, confusion and delay increase downtime and data loss. In a hospital, where systems such as patient records, billing, and laboratory services must stay available, this can affect patient care. Keeping asset priorities, RTO/RPO targets, procedures, and contacts in one place reduces guesswork during a crisis, and recording tests and lessons learned helps the hospital find weaknesses before a real incident. The system is built with ASP.NET Core MVC and C#, using Razor Views, HTML5, CSS3, and JavaScript for the interface. Entity Framework Core connects it to a Microsoft SQL Server database, and ASP.NET Core Identity handles login and role-based access. ASP.NET Core MVC keeps the code organized, SQL Server suits the relational data, and Identity provides built-in role management. Development uses Visual Studio Code, SQL Server Management Studio, and Git/GitHub. |
| **System Flow** | Log In → Check User Role → Register and Prioritize Assets → Manage Recovery Procedures → Maintain Emergency Contacts → Log Recovery Tests → Track Recovery Tasks → Document Lessons Learned |
| **Date Submitted:** | October 26, 2026 |
| **Teacher’s Feedback** | _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ **________________________________            __________________________   ** Student’s Signature (after feedbacking)                       Teacher’s Signature |
|  |  |

## Teacher's Remarks

- Improve system flow
- Include how the system handles successful and failed recovery tests, particularly in RTO/RPO Tracking

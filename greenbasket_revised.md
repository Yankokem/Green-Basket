# PROJECT PROPOSAL

|  |  |
| --- | --- |
| **NAME** | **RUSSEL M. LABIAGA** |
| **PROJECT TITLE  (specific)** | **Green Basket**: A Web-Based Disaster Recovery Planning and Readiness Management System with RTO/RPO Tracking |
| **SUBJECT:** **CODE:** **TIME:** | **System Administration and Maintenance – IT23/L** **3493** **7:30-9:30 PM** |
| **PROJECT NAME** | **#37 Disaster Recovery Planner** |
| **Company/Institution** | **Hospital** |
| **Technologies and Framework:** | **Programming Language** - C# - JavaScript **Backend Technologies** - ASP.NET Core MVC - ASP.NET Core Identity - Entity Framework Core **Frontend Technologies** - Razor Views (.cshtml) - HTML5/CSS3 - JavaScript **Database** - Microsoft SQL Server |
| **Core Techniques** | **Asset prioritization** classify systems by criticality to decide restoration order. **RTO/RPO tracking** Define the maximum downtime and data loss allowed for each asset. **Dependency mapping** Record which backups or other assets each system needs to be recovered. **Recovery testing and comparison** Compare actual recovery time and data loss from drills against targets, classifying drills as Passed or Failed. **Remediation & task tracking** Trigger corrective tasks and procedure updates when drills fail, tracking status and due dates until re-test. **Lessons-learned documentation** Record root causes and findings from both successful and failed drills to improve the plan. |
| **Development Tools** | **Visual Studio Code** Development environment **Git/GitHub** Version control and collaboration **SQL Server Management Studio (SSMS)** Database design and management |
| **Monitoring and Alert Features** | Secure login with three user roles; Asset registry with priority level, target RTO/RPO, readiness status, and backup dependency; Recovery procedure management (steps per asset); Emergency contact directory; Recovery test logging with automatic RTO/RPO compliance comparison (Met vs. Overrun/Breached) and lessons learned; Corrective recovery task tracking (assignee, status, due date) linked to drill outcomes |
| **Target Users** | System Administrators, IT Security Officers / Cybersecurity Analysts, Hospital Management |
| **User Roles** | **Administrator** Full access. Manages user accounts and roles, and can create, view, edit, and delete all records. **DR Coordinator** Maintains the recovery plan. Can create, view, and edit assets, procedures, contacts, tests, and tasks, but cannot delete records or manage users. **Staff** Read-only. Can view assets, procedures, contacts, tests, and tasks, but cannot modify any data. |
| **LOGO/ICON** |  |
| **Project Objectives:** | Develop a centralized web-based system for managing disaster recovery plans, replacing scattered spreadsheets and documents. Maintain a prioritized registry of IT assets with RTO/RPO targets, backup dependencies, recovery procedures, and contacts. Evaluate recovery drills against RTO/RPO targets to identify readiness or breaches, drive corrective tasks and lessons learned, and ensure continuous improvement through re-testing. Provide role-based access for Administrators, DR Coordinators, and Staff. |
| **Project Description:** | The Disaster Recovery Planner is a web-based information system that helps a hospital prepare for IT-related incidents such as ransomware, destructive malware, system failure, or data loss. It does not perform backups or restores itself. It is the central place where the recovery plan is documented, so IT administrators and disaster recovery coordinators know which systems to restore first, which steps to follow, who to contact, and how fast recovery must happen. Staff members have read-only access. During an incident, confusion and delay increase downtime and data loss. In a hospital, where systems such as patient records, billing, and laboratory services must stay available, this can affect patient care. Keeping asset priorities, RTO/RPO targets, procedures, and contacts in one place reduces guesswork during a crisis, while evaluating recovery tests against RTO/RPO targets ensures failures trigger corrective tasks, procedure revisions, and lessons learned before a real incident occurs. The system is built with ASP.NET Core MVC and C#, using Razor Views, HTML5, CSS3, and JavaScript for the interface. Entity Framework Core connects it to a Microsoft SQL Server database, and ASP.NET Core Identity handles login and role-based access. |
| **System Flow** | Log In → Check User Role (*Staff*: Read-only view across modules; *Admin*: User/role management + full access; *Admin/DR Coordinator*: Full planning & testing workflow) → Plan Setup (Register & Prioritize Assets with Target RTO/RPO → Document Recovery Procedures → Maintain Emergency Contacts) → Conduct & Log Recovery Test (Record Actual RTO/RPO) → Evaluate Actual vs. Target RTO/RPO → **If Passed:** Confirm Asset Readiness & record observations in Lessons Learned → await next scheduled drill; **If Failed:** Flag RTO/RPO Breach & overrun margin → require root cause in Lessons Learned → create/assign corrective Recovery Tasks & update Recovery Procedures → track tasks until Done → loop back to Re-Test Asset |
| **Date Submitted:** | October 26, 2026 |
| **Teacher’s Feedback** | _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ _________________________________________________________________ **________________________________            __________________________   ** Student’s Signature (after feedbacking)                       Teacher’s Signature |
|  |  |

---

## Improved System Flow

Instead of a linear one-way path, the system operates as a **role-aware, continuous readiness loop** across the existing modules (Users & Roles, Assets, Recovery Procedures, Contacts, Recovery Tests, and Recovery Tasks):

```text
Log In
  │
  ▼
Check User Role
  ├─► Staff ──────────────► Read-Only View (Assets, Procedures, Contacts, Tests, Tasks)
  ├─► Admin ──────────────► Manage Users & Roles (plus full plan access below)
  └─► Admin / DR Coordinator
        │
        ▼
  [1. Plan Setup & Preparation]
  Register & Prioritize Assets (Define Target RTO, Target RPO, Backup Dependencies)
        ├──► Manage Recovery Procedures (Step-by-step restoration per asset)
        └──► Maintain Emergency Contacts (Escalation list per asset)
        │
        ▼
  [2. Drill Execution & RTO/RPO Evaluation]
  Log Recovery Test (Record Actual RTO, Actual RPO, Execution Notes)
        │
        ▼
  Compare Actual vs. Target RTO/RPO
        │
        ├──► PASS (Actual ≤ Target for both RTO & RPO)
        │          │
        │          ▼
        │    Mark Asset "DR Ready" / Compliant
        │    Document efficiency notes in Lessons Learned
        │    Wait for next scheduled drill
        │
        └──► FAIL (Actual > Target for RTO or RPO, or drill incomplete)
                   │
                   ▼
             Flag Asset "At Risk" & Display Breach Margin (e.g., +30m Over RTO)
                   │
                   ▼
             Require Root Cause Analysis (Lessons Learned)
                   │
                   ▼
             Create/Assign Corrective Recovery Tasks & Update Recovery Procedures
                   │
                   ▼
             Track Tasks until "Done" ──► Loop back to Re-Test Asset
```

---

## Handling Successful and Failed Recovery Tests (RTO/RPO Tracking)

All test evaluation and follow-up behavior connects existing modules without introducing any extra module:

### 1. Successful Recovery Test (**Passed**)
* **Condition:** `Actual RTO ≤ Asset Target RTO` **and** `Actual RPO ≤ Asset Target RPO`, with restoration steps completed successfully.
* **System Handling:**
  * **RTO/RPO Tracking:** Both metrics are flagged as **Met** (compliant).
  * **Asset Readiness:** Confirms the asset's recovery plan meets hospital downtime and data-loss tolerances.
  * **Documentation:** Optional observations or speed-up ideas are recorded under **Lessons Learned**.
  * **Next Step:** No urgent remediation is required; the system remains ready until the next periodic drill.

### 2. Failed Recovery Test (**Failed / Breached**)
* **Condition:** `Actual RTO > Asset Target RTO`, **or** `Actual RPO > Asset Target RPO`, **or** the restoration drill could not be completed.
* **System Handling:**
  * **RTO/RPO Tracking:** Highlights the specific metric(s) breached and calculates the overrun margin (e.g., *+15m Over RTO* or *+10m Over RPO*), preventing an over-target drill from being treated as compliant.
  * **Asset Readiness:** Flags the asset as needing attention rather than DR-ready.
  * **Lessons Learned (Required):** Captures the root cause of the failure (e.g., slow backup throughput, missing dependency, or outdated restoration steps).
  * **Remediation Loop (Existing Modules):** Drives the coordinator to create follow-up **Recovery Tasks** (assigned to personnel with due dates) and/or revise the asset's **Recovery Procedures**. Once tasks are marked *Done*, the asset loops back for a **re-test** to verify compliance.

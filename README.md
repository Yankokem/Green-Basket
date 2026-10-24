# 🛡️ Green Basket — Hospital Disaster Recovery Planner

> **System #37: Disaster Recovery Planner**  
> **Assigned to:** LABIAGA  
> **Institution:** Hospital IT Environment  
> **Problem Domain:** Operational impact following ransomware, destructive malware, system failure, or critical clinical data loss.

---

## 📌 Project Overview

**Green Basket** is a hospital IT disaster recovery planning and readiness information system. It serves as the single source of truth for the IT emergency response team *before* and *during* a disaster—ensuring personnel know which clinical systems to restore first, who to contact, what technical procedures to execute, and how quickly operations must resume.

> [!NOTE]  
> Green Basket is a **planning and readiness information system**. It maintains plans, contacts, dependencies, and target metrics; it does not perform raw disk backups or restorations directly.

---

## 💻 Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core MVC (.NET 8 / C#) |
| **Frontend** | Razor Views (`.cshtml`), Tailwind CSS (CDN), Lucide Icons *(Zero Bootstrap)* |
| **Database** | Microsoft SQL Server LocalDB (`(localdb)\mssqllocaldb`) |
| **Data Access** | Entity Framework Core (Code-First) |
| **Authentication** | ASP.NET Core Identity with Role-Based Authorization |
| **Tools** | Visual Studio Code, Git/GitHub, SQL Server Management Studio (SSMS) |

---

## 👥 User Roles & Permission Matrix

The system implements 3 distinct roles enforced at both the Controller (`[Authorize(Roles = "...")]`) and View levels:

| Module | Admin (`Alexander Hayes`) | DR Coordinator (`Marcus Vance`) | Staff (`Clara Oswald`) |
|---|:---:|:---:|:---:|
| **Assets** | Full CRUD | Create, Read, Update | Read-Only |
| **Recovery Procedures** | Full CRUD | Create, Read, Update | Read-Only |
| **Contacts** | Full CRUD | Create, Read, Update | Read-Only |
| **Recovery Tests** | Full CRUD | Create, Read, Update | Read-Only |
| **Recovery Tasks** | Full CRUD | Create, Read, Update | Read-Only |
| **User & Role Management** | ✅ Yes | ❌ No | ❌ No |

---

## 🔑 Demo Accounts

The database seeds automatically on first startup with pre-configured accounts:

| Name | Role | Email | Password | Scope |
|---|---|---|---|---|
| **Alexander Hayes** | Admin | `admin@greenbasket.local` | `123` | Full access, user management, and deletion |
| **Marcus Vance** | DR Coordinator | `coordinator@greenbasket.local` | `123` | Plan authoring & updates (no delete) |
| **Clara Oswald** | Staff | `staff@greenbasket.local` | `123` | Read-only observation across all modules |

> [!TIP]  
> The login screen at `/Account/Login` includes **1-click quick-fill buttons** for each demo role so you can switch between roles instantly during grading or testing.

---

## 🗄️ Core Data Model (Asset-Centric)

All disaster recovery modules anchor to a central **Asset**:

```
                 ┌─────────────────────────────────────┐
                 │                Asset                │
                 │ (Name, Type, Owner, Priority,       │
                 │  RTO, RPO, BackupDependency)        │
                 └──────────────────┬──────────────────┘
                                    │
         ┌──────────────────────────┼──────────────────────────┐
         │ 1:N                      │ 1:N                      │ 1:N
┌────────┴──────────────┐  ┌────────┴──────────────┐  ┌────────┴──────────────┐
│  Recovery Procedures   │  │   Emergency Contacts  │  │    Recovery Tests     │
│ (Title, Steps)        │  │ (Name, Role, Phone,   │  │ (TestDate, Result,    │
└───────────────────────┘  │  Email)               │  │  ActualRTO, ActualRPO, │
                           └───────────────────────┘  │  Notes, LessonsLearned)│
                                                      └───────────────────────┘
                                    │ 1:N
                           ┌────────┴──────────────┐
                           │    Recovery Tasks     │
                           │ (Title, AssignedTo,   │
                           │  Status, DueDate)     │
                           └───────────────────────┘
```

### Key Metrics Defined:
- **RTO (Recovery Time Objective):** Maximum acceptable downtime for a clinical system (e.g., 60 minutes for Epic EMR).
- **RPO (Recovery Point Objective):** Maximum acceptable data loss measured in time (e.g., 15 minutes of clinical transaction loss).
- **Asset Priority:** Tier 1 (Critical), Tier 2 (High), Tier 3 (Medium), Tier 4 (Low) deciding restore precedence.
- **RTO/RPO Compliance Evaluation:** Drill results dynamically calculate and badge whether actual failover times met target thresholds or exceeded them.

---

## 🚀 Getting Started

### 1. Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (LocalDB `(localdb)\mssqllocaldb` or full SQL Server instance)

### 2. Clone the Repository
```bash
git clone https://github.com/Yankokem/Green-Basket.git
cd Green-Basket
```

### 3. Database Setup
The connection string in `appsettings.json` points to LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GreenBasket_DRP;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
*Note: EF Core automatically creates the database schema and populates sample hospital assets, procedures, contacts, tests, tasks, and users on first run.*

### 4. Run the Project
```bash
dotnet run
```
Navigate to `http://localhost:5000` (or the URL displayed in the terminal) in your browser.

---

## 🎨 UI Architecture Highlights
- **Top Navigation Layout:** Optimized horizontal real estate for dense clinical disaster recovery matrices without a restricting sidebar.
- **Tailwind CSS + Lucide Icons:** Clean, modern, responsive design system.
- **Reusable Partial Components:** Centralized in `Views/Shared/Components/` (`_Badge`, `_Button`, `_PageHeader`, and consistent form inputs).
- **Status & Compliance Indicators:** Color-coded badges for Priority levels, Drill outcomes (*Met Target* vs *Exceeded*), and Task statuses (*Pending*, *In Progress*, *Done*).

---

## 📄 License
Academic project developed for System #37 (Disaster Recovery Planner).

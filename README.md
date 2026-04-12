# 📋 SistemaPresença

A web-based academic attendance management system developed in **ASP.NET Core 3.1 (MVC)**, integrating **SQL Server**, **MongoDB**, and **IoT biometric devices** via the **FIWARE Helix IoT** platform over MQTT. Students register attendance automatically through a fingerprint reader connected to an IoT agent.

---

## 📐 What it does

The system manages the full academic attendance workflow:

- Teachers register students, subjects, and classes
- Each student is provisioned as an IoT device on the FIWARE Helix platform, linked to their biometric ID
- When a student places their finger on the reader, the biometric device sends an MQTT message to the IoT Agent
- The system polls MongoDB (Orion Context Broker history) to retrieve new biometric events, cross-references them with the class schedule, and persists attendance records in SQL Server
- Teachers can query attendance reports filtered by student, class, or subject — with absent students automatically filled in

---

## ✨ Features

- **Student management** — register with name, email, phone, CPF (validated), photo, and biometric ID
- **Subject and class management** — CRUD for academic subjects and scheduled classes
- **Biometric IoT provisioning** — each new student is automatically provisioned on the FIWARE Helix IoT Agent via REST API (MQTT transport, UltraLight protocol)
- **Automated attendance ingestion** — reads biometric events from MongoDB (Orion STH-Comet history) and inserts into SQL Server, deduplicating by datetime
- **Attendance query** — filterable grid by student and/or class; absent students are automatically included in the result
- **Attendance percentage per subject** — calculated on the dashboard from present records vs. total expected presences
- **Session-based authentication** — teacher and student login with BCrypt password hashing
- **Stored procedure-based data access** — all CRUD operations go through SQL Server stored procedures via a generic `BaseDAO<T>`

---

## 🏗️ Architecture

```
Browser (Razor Views)
        │
ASP.NET Core MVC Controllers
        │
       DAO Layer (BaseDAO<T> + specifics)
      /          \
SQL Server     MongoDB (via MongoDB.Driver)
(Azure)        (FIWARE STH-Comet history)
        │
FIWARE Helix IoT Platform (REST API)
        │
   MQTT Broker → Biometric Device (fingerprint reader)
```

**Data flow for attendance:**

1. Student touches the fingerprint reader
2. The IoT device sends an MQTT message to the FIWARE IoT Agent
3. Orion Context Broker stores the event in MongoDB (STH-Comet)
4. The system fetches new MongoDB documents per student, compares with already-recorded attendance datetimes, and inserts new records into SQL Server

---

## 🗂️ Project Structure

```
SistemaPresenca/
├── Controllers/
│   ├── BaseController.cs              # Generic CRUD controller (T : BaseViewModel)
│   ├── StudentController.cs           # Student CRUD + IoT provisioning on insert
│   ├── TeacherController.cs           # Teacher CRUD
│   ├── SubjectController.cs           # Subject CRUD
│   ├── ClassController.cs             # Class CRUD
│   ├── AttendanceQueryController.cs   # Attendance dashboard + MQTT read trigger
│   ├── ClassQueryController.cs        # Class listing with filters
│   ├── ListingQueryController.cs      # General listing view
│   ├── LoginController.cs             # Authentication (teacher + student)
│   ├── HomeController.cs              # Landing page
│   ├── AboutController.cs             # About page
│   └── ControllerHelper.cs            # Session auth helpers
├── DAO/
│   ├── BaseDAO.cs                     # Generic DAO: Insert, Update, Delete, Query, GetListing
│   ├── AttendanceDAO.cs               # Attendance + MongoDB ingestion logic
│   ├── ClassDAO.cs                    # Class queries including QueryByDate (for MQTT matching)
│   ├── StudentDAO.cs                  # Student DAO with biometric ID generation
│   ├── SubjectDAO.cs                  # Subject DAO
│   ├── TeacherDAO.cs                  # Teacher DAO
│   ├── LoginDAO.cs                    # Login DAO (BCrypt)
│   └── HelperDAO.cs                   # ADO.NET executor: stored procs + SQL functions
├── Models/
│   ├── BaseViewModel.cs               # Base model with ID
│   ├── StudentViewModel.cs            # Student + image + biometric ID
│   ├── ClassViewModel.cs              # Class with subject reference
│   ├── AttendanceViewModel.cs         # Presence record with student/class join data
│   ├── CommandViewModel.cs            # MongoDB document model (STH-Comet)
│   └── ...                            # Other view models
├── HelixConnection/
│   └── MQTTConnection.cs              # REST calls to FIWARE IoT Agent and Orion CB
├── Helpers/
│   ├── PasswordHasher.cs              # BCrypt wrapper
│   └── ValidationHelpers.cs           # CPF validation, uniqueness check
├── Enumerators/
│   ├── EnumClassStatus.cs
│   └── EnumTableType.cs
├── Views/                             # Razor views per controller + shared layout
├── wwwroot/                           # Static assets (Bootstrap, jQuery, CSS, images)
├── appsettings.json                   # SQL Server connection string
└── Startup.cs                         # DI, session, routing config
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | ASP.NET Core 3.1 (MVC) |
| Frontend | Razor Views + Bootstrap 4 + jQuery |
| Primary database | SQL Server (Azure) — ADO.NET via stored procedures |
| IoT event store | MongoDB — FIWARE STH-Comet history |
| IoT platform | FIWARE Helix IoT (Orion Context Broker + IoT Agent) |
| IoT protocol | MQTT / UltraLight 2.0 |
| Auth | Session-based + BCrypt (`BCrypt.Net-Next`) |
| HTTP client | RestSharp |
| MongoDB client | MongoDB.Driver 2.18 |

---

## 🚀 Getting Started

### Prerequisites

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
- SQL Server instance with the `SistemaAcademico` database and stored procedures
- FIWARE Helix IoT platform (optional — required only for biometric attendance)

### Running

```bash
# Clone the repository
git clone https://github.com/GabrielGutierri/SistemaPresenca.git
cd SistemaPresenca

# Restore dependencies
dotnet restore

# Run
dotnet run
```

Then open `https://localhost:5001` in the browser.

### Configuration

Update `appsettings.json` with your own SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "SQLServerDbConnectionString": "Data Source=<server>;Initial Catalog=SistemaAcademico;User ID=<user>;Password=<password>"
  }
}
```

> **Note:** The original project connects to an Azure SQL instance and a FIWARE Helix server hosted at a fixed IP. Both are inactive. To run locally, point the connection string to your own SQL Server and optionally stub out the `MQTTConnection` calls.

---

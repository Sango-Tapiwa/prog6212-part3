# Contract Monthly Claim System (CMCS)

## 📖 What the App is About

The **Contract Monthly Claim System (CMCS)** is a .NET Core MVC web application that streamlines how Independent Contractor (IC) lecturers submit and track monthly claims. Instead of handling paper forms or emails, lecturers submit claims digitally, while Coordinators, Managers, and HR process approvals and manage user data.
The system improves accuracy, reduces delays, and provides full transparency during the approval process.

---

## 🚀 How the App Runs

### 1. **Login & Role-Based Access**

* Users must **log in** to access the system.
* HR creates all user accounts (no public registration).
* Four system roles:

  * **Lecturer**
  * **Programme Coordinator**
  * **Academic Manager**
  * **HR (Super User)**

Each role sees a customised dashboard.

---

## 👩‍🏫 Lecturer Workflow (Updated per feedback)

* Lecturer logs in and automatically sees **their profile data**:

  * Name
  * Surname
  * Hourly rate
    *(All pulled from HR; lecturers can no longer enter their own rate.)*
* When submitting a claim, the lecturer only inputs:

  * Date worked
  * Task/Activity
  * Hours worked
  * Attachments (supporting documents)
* The system performs:

  * **Auto-calculation** of the claim amount (HourlyRate × HoursWorked)
  * **Validation** preventing claims over **180 hours per month**
* Lecturer can view:

  * Claim details
  * Current approval stage
  * Whether the coordinator and manager have reviewed the claim
  * Final approval status

---

## 🧑‍🏫 Programme Coordinator Workflow

* Sees all lecturer claims assigned to their programme.
* Reviews claims by checking:

  * Hours
  * Auto-calculated totals
  * Uploaded documents
* Can **approve** or **reject** claims.
* Approved claims move to the Academic Manager.

---

## 🧑‍💼 Academic Manager Workflow

* Sees only coordinator-approved claims.
* Performs final verification.
* Can **approve or reject**.
* Once approved here, the claim shows as **Fully Approved** on the lecturer’s side.

---

## 🔄 Approval Flow (Improved)

The approval chain now follows a strict two step process:

1. **Lecturer → Coordinator**
2. **Coordinator → Manager**
3. **Manager → Final Status** (Approved or Rejected)

A claim is only considered approved after **both** Coordinator and Manager approve it.
Lecturers immediately see the updated status in their tracking view.

---

## 🧑‍💻 HR Workflow (New Role Added)

HR is the **super user** of the system.

HR can:

* Create all user accounts (name, surname, email, role, hourly rate)
* Update any user’s profile
* Modify hourly rate and lecturer details
* View all claims in the system
* Generate reports and invoices using **LINQ**
* Export data as:

  * **PDF** (ideal for invoices)
  * **CSV** (ideal for summaries)

Registration is disabled for all other users HR handles all onboarding.

---

## 📊 Tracking & Transparency

Across the system:

* Every user sees only what is relevant to their role.
* Lecturers track claim statuses in real time.
* Coordinators and Managers clearly see where a claim stands.
* HR has full system visibility.

---

## 🛠️ How It Runs in Practice

* Built using **ASP.NET Core MVC**
* Uses **Entity Framework Core** for database interaction
* Database: Microsoft SQL Server (which is the approved for this POE)The app is configured with a SQL Server connection string.
* Bootstrap is used for a clean and responsive UI
* Runs locally in **Visual Studio 2022**
* Session-based authentication ensures secure access

---
## References
1.	Microsoft. (2023). ASP.NET Core MVC overview. Available at: https://learn.microsoft.com/aspnet/core/mvc [Accessed 2 Sept. 2025].
2.	W3Schools. (n.d.) ASP Tutorial. Available at: https://www.w3schools.com/asp/default.ASP [Accessed: 26 August 2025].




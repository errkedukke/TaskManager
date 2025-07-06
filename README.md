# 🗂️ Task Management Web API

A clean, modular .NET Web API for managing tasks and users, built as a coding assignment for Login VSI via CoderStaffing.

---

## 🧱 Architecture

The solution follows **Clean Architecture** principles with a layered structure:

```
src/
├── API/
│   └── TaskManager.API                 → Entry point, controllers, setup
├── Core/
│   ├── TaskManager.Domain              → Entities, enums, domain events
│   └── TaskManager.Application         → Interfaces, services, business logic
├── Infrastructure/
│   └── TaskManager.Infrastructure      → Background Services
│   └── TaskManager.Persistence         → EF Core context, configs, seeders

tests/
├── API/
│   └── TaskManager.API.Tests           → Controller/API layer tests
└── Core/
    └── TaskManager.Application.Tests   → Unit tests for business logic
```

---

## 📦 Features

- ✅ Create and fetch **Users** with unique names  
- ✅ Create and fetch **Tasks** with unique titles and states  
- ✅ Automatically assign tasks to users on creation  
- ✅ Reassign tasks every 2 minutes in the background  
- ✅ Prevent reassignment to the current or previous user  
- ✅ Ensure all users are eventually assigned before the task is marked `Completed`  
- ✅ Task assignment history tracked for auditing  
- ✅ Fully unit-tested core logic (NUnit + Moq)

---

### Run the API Or Run Unit Tests

Sample data is seeded in in-memory DB during the startap run if the environment is Development:
```
TaskManager.Persistence/Data/
├── Users.json
├── TaskAssignmentRecords.json
└── TaskItems.json

```


## 🧠 Design Highlights

- `BackgroundService` handles scheduled reassignment logic
- Domain event triggers assignment after task creation
- Service + repository layers decouple infrastructure from business logic
- Exception-safe, testable, and extensible architecture

---

## 🤝 Author

Built by **Guga R** as a technical assignment for Login VSI via CoderStaffing.  
Designed with care for clarity, robustness, and real-world maintainability.

---

## 📫 Contact

If you'd like to connect or give feedback:  
📧 ruxadze0@gmail.com

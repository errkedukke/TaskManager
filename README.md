# ✅ Task Management Web API – Assignment Checklist

This project is a simple Web API for managing users and tasks with automatic task assignment and reassignment logic.

---

## 🏗️ General Structure

- ✅ Create Web API project (no UI)
- ✅ Implement `User` entity with:
  - ✅ Unique `Name`
- ✅ Implement `TaskItem` entity with:
  - ✅ Unique `Title`
  - ✅ `State` enum: `Waiting`, `InProgress`, `Completed`
  - ⬜ One assigned user
  - ⬜ Previous assigned user
  - ⬜ Assignment history (track all users the task has been assigned to)

---

## 🔧 API Functionality

### 👤 Users
- ✅ Create user endpoint
- ✅ Get all users endpoint

### 📋 Tasks
- ✅ Create task endpoint
- ✅ Get all tasks endpoint
- ⬜ On task creation:
  - ⬜ Automatically assign to a random available user
  - ⬜ If no users are available, set task state to `Waiting`

---

## ⏱️ Background Service (runs every 2 minutes)

- ⬜ Reassign active tasks to a **new random user**:
  - ⬜ Not the current assigned user
  - ⬜ Not the previously assigned user (last round)
  - ⬜ Can be a user assigned 2+ rounds ago
  - ⬜ If no eligible user, keep task in `Waiting`
- ⬜ Track assignment history for each task
- ⬜ Mark task as `Completed` and unassign once it has been assigned to **all users**, including newly created ones

---

## ⚠️ Edge Case Handling

- ⬜ Newly created users should eventually receive all incomplete tasks
- ⬜ `Completed` tasks should not be reassigned
- ⬜ If no users are available, task remains in `Waiting`

---

## 💡 Project Requirements

- ✅ Easy to run/debug (includes unit tests)
- ✅ Clean architecture (Domain, Application, Infrastructure, API)
- ✅ Structured logging (e.g. `_logger.LogInformation("...")`)
- ✅ Lightweight DB (In-Memory or SQLite for simplicity)

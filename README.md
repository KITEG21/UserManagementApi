# 🚀 UserManagementApi

A high-performance, modular, and secure RESTful API built with **FastEndpoints** and structured using the **Clean Architecture** pattern. This API integrates modern tooling like **Mapperly**, **FluentValidation**, and **EF Core** with **PostgreSQL**, and supports **JWT-based authentication and authorization**.

---

## 🧱 Project Structure

The project follows **Clean Architecture**, separating concerns into well-defined layers:


---

## ⚙️ Technologies Used

- **[FastEndpoints](https://fast-endpoints.com/)** – Fast and minimalist .NET API framework
- **[Mapperly](https://mapperly.riok.app/)** – Zero-runtime overhead object mapping
- **[FluentValidation](https://docs.fluentvalidation.net/en/latest/)** – Strongly-typed model validation
- **[EF Core](https://learn.microsoft.com/en-us/ef/core/)** – ORM for PostgreSQL
- **[PostgreSQL](https://www.postgresql.org/)** – Relational database backend
- **[JWT](https://jwt.io/)** – Token-based authentication
- **Clean Architecture** – For separation of concerns and scalability

---

## 🔐 Authentication

Authentication is handled via **JWT Bearer tokens**. Secure endpoints require a valid token passed in the `Authorization` header:

User registration and login endpoints are exposed for token generation.

---

Let me know if you'd like me to include example endpoint definitions or Docker integration in the README.



# 🚀 Vide Store - E-Commerce Web API

![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&style=for-the-badge&logoColor=white)
![JSON](https://img.shields.io/badge/JSON-000?logo=json&style=for-the-badge&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)

<p align="center">
  <b>Vide Store is a modern e-commerce platform built with ASP.NET Core API designed with a focus on Clean Architecture.</b>
</p>

## 📚 Project Overview
Vide Store is a scalable and secure e-commerce backend API built using **ASP.NET Core** and **Entity Framework Core**. It follows **Clean Architecture** principles to ensure maintainability and extensibility. The platform supports efficient inventory management, order processing, authentication, and email notifications.

## 🌟 Features
- **Redis Caching**: Enhances performance through efficient caching mechanisms for cart and session data.
- **Authentication and Authorization**: Implements **JWT Authentication**, **Refresh Tokens**, and **Google OAuth** for secure access.
- **Email Services**: Integrated **SMTP** for email verification and user notifications.
- **Background Jobs**: Utilizes **Hangfire** for scheduled tasks like email dispatching and order processing.
- **Database Management**: Built on **Microsoft SQL Server** with a structured relational model.
- **Role-Based Access Control**: Provides granular access permissions using **ASP.NET Core Identity**.
- **Design Patterns**: Utilizes **Repository Pattern**, **Unit of Work**, and **Specification Pattern** for organized code.

---

## 💎 Prerequisites
- [.NET 8](https://dotnet.microsoft.com/pt-br/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/)
- [Redis](https://redis.io/)

---

## 🎇 Setup and Configuration

1. **Clone the Repository**:
```bash
git clone "https://github.com/m7mdraafat/VideStore.API"
cd VideStore
```

2. **Restore Dependencies**:
```bash
dotnet restore
```

3. **Update Database Connection**:
Edit `appsettings.json` with your SQL Server connection string.

4. **Apply Migrations**:
```bash
dotnet ef database update
```

5. **Run the Application**:
```bash
dotnet run --project VideStore.Api
```
Access the API at `https://localhost:5001`.

---

## 📊 Database ERD
The **Entity-Relationship Diagram (ERD)** showcases database tables, relationships, and constraints to visualize data flow.

---

## 🔄 API Versioning
API versioning ensures backward compatibility, supporting version specification via URL path, query string, or headers.

---

## 🔌 API Endpoints

### **Products:**
- `GET /api/products` - Get all products.
- `GET /api/products/{id}` - Get product by ID.
- `POST /api/products` - Add a new product.
- `PUT /api/products/{id}` - Update product details.
- `DELETE /api/products/{id}` - Delete a product.

### **Categories:**
- `GET /api/categories` - Get all categories.
- `POST /api/categories` - Add a new category.

### **Accounts:**
- `POST /api/accounts/register` - Register a new user.
- `POST /api/accounts/login` - Login with credentials.
- `POST /api/accounts/forgot-password` - Request password reset.
- `POST /api/accounts/reset-password` - Reset password.

### **Authentications:**
- `POST /api/auth/refresh-token` - Refresh access tokens.

### **Shopping Cart:**
- `POST /api/cart/add` - Add item to cart.
- `GET /api/cart` - Get cart details.
- `DELETE /api/cart/remove/{id}` - Remove item from cart.

### **Orders:**
- `POST /api/orders` - Place an order.
- `GET /api/orders` - Get order history.

---

## 🛠 Technologies Used
- **Backend**: ASP.NET Core 8, Entity Framework Core, ASP.NET Core Identity.
- **Database**: SQL Server.
- **Caching**: Redis.
- **Security**: JWT Tokens, Refresh Tokens, Google OAuth.
- **Background Jobs**: Hangfire.
- **Design Patterns**: Repository, Unit of Work, Specification.

---

## 🤝 Contribution
Contributions are welcome! Fork the repository, make changes, and submit a pull request.

---

## 📜 License
This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📬 Contact
- **Email**: [mohamedraafat.engineer@gmail.com](mailto:mohamedraafat.engineer@gmail.com)
- **GitHub**: [m7mdraafat](https://github.com/m7mdraafat)
- **LinkedIn**: [Mohamed Raafat](https://www.linkedin.com/in/mohamed-raafat-701290252/)



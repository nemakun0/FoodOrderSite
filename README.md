# 🚀 FoodOrderSite  
### Full-Stack Multi-Role Food Ordering Platform (ASP.NET Core MVC)

**FoodOrderSite** is a full-stack web application that simulates a real-world **food delivery marketplace** architecture. The platform supports multiple user roles, dynamic restaurant management, and a complete ordering workflow, reflecting modern e-commerce system design principles.

This project was developed to demonstrate **enterprise-level MVC architecture**, role-based access control, relational database modeling, and scalable web application structure.

---

## 🎯 Project Purpose

The goal of this project is to design and implement a **multi-actor digital food ordering ecosystem** where:

- Customers can browse restaurants and place orders  
- Restaurant owners manage menus and incoming orders  
- System administrators oversee platform operations  

It models the architectural foundations behind platforms like **Yemeksepeti / UberEats**, scaled for academic and portfolio demonstration.

---

## 🏗️ System Architecture

The project follows **Layered MVC Architecture**:

| Layer | Responsibility |
|------|----------------|
| Presentation Layer | Razor Views + Bootstrap UI |
| Application Layer | Controllers, Business Logic |
| Data Layer | Entity Framework Core ORM |
| Identity Layer | ASP.NET Core Identity |
| Storage | SQL Server |

✔ Role-based authorization  
✔ Separation of concerns  
✔ Code-first database modeling  

---

## 🧩 Core Platform Modules

### 🌍 Discovery System
- Restaurant listing & cards
- Category filtering
- Dynamic menu browsing

### 🛒 Ordering System
- Cart management logic
- Order creation pipeline
- Order history tracking

### 👤 Customer Management
- Authentication & registration
- Address storage
- Review system

### 🏪 Restaurant Management Panel
- Add/edit/delete food items
- Upload images
- Manage restaurant data
- View incoming orders

### 🛡️ Admin Control Panel
- User & role management
- Category management
- Platform-wide reporting
- System analytics

---

## 💻 Technologies Used

| Category | Stack |
|----------|------|
| Framework | ASP.NET Core MVC (.NET 8) |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Identity |
| UI | Bootstrap + Razor Views |
| File Handling | Custom Helper Classes |
| Client-side | JavaScript, jQuery |

---

## 🗄️ Database Design

The system uses a **relational schema** with normalized entities:

- Users (Identity)
- Restaurants
- Food Items
- Categories
- Orders
- OrderItems
- Customer Addresses
- Reviews

EF Core migrations ensure maintainable schema evolution.

---

## ⚙️ Key Engineering Concepts Demonstrated

✔ Role-Based Authorization  
✔ Multi-User System Design  
✔ E-commerce Style Cart Logic  
✔ Session & State Management  
✔ File Upload & Image Handling  
✔ ViewModel-based Data Transfer  
✔ Scalable Controller Organization  
✔ Real-world Database Relationships  

---

## 📊 Functional Highlights

- Dynamic restaurant discovery  
- Full cart-to-order pipeline  
- Multi-layout UI (Admin / Customer / Seller)  
- Image upload integration  
- Review & rating system  
- Order tracking history  
- Reporting & analytics module  

---

## 🚀 Setup

```bash
git clone https://github.com/nemakun0/FoodOrderSite.git
cd FoodOrderSite
```

Configure `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=FoodOrderSiteDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Run migrations:

```bash
dotnet ef database update
```

Run application:

```bash
dotnet run
```

---

## 📈 What This Project Proves

This project demonstrates the ability to:

- Design **multi-role web systems**
- Implement **production-style MVC architecture**
- Model complex relational data
- Build **scalable feature modules**
- Develop end-to-end full-stack applications

It reflects practical knowledge of **software architecture, database engineering, and web platform development**.

---

## 👩‍💻 Developer

GitHub: https://github.com/nemakun0

---

## 📄 License

Educational and portfolio demonstration project.

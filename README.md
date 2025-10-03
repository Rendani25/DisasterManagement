Disaster Management & Volunteer Platform

📌 Overview

The Disaster Management & Volunteer Platform is a web application built with ASP.NET Core MVC to connect communities, volunteers, donors, and administrators during times of crisis.
It allows users to:

Report incidents and emergencies.

Donate resources to affected communities.

Register as volunteers and manage tasks.

Admins to oversee incidents, donations, and volunteer activities.


This system provides a centralized platform to coordinate disaster response and improve efficiency in relief operations.


---

🚀 Features

👥 For Users

Report incidents with location, description, and attachments (photos/videos).

Create and track donations with itemized lists.

Register volunteer profiles (skills, availability, hours contributed).

View and accept assigned tasks.


🛠️ For Admins

Manage incidents, donations, and volunteer tasks from an Admin Dashboard.

Approve or reject donations and volunteer profiles.

Assign and track volunteer tasks.

Access analytics (total incidents, donations, volunteers, etc.).


🖥️ General

Authentication & Authorization with ASP.NET Identity.

Role-based access (Admin vs User).

Mobile-friendly Bootstrap 5 UI.

Landing page with project mission and calls to action.



---

🏗️ Tech Stack

Backend: ASP.NET Core MVC 7.0

Frontend: Bootstrap 5, Razor Pages

Database: Microsoft SQL Server (Entity Framework Core ORM)

Authentication: ASP.NET Identity (with role management)

Hosting: Azure / IIS (configurable)



---

📂 Project Structure

DisasterManagement/
│
├── Areas/
│   └── Admin/                # Admin area controllers & views
│
├── Controllers/              # Main app controllers (Home, Landing, Donations, etc.)
│
├── Models/                   # Entity models (IncidentReport, Donation, VolunteerTask, etc.)
│
├── Views/                    # Razor views for UI
│
├── Data/                     # ApplicationDbContext & DB migrations
│
├── wwwroot/                  # Static files (CSS, JS, images)
│
├── Program.cs                # App configuration & middleware
├── appsettings.json          # Connection strings & config
└── README.md                 # Project documentation

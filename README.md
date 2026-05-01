# Triathlon Competition Management System (.NET Implementation)

## 📌 Project Overview
This project is a distributed **Client-Server** application developed in **C#** for managing triathlon event results in real-time. It features a robust multi-layered architecture designed to handle concurrent users, secure authentication, and instant data synchronization across a network.

The system is built as a modular solution, emphasizing clean code, high-performance networking via sockets, and a responsive desktop user interface.

## ✨ Core Functionalities & Features

### 🔐 Security & Authentication
*   **Encrypted Credentials**: Secure login system for referees. Passwords are encrypted before being processed or stored, ensuring sensitive data protection.
*   **Concurrent Session Control**: The server manages active connections, ensuring a secure and stable environment for each authenticated referee.

### 📊 Real-Time Synchronization (Observer Pattern)
*   **Automated Notifications**: Implements a real-time update mechanism. Whenever a referee adds or modifies a result, all other connected clients are notified instantly without requiring a manual refresh.
*   **Push Communication**: The server-side broadcast logic ensures that the latest competition standings are consistent across all active client instances.

### 📈 Competition & Result Management
*   **Global Participant View**: A comprehensive dashboard showing all participants in alphabetical order, including their total cumulative points across all trials.
*   **Trial Ranking Reports**: Specific views for each trial, where participants are ranked in descending order based on their performance in that particular event.
*   **Result Operations**: Referees can efficiently add new scores or modify existing ones for their assigned trials, with changes reflected globally in real-time.

## 🏗️ Technical Architecture
The application is organized into distinct modules to ensure high maintainability and a clear separation of concerns:

### 1. Persistence Layer (Relational Database)
*   **Native ADO.NET / JDBC Style Implementation**: Data persistence is handled through a robust relational database layer using optimized SQL queries.
*   **Relational Storage**: Powered by **SQLite**, providing a reliable and portable storage solution for athletes, referees, and event results.
*   
### 2. Networking & Distributed Logic
*   **Custom RPC Protocol**: A proprietary Remote Procedure Call (RPC) protocol implemented over **TCP/IP Sockets** for efficient client-server communication.
*   **Multi-threaded Concurrent Server**: The server architecture handles multiple simultaneous client connections using a dedicated thread-per-client or thread-pool model.
*   **DTO Pattern**: Uses Data Transfer Objects to minimize network overhead and decouple the presentation layer from the internal data models.

### 3. Presentation Layer (GUI)
*   **Desktop Interface**: A responsive UI built with C# UI frameworks (Windows Forms / WPF), following the **MVC/MVP** design patterns for clear logic separation.
*   **Safe Thread UI Updates**: Handles asynchronous data arrival from the network to safely update the user interface without causing freezes or crashes.

## 📁 Project Modules
The solution is structured as follows:
*   **`Common`**: Domain models, serializable DTOs, and shared interfaces.
*   **`Server`**: Business logic, repository implementations, and the core network server.
*   **`Client`**: UI views, controllers, and the RPC Proxy responsible for communicating with the server.
*   **`Networking`**: The communication engine, including request handlers, response builders, and socket workers.

## 💻 Execution Instructions
1.  **Start the Server**: Run the Server project. It will initialize the database connection and start listening for incoming client requests.
2.  **Launch Clients**: Start multiple client instances to verify the real-time notification system and concurrent result management.
3.  **Configuration**: Connection strings and server settings can be adjusted in the application configuration files.

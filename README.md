# Chat Application

This is a simple MVC-based chat application that allows an admin to chat with multiple users simultaneously. The application supports text messages, images, documents, and voice recordings. It also includes features like user inactivity detection and message status indicators.

## Features

- **User Registration and Login**: Users can register and log in to access the chat application.
- **Admin Dashboard**: Admin can view a list of active users and chat with multiple users simultaneously.
- **Real-time Chat**: Real-time messaging with send and seen status flags.
- **File Sharing**: Users can send images, documents, and voice recordings.
- **User Inactivity Detection**: Automatic message sent to users after 1 minute of inactivity.
- **Character and Time Limits**: Maximum character and time limits for user messages.

## Technologies Used

- **Backend**: ASP.NET MVC, Entity Framework
- **Frontend**: HTML, CSS, JavaScript
- **Real-time Communication**: SignalR
- **Database**: SQL Server

## Architecture

The application follows the Clean Architecture pattern and is divided into four layers:

1. **Application Layer**: Contains DTOs, configurations, Helper Classes, interfaces, and services.
2. **Presentation Layer**: Contains the MVC controllers and views.
3. **Core Layer**: Contains the entities and enums.
4. **Infrastructure Layer**: Contains the database context, repositories, and UnitOfWork implementation.

### Design Patterns

- **Repository Pattern**: To encapsulate the logic for accessing the data source.
- **UnitOfWork**: To manage database transactions and coordinate the work of multiple repositories.
- **Service Layer**: To encapsulate the business logic of the application and Mapping.
- **AutoMapper**: For object-object mapping between DTOs and entities.


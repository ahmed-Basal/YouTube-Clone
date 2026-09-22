# 🎬 VidPulse — Enterprise Video Platform (Modular Monolith)

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0%20LTS-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/Language-C%23%2013-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Architecture](https://img.shields.io/badge/Architecture-Modular%20Monolith-007ACC?logo=azure-pipelines&logoColor=white)](#-modular-monolith-architecture)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20%7C%20EF%20Core%2010-CC292B?logo=microsoftsqlserver&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![Media Storage](https://img.shields.io/badge/CDN-Cloudinary%20Integration-3448C5?logo=cloudinary&logoColor=white)](https://cloudinary.com/)
[![Pattern](https://img.shields.io/badge/Pattern-CQRS%20%2B%20Domain%20Events%20(MediatR)-FF6F00)](#-inter-module-event-flow)

**VidPulse** is a high-performance video sharing, streaming, and channel ecosystem inspired by YouTube, re-engineered from a traditional 3-tier structure into an enterprise-grade **Modular Monolith**. It guarantees strict domain isolation, independent persistence contexts per module, event-driven decoupling via **MediatR**, and high-volume media upload capabilities.

---

## 📑 Table of Contents

- [🏛️ Modular Monolith Architecture](#️-modular-monolith-architecture)
- [📊 Database & Entity-Relationship Model (ERD)](#-database--entity-relationship-model-erd)
- [🔄 Inter-Module Event Flow](#-inter-module-event-flow)
- [⚡ Sequence Workflows](#-sequence-workflows)
  - [1. User Registration & Automatic Channel Bootstrapping](#1-user-registration--automatic-channel-bootstrapping)
  - [2. Video Upload, Transcoding & Embedding Pipeline](#2-video-upload-transcoding--embedding-pipeline)
  - [3. Video Deletion & Domain Event Cascade Cleanup](#3-video-deletion--domain-event-cascade-cleanup)
- [📦 Deep Dive: Domain Modules](#-deep-dive-domain-modules)
  - [Users Module (`Modules/Users`)](#1-users-module-modulesusers)
  - [Channels Module (`Modules/Channels`)](#2-channels-module-moduleschannels)
  - [Videos Module (`Modules/Videos`)](#3-videos-module-modulesvideos)
  - [Interactions Module (`Modules/Interactions`)](#4-interactions-module-modulesinteractions)
  - [Administration Module (`Modules/Administration`)](#5-administration-module-modulesadministration)
  - [Shared Kernel (`youtube.SharedKernel`)](#6-shared-kernel-youtubesharedkernel)
- [🌐 Web Host & Presentation Engine](#-web-host--presentation-engine)
- [🛣️ Complete Application Route Catalog](#️-complete-application-route-catalog)
- [🚀 Getting Started & Local Development](#-getting-started--local-development)
- [🛡️ Security, Performance & Scalability](#️-security-performance--scalability)

---

## 🏛️ Modular Monolith Architecture

Traditional 3-tier setups suffer from circular dependencies, monolithic `DbContext` god-objects, and leaky abstractions like generic `IRepository` and `IUnitOfWork`. VidPulse partitions business capabilities into **bounded contexts**, each owning its data model, business logic, and API contracts.

```mermaid
graph TB
    subgraph Presentation ["🖥️ Presentation Layer (Host)"]
        WebHost["youtube (ASP.NET Core 10 Web App)"]
        Controllers["Controllers & Vertical Feature Slices"]
        Views["Razor Views (VidPulse Dark Theme)"]
        WebHost --> Controllers
        WebHost --> Views
    end

    subgraph CoreContracts ["🧩 Core Infrastructure"]
        SK["youtube.SharedKernel"]
        Events["Domain Notifications & Base Types"]
        SK --- Events
    end

    subgraph BoundedContexts ["📦 Bounded Domain Modules"]
        direction TB
        ModUsers["Modules.Users<br/><b>Identity, Auth, Credentials</b>"]
        ModChannels["Modules.Channels<br/><b>Channels, Bios, Subscriptions</b>"]
        ModVideos["Modules.Videos<br/><b>Videos, Cloudinary, Categories, Views</b>"]
        ModInteractions["Modules.Interactions<br/><b>Reactions, Comments Engine</b>"]
        ModAdmin["Modules.Administration<br/><b>User Moderation, Category CRUD</b>"]
    end

    subgraph DataStorage ["🗄️ Persistence (Single SQL Server Database)"]
        UDB[("UsersDbContext<br/>AspNetUsers, AspNetRoles")]
        CDB[("ChannelsDbContext<br/>Channals, Subscriptions")]
        VDB[("VideosDbContext<br/>videos, Categories")]
        IDB[("InteractionsDbContext<br/>Comments, LikesDislikes")]
    end

    Controllers ==>|"CQRS Commands / Queries"| ModUsers
    Controllers ==>|"CQRS Commands / Queries"| ModChannels
    Controllers ==>|"CQRS Commands / Queries"| ModVideos
    Controllers ==>|"CQRS Commands / Queries"| ModInteractions
    Controllers ==>|"CQRS Commands / Queries"| ModAdmin

    ModUsers --> SK
    ModChannels --> SK
    ModVideos --> SK
    ModInteractions --> SK
    ModAdmin --> SK

    ModUsers --> UDB
    ModChannels --> CDB
    ModVideos --> VDB
    ModInteractions --> IDB

    ModUsers -.->|"MediatR Event: UserRegistered"| ModChannels
    ModVideos -.->|"MediatR Event: VideoDeleted"| ModInteractions
```

### Key Architectural Tenets

1. **Isolated DbContexts**: Each module manages an exclusive `DbContext`. No module ever queries or joins tables belonging to another context directly in SQL.
2. **Contract-Driven Boundaries**: Cross-module reads execute through dedicated read-only Queries and DTOs (e.g., `GetUserSummaryQuery`, `GetChannelSummaryQuery`).
3. **Eventual Consistency & Domain Events**: Mutative cross-module actions publish MediatR notifications (`INotification`) handled asynchronously by consumer modules.
4. **No Generic Repository Layer**: Handlers write EF Core LINQ queries directly with `.AsNoTracking()` and optimized projections, eliminating memory waste and performance overhead.

---

## 📊 Database & Entity-Relationship Model (ERD)

The system targets a unified SQL Server database (`vidpulsedb`) where table schemas are carefully mapped to preserve backward compatibility while maintaining bounded separation:

```mermaid
erDiagram
    AspNetUsers ||--o| Channals : "owns"
    AspNetUsers ||--o{ Subscriptions : "subscribes"
    AspNetUsers ||--o{ Comments : "posts"
    AspNetUsers ||--o{ LikesDislikes : "reacts"

    Channals ||--o{ videos : "publishes"
    Channals ||--o{ Subscriptions : "receives"

    Categories ||--o{ videos : "categorizes"

    videos ||--o{ Comments : "has"
    videos ||--o{ LikesDislikes : "receives"

    AspNetUsers {
        int Id PK
        string Name
        string UserName
        string Email
        string PasswordHash
        datetime2 CreateAt
    }

    Channals {
        int Id PK
        int AppUserId FK
        string Name
        string About
        datetime2 CreatedAt
    }

    Subscriptions {
        int AppUserId PK,FK
        int ChannalId PK,FK
    }

    Categories {
        int Id PK
        string Name
    }

    videos {
        int Id PK
        int ChannelId FK
        int CategoryId FK
        string Title
        string Description
        string VideoUrl
        string ThumbnailUrl
        int Views
        datetime2 CreatedAt
    }

    Comments {
        int Id PK
        int VideoId FK
        int AppUserId FK
        string Content
        datetime2 PostAt
    }

    LikesDislikes {
        int AppUserId PK,FK
        int VideoId PK,FK
        bit like
    }
```

---

## 🔄 Inter-Module Event Flow

Modules stay strictly decoupled by delegating side effects to MediatR notifications.

```mermaid
stateDiagram-v2
    direction LR

    [*] --> UserRegistration
    UserRegistration --> ModUsers: RegisterCommand
    ModUsers --> PublishUserRegistered: SaveChangesAsync()
    PublishUserRegistered --> ModChannels: UserRegisteredNotification
    ModChannels --> CreateDefaultChannel: UserRegisteredHandler
    CreateDefaultChannel --> [*]

    [*] --> VideoDeletion
    VideoDeletion --> ModVideos: DeleteVideoCommand
    ModVideos --> PublishVideoDeleted: SaveChangesAsync()
    PublishVideoDeleted --> ModInteractions: VideoDeletedNotification
    ModInteractions --> PurgeReactionsAndComments: VideoDeletedHandler
    PurgeReactionsAndComments --> [*]
```

| Event | Publisher | Consumer(s) | Business Action Taken |
|---|---|---|---|
| `UserRegisteredNotification` | `Modules.Users` | `Modules.Channels` | Automatically provisions a custom default channel for the new user. |
| `VideoDeletedNotification` | `Modules.Videos` | `Modules.Interactions` | Purges all comments, likes, and dislikes associated with the removed video. |
| `UserDeletedNotification` | `Modules.Administration` | `Modules.Channels`, `Modules.Videos` | Handles cascade deprovisioning when an account is banned or purged. |

---

## ⚡ Sequence Workflows

### 1. User Registration & Automatic Channel Bootstrapping

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant Controller as AccountController
    participant Mediator as MediatR
    participant UsersMod as Modules.Users (RegisterHandler)
    participant Identity as ASP.NET Identity (UserManager)
    participant ChannelsMod as Modules.Channels (UserRegisteredHandler)

    User->>Controller: POST /Account/Register (Register_vm)
    Controller->>Mediator: Send(RegisterCommand)
    Mediator->>UsersMod: Handle(RegisterCommand)
    UsersMod->>Identity: CreateAsync(AppUser, password)
    UsersMod->>Identity: AddToRoleAsync(user, "Member")
    UsersMod->>Mediator: Publish(UserRegisteredNotification(userId, userName))
    Mediator->>ChannelsMod: Handle(UserRegisteredNotification)
    ChannelsMod->>ChannelsMod: Create default channel record
    ChannelsMod-->>Mediator: Acknowledged
    UsersMod-->>Controller: Result.Success()
    Controller-->>User: Redirect to Login / Feed
```

### 2. Video Upload, Transcoding & Embedding Pipeline

```mermaid
sequenceDiagram
    autonumber
    actor Creator
    participant Controller as VideoController
    participant Cloudinary as CloudinaryService
    participant YouTube as YouTubeHelper
    participant Mediator as MediatR
    participant VideosMod as Modules.Videos (SaveVideoHandler)

    Creator->>Controller: POST /Video/CreateEditVideo (VideoEdit VM + Files)
    alt Video File Upload Provided
        Controller->>Cloudinary: UploadVideoAsync(file, "vidpulse_videos")
        Cloudinary-->>Controller: videoStreamUrl
    else YouTube URL Provided
        Controller->>YouTube: GetWatchUrl(youtubeUrl)
        Controller->>YouTube: GetThumbnailUrl(youtubeUrl)
        YouTube-->>Controller: parsedUrls
    end

    Controller->>Mediator: Send(SaveVideoCommand)
    Mediator->>VideosMod: Handle(SaveVideoCommand)
    VideosMod->>VideosMod: Insert / Update video entity
    VideosMod-->>Controller: Result.Success(videoId)
    Controller-->>Creator: Redirect to Studio / Video Watch Page
```

### 3. Video Deletion & Domain Event Cascade Cleanup

```mermaid
sequenceDiagram
    autonumber
    actor Creator
    participant Controller as VideoController
    participant Mediator as MediatR
    participant VideosMod as Modules.Videos (DeleteVideoHandler)
    participant InteractionsMod as Modules.Interactions (VideoDeletedHandler)

    Creator->>Controller: POST /Video/Delete (videoId)
    Controller->>Mediator: Send(DeleteVideoCommand(videoId, userId))
    Mediator->>VideosMod: Handle(DeleteVideoCommand)
    VideosMod->>VideosMod: Verify ownership & Delete from [videos]
    VideosMod->>Mediator: Publish(VideoDeletedNotification(videoId))
    Mediator->>InteractionsMod: Handle(VideoDeletedNotification)
    InteractionsMod->>InteractionsMod: DELETE FROM [Comments] WHERE VideoId = videoId
    InteractionsMod->>InteractionsMod: DELETE FROM [LikesDislikes] WHERE VideoId = videoId
    InteractionsMod-->>Mediator: Cleaned
    VideosMod-->>Controller: Success
    Controller-->>Creator: Redirect to Studio with notification
```

---

## 📦 Deep Dive: Domain Modules

### 1. Users Module (`Modules/Users`)
- **Primary Focus**: Security, Identity, authorization, and credential auditing.
- **Components**:
  - `AppUser`: Extends `IdentityUser<int>` with custom properties like `CreateAt` and display name.
  - `AppRole`: Extends `IdentityRole<int>` with role tracking.
  - `UsersDbContext`: Inherits `IdentityDbContext` targeting `AspNetUsers`, `AspNetRoles`, `AspNetUserClaims`, etc.
  - **Commands**: `LoginCommand`, `RegisterCommand`, `LogoutCommand`.
  - **Queries**: `GetUserSummaryQuery` (returns `UserSummaryDto`).

### 2. Channels Module (`Modules/Channels`)
- **Primary Focus**: Content creator presence, channel configuration, and subscription graph.
- **Components**:
  - `Channel`: Model representing channel metadata, bio, avatar, and creator association.
  - `Subscription`: Composite key `(AppUserId, ChannalId)` representing fan followings.
  - `ChannelsDbContext`: Maps `Channals` and `Subscriptions` tables.
  - **Commands**:
    - `CreateChannelCommand`: Allows creating a distinct channel profile.
    - `EditChannelCommand`: Updates bio and channel name.
    - `ToggleSubscribeCommand`: Subscribes or unsubscribes users, guarding against self-subscriptions.
  - **Event Handlers**: `UserRegisteredHandler` listens for new user creations to ensure every account has a studio channel ready.

### 3. Videos Module (`Modules/Videos`)
- **Primary Focus**: Media ingestion, content categorisation, streaming coordination, and view counts.
- **Components**:
  - `Video`: Title, description, streaming URLs, thumbnails, category, channel owner, view count.
  - `Category`: Categorisation tree (`Music`, `Gaming`, `News`, `Sports`, `Movies`).
  - `CloudinaryService`: Multi-part chunked upload for MP4, WebM, and Ogg with folder partitioning.
  - `HashingService`: Content validation and upload hashing.
  - `VideosDbContext`: Configured for fast querying of `videos` and `Categories`.
  - **Commands**: `SaveVideoCommand`, `DeleteVideoCommand`, `IncrementVideoViewsCommand`.
  - **Queries**: `GetVideoByIdQuery`, `GetHomeVideosQuery`, `GetVideosByCategoryQuery`.

### 4. Interactions Module (`Modules/Interactions`)
- **Primary Focus**: Social engagement, community conversations, and audience sentiment.
- **Components**:
  - `Comment`: User-authored text comments linked to `VideoId` with timestamps.
  - `LikeDislike`: Dual-state vote engine (`like = true/false`) per user per video.
  - `InteractionsDbContext`: Maps `Comments` and `LikesDislikes`.
  - **Commands**:
    - `ToggleLikeCommand`: Atomic toggle between Like, Dislike, and neutral state.
    - `AddCommentCommand`: Posts comment with sanitation.
    - `DeleteCommentCommand`: Authorized comment deletion.
  - **Event Handlers**: `VideoDeletedHandler` cleans up engagement history when videos are removed.

### 5. Administration Module (`Modules/Administration`)

- **Primary Focus**: Content moderation, category taxonomy, and administrative oversight.
- **Components**:
  - **Queries**: `GetAdminUsersQuery` (paginated list of registered members, roles, and status).
  - **Commands**:
    - `DeleteAdminUserCommand`: Admin-level user suspension and account removal.
    - `CreateCategoryCommand`, `EditCategoryCommand`, `DeleteCategoryCommand`: Taxonomy management.

### 6. Shared Kernel (`youtube.SharedKernel`)

- **Primary Focus**: Non-domain-specific contracts shared across modules without creating coupling.
- **Components**:
  - `BaseEntity`: Standard entity identifier contract.
  - `Result` & `Result<T>`: Functional operation result pattern avoiding throwing business exceptions.
  - `SD` (Static Details): Roles (`Admin`, `Member`), TempData keys, and system constants.
  - `YouTubeHelper`: Regular expression parsing for YouTube URLs (`watch?v=`, `youtu.be/`, `/embed/`) and high-res thumbnail extraction (`img.youtube.com/vi/{id}/maxresdefault.jpg`).

---

## 🌐 Web Host & Presentation Engine

The ASP.NET Core host (`youtube/`) acts purely as a coordinator:
- **Zero Business Logic in Controllers**: Controllers simply unpack HTTP inputs, wrap them into MediatR commands/queries, and return ViewModels.
- **High-Capacity File Uploads**: Kestrel and multipart form body size limits are configured for up to **100 MB** direct file uploads:
  ```csharp
  builder.Services.Configure<FormOptions>(opt => opt.MultipartBodyLengthLimit = 104857600);
  builder.WebHost.ConfigureKestrel(opt => opt.Limits.MaxRequestBodySize = 104857600);
  ```
- **Automated Database Seeding**: [DatabaseInitializer.cs](file:///c:/Users/ahmed/source/repos/youtube/youtube/DatabaseInitializer.cs) coordinates all module `DbContext`s on startup to seed:
  - System roles (`Admin`, `Member`)
  - Initial administrator and demo content creator accounts
  - Standard categories (`Music`, `Gaming`, `News`, `Sports`, `Movies`)
  - Pre-seeded channels and demo videos with thumbnails and views.

---

## 🛣️ Complete Application Route Catalog

| HTTP Method | Route | Controller | Purpose |
| --- | --- | --- | --- |
| `GET` | `/` | `HomeController.Index` | Home video feed with category filters & hero video |
| `GET` | `/Video/Watch/{id}` | `VideoController.Watch` | Full video playback, comments, likes, & recommendations |
| `POST` | `/Video/IncrementViews/{id}` | `VideoController.IncrementViews` | Asynchronous view counter increment |
| `GET` | `/Video/CreateEditVideo` | `VideoController.CreateEditVideo` | Video creator studio upload/edit form |
| `POST` | `/Video/CreateEditVideo` | `VideoController.CreateEditVideo` | Handles Cloudinary upload or YouTube URL linking |
| `POST` | `/Video/Delete/{id}` | `VideoController.Delete` | Video deletion with cascade event trigger |
| `POST` | `/Video/ToggleLike` | `VideoController.ToggleLike` | Like/Dislike reaction endpoint |
| `POST` | `/Video/AddComment` | `VideoController.AddComment` | Post a new comment |
| `POST` | `/Video/DeleteComment` | `VideoController.DeleteComment` | Remove an existing comment |
| `GET` | `/Channal` | `ChannalController.Index` | Channel studio management & videos grid |
| `POST` | `/Channal/Create` | `ChannalController.Create` | Create channel profile |
| `POST` | `/Channal/Edit` | `ChannalController.Edit` | Update channel profile |
| `POST` | `/Channal/ToggleSubscribe`| `ChannalController.ToggleSubscribe` | Toggle subscription status |
| `GET` | `/Account/Login` | `AccountController.Login` | Sign In view |
| `POST` | `/Account/Login` | `AccountController.Login` | Authenticate user session |
| `GET` | `/Account/Register` | `AccountController.Register` | User registration view |
| `POST` | `/Account/Register` | `AccountController.Register` | Create account & bootstrap channel |
| `POST` | `/Account/Logout` | `AccountController.Logout` | Terminate session |
| `GET` | `/Admin/Users` | `AdminController.Users` | Administrative member dashboard |
| `POST` | `/Admin/DeleteUser` | `AdminController.DeleteUser` | Remove user account |
| `GET` | `/Admin/Category` | `AdminController.Category` | Category management dashboard |
| `POST` | `/Admin/UpsertCategory` | `AdminController.UpsertCategory` | Create or update video category |
| `POST` | `/Admin/DeleteCategory` | `AdminController.DeleteCategory` | Delete video category |

---

## 🚀 Getting Started & Local Development

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB, SQL Express, or Docker instance)
- A free [Cloudinary](https://cloudinary.com/) account (if testing direct video uploads)

### 1. Clone & Configure
```bash
git clone https://github.com/ahmed-Basal/YouTube-Clone.git
cd YouTube-Clone
```

Edit `youtube/appsettings.json` with your database and Cloudinary keys:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=vidpulsedb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 2. Build the Solution
```bash
dotnet build youtube.slnx
```

### 3. Run the Application
```bash
dotnet run --project youtube/youtube.csproj
```
The application will automatically initialize the database, execute table migrations, seed categories, users, and channels, and start listening on:
```
http://localhost:5242
```

---

## 🛡️ Security, Performance & Scalability

- **SQL Injection Prevention**: Every query uses parameterized LINQ expressions compiled by EF Core.
- **XSS & Content Protection**: Razor handles contextual HTML encoding; all user inputs are strictly sanitized.
- **Optimized Projections**: Handlers query using `.Select()` projections and `.AsNoTracking()` to avoid Entity Tracker overhead.
- **Future Microservices Ready**: Because each module has a distinct bounded context, independent `DbContext`, and communicates via messages, any module can be extracted into an independent microservice with minimal refactoring.

---

## 📝 License

This project is open-source under the [MIT License](LICENSE).
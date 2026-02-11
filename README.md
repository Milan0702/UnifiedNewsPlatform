## UnifiedNewsPlatform

UnifiedNewsPlatform is a microservices-based news and blog aggregation platform. It periodically pulls news from public APIs and blog posts from RSS feeds, processes and deduplicates them, stores them in MongoDB, and serves a personalized, searchable feed to authenticated users via a React frontend.

The platform is designed to showcase:
- **Distributed architecture** using multiple backend services and background workers
- **Message-driven communication** via RabbitMQ
- **Polyglot persistence** with PostgreSQL, MongoDB, and Redis
- **Modern frontend** built with React and a simple API gateway in front

---

## Features

- **News aggregation**
  - Fetches top headlines from external APIs (e.g. NewsAPI) on a schedule
  - Categorizes articles (Technology, Business, Sports, General, etc.)

- **Blog aggregation**
  - Fetches blog posts from RSS feeds (e.g. Medium technology tag)
  - Normalizes them into the same article model as news

- **Content processing pipeline**
  - Consumes raw news and blog messages from RabbitMQ
  - Deduplicates by URL and standardizes article structure
  - Stores articles in MongoDB and warms Redis caches

- **Personalized user feed**
  - Users register and log in via JWT-based authentication
  - Users can set category and source preferences
  - Frontend shows a tailored feed based on saved preferences

- **Searchable content**
  - Full-text search over stored articles (MongoDB text index)
  - Cached search results in Redis for performance

- **Container-based deployment**
  - `docker-compose` to run the full stack locally
  - NGINX as a simple API gateway / reverse proxy

---

## High-Level Architecture

At a high level, the system is composed of:

- **Frontend**
  - React single-page application
  - Handles authentication UI, dashboard, preferences, search, and feed views

- **Backend APIs (ASP.NET Core)**
  - `UserService`: user registration, login, profile, and preferences (PostgreSQL)
  - `ContentDeliveryService`: authenticated content APIs backed by MongoDB + Redis cache

- **Background workers**
  - `NewsAggregator` (.NET worker): fetches news from external APIs and publishes raw items to RabbitMQ
  - `BlogAggregator` (Spring Boot): fetches RSS feeds and publishes raw items to RabbitMQ
  - `ContentProcessor` (.NET worker): consumes raw messages, deduplicates and stores into MongoDB, publishes to Redis

- **Infrastructure**
  - **RabbitMQ**: message broker for news and blog pipelines
  - **PostgreSQL**: relational store for users and preferences
  - **MongoDB**: document store for news and blog articles
  - **Redis**: cache layer for feeds, categories, and individual articles
  - **Mongo Express**: web UI for MongoDB (dev/admin convenience)
  - **NGINX**: reverse proxy / API gateway in front of frontend and backend APIs

**Typical communication paths:**
- Frontend → NGINX → `UserService` (`/api/auth`, `/api/users`)
- Frontend → NGINX → `ContentDeliveryService` (`/api/content/*`)
- `NewsAggregator` / `BlogAggregator` → RabbitMQ queues
- `ContentProcessor` → MongoDB + Redis
- `ContentDeliveryService` → MongoDB + Redis

---

## Tech Stack

- **Frontend**
  - React
  - Vite
  - React Router
  - Axios
  - (Styling via CSS / utility classes, e.g. Tailwind-like approach)

- **Backend & workers**
  - .NET 8 (ASP.NET Core Web API, Worker Services)
  - Spring Boot 3 (BlogAggregator)

- **Data & infrastructure**
  - PostgreSQL
  - MongoDB
  - Redis
  - RabbitMQ
  - NGINX
  - Docker & Docker Compose

---

## Repository Structure (Overview)

Key folders in this repository:

- `Frontend/`
  - React single-page application
  - Important files:
    - `src/App.jsx` – main routing and layout
    - `src/components/Navbar.jsx` – navigation bar
    - `src/pages/Dashboard.jsx` – overview and quick access to content
    - `src/pages/Feed.jsx` – personalized feed based on user preferences
    - `src/pages/Preferences.jsx` – UI for editing category/source preferences
    - `src/pages/SearchResults.jsx` – results page for search queries
    - `src/api/content.js` – content-related HTTP calls to backend

- `Services/UserService/`
  - ASP.NET Core Web API handling:
    - User registration and login (`AuthController`)
    - User profile management (`UserController`)
    - User preferences CRUD
  - Uses PostgreSQL via Entity Framework Core (`UserDbContext`)
  - Generates JWT tokens for authenticated access

- `Services/ContentDelivery/`
  - ASP.NET Core Web API responsible for:
    - Serving the main feed
    - Filtering by category
    - Full-text search over articles
    - Fetching single article details
  - Connects to MongoDB and Redis (cache-aside pattern)
  - Secured with JWT Bearer authentication

- `Services/ContentProcessor/`
  - .NET worker service that:
    - Subscribes to RabbitMQ queues for raw news and blog messages
    - Normalizes and deduplicates articles (unique index on URL)
    - Writes processed articles to MongoDB
    - Populates Redis caches for fast reads by `ContentDeliveryService`

- `Services/NewsAggregator/`
  - .NET worker service that:
    - Periodically calls external news APIs (e.g. NewsAPI)
    - Collects articles for multiple categories (technology, business, sports, general, etc.)
    - Publishes raw article messages into RabbitMQ

- `Services/BlogAggregator/`
  - Spring Boot application that:
    - Periodically fetches RSS feeds (e.g. Medium technology tag)
    - Uses Resilience4j for retries and circuit breaking
    - Publishes raw blog article messages into RabbitMQ

- Other root-level items:
  - `docker-compose.yml` – brings up the full environment with one command
  - `nginx/` – NGINX configuration for routing frontend and API calls (if present)

---

## End-to-End Data Flow

1. **News ingestion**
   - `NewsAggregator` periodically calls a news API for several categories.
   - Each fetched article is wrapped in a normalized message and sent to a RabbitMQ queue (e.g. `news.raw`).

2. **Blog ingestion**
   - `BlogAggregator` periodically reads from RSS feeds.
   - Each post is normalized into a common article format and sent to another RabbitMQ queue (e.g. `blogs.raw`).

3. **Processing and storage**
   - `ContentProcessor` consumes messages from both news and blog queues.
   - It deduplicates items (by URL), enriches data (categories, timestamps, source, etc.), and stores them in MongoDB.
   - It also writes helpful structures into Redis (recent articles, categories, individual article cache entries).

4. **Delivery to clients**
   - The React frontend authenticates with `UserService` to obtain a JWT.
   - The frontend calls `ContentDeliveryService` endpoints (through NGINX) such as:
     - `GET /api/content/feed` – main feed
     - `GET /api/content/category/{category}` – category section
     - `GET /api/content/search?q=` – search results
     - `GET /api/content/{id}` – article detail
   - `ContentDeliveryService` reads from Redis for speed, falling back to MongoDB when needed.

5. **Personalization**
   - Users set and update their preferences via `UserService` (`/api/users/preferences`).
   - The frontend combines user preferences with feed/category endpoints to build a personalized experience.

---

## Running the Project with Docker Compose

The easiest way to run the complete system (infrastructure + services + frontend) is with Docker Compose.

### Prerequisites

- Docker and Docker Compose installed

### Start all services

From the repository root:

```bash
docker-compose up --build
```

This will build and start:
- RabbitMQ (with management UI)
- PostgreSQL
- MongoDB
- Mongo Express
- Redis
- UserService
- NewsAggregator
- BlogAggregator
- ContentProcessor
- ContentDeliveryService
- Frontend
- NGINX gateway

### Default ports & URLs

- **Frontend via NGINX (recommended)**: `http://localhost`
- **Frontend container (direct)**: `http://localhost:3000` (React app served by NGINX in the container)
- **RabbitMQ management UI**: `http://localhost:15672` (default user `guest`, password `guest`)
- **PostgreSQL**: `localhost:5432` (user `admin`, password `password`, database `users_db`)
- **MongoDB**: `localhost:27017` (root user `admin`, password `password`)
- **Mongo Express**: `http://localhost:8081`
- **Redis**: `localhost:6379`

The API endpoints are exposed via NGINX under `/api/*`. For example:
- `http://localhost/api/auth/login`
- `http://localhost/api/users/profile`
- `http://localhost/api/content/feed`

> Note: In Docker, the .NET services are connected to other containers using internal hostnames (e.g. `postgres`, `mongo`, `redis`, `rabbitmq`) defined in `docker-compose.yml`.

---

## Environment Variables & Configuration

Most runtime configuration is provided through `docker-compose.yml` or per-service config files.

### docker-compose

Key environment variables in `docker-compose.yml`:

- **UserService**
  - `ConnectionStrings__DefaultConnection=Host=postgres;Database=users_db;Username=admin;Password=password`
  - `RabbitMQ__HostName=rabbitmq`

- **NewsAggregator**
  - `RabbitMQ__HostName=rabbitmq`
  - `NewsApi__Key=<your_news_api_key_here>` (set in compose; currently a placeholder/value is present)

- **BlogAggregator**
  - `SPRING_RABBITMQ_HOST=rabbitmq`

- **ContentProcessor**
  - `RabbitMQ__HostName=rabbitmq`
  - `ConnectionStrings__MongoDb=mongodb://admin:password@mongo:27017`
  - `ConnectionStrings__Redis=redis:6379`

- **ContentDeliveryService**
  - `ConnectionStrings__MongoDb=mongodb://admin:password@mongo:27017`
  - `ConnectionStrings__Redis=redis:6379`

Additional configuration (such as JWT settings, logging levels, etc.) is defined in each service’s `appsettings.json` / `application.properties`.

When running services outside Docker, you can either:
- Export equivalent environment variables locally, or
- Override connection strings in local configuration files to point to your local infrastructure (e.g. `localhost` for databases and RabbitMQ).

---

## Running Services Locally (Without Full Docker Compose)

You can also run individual components directly from your development environment while using Docker only for infrastructure (databases, queue, cache) if you prefer.

### 1. Start infrastructure with Docker

You may create a reduced compose file or reuse `docker-compose.yml` but comment out app services, leaving only:
- `rabbitmq`
- `postgres`
- `mongo`
- `mongo-express` (optional)
- `redis`

Then run:

```bash
docker-compose up
```

### 2. Run backend services

- **UserService**
  ```bash
  cd Services/UserService
  dotnet run
  ```
  Default URL is defined in `launchSettings.json` (e.g. `http://localhost:5073`).

- **ContentDeliveryService**
  ```bash
  cd Services/ContentDelivery
  dotnet run
  ```
  Default URL is defined in `launchSettings.json` (e.g. `http://localhost:5220`).

- **ContentProcessor**
  ```bash
  cd Services/ContentProcessor
  dotnet run
  ```

- **NewsAggregator**
  ```bash
  cd Services/NewsAggregator
  dotnet run
  ```

- **BlogAggregator**
  ```bash
  cd Services/BlogAggregator
  mvn spring-boot:run
  ```

Ensure the connection strings and RabbitMQ hosts in your local configs point to `localhost` (or the correct container hostnames) depending on your setup.

### 3. Run the frontend

```bash
cd Frontend
npm install
npm run dev
```

By default, Vite serves the app at something like `http://localhost:5173`. For development, you can:
- Call backend APIs directly (e.g. `http://localhost:5073/api/...`), or
- Configure a Vite dev server proxy so that `/api/*` is forwarded to your backend.

---

## Frontend Overview

Key aspects of the React frontend:

- **Routing & layout**
  - Implemented in `App.jsx` with `react-router-dom`
  - Contains routes for dashboard, feed, preferences, search results, auth pages, etc.

- **Authentication**
  - Uses a JWT-based flow:
    - Login/registration endpoints on `UserService`
    - Token stored on the client (e.g. `localStorage`)
  - Axios interceptors attach the token to outbound API calls.

- **Pages & components**
  - `Dashboard` – overview and quick links to categories
  - `Feed` – shows personalized/news feed based on saved preferences
  - `Preferences` – form to pick favorite categories/sources
  - `SearchResults` – displays search results for a query parameter `q`
  - `Navbar` – navigation and auth-related links

---

## Known Gaps / Future Improvements

- **Blog API endpoint**
  - There is a `Blog` page on the frontend that intends to call a `/content/blogs` endpoint, but the backend currently exposes generic article endpoints (feed/category/search) rather than a dedicated blogs endpoint. Implementing this endpoint or wiring blogs via filters would complete the feature.

- **Port and URL alignment**
  - When running under Docker + NGINX, ensure that the ASP.NET services are listening on the expected ports (often `8080`) so that NGINX routes resolve correctly. Environment variables such as `ASPNETCORE_URLS` may be needed depending on configuration.

- **Tests**
  - Some testing support exists (e.g. Spring Boot test dependencies for `BlogAggregator`), but the solution does not yet have comprehensive automated tests across all services and the frontend.

- **Hard-coded/demo keys**
  - The News API key in `docker-compose.yml` is for development/demo. Replace it with your own key or move it into a more secure configuration mechanism before production.

---

## Logging & Observability

- **.NET services**
  - Use the built-in ASP.NET Core logging infrastructure.
  - Log levels and sinks can be configured in each service’s `appsettings.json`.

- **Spring Boot (BlogAggregator)**
  - Uses standard Spring Boot logging (e.g. Logback).

- **Operational UIs**
  - **RabbitMQ Management**: inspect queues, messages, and service connections.
  - **Mongo Express**: inspect stored articles and collections.

You can extend this project with metrics (Prometheus / Grafana), centralized logging (ELK stack), or tracing (OpenTelemetry) as needed.

---

## Contributing

If you plan to extend this project:

- Follow existing naming conventions for services and namespaces/packages.
- Keep service boundaries clear (UserService for identity/preferences, ContentDelivery for reads, aggregators for ingestion, etc.).
- Write or extend unit/integration tests where appropriate.
- Consider adding linting and formatting tools in the frontend and backend for consistent code style.

---

## License

This repository currently does not declare a specific license.

If you intend to publish it publicly (e.g. on GitHub), add a `LICENSE` file (for example, MIT, Apache-2.0, or another license of your choice) and update this section accordingly.


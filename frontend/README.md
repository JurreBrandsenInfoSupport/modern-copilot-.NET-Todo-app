# Todo App Frontend

React + TypeScript frontend built with Vite for the .NET Todo API.

## Tech Stack

- **React 18** with TypeScript
- **Vite** — Fast build tooling
- **TanStack React Query** — Server state management
- **Tailwind CSS** — Utility-first styling
- **Axios** — HTTP client with JWT interceptor
- **React Router** — Client-side routing
- **Lucide React** — Icons

## Getting Started

### Prerequisites

- Node.js 20+
- npm 9+

### Install dependencies

```bash
cd frontend
npm install
```

### Development

```bash
npm run dev
```

The dev server runs on `http://localhost:5173` and proxies API requests to `http://localhost:5000`.

### Build for production

```bash
npm run build
```

Output is in the `dist/` folder.

### Docker

```bash
docker build -t todo-frontend .
docker run -p 3000:80 todo-frontend
```

Or use the root `docker-compose.yml` which includes the frontend service.

## Project Structure

```
src/
├── api/          # API service layer (axios client, typed API functions)
├── components/   # Reusable UI components (Layout, CommentsPanel)
├── context/      # React context providers (AuthContext)
├── pages/        # Route pages (Login, Dashboard, Tasks, Users, Health)
└── main.tsx      # Application entry point
```

## Features

- JWT-based authentication
- Task management (create, complete, filter by user)
- Comment system per task
- User registration
- Health check monitoring
- Responsive design with dark sidebar navigation

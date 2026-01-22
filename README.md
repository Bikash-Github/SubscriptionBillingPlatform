**Subscription Billing Platform (Enterprise-Grade Backend Architecture using .NET)**

**1. Project Description (Domain & Business Context)**

This project is built around the Subscription Management & Billing domain, a very common and high-demand backend use case in modern SaaS platforms.

**Real-world examples of this domain:**
  
  SaaS product subscriptions
  
  Membership platforms
  
  Paid feature access
  
  Recurring billing systems
  
  Enterprise licensing systems

**Core business responsibilities of this platform:**
  
  Authenticate users securely
  
  Manage user subscriptions
  
  Control subscription lifecycle (create, update, cancel)
  
  Integrate with billing/payment systems
  
  Enforce security, roles, and observability

**2. System Architecture Overview:**

AuthService

Shared Infrastructure

Local Credentials

Logging

OAuth 2.0

CorrelationId

AuthDB

Error Handling

JWT Creation

SubscriptionService

APIGateway

System Architecture

Subscription Operations

JWT Validation

Request Routing

SubscriptionDB

BillingService

Payment Processing

Billing operations

<img width="876" height="744" alt="image" src="https://github.com/user-attachments/assets/4e7fac64-aa51-421c-aad2-1444eed9749e" />

**3. Service Classification:**

Global Service (Cross-Cutting):

**AuthService**

**Purpose:** Central identity and authentication authority

**Responsibilities:**
  
  User authentication
  
  JWT token issuance
  
  Role management
  
  OAuth integration (Google)

**Why it is global?**
  
  Every other service trusts AuthService
  
  No other service issues tokens
  
  Prevents security duplication

**Shared Infrastructure service:**
  
  **Responsibilities:** 
    
    Logging
    
    Generate and propagate CorrelationId
    
    Error Handling
    
**Main Business Service (Core Domain):**

**SubscriptionService:**

  **Purpose:** Core business logic of the platform

  **Responsibilities:**
      
      Subscription lifecycle management
      
      Domain rules enforcement
      
      CQRS command & query handling
      
      Persistence using Dapper
    
    This service represents the heart of the system.

**Supporting Service (Secondary Domain)**
  
  **BillingService**
    
    **Purpose:** Handle billing-related operations
    
    **Responsibilities:** (Planned):
    
        Simulated billing responses
        
        Future payment processing
        
        Event-driven billing workflows
        
        Billing is intentionally isolated because:
        
        It is a high-risk domain
        
        Requires auditability
        
        Often integrates with third-party providers

**4. API Gateway (Entry Point):**
    
    Why an API Gateway exists
    
    The API Gateway acts as a facade for the entire system.

  **Responsibilities:**
    
    Single entry point for clients
    
    JWT validation
    
    Role-based authorization
    
    Request forwarding
    
    Correlation ID propagation

  **Future-ready for:**

      Rate limiting
      
      Throttling
      
      Request shaping
      
      Ocelot integration

**5. Solution File Overview:**

<img width="322" height="778" alt="image" src="https://github.com/user-attachments/assets/2ac7f458-3345-4dee-bd55-21dc0ea24f29" />


**6. AuthService – Deep Dive (Security Architecture):**

Authentication Methods Supported

  6.1. Local Authentication
    
    Email + Password
    
    Passwords hashed using BCrypt
    
    No plaintext storage

  6.2. Google OAuth (One-Tap Login)

    Google Identity Services
    
    No passwords stored for Google users
    
    Users auto-provisioned on first login

Google is used only for identity verification

JWT is always issued by AuthService

  **JWT Strategy**
    Single JWT authority
    
    Same token used across:
    
      API Gateway
      
      Subscription Service
      
      Billing Service
      
      Role claims embedded in token
      
      Swagger supports Authorize → Bearer Token testing.

**7. Observability & Diagnostics**
  
  **Correlation ID:**
  
    Global X-Correlation-Id
    
    Auto-generated if missing
    
    Propagated across services
    
    Included in logs and error responses

  **Logging:**
    
    Structured logging using Serilog
    
    **Current Implementation:** Console-based (free & local)
    
    **Ready for integrating in:**
    
      Grafana
      
      CloudWatch
      
      ELK stack

**8. Error Handling & API Standards**

    Centralized exception handling
    
    RFC 7807 ProblemDetails
    
    Consistent error contracts
    
| Scenario                | HTTP Status |
| ----------------------- | ----------- |
| Validation error        | 400         |
| Business rule violation | 409         |
| Unauthorized            | 401         |
| System failure          | 500         |

**Every error includes** 
    
    Meaningful message
    
    CorrelationId

**9. Health Checks & Container Readiness:**

Each service exposes 

  /health/live
  
  /health/ready

**Supports:**

  Container orchestration
  
  Kubernetes probes
  
  Deployment monitoring

**10. BillingService – Current & Upcoming**

**Current State:**
  
  Simulated billing endpoints
  
  Integrated call chain from SubscriptionService
  
  Correlation-aware logging

**Planned Enhancements:**
  
  Event-driven billing
  
  Outbox pattern
  
  Payment gateway abstraction
  
  Kafka-based integration

 **11. Upcoming / Planned Technical Enhancements:**
  
  These are intentionally designed but not over-implemented.

🔹 Redis
      
      Query caching
      
      Performance optimization
      
      Docker-based setup
      
      In-memory fallback abstraction

🔹 Kafka / RabbitMQ
      
      Subscription → Billing events
      
      Outbox pattern
      
      Async processing
      
      Failure resilience

🔹 Ocelot API Gateway
      
      Advanced routing
      
      Rate limiting
      
      Circuit breakers

🔹 Unit Testing
      
      Handler-level tests
      
      Domain rule tests
      
      CQRS-focused testing
      
      NUnit / xUnit

**12. Why This Project Matters**

  This project demonstrates:
  
    Real service boundaries
    
    Security-first thinking
    
    Clean Architecture discipline
    
    Domain-driven design
    
    Production-ready patterns
    
    No cloud lock-in
    
    No over-engineering

**13. How to Run Locally:**

    **Prerequisites**
    
        .NET 8 SDK
        SQL Server Express / LocalDB
        Docker (optional)
    **Steps**
    
        Clone repository
        Configure connection strings
    **Run services:**
    
        AuthService
        ApiGateway
        SubscriptionService
        BillingService

**14. Conclusion:**

This project represents how modern backend systems are actually designed and evolved in real organizations — with a strong focus on architecture clarity, security, scalability, and long-term maintainability.

Rather than being a feature-heavy demo, the emphasis is on:

    **Correct service boundaries**
    **Clean Architecture & DDD principles**
    **Security-first authentication and authorization**
    **Observability, diagnostics, and operational readiness**
    **Future extensibility without architectural rewrites**

The system is intentionally built using only free, local resources, making it easy to run, review, and extend — while still reflecting enterprise-grade design decisions.






👤 Author

**Bikash Pattanayak**
**Lead / Principal Backend Solutions Architect (.NET)**

**Specializing in:**

      ASP.NET Core & RESTful APIs using C#
      
      Clean Architecture, 
      
      DDD & CQRS
      
      Authentication & Authorization (JWT, OAuth)
      
      Distributed systems & microservices
      
      High-performance data access with Dapper
      
      DB Design with MS SQL Server.



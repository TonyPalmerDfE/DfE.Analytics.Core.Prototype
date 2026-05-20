## DfE Analytics Core - Architecture & event flow
The below describes the proposed analytics pipeline, its components, and how data flows through the system. 
The design is intentionally modular, allowing different dispatching and exporting options depending on the environment or scale of the application.

### High‑Level architecture
The pipeline is built around four core abstractions:
- Analytics Client — entry point for tracking events
- Enrichers — add contextual metadata
- Dispatcher — hands events off to an asynchronous processing mechanism
- Exporters — deliver events to final destinations.


Each component is replaceable, allowing the system to adapt to different hosting models, throughput requirements, and infrastructure.

### Event flow (Step‑by‑Step)
#### 1. Application creates an event
The application constructs an `AnalyticsEventEnvelope` containing:
- Event name
- Strongly typed event data
- Optional metadata

#### 2. Event sent to AnalyticsClient
 
`IAnalyticsClient.TrackAsync()` is called.

#### 3. Enrichers add context
All registered enrichers run in sequence, potentially adding:
- Correlation ID
- Environment metadata
- User/session/request details
- Any domain‑specific context

#### 4. Dispatcher hands off the event
The dispatcher’s responsibility is intentionally narrow:

Accept the enriched event and forward it to an asynchronous processing mechanism.

The mechanism is not prescribed. It could be:

- an in‑memory queue
- a bounded channel (default implementation)
- a distributed message bus (Kafka, Service Bus, RabbitMQ)
- a background job system
- a streaming or ingestion API
- a file or buffer writer

The dispatcher does not process or export events — it only hands them off.

#### 5. Worker or consumer processes events
A worker (or equivalent consumer) retrieves events from the chosen dispatch mechanism.

This could be:
- a hosted background service (default)
- a queue consumer
- a cloud function
- a batch processor

The worker is responsible for invoking all registered exporters.

#### 6. Exporters deliver the event
Exporters are the final stage. They may:
- write to console (default)
- send to an HTTP endpoint
- store in a database
- publish to a message queue
- forward to analytics platforms

Multiple exporters can run in parallel, consuming the same event and delivering it to different destinations.

#### Generic event flow diagram
```
 ┌──────────────────────────┐
 │   Application Code       │
 │  (Creates EventEnvelope) │
 └─────────────┬────────────┘
               │ TrackAsync()
               ▼
 ┌──────────────────────────┐
 │     AnalyticsClient      │
 └─────────────┬────────────┘
               │ EnrichAsync()
               ▼
 ┌──────────────────────────┐
 │   Enrichers (N of them)  │
 │ Add metadata, context    │
 └─────────────┬────────────┘
               │ DispatchAsync()
               ▼
 ┌──────────────────────────────────────────────┐
 │               Dispatcher Layer               │
 │  Forwards event to ANY async mechanism:      │
 │   • In‑memory queue                          │
 │   • Bounded channel (default)                │
 │   • Message bus (Kafka, Service Bus, etc.)   │
 │   • HTTP ingestion endpoint                  │
 │   • File/stream writer                       │
 └─────────────┬────────────────────────────────┘
               │
               ▼
 ┌──────────────────────────┐
 │   Worker / Consumer      │
 │ Reads events from chosen │
 │ dispatch mechanism       │
 └─────────────┬────────────┘
               │
               ▼
 ┌──────────────────────────┐
 │   Exporters (1..N)       │
 │ Console, API, Storage…   │
 └──────────────────────────┘
 ```
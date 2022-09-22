# Agenda

## Day 1

### Presentation
* Proto.Actor presentation
* https://realtimemap.skyrise.cloud/

### Create your first actor
* Props, PIDs - https://proto.actor/docs/props/ https://proto.actor/docs/pid/
* IActor, Func Actor https://proto.actor/docs/getting-started/#create-your-first-actor
* Messages, classes, records - importance of immutability

### Messaging
* Send vs Request vs RequestAsync vs Forward 
* Futures
* Await vs Reenter https://proto.actor/docs/reenter/#reentrancy

### Supervision
* OneForOne
* AlwaysRestart
* Exponential Backoff

### Routers
* Pool router
* Group router
* Round robin routers
* Consistent Hash routers

### Middleware https://proto.actor/docs/middleware/
* Receiver middleware
* Sender middleware
* Mailbox middleware
* Context decorator https://proto.actor/docs/context-decorator/
* Logger middleware
* Receive deadline middleware
* Tracing middleware

### Patterns
* Limit Concurrency
* Naive Throttling
* Throttling

### Remoting
* Set up a remote node
* Serialization Json vs Protobuf
* Remote Kinds
* Walkthrough Chat example

### Cluster
* Set up a cluster
* Cluster providers
* Identity lookups
* Members vs Clients
* Cluster Kinds   

### Persistence
#### Relational DBs
* Event sourcing
* Snapshots
#### KV Stores
* Snapshot


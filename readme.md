# Agenda

## Day 1

### Presentation
* Proto.Actor presentation

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

### Middleware
* Receiver middleware
* Sender middleware
* Mailbox middleware
* Context decorator
* Logger middleware
* Receive deadline middleware
* Tracing middleware

### Persistence
#### Relational DBs
* Event sourcing
* Snapshots
#### KV Stores
* Snapshot

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

## Day 2

### Mixing local and cluster actors
* Demo realtime map
* Show how local resources can be wrapped in local actors
* Show how local actors on a separate layer can keep connections alive

### Typed cluster actors
* Show how to use Typed/Code generated actors

### Proof of Concept development and brainstorming


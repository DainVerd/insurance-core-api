# insurance-core-api
Insurance core api C# project


## Tradeoffs

### Synchronous Repository Architecture
Decision: All repository and storage interactions are fully synchronous.
Rationale: Since the storage mechanism is strictly in-memory (ConcurrentDictionary<Guid, T>),
operations execute instantly within CPU and RAM.Forcing asynchronous wrappers (async/await, Task.FromResult,
and CancellationToken) would add unnecessary boilerplate, increase code complexity, and decrease overall readability without providing any real performance benefits for in-memory operations
For Prod: In a real production environment interacting with an external I/O bound database, these methods must be asynchronous to boost app throughput.
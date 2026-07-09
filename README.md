# insurance-core-api

Insurance core api C# project stored in [this github repo](https://github.com/DainVerd/insurance-core-api)

- [How to launch project locally](#how-to-launch-project-locally)
- [How to run tests](#how-to-run-tests)
  - [Using Visual Studio](#using-visual-studio)
  - [Using console](#using-console)
- [Tradeoffs](#tradeoffs)
  - [Sync repo architecture](#synchronous-repository-architecture)
  - [DateOnly and DateOffset](#dateonly-for-policy-and-dateoffset-for-claim-entity)
  - [Filtering in memory values](#overlap-filtering-is-done-in-service-instead-of-repository)
  - [Manual mapping](#manual-mapping)

## how to launch project locally

This section is dedicated to containing information about how to launch a project locally.

Steps to launch the project (tested in Visual Studio 2026):

1. Clone the repo of the project.
2. Open the solution in Visual Studio.
3. Select as a startup project `Web.API` project
4. Launch the Web API project using the green triangle at the navbar or by clicking on the project itself, LHB, and in the dropdown, selecting Run project. It will launch the project and open the default browser.
5. In the opened browser window, you will see Swagger. You can now test its endpoints.

## How to run tests

### using visual studio

Open visual studio. Select what unit tests you want to launch. In case you want to launch all unit tests, then click on "Solution" in the tray and in the dropdown menu select "Run All Tests." In case you want specific unit tests for `Application` layer, `Infrastructure` layer, or `Integration` "tests," you must click on the dedicated project (they all have the postfix ".Tests"). Select one and, using the mouse, click with the right button on it. A dropdown menu will appear. Select``.

### using console

in the root of the project(where is .sln file) run:

```bash
dotnet test
```

it will launch all projects containing unit tests `Application.Tests`, `Infrastructure.Tests`, `WebApi.IntegrationTests`.

If you want to launch only specific unit tests then use one of this commands

```bash
dotnet test Application.Tests/Application.Tests.csproj
dotnet test Infrastructure.Tests/Infrastructure.Tests.csproj
dotnet test WebApi.IntegrationTests/WebApi.IntegrationTests.csproj
```

## Tradeoffs

### Synchronous Repository Architecture

Decision: All repository and storage interactions are fully synchronous.

Rationale: Since the storage mechanism is strictly in-memory (ConcurrentDictionary<Guid, T>),
operations execute instantly within CPU and RAM.Forcing asynchronous wrappers (async/await, Task.FromResult,
and CancellationToken) would add unnecessary boilerplate, increase code complexity, and decrease overall readability without providing any real performance benefits for in-memory operations

For Prod: In a real production environment interacting with an external I/O bound database, these methods must be asynchronous to boost app throughput.

### DateOnly for Policy and DateOffset for Claim entity

Decision: For policy fields, start/end date uses DateOnly, but for Claim IncidentDate, I am using DateTimeOffset.
Rationale: Policy fields Start/end dates are dates. Only type because from an FE perspective, someone will set policy from date "x" to date "y," e.g., a person as a trainee works in a company from 1 July to 31 July, and setting time to this data would be overhead that is useless. In case of quitting the job earlier, the admin would just turn off the policy until a specific date or turn it off now. The Claim IncidentDate property is of type DateTimeOffset because in this case the time value could be a good hint for the investigation team when something goes bad or for login. It would be faster to find in logs what happened if you had a timestamp instead of just a date. Also, multiple incidents could happen in one day, but they would have different time values.

For Prod: Same approach. But it could change if FE wants to be more precise to set when applying policy. For example, from 1 am to 2 am. In most scenarios, policy is set from one date to another without specific time.

### Overlap filtering is done in service instead of repository

Decision: Overlap checking functionality is split between the ClaimRepository and the ClaimService method Active.

Rationale: Because in task I needed to use an in-memory DB, all data are already loaded into memory. If I used a "real" DB, for example, PostgreSQL, I would put all the LINQ query logic into ClaimRepository; this way, I would avoid all DB data loading into memory. Query will do the job for me.

For Prod: As mentioned before, for real projects that use a DB like PostgreSQL, MSSQL, or SQLite, i.e., not in-memory databases, I would put a LINQ query to ClaimRepository to only make a request to the DB and get the desired answer instead of pulling all the data to memory like I have currently.

### manual mapping

Decision: All mapping from entity to DTO is done manually.

Rationale: For simple entities and simple projects, using AutoMapper would be overhead.

For Prod: In real projects, it's better to use AutoMapper, especially for huge (a lot of properties) entities/DTOS, etc.

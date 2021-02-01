# CTeleport test assignment

## Getting started
1. Run `docker-compose up` from the root directory.
2. Go to `http://localhost:5000/api/distance` and specify `firstiata` and `secondiata` query parameters. 
3. Additionally you can monitor application's health by navigating to `http://localhost:5000/health`.

## Project structure

### Domain
This is the center of the clean architecture. Domain project contains the problem domain models and the domain-specific logic. So-called `Rich Domain Model` pattern is used to implement `IataPoint` - the core model of the application.

### Application
The Application project contains all the application logic such as coordination and communication with outside layers. It depends on the domain layer, but has no other dependencies. This layer defines contracts that can be implemented by outside layers, such as infrastructure. For example, the Application layer defines `IPlaceInfoProvider` which implemented in the Infrastructure layer. 

### Api
The entry point to the application - the Rest API based on ASP.NET Core 5.

### Infrastructure
Infrastructure project contains all the necessary infrastructure. All the external resources such as DB, cache, file systems and third party APIs should be part of the infrastructure. For example CTeleport's airports API client is implemented in the Infrastructure layer, as well as `Repository` to accessed cached data.  

### SharedCore
All the utility classes used throughout the whole solution. In a real-world application, this project most likely will be moved to a NuGet package, since all the applications can use the same abstractions such as `Result` or `ConfigurationExtansions`.

### Testing
Each layer/project has a sibling unit-tests project to cover it by tests. In Addition, there is another test project called `IntegrationTests` which contains several integration API tests and one end-to-end test, see `IntegrationTests.DistanceMeterServiceTests` (placed here for simplicity, but in a real-world application such tests should be kept separately).

### Some aspects were deliberately skipped to make the solution doable in a short period of time:
1. Logging
2. Swagger API documentation
3. HTTPS
4. I18N
5. Mapping (automapper or something similar)
# **Clean Architecture - Rest API**

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/synboxdev/RestApi-CleanArchitecture/dotnet.yml)

## 📄 <b>Table of contents</b>

* [About](#about)
* [Project structure](#project-structure)
* [Key features](#key-features)
* [Technology stack](#technology-stack)
* [Getting started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [Getting the project](#getting-the-project)
* [Running the project](#running-the-project)
    * [CMD](#cmd)
    * [IDE Tooling](#ide-tooling)
    * [Docker Compose](#docker-compose)
* [Using the project](#using-the-project)
* [Testing the project](#testing-the-project)
* [Side notes](#side-notes)
* [Contributing & Issues](#contributing--issues)
* [Code of Conduct](#code-of-conduct)
* [Credits & Licensing](#credits--licensing)

## **About**

This project is a sample of a [**RESTful API**](https://aws.amazon.com/what-is/restful-api/) built with primary focus on clean architecture design pattern and application of [**SOLID principles**](https://en.wikipedia.org/wiki/SOLID).

The main purpose of this repository is to serve as a boilerplate example of a functional API project, with good foundational structure and foresight for potential future <strong>scalability</strong>, <strong>testability</strong> and <strong>maintainability</strong>. 

## **Project structure**

Project follows a variation of so called [**Clean architecture**](https://www.geeksforgeeks.org/complete-guide-to-clean-architecture/) software design approach, with implementation of layered systems, focus on separation of concerns, also often described as 'onion architecture'.

This project has the following structure:

- **API**: Contains the presentation layer, which exposes the functionality of the system to the outside world. It includes controllers, initial system configurations, global middleware. Think of this as the layer through which the users communicate with our systems.
- **Core**: Contains two key parts of the system, that act as the middle-men between the API and the backend infrastructure.
    - **Application**: Contains the application layer, which implements the use cases of the system. It includes business services, commands, queries, handlers, mapping profiles, DTOs and their validators.
    - **Domain**: Contains the domain layer, which includes entity DTOs amd shared models.
- **Infrastructure**: Contains three key parts of the system, which are the back-bone of the entire system.
    - **Access**: Contains commands and queries that directly call database implementations, as well as the data entities themselves.
    - **Persistence**: This is <strong>the</strong> spine of the system. It includes database contexts, migrations, utility sub-systems, and most importantly - implementation of Access layer's commands and queries that directly interact with the databases.
    - **Background**: This is self-contained service, entirely responsible for background-running tasks that interact only with the backend systems i.e. database. Things such as scheduled database cleanup, backup tasks, etc. Slightly more details about this - read [side notes](#side-notes) section.
- **Tests**: Even though tests aren't part of the 'main' project's ensemble, they are still an absolutely vital part. Tests are structured following identical folder-hierarchy structure as the main project itself.

## **Technology stack**

* #### [**C#**](https://learn.microsoft.com/en-us/dotnet/csharp/) / [**.NET 9**](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) / [**Entity Framework**](https://learn.microsoft.com/en-us/aspnet/entity-framework)
* #### [**Visual Studio 2022**](https://visualstudio.microsoft.com/vs/) / [**Visual Studio Code**](https://code.visualstudio.com/)
* #### [**PostgreSQL**](https://www.postgresql.org/) / [**pgAdmin**](https://www.pgadmin.org/)
* #### [**Git**](https://learn.microsoft.com/en-us/devops/develop/git/what-is-git) / [**Sourcetree**](https://www.sourcetreeapp.com/)
* #### [**Docker (Docker Desktop)**](https://www.docker.com/)

## **Key features**

- Implementation of [**Clean architecture**](https://www.geeksforgeeks.org/complete-guide-to-clean-architecture/) and [**SOLID**](https://en.wikipedia.org/wiki/SOLID) software design principles
- [**.NET 9**](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) with [**Entity Framework Core**](https://learn.microsoft.com/en-us/ef/core/)
- [**Command Query Responsibility Segregation (CQRS)**](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) with [**MediatR pattern**](https://en.wikipedia.org/wiki/Mediator_pattern)
- [**JWT Token & Authentication**](https://en.wikipedia.org/wiki/JSON_Web_Token) with [**ASP.NET Core Identity**](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&tabs=visual-studio) 
- Global exception middleware handling and data validation using [**Fluent Validation**](https://docs.fluentvalidation.net/en/latest/) library
- Standardized API response modeling
- Containerization using [**Docker**](https://www.docker.com/) and [**Docker Compose**](https://docs.docker.com/compose/)
- Usage of [**PostgreSQL**](https://www.postgresql.org/) open-source database
- Unit testing, integration testing and infrastructure configuration testing using [**xUnit**](https://xunit.net/) and [**Moq**](https://github.com/devlooped/moq) framework

## **Getting started**

### **Prerequisites**
- [**.NET 9 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [**Docker**](https://www.docker.com/)
- [**PostgreSQL**](https://www.postgresql.org/download/)
- IDE of your choice. I use [**Visual Studio**](https://visualstudio.microsoft.com/downloads/) / [**Visual Studio Code**](https://code.visualstudio.com/)
- [**Git**](https://git-scm.com/downloads) (Optional, if you want to use it to clone the repository)

### **Getting the project**
There are multiple ways of doing this. Here's two:
1. <strong>Method 1</strong> - run the following command using a CMD terminal in an empty directory:

   ```bash
   git clone https://github.com/synboxdev/RestApi-CleanArchitecture.git
   ```
Afterwards, the project's repository will be downloaded to the directory inside of which you've ran this command.

2. <strong>Method 2</strong> - Downloading zipped repository and extracting it locally. [**Download**](https://github.com/synboxdev/RestApi-CleanArchitecture/archive/refs/heads/main.zip) zipped repository, and extract the files to a folder of your choosing.

## **Running the project**

Once you've retrieved the repository to your local machine, before running the following, do the following:
1. Build the solution to restore dependencies.
2. Modify the connection strings in the appsettings files which appropriate values to your local PostgreSQL database
3. Start the project with one of the methods described below

Additional note - the database migrations will be automatically applied on project's start-up. If databases do not exist - they will be created. As long as connections strings are correct, and the project is able to communicate with the database - there shouldn't be any problems. 

### **CMD**

You start the project locally, by executing the following command in the root directory of the solution

   ```bash
   dotnet run --project Hestia.Api --launch-profile Local
   ```
And then open the following URL in your browser - `http://localhost:7021/swagger/index.html`

### **IDE Tooling**

If you are using Visual Studio - you can simply start the project with the GUI interface - navigate to **'Debug'** tab -> **'Start without debugging'**. Or use the default shortcut of **CTRL + F5**.

### **Docker Compose**

**IMPORTANT** - You'll have to generate and trust a HTTPS dev certificate for local development. Follow this documentation [Hosting ASP.NET Core images with Docker Compose over HTTPS](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-9.0). 

Once you've generated and trusted a HTTPS certificate - navigate to root directory of the project and open up 'docker-compose.yml' file with a text editor of your choice. Modify the certificate's 'Password' and 'Path' fields with appropriate values. If you've followed the documentation as-in, the only thing you'll have to adjust is the password values.

Now, make sure that Docker is running, and you can execute the following command in the root folder of the solution

   ```bash
   docker-compose up -d
   ```

Once the docker container, containing both the API and the database are running, you can open and access it via browser in the following URL - `https://localhost:8082/swagger/index.html`

## **Using the project**

Once you've successfully cloned the project to your local machine, and you've got it up and running, and are able to access it via the browser, now can you actually use it! As you may already tell, there isn't much functionality - just two endpoints for authentication, and four endpoints of basic CRUD functionality. That's obviously by design, as mentioned in the introductory section - primary focus is on the architectural design and structure of the solution, rather than actual 'functionality' of it.

1. To start using the system, firstly, you need to call `api/Authentication/Register` endpoint, and create an authenticated user of the system. For example, if we send a request with the following values:

```bash
curl -X 'POST' \
  'https://localhost:7021/api/Authentication/Register' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "name": "name123",
  "username": "username123",
  "password": "password123",
  "email": "mail@mail.com",
  "role": "Admin"
}'
```

We can expect the following result:

```bash
{
  "success": true,
  "statusCode": "Created",
  "message": "User by the username 'username123' and email 'mail@mail.com' has been successfully created!",
  "data": {
    "username": "username123",
    "email": "mail@mail.com",
    "role": "Admin"
  }
}
```

2. Next, we need to authenticate ourselves with the system, and receive a JWT Token, which will be necessary to provide, to make calls to other endpoints, which will demand us to be authenticated. We send the following request, to the `api/Authentication/Login` endpoint:

```bash
curl -X 'POST' \
  'https://localhost:7021/api/Authentication/Login' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "username123",
  "password": "password123",
  "email": "mail@mail.com"
}'
```

We can expect the following result:

```bash
{
  "success": true,
  "statusCode": "OK",
  "message": "Login for the user 'username123' was successful!",
  "data": {
    "username": "username123",
    "email": "mail@mail.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjNhOGM1N2I2LTI2MzMtNDM0Yi1hZWIxLTNhZWQ0MWM2YzczZCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ1c2VybmFtZTEyMyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Im1haWxAbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTczMzQxMDk1NSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAyMSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMjEifQ.Nk5gpSyz71seMyjaGl2QeLeVBFpVgvID3h8S3UEx1m4"
  }
}
```

Once we receive an JWT token, which is eligible for 1 hour - we can now make calls to authenticated endpoints! If you're using swagger, you can press the 'Authorize' button top-right of the page, and paste in the 'token' value from the response that we just got. Slightly more details about this - read [side notes](#side-notes) section.

## **Testing the project**

If you wish to run the tests:

1. Open the project's solution with the IDE of your choice
2. Build the solution to restore dependencies
3. Navigate to toolbar section -> **'Test'** -> **'Run all tests'**
4. Results should be visible in the Test Explorer window. If this windows is NOT opened - navigate to the toolbar section -> **'View'** -> **'Test Explorer'**

## **Side notes**

Few noteworthy things regarding the design decisions and functional implementations in this project:
- To state the obvious, even if repeating myself - this is merely an example on how a Web API project, using identical or analogous technology stack <i>could</i> be structured from an architectural standpoint. There is no perfect, 'one-size-fits-all' type structure. It varies business-by-business, solution-by-solution. So take this, and any other architectural 'sample' project with a grain of salt, learn from it, and adjust your own project by your own needs.
- In a 'real-world' environment, both the authentication part and the background-task part of the system would most likely be their own, separate projects, designed and built by their own requirements. They are included in this project just to showcase them alongside the basic CRUD functionalities. Obviously, just any user wouldn't be able to Register other users with Admin roles, nor is there a need to put the load of background-running tasks on the 'main' business data processing application.
- **'What's the point of 'User' and 'TokenLog' entities, if you're already utilizing authentication with ASP.NET Identity?'**. The idea behind separate 'User' and 'TokenLog' entities, as well as separate database context, is so that they can represent business-specific authenticated entities. We can then tie authenticated users to their identity entities, as well as fetch the token which they are using to authenticate themselves with our system, and automatically match their data (e.g. products) with their unique UserIds. Additionally, we can separate the entire authentication system from the 'main' business project, since even the database context for the authentication-related entities is separated, which allows us to use entirely different database engine or even entirely different tech stack for authentication service, if we wish to do so.
- **'What's the point of multiple identification fields for Product entity?'**. Well, 'Id' is a GUID value which is entirely managed by our system, we can be sure that its unique. That's not the case for the data that our users may send to us. That's where 'ExternalId' of type string comes into play - we do not know, what type of field, or what value do our users (e.g. Supplier companies) track uniqueness of their products within their own, internal systems - maybe its an integer, auto-incrementing field, maybe its a product's barcode value, or maybe its GUID value. This give us our system a bit more flexibility. Obviously, the 'ExternalId' as a concept can be entirely removed, and the system can operate relying fully on the internal 'Id', which you'd provide as a response to 'Create' endpoint, and all other endpoints would require that specific Id to be provided, for any other changes to be made.

## **Contributing & Issues**

Any and all positive contributions are welcome, including features, issues, documentation, guides, and more. See [Contributing documentation](CONTRIBUTING.md) for more details.

## **Code of Conduct**
This project and everyone participating in it is governed by the [Code of Conduct](CODE_OF_CONDUCT.md). By participating and contributing the the project and its contents, you are expected to uphold this code.

## **Credits & Licensing**

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

For any and all inquires, feel free to [contact me](https://github.com/synboxdev).

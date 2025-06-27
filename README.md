# Employee Management System

## Project Overview

The Employee Management System is a robust web application designed to streamline the management of employee data, departments, and tasks within an organization. Built with ASP.NET Core and adhering to Clean Architecture principles, this system provides a clear separation of concerns, making it highly maintainable, scalable, and testable. It offers comprehensive CRUD (Create, Read, Update, Delete) operations for employees, departments, and tasks, ensuring efficient data handling and organizational workflow.

## Key Features

*   **Employee Management**: Full CRUD operations for employee records, including personal details, contact information, and assigned departments.
*   **Department Management**: Efficient creation, retrieval, updating, and deletion of department information.
*   **Task Assignment and Tracking**: Assign tasks to employees, track their progress, and manage task details.
*   **User Authentication and Authorization**: Secure access control with user login and role-based permissions.
*   **Clean Architecture**: A well-structured codebase promoting maintainability, testability, and scalability through clear separation of layers.
*   **Responsive UI**: A user-friendly interface built with ASP.NET Core MVC, ensuring accessibility across various devices.

## Technical Stack and Dependencies

*   **Backend**: ASP.NET Core (.NET 9)
*   **Database**: SQL Server (with Entity Framework Core for ORM)
*   **Frontend**: HTML, CSS, JavaScript, jQuery, Bootstrap
*   **ORM**: Entity Framework Core
*   **Architecture**: Clean Architecture, Repository Pattern, Unit of Work Pattern
*   **Dependency Injection**: Built-in ASP.NET Core DI

## Project Structure

The project is organized into several layers, following the principles of Clean Architecture:

*   **EmployeeManagement.Domain**: Contains the core business entities, aggregates, and interfaces for repositories. This layer is independent of any infrastructure concerns.
*   **EmployeeManagement.Application**: Houses the application-specific business rules, use cases, and application services. It orchestrates the flow of data between the UI and the Domain layer.
*   **EmployeeManagement.Infrastructure**: Implements the interfaces defined in the Domain layer, providing concrete implementations for data access (Entity Framework Core) and other external services.
*   **EmployeeManagement.Persistence**: (Often part of Infrastructure or a separate layer) Handles database context and migrations.
*   **EmployeeManagement.Web (or EmployeeManagementSystem)**: The presentation layer, typically an ASP.NET Core MVC or API project, responsible for handling user requests, routing, and presenting data. It depends on the Application layer.




### Clean Architecture Layers Explained

Clean Architecture emphasizes a separation of concerns, organizing code into concentric circles, where inner circles are policies and outer circles are mechanisms. This design ensures that business rules are independent of frameworks, databases, and UI.

1.  **Domain Layer (EmployeeManagement.Domain)**:
    *   **Purpose**: This is the innermost circle, containing the enterprise-wide business rules. It defines the core entities, value objects, and interfaces that represent the business concepts (e.g., `Employee`, `Department`, `Task`). It is completely independent of any external concerns.
    *   **Key Components**: Entities, Value Objects, Aggregates, Domain Services, Repository Interfaces.
    *   **Dependencies**: None. This layer defines the contracts that other layers must adhere to.

2.  **Application Layer (EmployeeManagement.Application)**:
    *   **Purpose**: This layer contains the application-specific business rules. It orchestrates the flow of data to and from the Domain entities and is responsible for implementing the use cases of the system. It defines interfaces for external services (like data access or notifications) that are implemented in the Infrastructure layer.
    *   **Key Components**: Application Services, DTOs (Data Transfer Objects), Command/Query Handlers (if using CQRS), Interfaces for Infrastructure services (e.g., `IEmployeeService`, `IDepartmentService`).
    *   **Dependencies**: Depends on the Domain layer. It does not depend on the Infrastructure or Presentation layers.

3.  **Infrastructure Layer (EmployeeManagement.Infrastructure)**:
    *   **Purpose**: This layer is responsible for implementing the interfaces defined in the Domain and Application layers. It handles all external concerns such as database interactions, file system access, external API calls, and third-party integrations.
    *   **Key Components**: Concrete implementations of repositories (e.g., `EmployeeRepository`, `DepartmentRepository`), Entity Framework Core `DbContext`, external service implementations, logging, and configuration.
    *   **Dependencies**: Depends on the Domain and Application layers. It should not contain business logic.

4.  **Presentation Layer (EmployeeManagement.Web / EmployeeManagementSystem)**:
    *   **Purpose**: This is the outermost layer, responsible for handling user interactions and presenting data. It includes controllers, views (for MVC applications), and API endpoints. It orchestrates calls to the Application layer to execute use cases.
    *   **Key Components**: Controllers, Views, ViewModels, API Endpoints, UI components.
    *   **Dependencies**: Depends on the Application layer to interact with the business logic. It should not contain business logic itself.

This layered approach ensures that changes in external concerns (like switching databases) do not affect the core business logic, and vice-versa, promoting a highly maintainable and adaptable system.




## API Endpoints

This section details the API endpoints available in the Employee Management System, including their HTTP methods, parameters, and example request/response payloads.

### DepartmentsController

The `DepartmentsController` handles all operations related to department management.

*   **GET /Departments/Index**
    *   **Description**: Retrieves a list of all departments, with optional search functionality.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `searchString` (optional): A string to filter departments by name.
    *   **Example Request**:
        ```
        GET /Departments/Index?searchString=IT
        ```
    *   **Example Response (HTML View)**:
        (Returns an HTML view of departments)

*   **GET /Departments/CreateModal**
    *   **Description**: Returns a partial view for creating a new department.
    *   **HTTP Method**: `GET`
    *   **Parameters**: None
    *   **Example Request**:
        ```
        GET /Departments/CreateModal
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a form for department creation)

*   **POST /Departments/CreateModal**
    *   **Description**: Creates a new department.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `DepartmentViewModel` object containing department details.
            ```json
            {
                "Name": "New Department Name"
            }
            ```
    *   **Example Request**:
        ```
        POST /Departments/CreateModal
        Content-Type: application/json

        {
            "Name": "Human Resources"
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true
        }
        ```
        *   **Error Response (JSON)**:
        ```json
        {
            "success": false,
            "message": "Error message here"
        }
        ```

*   **GET /Departments/EditModal/{id}**
    *   **Description**: Returns a partial view for editing an existing department.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the department to edit.
    *   **Example Request**:
        ```
        GET /Departments/EditModal/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a form pre-filled with department details)

*   **POST /Departments/EditModal**
    *   **Description**: Updates an existing department.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `DepartmentViewModel` object containing updated department details.
            ```json
            {
                "Id": 1,
                "Name": "Updated Department Name"
            }
            ```
    *   **Example Request**:
        ```
        POST /Departments/EditModal
        Content-Type: application/json

        {
            "Id": 1,
            "Name": "Marketing Department"
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "department": {
                "id": 1,
                "name": "Marketing Department",
                "employeeCount": 10,
                "totalSalary": "$50,000.00"
            }
        }
        ```
        *   **Error Response (JSON)**:
        ```json
        {
            "success": false,
            "message": "Failed to update department. Please try again."
        }
        ```

*   **GET /Departments/DeleteModal/{id}**
    *   **Description**: Returns a partial view for confirming deletion of a department.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the department to delete.
    *   **Example Request**:
        ```
        GET /Departments/DeleteModal/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a confirmation for department deletion)

*   **POST /Departments/DeleteModal**
    *   **Description**: Deletes a department.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `DepartmentViewModel` object containing the ID of the department to delete.
            ```json
            {
                "Id": 1
            }
            ```
    *   **Example Request**:
        ```
        POST /Departments/DeleteModal
        Content-Type: application/json

        {
            "Id": 1
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true
        }
        ```
        *   **Error Response (JSON)**:
        ```json
        {
            "success": false,
            "message": "Cannot delete department with assigned employees."
        }
        ```




### EmployeesController

The `EmployeesController` handles all operations related to employee management.

*   **GET /Employees/Index**
    *   **Description**: Retrieves a list of all employees, with optional search functionality.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `searchString` (optional): A string to filter employees by name.
    *   **Example Request**:
        ```
        GET /Employees/Index?searchString=John
        ```
    *   **Example Response (HTML View)**:
        (Returns an HTML view of employees)

*   **GET /Employees/CreateModal**
    *   **Description**: Returns a partial view for creating a new employee.
    *   **HTTP Method**: `GET`
    *   **Parameters**: None
    *   **Example Request**:
        ```
        GET /Employees/CreateModal
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a form for employee creation)

*   **POST /Employees/CreateModal**
    *   **Description**: Creates a new employee.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `EmployeeViewModel` object containing employee details, including an optional `ImageFile`.
            ```json
            {
                "FirstName": "Jane",
                "LastName": "Doe",
                "Email": "jane.doe@example.com",
                "PhoneNumber": "123-456-7890",
                "DateOfBirth": "1990-01-01",
                "Salary": 60000,
                "DepartmentId": 1,
                "ManagerId": null
            }
            ```
    *   **Example Request**:
        ```
        POST /Employees/CreateModal
        Content-Type: multipart/form-data

        --boundary
        Content-Disposition: form-data; name="FirstName"

        Jane
        --boundary
        Content-Disposition: form-data; name="LastName"

        Doe
        --boundary
        Content-Disposition: form-data; name="Email"

        jane.doe@example.com
        --boundary
        Content-Disposition: form-data; name="PhoneNumber"

        123-456-7890
        --boundary
        Content-Disposition: form-data; name="DateOfBirth"

        1990-01-01
        --boundary
        Content-Disposition: form-data; name="Salary"

        60000
        --boundary
        Content-Disposition: form-data; name="DepartmentId"

        1
        --boundary
        Content-Disposition: form-data; name="ImageFile"; filename="profile.jpg"
        Content-Type: image/jpeg

        <binary image data>
        --boundary--
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "employee": {
                "id": 1,
                "firstName": "Jane",
                "lastName": "Doe",
                "fullName": "Jane Doe",
                "salary": "60000.00",
                "imagePath": "/uploads/images/profile.jpg",
                "departmentName": "Human Resources",
                "managerName": "-"
            }
        }
        ```
        *   **Error Response (JSON)**:
        ```json
        {
            "success": false,
            "message": "Failed to create employee. Please try again."
        }
        ```

*   **GET /Employees/EditModal/{id}**
    *   **Description**: Returns a partial view for editing an existing employee.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the employee to edit.
    *   **Example Request**:
        ```
        GET /Employees/EditModal/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a form pre-filled with employee details)

*   **POST /Employees/EditModal**
    *   **Description**: Updates an existing employee.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `EmployeeViewModel` object containing updated employee details, including an optional `ImageFile`.
            ```json
            {
                "Id": 1,
                "FirstName": "Jane",
                "LastName": "Smith",
                "Email": "jane.smith@example.com",
                "PhoneNumber": "098-765-4321",
                "DateOfBirth": "1990-01-01",
                "Salary": 65000,
                "DepartmentId": 2,
                "ManagerId": 5
            }
            ```
    *   **Example Request**:
        ```
        POST /Employees/EditModal
        Content-Type: multipart/form-data

        --boundary
        Content-Disposition: form-data; name="Id"

        1
        --boundary
        Content-Disposition: form-data; name="FirstName"

        Jane
        --boundary
        Content-Disposition: form-data; name="LastName"

        Smith
        --boundary
        Content-Disposition: form-data; name="Email"

        jane.smith@example.com
        --boundary
        Content-Disposition: form-data; name="PhoneNumber"

        098-765-4321
        --boundary
        Content-Disposition: form-data; name="DateOfBirth"

        1990-01-01
        --boundary
        Content-Disposition: form-data; name="Salary"

        65000
        --boundary
        Content-Disposition: form-data; name="DepartmentId"

        2
        --boundary
        Content-Disposition: form-data; name="ManagerId"

        5
        --boundary
        Content-Disposition: form-data; name="ImageFile"; filename="new_profile.png"
        Content-Type: image/png

        <binary image data>
        --boundary--
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "employee": {
                "id": 1,
                "firstName": "Jane",
                "lastName": "Smith",
                "fullName": "Jane Smith",
                "salary": "65000.00",
                "imagePath": "/uploads/images/new_profile.png",
                "departmentName": "Marketing",
                "managerName": "John Doe"
            }
        }
        ```

*   **GET /Employees/DeleteModal/{id}**
    *   **Description**: Returns a partial view for confirming deletion of an employee.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the employee to delete.
    *   **Example Request**:
        ```
        GET /Employees/DeleteModal/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a confirmation for employee deletion)

*   **POST /Employees/DeleteModal**
    *   **Description**: Deletes an employee.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `EmployeeViewModel` object containing the ID of the employee to delete.
            ```json
            {
                "Id": 1
            }
            ```
    *   **Example Request**:
        ```
        POST /Employees/DeleteModal
        Content-Type: application/json

        {
            "Id": 1
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "id": 1
        }
        ```




### TasksController

The `TasksController` handles all operations related to task management.

*   **GET /Tasks/Index**
    *   **Description**: Retrieves a list of tasks based on the user's role (tasks assigned by manager or tasks assigned to employee).
    *   **HTTP Method**: `GET`
    *   **Parameters**: None
    *   **Example Request**:
        ```
        GET /Tasks/Index
        ```
    *   **Example Response (HTML View)**:
        (Returns an HTML view of tasks relevant to the current user)

*   **GET /Tasks/Create**
    *   **Description**: Returns a view for creating a new task (accessible only to Managers).
    *   **HTTP Method**: `GET`
    *   **Parameters**: None
    *   **Example Request**:
        ```
        GET /Tasks/Create
        ```
    *   **Example Response (HTML View)**:
        (Returns an HTML view containing a form for task creation)

*   **POST /Tasks/Create**
    *   **Description**: Creates a new task (accessible only to Managers).
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `model`: `TaskMB` object containing task details.
            ```json
            {
                "Title": "Develop new feature",
                "Description": "Implement user authentication module.",
                "DueDate": "2025-07-31T23:59:59",
                "EmployeeId": 1,
                "Status": 0 // Pending
            }
            ```
    *   **Example Request**:
        ```
        POST /Tasks/Create
        Content-Type: application/json

        {
            "Title": "Develop new feature",
            "Description": "Implement user authentication module.",
            "DueDate": "2025-07-31T23:59:59",
            "EmployeeId": 1,
            "Status": 0
        }
        ```
    *   **Example Response (Redirect)**:
        (Redirects to `/Tasks/Index` on success)

*   **GET /Tasks/MyTasks**
    *   **Description**: Retrieves tasks assigned to the currently logged-in employee.
    *   **HTTP Method**: `GET`
    *   **Parameters**: None
    *   **Example Request**:
        ```
        GET /Tasks/MyTasks
        ```
    *   **Example Response (HTML View)**:
        (Returns an HTML view of tasks assigned to the employee)

*   **POST /Tasks/UpdateStatus**
    *   **Description**: Updates the status of a specific task.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `taskId` (int): The ID of the task to update.
        *   `newStatus` (int): The new status for the task (e.g., 0 for Pending, 1 for InProgress, 2 for Completed).
    *   **Example Request**:
        ```
        POST /Tasks/UpdateStatus
        Content-Type: application/x-www-form-urlencoded

        taskId=1&newStatus=1
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "message": "Task status updated successfully."
        }
        ```

*   **GET /Tasks/Details/{id}**
    *   **Description**: Retrieves details of a specific task.
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the task.
    *   **Example Request**:
        ```
        GET /Tasks/Details/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view with task details)

*   **GET /Tasks/Edit/{id}**
    *   **Description**: Returns a partial view for editing a task. The view differs based on user role (Manager or Employee).
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the task to edit.
    *   **Example Request**:
        ```
        GET /Tasks/Edit/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a form for task editing)

*   **POST /Tasks/Edit/{id}**
    *   **Description**: Updates an existing task.
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `id` (int): The ID of the task to update.
        *   `model`: `TaskMB` object containing updated task details.
            ```json
            {
                "Id": 1,
                "Title": "Updated Task Title",
                "Description": "Updated task description.",
                "DueDate": "2025-08-15T10:00:00",
                "EmployeeId": 2,
                "Status": 1
            }
            ```
    *   **Example Request**:
        ```
        POST /Tasks/Edit/1
        Content-Type: application/json

        {
            "Id": 1,
            "Title": "Updated Task Title",
            "Description": "Updated task description.",
            "DueDate": "2025-08-15T10:00:00",
            "EmployeeId": 2,
            "Status": 1
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "message": "Task updated successfully."
        }
        ```

*   **POST /Tasks/UpdateAssignment/{id}**
    *   **Description**: Reassigns a task to a different employee (accessible only to Managers).
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `id` (int): The ID of the task to reassign.
        *   `model`: `TaskMB` object containing the new `EmployeeId`.
            ```json
            {
                "Id": 1,
                "EmployeeId": 3
            }
            ```
    *   **Example Request**:
        ```
        POST /Tasks/UpdateAssignment/1
        Content-Type: application/json

        {
            "Id": 1,
            "EmployeeId": 3
        }
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "message": "Task reassigned successfully",
            "task": { ... updated task details ... }
        }
        ```

*   **GET /Tasks/AssignTask/{id}**
    *   **Description**: Returns a partial view for assigning a task to an employee (accessible only to Managers).
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the task to assign.
    *   **Example Request**:
        ```
        GET /Tasks/AssignTask/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view with employee selection for task assignment)

*   **GET /Tasks/Delete/{id}**
    *   **Description**: Returns a partial view for confirming deletion of a task (accessible only to Managers).
    *   **HTTP Method**: `GET`
    *   **Parameters**:
        *   `id` (int): The ID of the task to delete.
    *   **Example Request**:
        ```
        GET /Tasks/Delete/1
        ```
    *   **Example Response (HTML Partial View)**:
        (Returns an HTML partial view containing a confirmation for task deletion)

*   **POST /Tasks/DeleteConfirmed/{id}**
    *   **Description**: Deletes a task (accessible only to Managers).
    *   **HTTP Method**: `POST`
    *   **Parameters**:
        *   `id` (int): The ID of the task to delete.
    *   **Example Request**:
        ```
        POST /Tasks/DeleteConfirmed/1
        Content-Type: application/x-www-form-urlencoded

        id=1
        ```
    *   **Example Response (JSON)**:
        ```json
        {
            "success": true,
            "message": "Task deleted successfully."
        }
        ```




## Service Layer Functions

The service layer in the `EmployeeManagement.Application` project encapsulates the application's business logic and orchestrates operations across different domain entities. It acts as an intermediary between the controllers (presentation layer) and the domain/infrastructure layers.

### IDepartmentService and DepartmentService

`IDepartmentService` defines the contract for department-related operations, and `DepartmentService` provides its implementation. This service is responsible for managing department data, including CRUD operations and business rules related to departments.

#### Key Methods:

*   **`Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync()`**:
    *   **Description**: Retrieves all departments, including their associated employees to calculate `EmployeeCount` and `TotalSalary`.
    *   **Business Logic**: Iterates through departments and their employees to aggregate employee count and total salary for each department.

*   **`Task<DepartmentViewModel> GetDepartmentByIdAsync(int id)`**:
    *   **Description**: Retrieves a single department by its ID, along with its employee count and total salary.
    *   **Business Logic**: Fetches a specific department and calculates aggregated employee data.

*   **`Task<DepartmentViewModel> CreateDepartmentAsync(DepartmentViewModel departmentViewModel)`**:
    *   **Description**: Creates a new department.
    *   **Business Logic**: Maps the `DepartmentViewModel` to a `Department` entity, sets creation and update timestamps, and persists the new department to the database.

*   **`Task<DepartmentViewModel> UpdateDepartmentAsync(DepartmentViewModel departmentViewModel)`**:
    *   **Description**: Updates an existing department.
    *   **Business Logic**: Retrieves the existing department, maps updated data from the `DepartmentViewModel`, and saves changes to the database.

*   **`Task DeleteDepartmentAsync(int id)`**:
    *   **Description**: Deletes a department by its ID.
    *   **Business Logic**: Before deletion, it checks if the department has any assigned employees using `CanDeleteDepartmentAsync`. If employees are present, it prevents deletion and throws an `InvalidOperationException`.

*   **`Task<bool> CanDeleteDepartmentAsync(int id)`**:
    *   **Description**: Checks if a department can be deleted (i.e., if it has no assigned employees).
    *   **Business Logic**: Determines if the `Employees` collection for the given department is empty.

*   **`Task<IEnumerable<DepartmentViewModel>> SearchDepartmentsAsync(string searchTerm)`**:
    *   **Description**: Searches for departments by a given search term.
    *   **Business Logic**: Filters departments based on the `searchTerm` and calculates `EmployeeCount` and `TotalSalary` for the matching departments.

#### Design Patterns Implemented:

*   **Repository Pattern**: The `DepartmentService` interacts with the database through `_unitOfWork.Departments`, which is an instance of a generic repository (`IGenericRepository<Department>`). This abstracts the data access logic from the business logic.
*   **Unit of Work Pattern**: The `_unitOfWork` instance (`IUnitOfWork`) ensures that all operations within a business transaction are treated as a single unit. Changes are committed together via `_unitOfWork.SaveChangesAsync()`.
*   **AutoMapper**: Used for mapping between `DepartmentViewModel` and `Department` entities, reducing boilerplate code for object-to-object mapping.




### IEmployeeService and EmployeeService

`IEmployeeService` defines the contract for employee-related operations, and `EmployeeService` provides its implementation. This service handles the business logic for managing employee data, including CRUD operations, image handling, and manager assignments.

#### Key Methods:

*   **`Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync()`**:
    *   **Description**: Retrieves all employees, including their associated department and manager information.
    *   **Business Logic**: Fetches employee data and manually maps it to `EmployeeViewModel` to include `DepartmentName` and `ManagerName`.

*   **`Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id)`**:
    *   **Description**: Retrieves a single employee by their ID.
    *   **Business Logic**: Fetches a specific employee and maps it to `EmployeeViewModel`.

*   **`Task<Employee?> GetEmployeeByUserIdAsync(string userId)`**:
    *   **Description**: Retrieves an employee associated with a specific user ID.
    *   **Business Logic**: Queries the employee data based on the `UserId` property.

*   **`Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile)`**:
    *   **Description**: Creates a new employee record and handles image file uploads.
    *   **Business Logic**: Maps the `EmployeeViewModel` to an `Employee` entity, saves the provided `imageFile` to the file system, updates the `ImagePath`, and persists the new employee.

*   **`Task<EmployeeViewModel> UpdateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile)`**:
    *   **Description**: Updates an existing employee record and handles image file replacement.
    *   **Business Logic**: Retrieves the existing employee, updates its properties from the `EmployeeViewModel`, deletes the old image if a new one is provided, saves the new image, and persists the changes.

*   **`Task<bool> DeleteEmployeeAsync(int id)`**:
    *   **Description**: Deletes an employee by their ID.
    *   **Business Logic**: Checks if the employee manages any other employees. If so, deletion is prevented. Otherwise, it deletes the employee record and their associated image file.

*   **`Task<IEnumerable<EmployeeViewModel>> GetEmployeesByManagerAsync(int managerId)`**:
    *   **Description**: Retrieves all employees managed by a specific manager.
    *   **Business Logic**: Filters employees based on their `ManagerId`.

*   **`Task<IEnumerable<EmployeeViewModel>> SearchEmployeesAsync(string searchTerm)`**:
    *   **Description**: Searches for employees by first name, last name, or department name.
    *   **Business Logic**: Performs a case-insensitive search across specified fields and returns matching employee view models.

*   **`Task AssignManagerAsync(int employeeId, int managerId)`**:
    *   **Description**: Assigns a manager to a specific employee.
    *   **Business Logic**: Updates the `ManagerId` of the employee.

*   **`Task<string> SaveEmployeeImageAsync(IFormFile image)`**:
    *   **Description**: Saves an uploaded employee image to the server.
    *   **Business Logic**: Validates file size and format, generates a unique filename, saves the image to the `wwwroot/uploads` directory, and returns the relative path.

*   **`Task<EmployeeViewModel> GetManagerEmployeeByUserIdAsync(string userId)`**:
    *   **Description**: Retrieves the employee record for a given user ID, assuming that user is a manager.
    *   **Business Logic**: Fetches the employee associated with the provided `userId`.

#### Design Patterns Implemented:

*   **Repository Pattern**: Similar to `DepartmentService`, `EmployeeService` utilizes `IUnitOfWork` and generic repositories (`IGenericRepository<Employee>`) for data access.
*   **Unit of Work Pattern**: Ensures transactional integrity for employee-related operations.
*   **AutoMapper**: Used for mapping between `EmployeeViewModel` and `Employee` entities.




### ITaskService and TaskService

`ITaskService` defines the contract for task-related operations, and `TaskService` provides its implementation. This service handles the business logic for managing tasks, including creation, assignment, status updates, and retrieval based on various criteria.

#### Key Methods:

*   **`Task<IEnumerable<TaskMB>> GetAllTasksAsync()`**:
    *   **Description**: Retrieves all tasks in the system.
    *   **Business Logic**: Fetches all task records.

*   **`Task<TaskMB> GetTaskByIdAsync(int id)`**:
    *   **Description**: Retrieves a single task by its ID.
    *   **Business Logic**: Fetches a specific task record.

*   **`Task<TaskMB> CreateTaskAsync(TaskMB taskViewModel)`**:
    *   **Description**: Creates a new task and assigns it to an employee.
    *   **Business Logic**: Validates the existence of the assigned employee, maps the `TaskMB` to a `TaskItem` entity, persists the task, and creates an `EmployeeTask` entry to link the task to the employee. It uses a database transaction to ensure atomicity.

*   **`Task<bool> UpdateTaskAsync(TaskMB taskViewModel)`**:
    *   **Description**: Updates an existing task.
    *   **Business Logic**: Retrieves the existing task, updates its properties from the `TaskMB`, and persists the changes.

*   **`Task<bool> DeleteTaskAsync(int id)`**:
    *   **Description**: Deletes a task by its ID.
    *   **Business Logic**: Deletes the task and any associated `EmployeeTask` assignments.

*   **`Task<IEnumerable<TaskMB>> GetTasksByEmployeeAsync(int employeeId)`**:
    *   **Description**: Retrieves all tasks assigned to a specific employee.
    *   **Business Logic**: Queries `EmployeeTask` entries to find tasks assigned to the given employee.

*   **`Task<IEnumerable<TaskMB>> GetTasksByManagerAsync(int managerId)`**:
    *   **Description**: Retrieves all tasks created by a specific manager.
    *   **Business Logic**: Filters tasks based on the `CreatedByManagerId`.

*   **`Task<IEnumerable<TaskMB>> GetTasksByStatusAsync(TaskStatus status)`**:
    *   **Description**: Retrieves tasks based on their current status.
    *   **Business Logic**: Filters tasks by the `Status` enumeration.

*   **`Task<IEnumerable<TaskMB>> GetOverdueTasksAsync()`**:
    *   **Description**: Retrieves all tasks that are overdue.
    *   **Business Logic**: Filters tasks where `DueDate` is in the past and `Status` is not `Completed`.

*   **`Task<bool> UpdateTaskStatusAsync(int taskId, TaskStatus newStatus)`**:
    *   **Description**: Updates the status of a specific task.
    *   **Business Logic**: Changes the `Status` of the task and updates its `UpdateDate`.

*   **`Task AssignTaskToEmployeeAsync(int taskId, int employeeId)`**:
    *   **Description**: Assigns a task to a specific employee.
    *   **Business Logic**: Creates a new `EmployeeTask` entry if the task is not already assigned to the employee.

*   **`Task UnassignTaskFromEmployeeAsync(int taskId, int employeeId)`**:
    *   **Description**: Unassigns a task from a specific employee.
    *   **Business Logic**: Deletes the `EmployeeTask` entry.

*   **`Task<TaskMB> UpdateTaskAssignmentAsync(int taskId, int newEmployeeId)`**:
    *   **Description**: Reassigns a task to a new employee.
    *   **Business Logic**: Updates the `EmployeeId` for a given task and refreshes the task details.

#### Design Patterns Implemented:

*   **Repository Pattern**: `TaskService` interacts with `IUnitOfWork` and generic repositories (`IGenericRepository<TaskItem>`, `IGenericRepository<EmployeeTask>`) for data access.
*   **Unit of Work Pattern**: Ensures transactional integrity for task-related operations, especially during task creation which involves multiple entities.
*   **AutoMapper**: Used for mapping between `TaskMB` and `TaskItem` entities.




## Core Business Logic

The core business logic of the Employee Management System resides primarily within the **Domain Layer** (`EmployeeManagement.Domain`) and is orchestrated by the **Application Layer** (`EmployeeManagement.Application`). This separation ensures that the fundamental rules governing the system's operations are independent of external concerns like databases or user interfaces.

### Key Business Rules and Processes:

1.  **Employee Management**: 
    *   **Employee Uniqueness**: While not explicitly enforced by a unique constraint in the current `Employee` entity, in a production system, email or a unique identifier would typically be used to ensure each employee record is distinct.
    *   **Manager Assignment**: Employees can be assigned a manager. A manager is also an employee. The system supports hierarchical relationships between employees.
    *   **Employee Deletion**: An employee cannot be deleted if they are currently managing other employees. This rule prevents orphaned records and maintains data integrity.
    *   **Image Handling**: Employee profiles can include an image. The system handles the saving, updating, and deletion of these images, ensuring proper file management and validation (e.g., file size, allowed extensions).

2.  **Department Management**: 
    *   **Department Deletion**: A department cannot be deleted if there are employees currently assigned to it. This rule ensures that all employees have an associated department and prevents data inconsistencies.
    *   **Employee Count and Total Salary Calculation**: The system dynamically calculates the number of employees and the total salary within each department. This aggregation is performed at the application service level when retrieving department data, ensuring up-to-date metrics.

3.  **Task Management**: 
    *   **Task Assignment**: Tasks are assigned to specific employees. Managers can create and assign tasks to employees they manage.
    *   **Task Status Workflow**: Tasks progress through various statuses (e.g., Pending, InProgress, Completed). The system allows for updating task statuses.
    *   **Task Reassignment**: Managers can reassign tasks to different employees, but only to employees they manage.
    *   **Task Deletion**: Tasks can be deleted, which also removes their assignments to employees.
    *   **Overdue Task Identification**: The system can identify tasks that are past their due date and are not yet completed.

### Business Entities (Domain Layer):

*   **`Employee`**: Represents an individual employee with properties such as `FirstName`, `LastName`, `Email`, `Salary`, `DepartmentId`, `ManagerId`, and `ImagePath`. It also includes navigation properties to `Department` and `Manager` (self-referencing `Employee`).
*   **`Department`**: Represents an organizational department with properties like `Name`. It includes a navigation property to a collection of `Employees`.
*   **`TaskItem`**: Represents a task with properties such as `Title`, `Description`, `DueDate`, `Status`, `EmployeeId` (assigned employee), and `CreatedByManagerId` (manager who created the task).
*   **`EmployeeTask`**: A many-to-many join entity (though not explicitly used as such in the service layer, it exists in the infrastructure for task assignments) linking `Employee` and `TaskItem`.
*   **`ApplicationUser`**: Extends `IdentityUser` for authentication and authorization purposes, linking to an `Employee` record via `UserId`.

These entities, along with their defined relationships and behaviors, form the core of the system's business domain, ensuring data integrity and adherence to business rules.




## Design Patterns Implemented

The Employee Management System leverages several architectural and design patterns to achieve its goals of maintainability, scalability, and testability. The primary patterns employed are:

### 1. Repository Pattern

*   **Purpose**: The Repository Pattern abstracts the data access logic from the business logic. It provides a clear separation between the domain model and the data access layer, making the application independent of the underlying data storage technology.
*   **Implementation**: In this project, the `EmployeeManagement.Infrastructure` project contains concrete implementations of repositories (e.g., `GenericRepository<T>`, `DepartmentRepository`, `EmployeeRepository`, `TaskRepository`). These repositories implement interfaces defined in `EmployeeManagement.Application.Interfaces` (e.g., `IGenericRepository<T>`, `IDepartmentRepository`, `IEmployeeRepository`, `ITaskRepository`).
*   **Benefits**:
    *   **Decoupling**: The application layer (business logic) does not need to know how data is persisted or retrieved. It interacts with abstract interfaces.
    *   **Testability**: It allows for easier unit testing of business logic by mocking the repository interfaces, without needing a real database connection.
    *   **Maintainability**: Changes to the data access technology (e.g., switching from SQL Server to PostgreSQL) can be made within the repository implementations without affecting the application or domain layers.
    *   **Centralized Data Access Logic**: All data querying and persistence logic for a specific entity is encapsulated in one place.

### 2. Unit of Work Pattern

*   **Purpose**: The Unit of Work Pattern ensures that a business transaction that involves multiple repository operations is treated as a single atomic operation. All changes are committed or rolled back together, maintaining data consistency.
*   **Implementation**: The `IUnitOfWork` interface (in `EmployeeManagement.Application.Interfaces`) and its concrete implementation `UnitOfWork` (in `EmployeeManagement.Infrastructure`) manage the `DbContext` and coordinate multiple repositories. Instead of each repository having its own `DbContext` and `SaveChanges` method, the `UnitOfWork` provides a single `SaveChangesAsync()` method to commit all changes made across various repositories within a single business transaction.
*   **Benefits**:
    *   **Data Consistency**: Guarantees that all operations within a logical transaction succeed or fail together, preventing partial updates.
    *   **Simplified Transaction Management**: Business services don't need to manage database transactions directly; they simply interact with the `UnitOfWork`.
    *   **Reduced Database Roundtrips**: Multiple changes can be batched and committed in a single database operation, improving performance.

### 3. Dependency Injection (DI)

*   **Purpose**: Dependency Injection is a technique where an object receives other objects that it depends on. This promotes loose coupling between components.
*   **Implementation**: ASP.NET Core has built-in support for Dependency Injection. In this project, services (e.g., `IDepartmentService`, `IEmployeeService`, `ITaskService`) and repositories (e.g., `IUnitOfWork`) are registered with the DI container in `Program.cs`. Controllers and services then receive their dependencies through constructor injection.
*   **Benefits**:
    *   **Loose Coupling**: Components are not tightly bound to their implementations, making them easier to change, test, and reuse.
    *   **Testability**: Dependencies can be easily mocked or stubbed during unit testing.
    *   **Maintainability and Scalability**: Facilitates easier refactoring and extension of the codebase.

### 4. AutoMapper

*   **Purpose**: AutoMapper is an object-object mapper. It helps in transforming an input object of one type into an output object of another type. This is particularly useful for mapping between domain entities and ViewModels or DTOs.
*   **Implementation**: AutoMapper is configured in the `EmployeeManagement.Application` project (e.g., in `MappingProfile.cs`). It is used extensively in the service layer to map between `Employee`, `Department`, `TaskItem` entities and their corresponding `ViewModel` representations.
*   **Benefits**:
    *   **Reduced Boilerplate Code**: Eliminates the need to write repetitive mapping code manually.
    *   **Improved Readability**: Makes the code cleaner and more focused on business logic rather than data transformation.
    *   **Maintainability**: If a property name changes, only the mapping configuration needs to be updated, not every place where manual mapping occurs.




## Installation and Running the Project

To set up and run the Employee Management System locally, follow these steps:

### Prerequisites

Before you begin, ensure you have the following installed on your system:

*   **.NET SDK 9.0 (or later)**: The project is built with ASP.NET Core 9. You can download it from [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0).
*   **SQL Server**: The project uses SQL Server for its database. You can use SQL Server Express, Developer Edition, or a full SQL Server installation. [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
*   **SQL Server Management Studio (SSMS)** or **Azure Data Studio**: For managing your SQL Server database. [https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) or [https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
*   **Git**: For cloning the repository. [https://git-scm.com/downloads](https://git-scm.com/downloads)
*   **Visual Studio 2022 (or Visual Studio Code)**: Recommended IDE for development. [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)

### 1. Clone the Repository

Open your terminal or command prompt and run the following command to clone the project:

```bash
git clone https://github.com/MostafaElmarakpy/EmployeeManagement.git
cd EmployeeManagement
```

### 2. Database Setup

#### a. Update Connection String

Open the `appsettings.json` file located in the `EmployeeManagementSystem` project (`EmployeeManagement/EmployeeManagementSystem/appsettings.json`). Update the `DefaultConnection` string to point to your SQL Server instance.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YourServerName;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  // ... other settings
}
```

Replace `YourServerName` with the name of your SQL Server instance (e.g., `.` for localdb, `(localdb)\MSSQLLocalDB`, or your server's IP address/hostname).

#### b. Apply Migrations

Navigate to the `EmployeeManagementSystem` directory in your terminal:

```bash
cd EmployeeManagement/EmployeeManagementSystem
```

Run the following .NET Entity Framework Core commands to create the database and apply migrations:

```bash
dotnet ef database update
```

This command will create the `EmployeeManagementDb` database (if it doesn't exist) and apply all pending migrations, setting up the necessary tables and initial data.

### 3. Run the Application

After setting up the database, you can run the application from the `EmployeeManagementSystem` directory:

```bash
dotnet run
```

The application will start, and you will see output similar to this:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\path\to\EmployeeManagement\EmployeeManagementSystem
```

Open your web browser and navigate to the URL provided (e.g., `https://localhost:7000` or `http://localhost:5000`).

## Usage Instructions

Upon running the application, you will be greeted with the Employee Management System interface. Here's how to interact with the system:

### User Roles

The system supports different user roles with varying permissions:

*   **Admin**: Full access to manage employees, departments, and tasks. Can create, edit, and delete all records.
*   **Manager**: Can manage employees and tasks within their scope (e.g., assign tasks to employees they manage).
*   **Employee**: Can view their own tasks and potentially update their own profile.

### Authentication

*   **Register**: New users can register for an account. During registration, you might be prompted to assign a role (e.g., Admin, Manager, Employee) if the application is configured for it, or roles might be assigned by an administrator post-registration.
*   **Login**: Use your registered credentials to log in.

### Navigation

*   **Dashboard/Home**: Provides an overview of the system.
*   **Employees**: Manage employee records (CRUD operations).
*   **Departments**: Manage department records (CRUD operations).
*   **Tasks**: Manage tasks, assign them to employees, and update their statuses.
*   **My Tasks**: (For employees) View tasks assigned to them.

### Interacting with Data (CRUD Operations)

Most sections (Employees, Departments, Tasks) provide a consistent interface for performing CRUD operations:

*   **Create**: Click the "Create New" or similar button to add a new record. A modal form will typically appear.
*   **Edit**: Click the "Edit" button next to a record to modify its details. A modal form pre-filled with existing data will appear.
*   **Details**: Click the "Details" button to view comprehensive information about a record.
*   **Delete**: Click the "Delete" button to remove a record. A confirmation modal will appear to prevent accidental deletions.

### API Endpoints (for developers)

As documented in the [API Endpoints](#api-endpoints) section, the system exposes various endpoints for programmatic interaction. You can use tools like Postman or curl to test these endpoints.

For example, to get all departments:

```bash
curl -X GET https://localhost:7000/Departments/Index
```

(Note: Replace `https://localhost:7000` with the actual URL your application is running on.)




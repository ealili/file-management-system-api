### File System Management API built in .NET 8

#### API's intented use:
- Retrieve .pdf files from a given directory
- Store file information as a record in a sql database
- Marks the file status as "Imported"
- Move them in a given (destination) directory
- Upon successful move, the file status gets updated to "Moved"
- Powers Vue frontend which provides a nice user interface where the user can log in and view the imported/moved files, and have the power to update the file status as desired.

##### Additional features:
- Provides JWT authentication
- Global Error Handling

#### In order to start using the API the following are required:

1. Update the appsettings.json and to your configuration settings.
2. Update database using the IntialMigration
    ```
    dotnet ef database update
    ```
3. Run the project using
    ```
    dotnet run
    ```
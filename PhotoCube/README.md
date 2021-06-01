# PhotoCube as a Competitor in the Lifelog Search Challenge
## Info:
This PhotoCube Client and Server implementations are  developed for research purposes by Jihye Shin and Alexandra Waldau as part of their MSc. thesis in Software Design at the IT University of Copenhagen.

This initial PhotoCube implementation was written by Peter Øvergård Clausen, and his repository is [https://github.com/PeterOeClausen/Thesis](https://github.com/PeterOeClausen/Thesis). This document is based on Peter's installation guide. Future changes to the implementation will happen in the ITU-PITLab/PhotoCube repository.

The PhotoCube uses the M^3 data model, initially proposed by Björn Thór Jónsson. It is further developed here to meet support participating in the Lifelog Search Challenge (LSC).

The PhotoCube Client is developed in React using Typescript. It also uses the ThreeJS library for 3D rendering.

The PhotoCube Server is developed in C# in the .NET CORE framework, using Entity Framework CORE to communicate with an SQL database when running. The initial data insertion is done by running SQL script. It also uses Newtonsoft's Json.NET framework to serialize and parse JSON.

## Prerequisites:
On a Windows PC:

Tip: Hold ctrl when clicking on the links to open them in a new tab:
* Download and install [Visual Studio IDE (Community edition is free)](https://visualstudio.microsoft.com/vs/) (Required for running and developing server).
* Download and install [Visual Studio Code IDE](https://code.visualstudio.com/) (Recommended for client development).
* Download and install [Node](https://nodejs.org/en/) (Required for React client).
* Download and install [SQL Server (Express and Developer editions are free, scroll down a bit)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Required for running development server) **REMEMBER TO CHECK LOCALDB DURING INSTALLATION**.
* Download and install the Google Chrome Browser or Edge to use the client application with. (Mozilla and other browsers are not yet supported).

Please restart your computer after installing the frameworks, before trying out the code.

Note: PhotoCube also runs on MacOS using Postgres. The *DatasetInsertSQLGenerator.cs* in Installation step 2 supports both Mac and Windows.

## Download the code:
Either clone this repository or download it as a zip file with the green button on the top-right of this page.

## Download the dataset:
Ask Björn Thór Jónsson for the LSC dataset: [bjth@itu.dk](mailto:bjth@itu.dk).

The files needed are:
  - ImageTags file in csv format, containing file paths of images and the associated tags
  - Hierarchy file in json format, containing the json tree of tags

## Installing and running the server:
### Step 0: Open the ObjectCubeServer solution file in Visual Studio:
Open the *ObjectCubeServer.sln* solution file in Visual Studio. This can be found in the *Server/ObjectCubeServer/* directory.

If Visual Studio says that you need to download and install extensions to make it work, please do so.

### Step 1: Enter a connection-string:

Add a connection-string to your SQL database in the file: *ObjectCubeServer/Models/Contexts/ObjectContext.cs* around line 183. Eg:
```
optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = ObjectData; Trusted_Connection = True; AttachDbFileName=C:\\Databases\\ObjectDB.mdf");
```

Note that the connection-string identifies the Server, this is usually "Server = (localdb)\\mssqllocaldb;", the name of the database: "Database = ObjectData;", that it's a trusted connection: "Trusted_Connection = True;" and the path to the database file (.mdf) "AttachDbFileName=C:\\Databases\\ObjectDB.mdf". The ObjectDB.mdf file will be created later with the command "Update-Database".

Also note that the directory *C:\\Databases* needs to exist.

### Step 2: Enter the path to the LSC Dataset on your computer:
Please create an *App.Config* file under *Server\ObjectCubeServer* and specify the path to the LSC dataset and other required files on your computer. These file paths are needed to run *ConsoleAppForInteractingWithDatabase/DatasetInsertSQLGenerator.cs*. The content of the file is like this, where you replace the `value`s to your own paths. 

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>        
    <add key="pathToLscData" value="C:\lsc2020" />        
    <add key="LscErrorfilePath" value="ErrorLogFiles\FileLoadError.txt" />        
    <add key="LscTagFilePath" value="tags-and-hierarchies\210525_lscImageTags.csv" />
    <add key="LscJsonHierarchyPath" value="C:\lsc2020\tags-and-hierarchies\210525_lsc_hierarchy_unique_clean_tagname.json" />
    <add key="resultPath" value="C:\LSCImportExp\210526_Result_LSCWhole100K.csv" />        
    <add key="SQLPath" value="C:\LSCImportExp\210526_SQLInsertQueries_LSCWhole100K.sql" />
    </appSettings>
</configuration>
```

### Step 3: Rebuild the solution to download missing NuGet packages
Then we will compile the applications by right-clicking the Solution in the Solution Explorer and click *Rebuild Solution*. This will download and install all the NuGet packages needed:

![Rebuild%20solution.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/Rebuild%20solution.png)

### Step 4: Create the database schema migrations and run the migrations against the server:
If the folder *Migrations* exists in the ObjectCubeServer project, please right-click and delete it. We will generate it again next:

Open the Package Manager Console (tip: You can search for it in the upper right corner):

![PackageManagerConsole.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/PackageManagerConsole.png)

Be sure that Default Project is set to ObjectCubeServer, and that ObjectCubeServer is selected as StartUp Project:

![ObjectCubeServerDefaultProject.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/ObjectCubeServerDefaultProject.png)

Run:
```
Add-Migration init
```
To create the Migrations folder.

Then run:
```
Update-Database
```
To apply the migration to the database. This will create the database on the server.

### Step 5: Populate the server with data:

#### 1) Generate the SQL script
To populate the database with data, right-click the ConsoleAppForInteractingWithDatabase and select 'Set as StartUp Project'.
Then run the application by pressing the *Play* button in the top of Visual Studio to start the ConsoleAppForInteractingWithDatabase program:

![RunConsoleAppForInteractingWithDatabase.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/RunConsoleAppForInteractingWithDatabase.png)

When the Console Application says "Press any key to shut down." the database is populated. This process usually takes around 30 minutes. Leave it running until it displays the insertion completed message as below.

![InsertionComplete.jpg](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/InsertComplete.jpg)

#### 2) Run the SQL script

The generated SQL script needs to be run on a database. On the Command Prompt, run the command below. It takes around 30 minutes.

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -d {database name} -i {filepath}
```

### Step 6: Run the server:

You can now run the server by right-clicking the ObjectServer project, select 'Set as StartUp Project' and then run the application by pressing the play button in the top of Visual Studio.

## Installing and running the client:
Note that when running these commands, it may give "WARN" and "notice" messages, however you can ignore these and keep going with the installation.

### Step 1: Install React:
After installing Node and restarting, open cmd and install React:
```
npm install react
```
### Step 2: Install the project:
Navigate to the *Client/* directory and run:
```
npm install
```
This will download and install all required packages.

Tip: You can start the cmd in the current directory if you write "cmd" and press enter in FileExplorer, though this might not work in a simulated environment, then you may have to use "d:" to change to the d drive, and then "cd {yourPath}":

![OpenCmdInCurrentPath.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/OpenCmdInCurrentPath.png)

### Step 3: Run the client:
Run the client with the command:
```
npm start
```
A browser tab should open automatically with the client application.

## Other (needed if you are going to make changes to the database or downloaded npm packages on a seperate computer):
If you need to delete the data in the database, run:
```
Drop-Database
```
in the package manager console. This will delete the database.

If you have made changes to the DB Schema, I recommend deleting the *Migrations* directory, running 'Drop-Database' followed by
```
Add-Migration init
```
in the Package Manager Console. This will recreate the Migrations directory and create the Migrations.
Lastly, update the schema by running 'Update-Database'.

You can apply changes to the database schema with the command:
```
Update-Database
```

If you have worked on the client application on one computer, and installed packages using npm, install these packages by running 'npm install' before running 'npm start'.

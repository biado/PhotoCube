# PhotoCube as a Competitor in the Lifelog Search Challenge

## Info:
This PhotoCube Client and Server implementations are  developed for research purposes by Jihye Shin and Alexandra Waldau as part of their MSc. thesis in Software Design at the IT University of Copenhagen.

This initial PhotoCube implementation was written by Peter Øvergård Clausen, and his repository is [https://github.com/PeterOeClausen/Thesis](https://github.com/PeterOeClausen/Thesis). This document is based on Peter's installation guide. Future changes to the implementation will happen in the ITU-PITLab/PhotoCube repository.

The PhotoCube uses the M^3 data model, initially proposed by Björn Thór Jónsson. It is further developed here to meet support participating in the Lifelog Search Challenge (LSC).

The PhotoCube Client is developed in React using Typescript. It also uses the ThreeJS library for 3D rendering.

The PhotoCube Server is developed in C# in the .NET CORE framework, using Entity Framework CORE to communicate with an SQL database when running. The initial data insertion is done by running SQL script. It also uses Newtonsoft's Json.NET framework to serialize and parse JSON.

This document only covers the case of using Postgres database, as MSSQL did not support some syntax for denormalization steps. However PhotoCube was developed using MSSQL database. Check Peter's repository to see the different version of the installation guide.

This README file covers the overall setup process of PhotoCube:
1. Downloads
2. Server Installation
3. Client Installation
4. Database Tuning (Denormalization)

## 1. Downloads

### Download the prerequisites:
Tip: Hold ctrl when clicking on the links to open them in a new tab:
* Download and install [Visual Studio IDE (Community edition is free)](https://visualstudio.microsoft.com/vs/) (Required for running and developing server).
* Download and install [Visual Studio Code IDE](https://code.visualstudio.com/) (Recommended for client development).
* Download and install [Node](https://nodejs.org/en/) (Required for React client).
* Download and install [PostgreSQL](https://www.postgresql.org/download/)
* Download and install the Google Chrome Browser or Edge to use the client application with. (Mozilla and other browsers are not yet supported).

Additionally for MacOS:
* Download .NET Core SDK 2.1 for macOS, and place it in the same folder as the existing SDK. Link: https://dotnet.microsoft.com/download/dotnet/2.1

Please restart your computer after installing the frameworks, before trying out the code.

### Download the code:
Either clone this repository or download it as a zip file with the green button on the top-right of this page.

### Download the dataset:
Ask Björn Thór Jónsson for the LSC dataset: [bjth@itu.dk](mailto:bjth@itu.dk).

The files needed are:
  - To generate data insertion script by yourself (using the server's *ConsoleAppForInteractingWithDatabase* project):
    - ImageTags file in csv format, containing file paths of images and the associated tags
    - Hierarchy file in json format, containing the json tree of tags
    - (Or you can ask for the resulting file (PSQL1.sql))
  - For loading and tuning database:
    - lscDDL.sql (Defines the database structure)
	- PSQL1.sql (Inserts all LSC data to the database)
	- tuning-pre.sql (Denormalization)
	- tuning-post.sql (In case you update the data rows, run this afterwards)

## 2. Server Installation

### Step 0: Open the ObjectCubeServer solution file in Visual Studio:
Open the *ObjectCubeServer.sln* solution file in Visual Studio. This can be found in the *Server/ObjectCubeServer/* directory.

If Visual Studio says that you need to download and install extensions to make it work, please do so.

### Step 1: Enter a connection-string:
Add a connection-string to your database in the file: *ObjectCubeServer/Models/Contexts/ObjectContext.cs* switch statement around line 163. Eg:
```
case "DESKTOP-123456": // Put your computer name
    if (connectionString != null)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
    else
    {
		optionsBuilder.UseNpgsql("Server = localhost; Port = 5432; Database = PC; User Id = photocube; Password = postgres;"); // put your connection-string
    }
    break;
```

Guide on how to find your computer name: https://it.umn.edu/services-technologies/how-tos/find-your-computer-name

### Step 2: Enter the path to the LSC Dataset on your computer:
**Note: This step is needed only if you are generating SQL script to insert the data to the database (i.e. running Step 5-1). You can skip this part unless you change the data mapping.**

Please create a file named *App.Config* under *Server\ObjectCubeServer* and specify the path to the LSC dataset and other required files on your computer. These file paths are needed to run *ConsoleAppForInteractingWithDatabase/DatasetInsertSQLGenerator.cs*. The content of the file is something like below, where you replace the `value`s to your own paths. 

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

### Step 4: Create the database schema
First, run PostgresSQL locally and create a new database.

Then, run below command on the Command Prompt (within the folder the sql file is located), to create the schema on the database.
```
psql -q -U {user name} {database name} < lscDDL.sql

//Example:
psql -q -U postgres PC < lscDDL.sql
```


### Step 5: Populate the server with data:

#### 1) Generate the SQL script
**Note: This step is needed only if you are generating SQL script to insert the data to the database. You can skip this part unless you change the data mapping.**
To populate the database with data, right-click the ConsoleAppForInteractingWithDatabase and select 'Set as StartUp Project'.
Then run the application by pressing the *Play* button in the top of Visual Studio to start the ConsoleAppForInteractingWithDatabase program:

![RunConsoleAppForInteractingWithDatabase.png](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/RunConsoleAppForInteractingWithDatabase.png)

When the Console Application says "Press any key to shut down." the database is populated. This process usually takes around 30 minutes. Leave it running until it displays the insertion completed message as below.

![InsertionComplete.jpg](https://github.itu.dk/jish/Thesis/blob/master/PhotoCube/userManualImages/InstallationManualImages/InsertComplete.jpg)

#### 2) Run the SQL script
The generated SQL script needs to be run on a database. On the Command Prompt (within the folder the sql file is located), run the command below. It takes around 30 minutes.

```
psql -q -U {user name} {database name} < PSQL1.sql

//Example:
psql -q -U postgres PC < PSQL1.sql
```

### Step 6: Run the server:
You can now run the server by right-clicking the ObjectServer project, select 'Set as StartUp Project' and then run the application by pressing the play button in the top of Visual Studio.

## 3. Client Installation
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

### Step 3: Create `.env` file with the correct port number:
Please create a file named *.env* under *PhotoCube\Client* and specify the server address with the correct port number on your computer. The content of the file is as below, where you replace the value for the secone entry, `REACT_APP_BASE_URL`. Put 44317 for Windows OS, and {????} for MacOS.

```
REACT_APP_IMAGE_SERVER = http://bjth.itu.dk:5002/
REACT_APP_BASE_URL = https://localhost:44317/api
```

### Step 4: Run the client:
Run the client with the command:
```
npm start
```
A browser tab should open automatically with the client application.

## 4. Database Tuning (Denormalization)
Run below command on the Command Prompt (within the folder the sql file is located), to set up denormalization.
```
psql -U {user name} {database name} < tuning-pre.sql

//Example:
psql -U postgres PC < tuning-pre.sql
```

If you update the data rows, you need to update the denormalized views by running the command below.
```
psql -q -U {user name} {database name} < tuning-post.sql

//Example:
psql -q -U postgres PC < tuning-post.sql
```
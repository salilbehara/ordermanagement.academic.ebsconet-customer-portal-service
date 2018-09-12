# Customer Service Portal API
The Customer Service Portal API provides work item tracker data, including work item details, attached files, and user comments. It also allows clients to POST work items, files, and user comments. The API is designed to be used by the Customer Service Portal micro UI and any other client applications that need to send and receive work item tracker data.

The API is built on ASP.NET Core 2.0.

## Features
Since all data available via the API is dependent on a work item ID, the API has a single root-level route: /workitems. All other routes are children of /workitems. The available routes are:
* /workitems?customerCode={customerCode}
* /workitems/{workItemId}
* /workitems/{workItemId}/files
* /workitems/{workItemId}/files/{workItemFileId}
* /workitems/{workItemId}/comments/
* /workitems/{workItemId}/comments/{workItemCommentId}

For all Customer Service Portal API information, the Swagger documentation should be considered authoritative. This documentation is available in the dev environment at http://dev1.customerservice.ebsconet.com:8080. (Swagger documentation is generated using the [Swashbuckle package](https://www.nuget.org/packages/Swashbuckle/), which auto-detects API metadata from the source code and can also read the contents of XML comments to produce additional documentation.)

## Development

### Dependencies
To work on the API, you'll need the following on your local machine:
* [.NET Core 2.0.9 or greater](https://www.microsoft.com/net/download/dotnet-core/2.0 ".NET Core 2.0")
* [Visual Studio Code](https://code.visualstudio.com/ "Visual Studio Code") or [Visual Studio (full version)](https://visualstudio.microsoft.com/vs/community/ "Visual Studio (full version)")
* [Postman ](https://www.getpostman.com/apps) (for testing the API)

### Building the Solution
#### Visual Studio
You can use the build command in Visual Studio as you would with a .NET Framework app.

#### Visual Studio Code
On the integrated command line (CTRL + ~), run the following from the root directory:
`dotnet build src`

### Testing
Unit tests are writing using the [XUnit](https://xunit.github.io/) testing framework and [Moq](https://github.com/Moq/moq4/wiki/Quickstart) (for mocking dependencies).

#### Visual Studio
Visual Studio 2017 and newer [should automatically detect XUnit tests](http://xunit.github.io/docs/getting-started-desktop#run-tests-visualstudio) without the need for an additional plugin.  You can run tests from the Test Explorer window (available in __Tests > Windows > Test Explorer__).

#### Visual Studio Code
On the integrated command line (CTRL + ~), run the following:
`dotnet test src`

This will detect and run all unit tests in the solution. Unfortunately, [dotnet test currently throws an error when it encounters a project that is not a test project](https://github.com/Microsoft/vstest/issues/1129) (i.e., a project that doesn't have the XUnit test runner NuGet package listed in its .csproj file). However, the tests still run.

Alternately, you can run `dotnet test` at the .csproj level instead. From the root directory, run:
`dotnet test src/ebsco.svc.customerserviceportal.test/ebsco.svc.customerserviceportal.test.csproj`

### Running the Application
#### In Visual Studio
You can run the API by clicking the __Run__ button in Visual Studio or by pressing F5. The API will be launched on your localhost.

#### In Visual Studio Code
On the integrated command line (CTRL + ~), run the following:
`dotnet run --project src\ebsco.svc.customerserviceportal\ebsco.svc.customerserviceportal.csproj`

The API will be launched on your localhost.

#### With Docker
You can run the API in Docker (and test the pipeline's Dockerfile) by running Docker for Windows on your local machine. To install Docker:

##### Installing Docker for Windows
You can install manually from https://store.docker.com/editions/community/docker-ce-desktop-windows, but you have to create a user account.

Alternately, you can use the chocolatey package manager for Windows, which allows Linux-style command-line installation of various development packages
* Install Chocolatey by following the steps [here](https://chocolatey.org/docs/installation).
* When chocolatey is installed, open a command prompt as admin, and run ```choco install docker-for-windows --pre -y```
* Once Docker is installed, you may have to log out of your machine and log back in before docker commands are available from the command line.
* Docker may also have to change settings in order to allow containerization, which may require a reboot.

##### Enabling Virtualization on Your Local Machine
You may have to enable virtualization on your machine. If you have an HP EliteBook, follow these steps:
* Restart your machine.
* On the HP loading machine, press ESC.
* From the startup menu, press F10 to modify BIO settings.
* Click __Advanced__.
* Click __System Options__.
* Select the following check boxes: __Virtualization Technology (VTx)__ and __Virtualization Technology for Directed I/O (VTd)__.
* Save your changes, and exit.

##### Running the API in a Docker Container
On the integrated command line (CTRL + ~), run the following:
`docker build --tag name:local -f pipeline/JenkinsBuild.dockerfile --build-arg bin_dir=published .`

## Live Locations
The API has been deployed to the following environments.

* [Dev1](http://dev1.customerservice.ebsconet.com:8080)
* QA (TBD)
* Mock (TBD)
* Production (TBD)

## Configuration
Configuration is managed in __src/ebsco.svc.customerserviceportal/appsettings.{environmentName}.json__. For your local environment, appsettings.Development.json is used. The most important parameters for local development are:
* __WorkItemTrackerService_URL__: the location of the Work Item Tracker SOAP service (dependency).
* __Media Server Service_URL__: the location of the Media Server SOAP service (dependency).

## CI/CD
The API has a [Jenkins CI/CD pipeline](http://idc-v-apjenk01.ebsco.com:8080/job/ordermanagement.academic.ebsconet-customer-portal-service/) for build, test, and deployment. The Jenkinsfile in the root directory contains the Jenkins script. 

A Docker container is used to build the solution and run its unit tests. PowerShell scripts are executed in target environments to make sure they are properly configured to support the API. See the pipeline folder for more information.

## Service Dependencies
* __[Work Item Tracker Service](https://github.com/EBSCOIS/ordermanagement.shared.service.workitemtracker)__: a SOAP service that provides work item tracker data.
* __[Media Server Service](http://cithqaptfs01p:8080/tfs/ISS%20Development/Business%20Systems%20Development/_versionControl?path=%24%2FCore%20Services%2FServices%2Febsco.svc.mediaserver&version=&_a=contents)__: a SOAP service that provides files from the EBSCO media server. The API calls this service to retrieve and store work item attachments.

## Reference Material
* [Yammer Channel](https://www.yammer.com/ebsco.com/#/threads/inGroup?type=in_group&feedId=15562019)

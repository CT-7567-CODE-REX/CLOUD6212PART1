# CoffeeNChill Canteen Management System – Part 1

## Project Overview

CoffeeNChill is a cloud-enabled canteen management system built with Azure Functions.

The system provides HTTP endpoints for:

- Managing menu items using Azure Table Storage
- Uploading, listing and downloading staff documents using Azure Blob Storage
- Uploading and retrieving an assignment file using Azure Blob Storage
- Running Azure Storage locally using Azurite
- Running the Azure Functions application in Docker
- Testing all endpoints using Postman

## Technologies Used

- Visual Studio 2022
- .NET 9
- Azure Functions v4
- Azure Table Storage
- Azure Blob Storage
- Azure.Storage.Blobs
- Azure.Data.Tables
- Azurite
- Docker
- Postman
- Git and GitHub
- Docker Hub

## Storage

### Menu Storage

Menu items are stored in the Azure Table:

```text
MenuItems

The menu item category is stored as the PartitionKey and the menu item ID/SKU is stored as the RowKey.

Azurite is used to emulate Azure Table Storage locally.

Staff Document Storage

Staff documents are stored in the Azurite Blob container:

staff-docs

The container stores files such as:

Barista-Recipe.pdf
Assignment Storage

Assignment files are stored in the Azurite Blob container:

assignments

Assignment files are stored using the student number and file name.

Example:

ST10358794/CLOUD6212_Part1_Blob_Demonstration.pdf
API Endpoints
Menu
Method	Endpoint
POST	/api/menu
GET	/api/menu
GET	/api/menu/category/{category}
PUT	/api/menu/{category}/{id}
DELETE	/api/menu/{category}/{id}
Staff Documents
Method	Endpoint
POST	/api/documents/upload
GET	/api/documents
GET	/api/documents/download/{fileName}

The staff document upload endpoint uses multipart/form-data.

The form-data key is:

file
Assignment File
Method	Endpoint
POST	/api/Uploadassignmentfile
GET	/api/Getassignmentfile

The assignment endpoints use the following query parameters:

studentNumber
fileName

Example upload URL:

http://localhost:7071/api/Uploadassignmentfile?studentNumber=ST10358794&fileName=CLOUD6212_Part1_Blob_Demonstration.pdf

The assignment file is sent through Postman using:

Body → binary

Example retrieval URL:

http://localhost:7071/api/Getassignmentfile?studentNumber=ST10358794&fileName=CLOUD6212_Part1_Blob_Demonstration.pdf
Prerequisites

Install:

Visual Studio 2022
.NET 9 SDK
Azure Functions Core Tools
Docker Desktop
Postman
Git
Azure Storage Explorer
Run Azurite

Pull the official Azurite image:

docker pull mcr.microsoft.com/azure-storage/azurite

Run Azurite:

docker run -d --name coffeenchill-azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

Azurite uses:

Service	Port
Blob	10000
Queue	10001
Table	10002
Build the Functions Docker Image

Navigate to the Azure Functions project folder and run:

docker build -t coffeenchill-functions:v1.0 .
Docker Network

Create the network:

docker network create coffeenchill-network

Connect Azurite:

docker network connect coffeenchill-network coffeenchill-azurite

The shared Docker network allows the Functions container to communicate with the Azurite container.

Run the Functions Container

The Functions container requires the following configuration:

AzureWebJobsStorage
MenuStorageConnection
StaffDocumentsConnection
FUNCTIONS_WORKER_RUNTIME

The Azurite connection must reference the Azurite container name when running inside Docker.

Example:

docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 -e "AzureWebJobsStorage=AZURITE_CONNECTION" -e "MenuStorageConnection=AZURITE_CONNECTION" -e "StaffDocumentsConnection=AZURITE_CONNECTION" -e "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated" coffeenchill-functions:v1.0

The API is available at:

http://localhost:7071
Postman Testing

Postman was used to test:

Menu creation
Menu retrieval
Category filtering
Menu updates
Menu deletion
Duplicate and missing menu records
Staff document upload
Staff document listing
Staff document download
Missing staff document handling
Assignment file upload
Assignment file retrieval
Missing assignment handling

The final Postman collection is included in the repository.

Docker Hub
Functions Image
captainrex04/coffeenchill-functions:v1.0

https://hub.docker.com/r/captainrex04/coffeenchill-functions

Azurite Image
captainrex04/coffeenchill-azurite:v1.0

https://hub.docker.com/r/captainrex04/coffeenchill-azurite



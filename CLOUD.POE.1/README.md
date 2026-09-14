\# CoffeeNChill Canteen Management System – Part 1



\## Project Overview



CoffeeNChill is a cloud-enabled canteen management system built with Azure Functions.



The system provides HTTP endpoints for:



\- Managing menu items using Azure Table Storage

\- Uploading, listing and downloading staff documents using Azure Files

\- Running local Azure Storage using Azurite

\- Running the Azure Functions application in Docker

\- Testing all endpoints using Postman



\## Technologies Used



\- Visual Studio 2022

\- .NET 9

\- Azure Functions v4

\- Azure Table Storage

\- Azure Files

\- Azurite

\- Docker

\- Postman

\- Git and GitHub

\- Docker Hub



\## Storage



Menu items are stored in the:



```text

MenuItems



Azure Table.



Staff documents are stored in the Azure File Share:



staff-docs



Azurite is used locally for Azure Table Storage.



Azure Files is accessed through a real Azure Storage Account because Azurite does not emulate Azure File Shares.



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

Prerequisites



Install:



Visual Studio 2022

.NET 9 SDK

Azure Functions Core Tools

Docker Desktop

Postman

Git



An Azure Storage Account with a File Share named staff-docs is required.



Run Azurite



Pull the image:



docker pull mcr.microsoft.com/azure-storage/azurite



Run Azurite:



docker run -d --name coffeenchill-azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

Build the Functions Docker Image



From the Azure Functions project folder:



docker build -t coffeenchill-functions:v1.0 .

Docker Network



Create the network:



docker network create coffeenchill-network



Connect Azurite:



docker network connect coffeenchill-network coffeenchill-azurite

Run the Functions Container



The Functions container requires:



AzureWebJobsStorage

MenuStorageConnection

StaffDocumentsConnection

FUNCTIONS\_WORKER\_RUNTIME



Example:



docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 -e "AzureWebJobsStorage=AZURITE\_CONNECTION" -e "MenuStorageConnection=AZURITE\_CONNECTION" -e "StaffDocumentsConnection=AZURE\_FILES\_CONNECTION" -e "FUNCTIONS\_WORKER\_RUNTIME=dotnet-isolated" coffeenchill-functions:v1.0



The API is then available at:



http://localhost:7071

Postman Testing



Postman was used to test:



Menu creation

Menu retrieval

Category filtering

Menu updates

Menu deletion

Staff document upload

Staff document listing

Staff document download

Error responses



The Postman collection is included in the repository.



Docker Hub



Functions image:



captainrex04/coffeenchill-functions:v1.0



https://hub.docker.com/r/captainrex04/coffeenchill-functions



Azurite image:



captainrex04/coffeenchill-azurite:v1.0



https://hub.docker.com/r/captainrex04/coffeenchill-azurite



GitHub Repository



https://github.com/CT-7567-CODE-REX/CLOUD6212PART1



Team Contribution

Captain Rex



Responsible for:



Azure Functions implementation

Azure Table Storage integration

Azure Files integration

Postman testing

Azurite configuration

Docker configuration

Docker Hub publishing

GitHub source control

Documentation

Security



Sensitive files such as local.settings.json and .env are excluded from GitHub.



Azure Storage connection strings, passwords and access tokens are not stored in the repository.



Demonstration Video



YouTube link will be added after the final demonstration video is uploaded.


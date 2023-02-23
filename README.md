# MoneyPlus-Backend
## What is this?
Moneyplus-backend is a part of the MoneyPlus project which was developed for master's thesis written at Poznan University of Technology. This project is a mobile application which allows for controlling finances.

## How it works?
The project is divided into a frontend and a backend. This repository contains an API server based on REST architecture, which handles clients requests using HTTP protocol.

## How to test it?
If you want to test the API, you have to have an application which allows for creating HTTP requests. For this purpose you can use for example [Postman](https://www.postman.com/downloads) or a dedicated frontend application. The fontend application and all the informations about how to run it you can find [here](https://github.com/alicja-mruk/moneyplus-mobile).
- **If you decide to run a dedicated application:**
<br>Congratulations! Application is already integrated with API and all requests which will be sent will be handled by the server.

- **If you decide to use external application for testing API like Postman:**
  <br>This is where more possibilities arise, as you can test the server both globally and locally.
  
  - **If you choose the global version:**
		<br>To do this, you need to connect to the server, which is available at [this](https://moneyplusbackend.azurewebsites.net) adress. At this link you will also find simplified documentation of the available endpoints, and its more detailed version can be found [here](https://moneyplusbackend.azurewebsites.net/swagger/index.html).
  
  - **If you choose the local version:**
		<br>To run the server locally, you will need a runtime environment, which in this case will be Visual Studio 2022 along with the .NET framework in version 6. You will then need to download the repository and, after extracting the project, open it with the installed IDE.

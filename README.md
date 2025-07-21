# OLLAMACHAT

## Project Overview

OLLAMACHAT is a web application that allows users to chat with various Large Language Models (LLMs). It's built with ASP.NET Core and features a clean, modern architecture. The application supports multiple LLMs and uses a local SQLite database to store chat history.

## Features

*   **Chat Interface:** A simple and intuitive web interface for chatting with LLMs.
*   **Multi-Model Support:** Supports multiple LLMs, including models from OpenAI.
*   **Chat History:** Stores chat history in a local SQLite database.
*   **Background Job Processing:** Uses Hangfire for background job processing.
*   **API Documentation:** Includes Swagger for API documentation.

## Technologies

*   **.NET 7**
*   **ASP.NET Core**
*   **Entity Framework Core**
*   **SQLite**
*   **Hangfire**
*   **Swagger**
*   **Razor Pages**
*   **Minimal APIs**
*   **Mediator**

## Configuration

The application's configuration is stored in `appsettings.json`. The following settings can be configured:

*   **`Urls`:** The URL the application will run on.
*   **`ConnectionStrings:OLLAMACHAT`:** The connection string for the SQLite database.
*   **`OpenAISettings:ApiKey`:** Your OpenAI API key.
*   **`OpenAISettings:Models`:** A list of LLMs to make available in the application.

## How to Run

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Prikalel/OLLAMACHAT.git
    ```
2.  **Navigate to the web project directory:**
    ```bash
    cd OLLAMACHAT/Source/OLLAMACHAT.Web
    ```
3.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
4.  **Update the `appsettings.json` file:**
    *   Set your OpenAI API key in `OpenAISettings:ApiKey`.
5.  **Run the application:**
    ```bash
    dotnet run
    ```
6.  **Open your browser and navigate to the URL specified in `appsettings.json` (default is `http://localhost:5001`).**

# Online Betting Data Capture and Analytics System

## Setup Instructions

### 1. Database Setup

1. Ensure you have SQL Server 2022 Developer Edition installed.
2. Open SQL Server Management Studio (SSMS) or your preferred SQL client.
3. Connect to your local SQL Server instance.
4. Open the `DatabaseGenerate.sql` file located in the directory(/OBDC.Infrastructure/Scripts/DatabaseGenerate.sql) of the project.
5. Execute the script to create the necessary database, tables, and stored procedures.

### 2. RabbitMQ Setup

1. Install RabbitMQ:
For Windows:
a. Install Erlang: Download and install from https://www.erlang.org/downloads
b. Install RabbitMQ: Download and install from https://www.rabbitmq.com/install-windows.html

2. Enable the RabbitMQ Management plugin as an administrator by running below command:
rabbitmq-plugins enable rabbitmq_management

3. Open a web browser and navigate to http://localhost:15672/
4. Log in with the default credentials:
- Username: guest
- Password: guest
5. Create a new virtual host:
- Go to "Admin" > "Virtual Hosts"

6. Create the queue:
- Go to "Queues"
- Add a new queue with the following details:
  - Name: casino_wagers
  - Durability: Durable
  - Auto delete: No
7. Update your application configuration:
In your appsettings.json file, amend the following to match your RabbitMQ configs.
If you have a different user and virtual host, update the values accordingly.

  "RabbitMQSettings": {
    "Hostname": "localhost",
    "Username": "guest",
    "Password": "guest",
    "QueueName": "casino_wagers"
  }

### 3. Application Configuration

1. Open the `appsettings.json` file in both the API and Service projects.
2. Update the connection string under "ConnectionStrings" > "DefaultConnection" to match your SQL Server instance:
```json

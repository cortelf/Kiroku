# Kiroku

## Project Overview

Kiroku is a small .NET backend that allows you to conveniently store your journals. The main goal of the project is to provide a user-friendly and resource-efficient functionality for journaling. After recording, you can view the entries by connecting Grafana or another dashboard that enables SQL query visualization.

## How It Works

Kiroku uses a PostgreSQL database to save journal entries. To improve data storage efficiency, table partitioning by the log creation date is used. `Kiroku.PartitionWorker` automatically manages partitions in the background, creating new ones when needed and deleting those older than 7 days. The structure of the logs table in PostgreSQL is as follows:

```sql
CREATE TABLE logs (
    id serial NOT NULL,
    level log_level NOT NULL,
    project_id text NOT NULL,
    instance_id text NULL,
    event_code text NOT NULL,
    time timestamp with time zone NOT NULL,
    data jsonb NOT NULL
) PARTITION BY RANGE (time);
```
This table structure allows for the display of journal entries in SQL format but introduces certain limitations. Now, entries cannot be simply represented as text strings; instead, `event_code` is used, such as `UserCreatedEvent`. The `project_id` field is used to specify your project's identifier, and `instance_id` (optional) to denote a specific instance within the project. The `data` field contains additional information related to the event, such as `FirstName` for the `UserCreatedEvent`.
## System Advantages

The Kiroku system ensures high speed and efficiency in processing queries, even when dealing with an extensive database of logs. This is achieved through strict typing of key journal fields, as well as the use of indexing and table partitioning (partition), which significantly speeds up data search and processing.

## Deployment

To launch the Kiroku infrastructure, use the provided Dockerfile for the `Kiroku.PartitionWorker` and `Kiroku.Web` projects. Before starting, don't forget to perform database migrations. The PostgreSQL (Npgsql) database connection string can be configured via the `ConnectionStrings:Primary` variable. It can be set using the environment variables `ConnectionStrings__Primary` or another method proposed by Microsoft.

Command to run migrations:

```bash
dotnet ef database update --project Kiroku.Persistence -s Kiroku.Web
```

## Log Recording Methods

Kiroku offers two methods for logging: individually and in batches.

In the following examples, `MySocialNetwork` is used as `project_id` and `MySocialNetwork-SQ1` as `instance_id`. Note that `instance_id` is not mandatory – it can be omitted.

The logging level is determined by the [LogLevel](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-7.0) enumeration.

### Single Entry

```bash
curl -X 'POST' \
  'http://localhost:5026/logs/MySocialNetwork/MySocialNetwork-SQ1' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "Timestamp": "2023-11-02T11:25:09.449Z",
  "Level": "Trace",
  "Properties": {
     "FirstName": "John Doe"
   },
  "EventCode": "UserCreatedEvent"
}'
```

### Batch Entry

```bash
curl -X 'POST' \
  'http://localhost:5026/logs/batch/MySocialNetwork/MySocialNetwork-SQ1' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '[
  {
    "Timestamp": "2023-11-02T11:25:09.449Z",
    "Level": "Trace",
    "Properties": {
       "FirstName": "John Doe"
     },
    "EventCode": "UserCreatedEvent"
  },
  {
    "Timestamp": "2023-11-02T11:25:09.449Z",
    "Level": "Trace",
    "Properties": {
       "UserId": "ef2bd07e-6f57-4876-8215-07cf4aa3e30f"
     },
    "EventCode": "UserLoggedInEvent"
  }
]'
```

## Important Notes

It is important to note that the Kiroku project does not include tools for visualizing or exporting the recorded logs. It is assumed that users will use external tools, such as Grafana, for data visualization based on SQL queries. Additionally, at the moment, the project does not have integrations with popular logging libraries such as Serilog, and corresponding extensions are currently absent.
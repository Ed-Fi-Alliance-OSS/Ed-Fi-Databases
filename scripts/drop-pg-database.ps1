# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

<#
.SYNOPSIS
This script will drop a PostgreSQL database.

.DESCRIPTION
This was primarily created to aid in integration testing, so that a newly-created database can be dropped afterward to keep the server clean.

Requires the Postgres ODBC driver. Installation options:
- download from https://www.postgresql.org/ftp/odbc/versions/msi/, or
- choco install psqlodbc

.PARAMETER server
Database server name or IP address.

.PARAMETER port
Port number to connect to, 5432 by default.

.Parameter username
Username to connect as, "postgres" by default.

.Parameter password
Password for this user.

.PARAMETER databaseToDrop
Name of the database that needs to be dropped.

.EXAMPLE
.\drop-database.ps1 -server localhost -password password -databaseToDrop nameOfDatabase

.EXAMPLE
.\drop-database.ps1 -server localhost -port 5430 -userName superUser -password password -databaseToDrop nameOfDatabase
#>
function Invoke-DropPostgresDatabase(
    [Parameter(Mandatory=$true)]    
    [string]
    $server,

    [int]
    $port = 5432,

    [string]
    $username = "postgres",
    
    [Parameter(Mandatory=$true)]    
    [string]
    $password,
    
    [Parameter(Mandatory=$true)]
    [string]
    $databaseToDrop
)
{
    $DBConnectionString = "Driver={PostgreSQL Unicode(x64)};Server=$server;Port=$port;Database=postgres;Username=$username;Password=$password";
    $DBConn = New-Object System.Data.Odbc.OdbcConnection;

    $commands = @(
        "UPDATE pg_database SET datallowconn = 'false' WHERE datname = '$databaseToDrop';";
        "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '$databaseToDrop';";
        "DROP DATABASE ""$databaseToDrop"";"
    );

    try
    {
        $DBConn.ConnectionString = $DBConnectionString;
        $DBConn.Open();

        $commands.ForEach(
        {
            $DBCmd = $DBConn.CreateCommand();
            $DBCmd.CommandText = $_;
            $DBCmd.ExecuteNonQuery() | out-null;
        });

        Write-Host "Database $databaseToDrop has been dropped from $($server):$($port)";
    }
    finally 
    {
        $DBConn.Close();
        $DbConn.Dispose();
    }
}
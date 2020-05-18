# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

<#
.SYNOPSIS
This script will drop a SQL Server database.

.DESCRIPTION
This was primarily created to aid in integration testing, so that a newly-created database can be dropped afterward to keep the server clean.

.PARAMETER server
Database server name or IP address.

.PARAMETER port
Port number to connect to, 1433 by default.

.Parameter useIntegratedSecurity
Set the connection to use integrated security, overriding any username or password provided.

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

.EXAMPLE
.\drop-database.ps1 -server localhost -useIntegratedSecurity -databaseToDrop nameOfDatabase
#>
function Invoke-DropSqlServerDatabase(
    [Parameter(Mandatory=$true)]    
    [string]
    $server,

    [int]
    $port = 1433,

    [string]
    $username,

    [switch]
    $useIntegratedSecurity,
    
    [string]
    $password,
    
    [Parameter(Mandatory=$true)]
    [string]
    $databaseToDrop
)
{
    $ErrorActionPreference = "Stop";

    $DBConnectionString = "Server=$server,$port;Database=master;";
    if ($useIntegratedSecurity) {
        $DBConnectionString += "Integrated Security=SSPI;";
    }
    else {
        $DBConnectionString += "User ID=$username;Password=$password;";
    }

    $DBConn = New-Object System.Data.SqlClient.SqlConnection;

    $commands = @(
        "ALTER DATABASE [$databaseToDrop] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
        "DROP DATABASE [$databaseToDrop];"
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

        Write-Host "Database $databaseToDrop has been dropped from $($server),$($port)";
    }
    finally 
    {
        $DBConn.Close();
        $DbConn.Dispose();
    }
}
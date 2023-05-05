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
./drop-database.ps1 -server localhost -password password -databaseToDrop nameOfDatabase

.EXAMPLE
./drop-database.ps1 -server localhost -port 5430 -userName superUser -password password -databaseToDrop nameOfDatabase

.EXAMPLE
./drop-database.ps1 -server localhost -useIntegratedSecurity -databaseToDrop nameOfDatabase
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

    if ($port -ne 1433) {
        $server += ",$port";
    }

    $MasterConnectionString = "Server=$server;Database=master;";
    if ($useIntegratedSecurity) {
        $MasterConnectionString += "Integrated Security=SSPI;";
    }
    else {
        $MasterConnectionString += "User ID=$username;Password=$password;";
    }  

    $dropDatabase = @(
            "IF DATABASEPROPERTYEX('$databaseToDrop', 'Status') != 'RESTORING'"
            "BEGIN"
                "ALTER DATABASE [$databaseToDrop] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
            "END"
            "GO"
            "DROP DATABASE [$databaseToDrop]"
            "GO"
        ) -join "`n"
    Write-Host "Dropping the $databaseName Database."
    Invoke-SqlScript -connectionString $MasterConnectionString -sql $dropDatabase
    Write-Host "Database $databaseToDrop has been dropped from $($server)";
}
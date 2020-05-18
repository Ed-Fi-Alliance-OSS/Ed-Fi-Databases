# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

<#
.SYNOPSIS
Executes a complete integration test cycle with deploy to Postgres 10, Postgres 11, and SQL Server databases.

.DESCRIPTION
For each database platform, this script will run the deployment and then attempt to drop the created database, leaving the server in a clean state.

The default parameter values assume that SQL server is running on port 1433 on the localhost, and the two Postgres instances are running in Docker on the localhost on ports 5430 (PG10) and 5431 (PG11).

.PARAMETER sqlServerName
SQL Server server name or IP address. Defaults to "localhost".

.PARAMETER sqlServerPort
Port number to connect to. Defaults to 1433.

.Parameter pg11ServerName
Postgres 11 server name or IP address. Defaults to "localhost".

.Parameter pg11Port
Postgres 11 port. Defaults to 5431.

.Parameter bypassIntegratedSecurity
Do not use integrated security for SQL Server, instead use username and password.

.Parameter sqlUsernane
Username to connect to SQL Server. Defaults to "docker".

.Parameter sqlPassword
Password for the sqlUsername. Defaults to "docker".

.Parameter pg11Username
Username to connect to Postgres 10. Defaults to "postgres".

.Parameter pg11Password
Password for the pg10Username. Defaults to "docker".

.Parameter debug
Switch to turn on debug logging.

.PARAMETER databaseNameSuffix
Suffix appended to the databaseType to create the name of the database that will be created or re-used. Defaults to "ods-db-deploy-integration-test".

.PARAMETER databaseType
Either Admin, Ods, or Security. Default: Ods.

.PARAMETER deployChangesFeature
Switch to deploy Changes feature
#>
Param(
    [string]
    $sqlServerName = "localhost",

    [string]
    $pg11ServerName = "localhost",

    [int]
    $sqlServerPort = 1433,

    [int]
    $pg11Port = 5431,

    [switch]
    $bypassIntegratedSecurity,

    [string]
    $sqlUsername = "docker",
    
    [string]
    $sqlPassword = "docker",
    
    [string]
    $pg11Username = "postgres",
    
    [string]
    $pg11Password = "docker",
    
    [string]
    $databaseNameSuffix = "-db-deploy-integration-test",

    [switch]
    $debug,

    [string]
    $databaseType = "Ods",
    
    [switch]
    $deployChangesFeature = $True
)

# Load external files
. $PSScriptRoot\drop-ms-database.ps1
. $PSScriptRoot\drop-pg-database.ps1

$databaseName = $databaseType+$databaseNameSuffix

# Define "private" functions
function Invoke-EdFiDbDeploy
(
    [string] $connectionString,
    [string] $engine
)
{
    $filePaths = @(
        "$pwd\scripts\"
    )

    if ($deployChangesFeature)
    {
        $features = @(
            "Changes"
        )

        &dotnet run --project $PSScriptRoot\..\src\EdFi.Db.Deploy\EdFi.Db.Deploy.csproj `
            --configuration release `
            deploy `
            --connectionString $connectionString `
            --engine $engine `
            --database $databaseType `
            --filePaths $filePaths `
            --features $features
    }
    else
    {
        &dotnet run --project $PSScriptRoot\..\src\EdFi.Db.Deploy\EdFi.Db.Deploy.csproj `
            --configuration release `
            deploy `
            --connectionString $connectionString `
            --engine $engine `
            --database $databaseType `
            --filePaths $filePaths
    }

    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "Previous command exited with code $LASTEXITCODE";
    }
}

function IsDeployingChangesFeature()
{
    if ($deployChangesFeature -And $databaseType -eq "Ods")
    {
        return $True;
    }

    return $False;
}

function Invoke-SqlServerAssertions
{
    Write-Host "Starting SqlServerDeployTest..."

    # Database should be created
    if ($debug) { Write-Debug "Test: Database should be created."; }
    $result = Invoke-Sqlcmd -Query "SELECT [name] FROM sys.databases WHERE [name] = '$databaseName';" -ServerInstance $sqlServerName;
    if (!$result)
    {
        Write-Error "Database should be created.";
        return;
    }

    # DeployJournal table should be created
    if ($debug) { Write-Debug "Test: DeployJournal table should be created."; }
    $result = Invoke-Sqlcmd -Query "SELECT [TABLE_SCHEMA] AS [TableSchema], [TABLE_NAME] AS [TableName] FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'DeployJournal';" -ServerInstance $sqlServerName -Database $databaseName;
    if (!$result)
    {
        Write-Error "DeployJournal table should be created.";
        return;
    }

    $result = Invoke-Sqlcmd -Query "SELECT [ScriptName] FROM [dbo].[DeployJournal];" -ServerInstance $sqlServerName -Database $databaseName;

    # DeployJournal data should not be empty
    if ($debug) { Write-Debug "Test: DeployJournal data should not be empty."; }
    if (!$result)
    {
        Write-Error "DeployJournal data should not be empty.";
        return;
    }

    # Set expected data based on $deployChangesFeature switch
    [System.Collections.ArrayList]$scriptFileNamesExpected = @(
        "scripts.Artifacts.MsSql.Structure.$databaseType.0010-StructureFile.sql",
        "scripts.Artifacts.MsSql.Data.$databaseType.0010-DataFile.sql"
    );
    if (IsDeployingChangesFeature)
    {
        [void]$scriptFileNamesExpected.Add("scripts.Artifacts.MsSql.Structure.$databaseType.Changes.0010-StructureFile.sql");
    }
    $deployJournalExpectedRows = $scriptFileNamesExpected.Count;

    # DeployJournal data rows should be $deployJournalExpectedRows
    if ($debug) { Write-Debug "Test: DeployJournal data rows should be $deployJournalExpectedRows."; }
    if ($result.Length -ne $deployJournalExpectedRows)
    {
        Write-Error "DeployJournal data rows should be $deployJournalExpectedRows but was $($result.Rows.Count).";
        return;
    }

    for ($i = 0; $i -ne $deployJournalExpectedRows; $i++)
    {
        $scriptFileNameExpected = $scriptFileNamesExpected[$i];
        $scriptFileNameObtained = $result.Item($i).ScriptName;

        # DeployJournal script name $i+1 should be $scriptFileNamesExpected[$i]
        if ($debug) { Write-Debug "Test: DeployJournal script name $($i + 1) should be '$scriptFileNameExpected'."; }
        if ($scriptFileNameObtained -ne $scriptFileNameExpected)
        {
            Write-Error "DeployJournal script name $($i + 1) should be '$scriptFileNameExpected' but was '$scriptFileNameObtained'.";
            return;
        }
    }

    # edfi.SampleTable table should be created
    if ($debug) { Write-Debug "Test: edfi.SampleTable table should be created."; }
    $result = Invoke-Sqlcmd -Query "SELECT [TABLE_SCHEMA] AS [TableSchema], [TABLE_NAME] AS [TableName] FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_SCHEMA] = 'edfi' AND [TABLE_NAME] = 'SampleTable';" -ServerInstance $sqlServerName -Database $databaseName;
    if (!$result)
    {
        Write-Error "edfi.SampleTable table should be created.";
        return;
    }

    # Set query text based on $deployChangesFeature switch
    $selectSampleTableQueryText = "SELECT [SampleTableId], [TextColumn] FROM [edfi].[SampleTable];";
    if (IsDeployingChangesFeature)
    {
        $selectSampleTableQueryText = "SELECT [SampleTableId], [TextColumn], [ChangeVersion] FROM [edfi].[SampleTable];";
    }

    $result = Invoke-Sqlcmd -Query $selectSampleTableQueryText -ServerInstance $sqlServerName -Database $databaseName;

     # edfi.SampleTable data should not be empty
    if ($debug) { Write-Debug "Test: edfi.SampleTable data should not be empty."; }
    if (!$result)
    {
        Write-Error "edfi.SampleTable data should not be empty.";
        return;
    }

    # edfi.SampleTable data rows should be 3
    if ($debug) { Write-Debug "Test: edfi.SampleTable data rows should be 3." };
    if ($result.Length -ne 3)
    {
        Write-Error "edfi.SampleTable data rows should be 3 but has $($result.Length).";
        return;
    }

    if (IsDeployingChangesFeature)
    {
        # changes.ChangeVersionSequence sequence should be created
        if ($debug) { Write-Debug "Test: changes.ChangeVersionSequence sequence should be created."; }
        $result = Invoke-Sqlcmd -Query "SELECT [SEQUENCE_SCHEMA] AS [SequenceSchema], [SEQUENCE_NAME] AS [SequenceName] FROM [INFORMATION_SCHEMA].[SEQUENCES] WHERE [SEQUENCE_SCHEMA] = 'changes' AND [SEQUENCE_NAME] = 'ChangeVersionSequence';" -ServerInstance $sqlServerName -Database $databaseName;
        if (!$result)
        {
            Write-Error "changes.ChangeVersionSequence sequence should be created.";
            return;
        }
    }

    Write-Host "SqlServerDeployTest passed successfully."
}

function Invoke-RunSqlServerTest
{
    $sqlConnectionString = "Server=$sqlServerName,$sqlServerPort;Database=$databaseName;";
    if ($bypassIntegratedSecurity)
    {
        $sqlConnectionString += "User ID=$sqlUsername;Password=$sqlPassword";
    }
    else 
    {
        $sqlConnectionString += "Integrated Security=SSPI";
    }

    if ($debug) 
    {
        Write-Debug "SQL Server connection string: $sqlConnectionString";
    }

    Invoke-EdFiDbDeploy -connectionString $sqlConnectionString -engine SqlServer;

    Invoke-SqlServerAssertions;
}

function Invoke-CleanupSqlServer
{
    $splat = @{
        server = $sqlServerName;
        port = $sqlServerPort;
        useIntegratedSecurity = !$bypassIntegratedSecurity;
        userName = $sqlUsername;
        password = $sqlPassword;
        databaseToDrop = $databaseName;
    };
    Invoke-DropSqlServerDatabase @splat;
}

function Invoke-Pgsqlcmd
(
    [Parameter(mandatory=$true)]
    [string]
    $ConnectionString,

    [string]
    $Query
)
{
    $dbConnectionString = "Driver={PostgreSQL UNICODE(x64)};$ConnectionString";
    $dbConn = New-Object System.Data.Odbc.OdbcConnection;

    try
    {
        $dbConn.ConnectionString = $dbConnectionString;
        $dbConn.Open();

        $dbCmd = $dbConn.CreateCommand();
        $dbCmd.CommandText = $Query;

        $dataSet = New-Object system.Data.DataSet
        (New-Object system.Data.odbc.odbcDataAdapter($dbCmd)).fill($dataSet) | out-null
    }
    finally
    {
        $dbConn.Close();
        $dbConn.Dispose();
    }

    if (!$dataSet.Tables.Length)
    {
        Write-Error "Query does not return any value: $Query";
        exit -1
    }

    if (!$dataSet.Tables.Length -eq 1)
    {
        return $dataSet.Tables[0];
    }

    return $dataSet.Tables;
}

function Invoke-PostgresAssertions
(
    [Parameter(mandatory=$true)]
    [string]
    $connectionString
)
{
    Write-Host "Starting PostgresDeployTest..."

    # Database should be created
    if ($debug) { Write-Debug "Test: Database should be created."; }
    $result = Invoke-Pgsqlcmd -Query "SELECT datname FROM pg_database WHERE datname='$databaseName';" -ConnectionString $connectionString;
    if (!$result.Rows.Count)
    {
        Write-Error "Database should be created.";
        return;
    }

    # DeployJournal table should be created
    if ($debug) { Write-Debug "Test: DeployJournal table should be created."; }
    $result = Invoke-Pgsqlcmd -Query "SELECT table_schema AS TableSchema, table_name AS TableName FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'DeployJournal';" -ConnectionString $connectionString;
    if (!$result.Rows.Count)
    {
        Write-Error "DeployJournal table should be created.";
        return;
    }

    $result = Invoke-Pgsqlcmd -Query "SELECT ScriptName FROM ""public"".""DeployJournal"";" -ConnectionString $connectionString;

    # DeployJournal data should not be empty
    if ($debug) { Write-Debug "Test: DeployJournal data should not be empty."; }
    if (!$result.Rows.Count)
    {
        Write-Error "DeployJournal data should not be empty.";
        return;
    }

    # Set expected data based on $deployChangesFeature switch
    [System.Collections.ArrayList]$scriptFileNamesExpected = @(
        "scripts.Artifacts.PgSql.Structure.$databaseType.0010-StructureFile.sql",
        "scripts.Artifacts.PgSql.Data.$databaseType.0010-DataFile.sql"
    );
    if (IsDeployingChangesFeature)
    {
        [void]$scriptFileNamesExpected.Add("scripts.Artifacts.PgSql.Structure.$databaseType.Changes.0010-StructureFile.sql");
    }
    $deployJournalExpectedRows = $scriptFileNamesExpected.Count;

    # DeployJournal data rows should be $deployJournalExpectedRows
    if ($debug) { Write-Debug "Test: DeployJournal data rows should be $deployJournalExpectedRows."; }
    if ($result.Rows.Count -ne $deployJournalExpectedRows)
    {
        Write-Error "DeployJournal data rows should be $deployJournalExpectedRows but was $($result.Rows.Count).";
        return;
    }

    for ($i = 0; $i -ne $deployJournalExpectedRows; $i++)
    {
        $scriptFileNameExpected = $scriptFileNamesExpected[$i];
        $scriptFileNameObtained = $result.Rows[$i].ScriptName;

        # DeployJournal script name $i+1 should be $scriptFileNamesExpected[$i]
        if ($debug) { Write-Debug "Test: DeployJournal script name $($i + 1) should be '$scriptFileNameExpected'."; }
        if ($scriptFileNameObtained -ne $scriptFileNameExpected)
        {
            Write-Error "DeployJournal script name $($i + 1) should be '$scriptFileNameExpected' but was '$scriptFileNameObtained'.";
            return;
        }
    }

    # edfi.SampleTable table should be created
    if ($debug) { Write-Debug "Test: edfi.SampleTable table should be created."; }
    $result = Invoke-Pgsqlcmd -Query "SELECT table_schema AS TableSchema, table_name AS TableName FROM information_schema.tables WHERE table_schema = 'edfi' AND table_name = 'sampletable';" -ConnectionString $connectionString;
    if (!$result.Rows.Count)
    {
        Write-Error "edfi.SampleTable table should be created.";
        return;
    }

    # Set query text based on $deployChangesFeature switch
    $selectSampleTableQueryText = "SELECT SampleTableId, TextColumn FROM edfi.SampleTable;";
    if (IsDeployingChangesFeature)
    {
        $selectSampleTableQueryText = "SELECT SampleTableId, TextColumn, ChangeVersion FROM edfi.SampleTable;";
    }

    $result = Invoke-Pgsqlcmd -Query $selectSampleTableQueryText -ConnectionString $connectionString;

    # edfi.SampleTable data should not be empty
    if ($debug) { Write-Debug "Test: edfi.SampleTable data should not be empty."; }
    if (!$result.Rows.Count)
    {
        Write-Error "edfi.SampleTable data should not be empty.";
        return;
    }

    # edfi.SampleTable data rows should be 3
    if ($debug) { Write-Debug "Test: edfi.SampleTable data rows should be 3."; }
    if ($result.Rows.Count -ne 3)
    {
        Write-Error "edfi.SampleTable data rows should be 3 but has $($result.Rows.Count).";
        return;
    }

    if (IsDeployingChangesFeature)
    {
        # changes.ChangeVersionSequence sequence should be created
        if ($debug) { Write-Debug "Test: changes.ChangeVersionSequence sequence should be created."; }
        $result = Invoke-Pgsqlcmd -Query "SELECT sequence_schema AS SequenceSchema, sequence_name AS SequenceName FROM information_schema.sequences WHERE sequence_schema = 'changes' AND sequence_name = 'changeversionsequence';" -ConnectionString $connectionString;
        if (!$result.Rows.Count)
        {
            Write-Error "changes.ChangeVersionSequence sequence should be created.";
            return;
        }
    }

    Write-Host "PostgresDeployTest passed successfully."
}

function Invoke-RunPostgresTest
(
    [string] $serverName,
    [int] $port, 
    [string] $username, 
    [string] $password
)
{
    $sqlConnectionString = "Server=$serverName;Port=$port;Database=$databaseName;Username=$username;Password=$password";

    if ($debug) 
    {
        Write-Debug "Postgres connection string: $sqlConnectionString";
    }

    Invoke-EdFiDbDeploy -connectionString $sqlConnectionString -engine PostgreSQL;

    Invoke-PostgresAssertions -connectionString $sqlConnectionString
}

function Invoke-CleanupPostgres
(
    [string] $serverName,
    [int] $port, 
    [string] $username, 
    [string] $password
)
{
    $splat = @{
        server = $serverName;
        port = $port;
        userName = $username;
        password = $password;
        databaseToDrop = $databaseName;
    };
    Invoke-DropPostgresDatabase @splat;
}

function Write-Info
(
    $message
)
{
    Write-Host $message -ForegroundColor Yellow;
}
function Write-Debug
(
    $message
)
{
    Write-Host $message -ForegroundColor Cyan;
}

if ($deployChangesFeature)
{
    Write-Host "Deploy Changes feature only works for Ods database type.";
}

# Orchestrate test execution
Write-Info "Initial run on SQL Server";
Invoke-RunSqlServerTest;

Write-Info "Second run on SQL Server";
Invoke-RunSqlServerTest;

Write-Info "Clean up SQL Server";
Invoke-CleanupSqlServer;

Write-Info "Initial run on Postgres 11";
Invoke-RunPostgresTest -serverName $pg11ServerName -port $pg11Port -username $pg11Username -password $pg11Password;

Write-Info "Second run on Postgres 11";
Invoke-RunPostgresTest -serverName $pg11ServerName -port $pg11Port -username $pg11Username -password $pg11Password;

Write-Info "Cleanup Postgres 11";
Invoke-CleanupPostgres -serverName $pg11ServerName -port $pg11Port -username $pg11Username -password $pg11Password;

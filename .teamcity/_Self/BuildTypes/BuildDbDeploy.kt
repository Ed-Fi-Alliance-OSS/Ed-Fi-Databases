/*
 * SPDX-License-Identifier: Apache-2.0
 * Licensed to the Ed-Fi Alliance under one or more agreements.
 * The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
 * See the LICENSE and NOTICES files in the project root for more information.
 */

package _self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetPack
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.nuGetPublish

object BuildDbDeploy : BuildType({
    templates(_self.templates.NetCore31Packages)
    
    name = "EdFi.Db.Deploy"

    artifactRules = """%nuget.pack.output%\%PackageId%.%version.core%*.nupkg"""

    params {
        param("project.file.csproj", "%project.directory%/%project.name%/%project.name%.csproj")
        param("pathToSolutionFile", "src/EdFi.Db.Deploy.sln")
        param("msbuild.exe", "")
        param("PackageId", "EdFi%odsapi.package.suffix%.Db.Deploy")
        param("project.name", "EdFi.LoadTools")
        param("pathToTestFile", "tests/**/%msbuild.buildConfiguration%/EdFi.Db.Deploy.Tests.dll")
        param("version.major", "2")
        param("project.directory", "Ed-Fi-ODS/Utilities/DataLoading")
        param("dotnet.pack.parameters", "-p:NoWarn=NU5123 -p:PackageId=%PackageId%")
    }

    vcs {
        root(AbsoluteId("OdsPlatform_EdFiDatabases"))
    }

    steps {
        dotnetPack {
            name = "Pack Prerelease version - Library"
            id = "RUNNER_400"
            enabled = false
            projects = "%project.file.csproj%"
            configuration = "%msbuild.configuration%"
            outputDir = "%teamcity.build.checkoutDir%"
            skipBuild = true
            args = """-p:VersionPrefix=%version.core% --version-suffix "%version.prerelease%" %dotnet.pack.args%"""
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        nuGetPublish {
            name = "Publish Prerelease Version"
            id = "RUNNER_195"
            toolPath = "%teamcity.tool.NuGet.CommandLine.DEFAULT%"
            packages = """%nuget.pack.output%\%PackageId%.%version%.nupkg"""
            serverUrl = "%myget.feed%"
            apiKey = "******"
        }
        stepsOrder = arrayListOf("RUNNER_191", "RUNNER_189", "RUNNER_319", "RUNNER_400", "RUNNER_193", "RUNNER_194", "RUNNER_195")
    }
})

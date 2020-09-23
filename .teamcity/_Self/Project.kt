// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

package _self

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object DbDeployProject : Project({
    description = "Ed-Fi Db Deploy Build Configurations"

    params {
        param("build.feature.freeDiskSpace", "2gb")
        param("teamcity.ui.settings.readOnly","true")
    }

    template(_self.templates.NetCore31Packages)

    buildType(_self.buildTypes.BuildDbDeploy)
})

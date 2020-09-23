# Getting Started

To apply these build configuration settings on a new TeamCity environment (min
version: `2019.2.2`):

1. Create a "root" project that will contain this work as a sub-project, or
   identify an existing one to re-use.
1. Ensure these parameters are set in that root project:

    ```none
    github.organization = <org name or username>
    github.username = <username>
    github.accessToken.protected (password type) = <access token>
    github.accessToken = %github.accessToken.protected%
    ```

    If pull from an organization, use that organization's name. Else use own
    GitHub user name to pull from your fork.

1. Create a VCS Root:
    * Type: `Git`
    * Name: `Ed-Fi-Databases`
    * Fetch Url: `https://github.com/%github.organization%/Ed-Fi-Databases`
    * Default Branch: `%git.branch.default%`
    * Branch Specification: `%git.branch.specification%`
    * Authentication method: `password`
    * Username: `%github.username%`
    * Password: `%github.accessToken%`
1. Create a sub-project named "Db Deploy - Kotlin"
1. Set this parameters in the new project:

    ```none
    git.branch.default = main
    git.branch.specification = +:refs/heads/(*)
                               +:(refs/pull/*/head)
    ```

    **Tip**: while developing the TeamCity configurations, you can change `main`
    to a feature branch temporarily. However, if the Project.kt file also needs
    to define `git.branch.default` and `git.branch.specification` - otherwise
    they'll be wiped out when you import from version control. Thus you need to
    use the feature branch temporarily in source code. Alternately, if using a
    fork, you can push your changes directly to `main` in your fork instead of
    pushing them to a feature branch.

1. Turn on Versioned Settings:
    * Synchronization enabled: `true`
    * Project settings VCS root: `Ed-Fi-Databases`
    * When build starts: `use settings from VCS`
    * Store secure values outside of VCS: `true`
    * Settings format: `kotlin`
    * Generate portable DSL scripts: `true`
1. Click Apply.
1. If prompted to choose either committing current changes or reading from the
   VCS root, then choose to read from the VCS root.
1. Else click on the "Load project settings from VCS..." button.

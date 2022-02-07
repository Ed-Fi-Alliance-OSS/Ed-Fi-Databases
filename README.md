# Ed-Fi-Databases

Database deployment utility for the Ed-Fi Technical Suite 3 databases.

For more information, see:

* For more information, go to [Ed-Fi Tech Docs](https://techdocs.ed-fi.org/) for instructions on getting started, a summary of our code repositories and additional technical information.
* [How to Submit an Issue](https://techdocs.ed-fi.org/display/ETKB/How+To%3A+Submit+an+Issue)
* [How Submit a Feature Request](https://techdocs.ed-fi.org/display/ETKB/How+To%3A+Submit+a+Feature+Request)
* [Review on-going development work](https://tracker.ed-fi.org/projects/EDFI/)

## Developer Instructions

### Build Instructions

- Requires [.NET 6 SDK](https://dotnet.microsoft.com/download)
- No special commands to execute - simply call `dotnet build` in the `src`
  directory.

### Unit Testing

Tests can be run through Visual Studio or `dotnet test` in the `src` directory.

### Integration Testing

The `run-integration-tests.ps1` script executes a battery of tests with the
intention of verifying that EdFi.Db.Deploy successfully deploys scripts on both
SQL Server and PostgreSQL. To execute, open Powershell and call:

```powershell
scripts\run-integration-tests.ps1
```

## Contributing

The Ed-Fi Alliance welcomes code contributions from the community. Please read
the [Ed-Fi Contribution
Guidelines](https://techdocs.ed-fi.org/display/ETKB/Code+Contribution+Guidelines)
for detailed information on how to contribute source code.

Looking for an easy way to get started? Search for tickets with label
"up-for-grabs" in [Tracker](https://tracker.ed-fi.org/issues/?filter=14105); these are nice-to-have but low priority tickets that should not
require in-depth knowledge of the code base and architecture.

## Legal Information

Copyright (c) 2020 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.

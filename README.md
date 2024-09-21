# BuildSphere

![Build Status](https://img.shields.io/github/actions/workflow/status/phmatray/BuildSphere/build.yml?branch=main&label=build) ![NuGet](https://img.shields.io/nuget/v/BuildSphere) ![License](https://img.shields.io/github/license/phmatray/BuildSphere)

BuildSphere is a powerful and intuitive .NET global build tool designed to streamline the building and deployment of .NET applications to NuGet and GitHub. Leveraging modern tools like Nuke, MinVer, and Git-Cliff, BuildSphere simplifies your development workflow, ensuring consistent and efficient builds across all your projects.

## Table of Contents

- [Features](#features)
- [Dependencies](#dependencies)
- [Installation](#installation)
    - [Prerequisites](#prerequisites)
    - [Install BuildSphere as a Local Tool](#install-buildsphere-as-a-local-tool)
    - [Install BuildSphere as a Global Tool](#install-buildsphere-as-a-global-tool)
- [Configuration](#configuration)
- [Usage](#usage)
    - [Generating the Changelog](#generating-the-changelog)
    - [Building and Deploying](#building-and-deploying)
- [Continuous Integration](#continuous-integration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

- **Global and Local Tool Support:** Install BuildSphere globally for system-wide access or locally within specific repositories.
- **Automated Changelog Generation:** Utilize Git-Cliff to automatically generate and update your `CHANGELOG.md`.
- **Versioning with MinVer:** Automatically manage your project versions based on your Git history.
- **Seamless Deployment:** Easily build and deploy your .NET applications to NuGet and GitHub.
- **Consistent Builds with Nuke:** Leverage Nuke's robust build automation capabilities for reliable and repeatable builds.

## Dependencies

BuildSphere relies on the following tools:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [Nuke](https://nuke.build/) - A modern build automation system for .NET.
- [MinVer](https://github.com/adamralph/minver) - Automatic semantic versioning using Git tags.
- [Git-Cliff](https://github.com/orhun/git-cliff) - A Git-based changelog generator.

## Installation

### Prerequisites

Ensure you have the following installed on your machine:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [Git](https://git-scm.com/downloads)
- [Git-Cliff](https://github.com/orhun/git-cliff) (Install via Homebrew, Scoop, or other package managers)

### Install BuildSphere as a Local Tool

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/phmatray/BuildSphere.git
   cd BuildSphere
   ```

2. **Pack the Tool:**

   Navigate to the `./build` directory and create a NuGet package.

   ```bash
   cd build
   dotnet pack -c Release
   ```

   The NuGet package (`BuildSphere.0.1.0.nupkg`) will be available in the `./.nupkg` directory.

3. **Install as a Local Tool:**

   From your project directory where you want to use BuildSphere:

   ```bash
   dotnet new tool-manifest # If you don't have a tool manifest yet
   dotnet tool install --add-source ../BuildSphere/.nupkg BuildSphere
   ```

   > **Note:** Replace `../BuildSphere/.nupkg` with the relative path to the `.nupkg` file if your directory structure differs.

### Install BuildSphere as a Global Tool

1. **Pack the Tool:**

   As above, ensure you've created the NuGet package in the `./.nupkg` directory.

2. **Install as a Global Tool:**

   ```bash
   dotnet tool install -g --add-source ./build/.nupkg BuildSphere
   ```

   > **Note:** Adjust the `--add-source` path as necessary based on where your `.nupkg` file is located.

3. **Verify Installation:**

   ```bash
   build-sphere --version
   ```

   You should see the installed version of BuildSphere.

## Configuration

### GitHub Secret for NuGet API Key

To enable BuildSphere to deploy packages to NuGet, you need to set up a GitHub secret:

1. **Navigate to Your Repository's Settings:**

   Go to your GitHub repository, click on `Settings` > `Secrets and variables` > `Actions`.

2. **Add a New Secret:**

    - **Name:** `NUGET_API_KEY`
    - **Value:** Your NuGet API key.

   ![Add GitHub Secret](https://docs.github.com/assets/images/help/repository/repo-actions-secrets-add.png)

   > **Note:** Keep your API keys secure and never expose them in your code or logs.

### Git-Cliff Configuration

BuildSphere uses Git-Cliff to generate changelogs based on your Git commit history. The configuration is embedded within the tool, ensuring consistency across all projects.

If you need to customize `git-cliff.toml`, modify the embedded resource in the BuildSphere project and rebuild the tool.

## Usage

BuildSphere provides a suite of commands to automate your build and deployment processes.

### Generating the Changelog

To generate or update the `CHANGELOG.md` using Git-Cliff:

```bash
build-sphere GenerateChangeLog
```

**Steps Performed:**

1. **Validates** that Git-Cliff is installed and accessible.
2. **Retrieves** the embedded `git-cliff.toml` configuration.
3. **Generates** or updates the `CHANGELOG.md` based on commit history.
4. **Commits** the updated changelog if changes are detected.

### Building and Deploying

To build your project and deploy it to NuGet and GitHub:

```bash
build-sphere BuildAndDeploy
```

**Steps Performed:**

1. **Restores** NuGet packages.
2. **Builds** the project in Release mode.
3. **Runs** tests to ensure code quality.
4. **Packages** the application for deployment.
5. **Pushes** the package to NuGet and GitHub Releases.

> **Note:** Ensure that your GitHub Actions or CI/CD pipelines have access to the necessary secrets and configurations.

## Continuous Integration

Integrate BuildSphere into your CI/CD pipelines to automate builds and deployments.

### GitHub Actions Example

Here's an example of how to set up GitHub Actions to use BuildSphere:

```yaml
name: Build and Deploy

on:
  push:
    branches:
      - main
  pull_request:

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout Repository
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Install BuildSphere
        run: dotnet tool install -g --add-source ./build/.nupkg BuildSphere
        env:
          DOTNET_CLI_HOME: ${{ runner.temp }}

      - name: Add .NET Tools to PATH
        run: echo "${HOME}/.dotnet/tools" >> $GITHUB_PATH

      - name: Generate Changelog
        run: build-sphere GenerateChangeLog

      - name: Build and Deploy
        run: build-sphere BuildAndDeploy
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

> **Note:** Adjust the `--add-source` path based on where your `.nupkg` file is located.

## Contributing

Contributions are welcome! Whether it's reporting bugs, suggesting features, or submitting pull requests, your input helps make BuildSphere better.

### How to Contribute

1. **Fork the Repository:**

   Click the `Fork` button at the top-right corner of the repository page.

2. **Clone Your Fork:**

   ```bash
   git clone https://github.com/phmatray/BuildSphere.git
   cd BuildSphere
   ```

3. **Create a New Branch:**

   ```bash
   git checkout -b feature/YourFeatureName
   ```

4. **Make Your Changes:**

   Implement your feature or bug fix.

5. **Commit Your Changes:**

   ```bash
   git commit -m "feat: YourFeatureName"
   ```

6. **Push to Your Fork:**

   ```bash
   git push origin feature/YourFeatureName
   ```

7. **Open a Pull Request:**

   Navigate to the original repository and click `Compare & pull request`.

### Code of Conduct

Please adhere to the [Code of Conduct](CODE_OF_CONDUCT.md) in all interactions.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any questions or feedback, feel free to reach out:

- **GitHub Issues:** [BuildSphere Issues](https://github.com/phmatray/BuildSphere/issues)

---

Made with ❤️ by [Philippe Matray](https://github.com/phmatray)
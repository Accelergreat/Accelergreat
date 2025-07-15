# Migration Guide: Azure DevOps to GitHub Actions

This guide explains the migration from Azure DevOps pipelines to GitHub Actions for the Accelergreat project.

## Overview

The migration includes:
- **CI/CD Pipeline**: Automated build, test, and deployment
- **Windows Test Support**: SQL Server tests run on Windows runners
- **Azure Static Web Apps**: Documentation website deployment
- **NuGet Publishing**: Automatic on main branch, manual on PRs
- **Semantic Versioning**: Enhanced version management

## New Workflows

### 1. Main CI Workflow (`.github/workflows/ci.yml`)

**Features:**
- Builds on Ubuntu (faster, cost-effective)
- Tests on Windows (SQL Server dependency)
- Semantic versioning with alpha builds for PRs
- NuGet package creation and publishing
- Documentation building

**Jobs:**
- `build`: Compiles solution and creates NuGet packages
- `test`: Runs tests on Windows for SQL Server compatibility
- `documentation`: Builds DocFX documentation
- `deploy-nuget`: Auto-deploys to NuGet on main branch
- `deploy-nuget-manual`: Manual deployment for PRs

### 2. Documentation Deployment (`.github/workflows/deploy-docs.yml`)

**Features:**
- Builds and deploys documentation to Azure Static Web Apps
- Supports pull request previews
- Automatic cleanup when PRs are closed

## Required Secrets

Set up these secrets in your GitHub repository:

### For NuGet Publishing
```
NUGET_API_KEY: Your NuGet.org API key
```

### For Azure Static Web Apps
```
AZURE_STATIC_WEB_APPS_API_TOKEN: Your Azure Static Web Apps deployment token
```

## Setup Instructions

### 1. Azure Static Web Apps Setup

1. Create an Azure Static Web Apps resource in Azure Portal
2. Choose "Other" as the deployment source
3. Copy the deployment token from the resource overview
4. Add it as `AZURE_STATIC_WEB_APPS_API_TOKEN` secret in GitHub

### 2. NuGet API Key Setup

1. Go to [NuGet.org](https://www.nuget.org/)
2. Create an API key with push permissions
3. Add it as `NUGET_API_KEY` secret in GitHub

### 3. GitHub Environments

Create these environments in your repository settings:

- **production**: For automatic NuGet deployment (main branch)
- **nuget-manual**: For manual NuGet deployment (PRs)

### 4. Version Management

The project now uses semantic versioning with `version.json`:

```json
{
    "version": "4.0.0",
    "major": 4,
    "minor": 0,
    "patch": 0,
    "prerelease": null
}
```

**Version Rules:**
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes
- **PRERELEASE**: For preview versions

## Migration Changes

### From Azure DevOps
- **Build Agent**: Windows-2022 → Ubuntu-latest (build), Windows-latest (tests)
- **Versioning**: Build number → Semantic versioning
- **Artifacts**: Azure DevOps artifacts → GitHub Actions artifacts
- **Secrets**: Azure DevOps variables → GitHub Secrets

### Improvements
- **Faster Builds**: Ubuntu for compilation, Windows only for tests
- **Better Versioning**: Semantic versioning with automatic alpha builds
- **Preview Deployments**: Documentation previews for PRs
- **Cost Optimization**: Reduced Windows runner usage

## Testing

### SQL Server Tests
Tests requiring SQL Server now run on Windows runners:
- `Accelergreat.Tests.EntityFramework.SqlServer`
- `Accelergreat.Tests.EntityFramework.SqlServer.MigrationsDatabase`

### Other Tests
API tests run on Windows for consistency with the test environment.

## Deployment Strategy

### Automatic Deployment (Main Branch)
- Tests pass → NuGet packages published automatically
- Documentation updated on Azure Static Web Apps

### Manual Deployment (PRs)
- Tests pass → NuGet packages available for manual deployment
- Documentation preview available
- Requires approval from `nuget-manual` environment

## Troubleshooting

### Common Issues

1. **Missing Secrets**: Ensure all required secrets are set
2. **Test Failures**: Check Windows runner logs for SQL Server issues
3. **Documentation Build**: Verify DocFX configuration
4. **Version Conflicts**: Update version.json following semantic versioning

### Debugging

Enable debug logging by setting these repository secrets:
```
ACTIONS_STEP_DEBUG: true
ACTIONS_RUNNER_DEBUG: true
```

## Benefits

- **Cost Effective**: Reduced Windows runner usage
- **Faster Builds**: Parallel job execution
- **Better Versioning**: Semantic versioning with alpha builds
- **Modern CI/CD**: GitHub Actions integration
- **Preview Deployments**: Documentation previews for PRs

## Next Steps

1. Remove the old `azure-pipelines.yml` file
2. Update repository documentation
3. Train team on new workflow processes
4. Monitor initial deployments
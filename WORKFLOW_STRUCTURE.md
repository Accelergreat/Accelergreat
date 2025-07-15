# GitHub Actions Workflow Structure

## Overview
This setup is designed for **public repository security** - no automatic deployments that could be triggered by external contributors, but automatic deployment on main branch merges.

## Workflow Files

### 1. `ci.yml` - Continuous Integration
- **Triggers**: Push to main, Pull requests, Manual
- **Purpose**: Build validation and artifact creation
- **Security**: No deployments, just validation
- **Jobs**:
  - `build`: Builds solution, packs NuGet packages, uploads artifacts
  - `documentation`: Builds documentation, uploads artifacts

### 2. `validate-version.yml` - Version Validation
- **Triggers**: **All PRs** (not just version.json changes), Manual
- **Purpose**: Ensure version is updated in PRs
- **Security**: Read-only validation
- **Checks**:
  - Version.json format is valid
  - Version has changed from main branch
  - Version is greater than main branch version

### 3. `main.yml` - Production Auto-Deploy
- **Triggers**: Push to main branch (merges), Manual
- **Purpose**: **Automatic deployment** of both docs and NuGet when main is updated
- **Security**: Only runs on main branch pushes
- **Jobs**:
  - `deploy-production`: Builds, packs, and deploys both NuGet and docs

### 4. `deploy-docs.yml` - Manual Documentation Deployment
- **Triggers**: Manual only (`workflow_dispatch`)
- **Purpose**: Allow collaborators to manually deploy docs
- **Security**: Requires workflow dispatch permission (collaborators only)
- **Environment**: Configurable (production/staging)

### 5. `deploy-nuget.yml` - Manual NuGet Deployment
- **Triggers**: Manual only (`workflow_dispatch`)
- **Purpose**: Allow collaborators to manually deploy NuGet packages
- **Security**: Requires workflow dispatch permission (collaborators only)
- **Environment**: Configurable (production/staging)

## Security Model

### Public Repository Protection
- **No automatic deployments** on PRs or external contributions
- **Manual deployments** require collaborator permissions
- **Automatic deployment** only on main branch merges (trusted code)

### Permission Requirements
- **Read-only operations**: Anyone can trigger CI and validation
- **Manual deployments**: Only collaborators can run `workflow_dispatch`
- **Main deployment**: Automatic on main branch push

## Usage

### For Contributors (External)
1. Create PR → CI runs automatically
2. Version validation runs automatically
3. **Cannot trigger deployments** (security)

### For Collaborators (Internal)
1. Can manually trigger deployments via GitHub Actions UI
2. Can choose environment (production/staging)
3. Can deploy docs and NuGet independently

### For Main Branch
1. **Automatic deployment** when PR is merged to main
2. Both docs and NuGet deploy automatically
3. Uses latest version from version.json

## Version Management

### PR Requirements
- **Must update version.json** before merge
- Version must be greater than main branch
- Validation runs on **all PRs** (not just version.json changes)

### Versioning Strategy
- **Main branch**: Uses exact version from version.json
- **PR branches**: Adds `-alpha.{SHA}` suffix for testing
- **Token replacement**: `~(version)~` replaced in all files

## Workflows Summary

| Workflow | Trigger | Purpose | Security |
|----------|---------|---------|----------|
| `ci.yml` | Push/PR | Build validation | Public |
| `validate-version.yml` | All PRs | Version validation | Public |
| `main.yml` | Main push | Auto-deploy production | Automatic |
| `deploy-docs.yml` | Manual | Manual docs deploy | Collaborators |
| `deploy-nuget.yml` | Manual | Manual NuGet deploy | Collaborators |

This structure ensures public repository security while maintaining efficient CI/CD for the team.
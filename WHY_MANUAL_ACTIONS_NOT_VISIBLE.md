# Why Manual Actions Aren't Visible in GitHub

## The Problem
You can't see the manual deployment workflows (`deploy-docs.yml` and `deploy-nuget.yml`) in the GitHub Actions UI.

## The Cause
**GitHub only shows `workflow_dispatch` actions that exist on the DEFAULT BRANCH** (usually `main`).

When workflow files are only on a PR branch or feature branch, they won't appear in the "Run workflow" dropdown in the GitHub Actions UI.

## The Solution
To make the manual actions visible, you need to:

1. **Merge these workflows to the main branch** first
2. **OR** push them directly to main if you have permissions

## How to Fix

### Option 1: Merge This PR
Once you merge this PR to main, the manual workflows will immediately appear in:
- GitHub Actions tab → "Run workflow" dropdown
- You'll see "Deploy Documentation (Manual)" and "Deploy NuGet Packages (Manual)"

### Option 2: Push Directly to Main (if you have permissions)
```bash
# If you're a maintainer, you can push directly to main
git checkout main
git pull origin main
git merge your-branch-name
git push origin main
```

## After Merge
Once the workflows are on main, you'll see them in GitHub Actions:

1. Go to **Actions** tab
2. Click **"Run workflow"** dropdown
3. Select the workflow you want to run:
   - "Deploy Documentation (Manual)"
   - "Deploy NuGet Packages (Manual)"
4. Choose environment (production/staging)
5. Click **"Run workflow"**

## Workflow Visibility Rules

| Workflow Type | Visibility Requirements |
|---------------|------------------------|
| `push` triggers | Must be on the target branch |
| `pull_request` triggers | Must be on the base branch |
| `workflow_dispatch` triggers | **Must be on the default branch** |

## Current Status
Right now, you can see:
- ✅ CI workflows (they run on PRs)
- ✅ Version validation (runs on PRs)
- ❌ Manual deployment workflows (need to be on main)

## Next Steps
1. **Merge this PR** to get the manual workflows on main
2. **Test the workflows** by running them manually
3. **Verify automatic deployment** works when you push to main

The manual workflows are properly configured - they just need to be on the main branch to become visible!
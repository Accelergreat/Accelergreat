# CLAUDE.md — Accelergreat

This file is for Claude Code and Claude-powered agents. Read `AGENTS.md` for the full Accelergreat integration testing reference. This file supplements it with Claude-specific guidance.

---

## Quick orientation

Accelergreat is a .NET integration testing framework extending xUnit. It manages databases, web APIs, and other test dependencies through **components**, automatically resets state between tests, and runs tests in parallel.

**Before writing any integration tests, read `AGENTS.md` in full.** It contains the definitive rules, patterns, and examples.

---

## Key rules (short form)

- Inherit from `AccelergreatXunitTest`, not `IClassFixture<T>`
- Create `Startup : IAccelergreatStartup` in every test project — it registers components
- Use `GetComponent<TComponent>()` inside test methods to access dependencies
- Use `DbContextFactory.NewDbContext()` — never inject `DbContext` directly
- No manual setup/teardown — `ResetAsync` is called automatically between tests
- Component order in `Startup.Configure` matters — databases before APIs

---

## When adding tests to an existing project

1. Check for an existing `Startup.cs` and `accelergreat.json` — if present, the project already uses Accelergreat
2. Check `Components/` for reusable components before creating new ones
3. Extend existing components rather than duplicating them

## When adding Accelergreat to a new project

1. Add NuGet packages (see `AGENTS.md`)
2. Create `Startup.cs` implementing `IAccelergreatStartup`
3. Create component class(es) in `Components/`
4. Create `accelergreat.json` with `$schema` and connection config
5. Create `xunit.runner.json` for parallel execution

---

## Do not

- Do not use `WebApplicationFactory<T>` directly — use `WebAppComponent<T>` or `KestrelWebAppComponent<T>`
- Do not add `[Collection("X")]` attributes unless intentionally restricting parallelism
- Do not hardcode connection strings — use `accelergreat.json` and `ACCELERGREAT_ENVIRONMENT`
- Do not write cleanup/teardown logic — Accelergreat handles this

---

## Full reference

See `AGENTS.md` for: complete code patterns, all component types, configuration schema, microservices setup, parallel execution, and project structure.

# Internal Maintenance Guide

This folder contains internal project documentation and is excluded from the public Docusaurus site.

## Documentation Management

The site uses Docusaurus with a versioning plugin.

### Where to edit
- **Current Development (Next):** Edit files in the `/docs` directory at the project root.
- **Released Version (0.3.0):** Edit files in `/site/versioned_docs/version-0.3.0/`.
- **Sidebars:**
  - Next: `/site/sidebars.js`
  - 0.3.0: `/site/versioned_sidebars/version-0.3.0-sidebars.json`

### Generated Content
The "Runnable Examples" page is generated from real code.
- **Source:** Projects in `/examples/`.
- **Generator:** `scripts/generate-example-docs.sh`.
- **How to update:** Modify the code in `/examples/`, then run `bash scripts/generate-example-docs.sh`. Do not edit `docs/examples/README.md` manually.

### Previewing Changes
Run the following to start a local development server with live reload:
```bash
bash scripts/preview-docs.sh
```
The site will be available at `http://localhost:3000`.

### Releasing New Docs Versions
When a new version (e.g., `0.4.0`) is ready:
1. Ensure the root `/docs` folder is up to date.
2. Run:
   ```bash
   cd site
   npm run version 0.4.0
   ```
3. This creates a new snapshot in `versioned_docs`.

## Release Process

1. Update `RELEASE_NOTES.md` and `fsproj` version strings.
2. Commit and push to `main` (this triggers the GitHub Pages deployment).
3. Tag the release: `git tag v0.3.0 && git push origin v0.3.0`.
4. The `release.yml` workflow will build the NuGet packages and create a GitHub Release.

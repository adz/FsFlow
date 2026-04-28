# Docusaurus maintenance notes live in [DOCS.md](DOCS.md).

## Release process

1. Update `RELEASE_NOTES.md` and `fsproj` version strings.
2. Commit and push to `main` to trigger GitHub Pages.
3. Tag the release with `git tag v0.3.0 && git push origin v0.3.0`.
4. `release.yml` builds NuGet packages and creates a GitHub Release.

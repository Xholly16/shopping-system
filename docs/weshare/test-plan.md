# WeShare Test Plan (team document)

## Running WeShare
Image name and run command: (fill in from the brief, e.g. `docker run -p 8080:8080 <image>`)
Base URL for tests: set `WESHARE_URL`.

## Team and contributions
| Member | API tests | GUI tests | Page Objects |
|--------|-----------|-----------|--------------|
|        |           |           |              |

## Scope
In scope: (list the WeShare features the team will test)
Out of scope: (what and why)

## Approach
Test Automation Pyramid: API tests (HttpClient + NUnit), GUI tests (Selenium + Page Objects).
Programmed tests only. HTML reports via `dotnet test --logger "html;LogFileName=weshare-report.html"`.

## Manual test cases
| ID | Feature | Steps | Expected | Result |
|----|---------|-------|----------|--------|

## Defect log
| ID | Description | Severity | Status |
|----|-------------|----------|--------|

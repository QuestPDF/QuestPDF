# Security Policy

**Effective Date:** 6 July 2026  
**Version:** 3.0

## Introduction

QuestPDF is a document-generation library that runs entirely within your application process. We take security reports seriously, especially when QuestPDF processes content that may originate from untrusted sources — such as text, images, SVG content, fonts, attachments, or document metadata.

## Reporting a Vulnerability

Please do not report security vulnerabilities through public GitHub issues, discussions, or pull requests.
Instead, please use one of the following channels:

1. **GitHub Private Vulnerability Reporting** (preferred) — use the ["Report a vulnerability"](https://github.com/QuestPDF/QuestPDF/security/advisories/new) form.
2. **Email** — contact contact@questpdf.com with a description of the issue.

## What to Include

When reporting, please include as much of the following as possible:

- The affected QuestPDF version.
- The operating system, CPU architecture, and .NET runtime version.
- A description of the vulnerability and its potential impact.
- A minimal reproduction or proof of concept.
- The expected security impact and a realistic attack scenario.
- Whether untrusted input is required to trigger the issue.
- Any known workaround or mitigation.

Please avoid including confidential customer data in your report. If possible, use a minimal synthetic reproduction.

## Response Process

We will use commercially reasonable efforts to address security reports promptly:

- **Acknowledgment**: we aim to confirm receipt of your report within a few business days.
- **Assessment**: we will evaluate the report and let you know whether the issue is accepted as a vulnerability.
- **Resolution**: confirmed vulnerabilities are prioritised based on severity and complexity. Critical issues may be patched ahead of the next scheduled release.

We will coordinate with you before any public disclosure and are happy to credit reporters in release notes (unless you prefer to remain anonymous).

## Safe Harbour

We will not pursue legal action against good-faith security research conducted under this policy.

Research is conducted in good faith when it stays within the scope described in this policy, avoids privacy violations, data destruction, and degradation of services for others, does not exploit a finding beyond what is necessary to demonstrate it, and follows the coordinated disclosure process described below.

Good-faith security research conducted under this policy is authorised notwithstanding any reverse-engineering restriction in the applicable License Agreement or Community License.

## Supported Versions

Security fixes are provided through the latest stable QuestPDF release only. We do not maintain separate patch branches for previous minor or major releases.

For example, if the latest release is `2026.6.2` and a vulnerability is discovered, the fix will ship as `2026.6.3`. Earlier releases such as `2026.3.Y` or `2025.X.Y` will not receive a separate patch.

This approach works well in practice because QuestPDF maintains strict backwards compatibility — we treat breaking changes as a last resort. Updating to the latest version is, in the vast majority of cases, a straightforward operation. This lets us focus our efforts on a single, well-tested release rather than maintaining parallel patch branches, resulting in faster and more reliable fixes for everyone.

Access to a patched release requires an active Update and Support Term; Community License users can always use the latest release free of charge.

## Scope

Reports are generally in scope when they involve:

- Vulnerabilities in the QuestPDF library.
- Vulnerabilities in bundled native dependencies (Skia, qpdf) when exploitable through QuestPDF.
- Malformed or untrusted input — such as SVG, images, fonts, text, or document metadata — that leads to unexpected behaviour.
- Denial of service scenarios, including unexpected infinite loops, thread deadlocks, or unbounded memory allocations triggered by specific layout configurations or input.
- Arbitrary file access, information disclosure, memory corruption, or remote code execution.

Reports are generally out of scope when they involve:

- Layout differences, rendering bugs, PDF compliance issues, or accessibility issues without a security impact — please report these as regular [GitHub issues](https://github.com/QuestPDF/QuestPDF/issues).
- The QuestPDF website (questpdf.com) — please report website issues via email.
- Third-party integrations, forks, or wrappers not maintained by QuestPDF.
- Vulnerabilities in the host application, its infrastructure, or PDF viewers.
- Scenarios that require already-compromised application code or unrestricted local machine access.
- License, billing, or support questions.

## Security Considerations

QuestPDF is a PDF generation library that runs entirely within your application:

- **No network access**: QuestPDF makes no external API calls, does not require an internet connection, and performs no telemetry or data collection.
- **No remote data processing**: QuestPDF does not send document content to QuestPDF servers or external APIs. Document generation happens locally, inside your application process.
- **Native dependencies**: QuestPDF bundles native binaries (Skia for rendering, qpdf for document operations). These are built from source in our CI pipelines and distributed as platform-specific NuGet packages. We track upstream releases and apply security patches as part of our regular update cycle.

## Disclosure Policy

We follow coordinated disclosure practices:

- Security issues are handled privately until a fix is available.
- Once a fix is released, we may publish a GitHub Security Advisory and include details in the release notes when appropriate.
- We request that reporters do not publicly disclose vulnerability details until we have released a fix and published an advisory.

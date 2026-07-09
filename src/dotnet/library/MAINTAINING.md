# QuestPDF - Maintenance

## Updating Skia

In the QuestPDF.Native.Skia repository:

1. Sync the fork with the upstream Skia repository.
2. Checkout target milestone branch.
3. Cherry-pick custom QuestPDF-specific commits from previous supported milestone branch.
4. Resolve any conflicts.
5. Push to changes to the repository.

In the QuestPDF.Native.Skia.Build repository:

6. Update the `main.yaml` GitHub Action workflow, changing the Skia branch name in the `Build Skia` step.
7. Increment the QuestPDF Compatibility version in both native and managed code (search for `get_questpdf_version` usage).
8. Run the GitHub Action to build the repository.
9. Some tests may fail during build (e.g. Skia optimizes how PDF are created) - revisit if the output is correct, and adjust test cases in the managed project.
10. Final native artifacts are available to download from last GitHub Action run.

In the QuestPDF repository:

11. Merge new Skia native artifacts to the QuestPDF/Runtimes folder.
12. Update the QuestPDF Compatibility version in SkNativeDependencyCompatibilityChecker.
13. Commit changes.

## Updating Qpdf

In the QuestPDF.Native.Qpdf repository:

1. Sync the fork with the upstream Qpdf repository.
2. Checkout target release branch.
3. Cherry-pick custom QuestPDF-specific commits from previous supported release branch.
4. Resolve any conflicts.
5. Increment the QuestPDF Compatibility version in the native code (search for `get_questpdf_version` usage).
6. Push to changes to the repository.

In the QuestPDF.Native.Qpdf.Build repository:

7. Update the `main.yaml` GitHub Action workflow, changing the Qpdf branch name in the `Download QPDF` step.
7. Increment the QuestPDF Compatibility version in the managed code (search for `get_questpdf_version` usage).
8. Run the GitHub Action to build the repository. 
9. Final native artifacts are available to download from last GitHub Action run.

In the QuestPDF repository:

11. Merge new Qpdf native artifacts to the QuestPDF/Runtimes folder.
12. Update the QuestPDF Compatibility version in QpdfNativeDependencyCompatibilityChecker.
13. Commit changes.

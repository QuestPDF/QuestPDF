#!/usr/bin/env zx
// Builds the .NET Framework integration app against the packed QuestPDF package and runs it.
// .NET Framework has no publish model, so this uses dotnet build and the bin output directly.
import {
  integrationPath,
  outputFileNames,
  repoPath,
  runCli,
  runExecutable,
  validatePdf,
  validateXps
} from './integration-test-utils.mjs';

await runCli(async () => {
  const [packageVersion, targetFramework, platformTarget] = argv._;

  if (!packageVersion || !targetFramework || !platformTarget)
    throw new Error('Usage: zx run-netframework-test.mjs <package-version> <target-framework> <platform-target>');

  const project = integrationPath('QuestPDF.Tests.NetFramework', 'QuestPDF.Tests.NetFramework.csproj');
  const nugetConfig = integrationPath('nuget.config');
  const outputDirectory = repoPath('artifacts/integration-output/netframework', targetFramework, platformTarget);
  const executable = integrationPath('QuestPDF.Tests.NetFramework/bin/Release', targetFramework, 'QuestPDF.Tests.NetFramework.exe');

  const properties = [
    `-p:QuestPDFIntegrationVersion=${packageVersion}`,
    `-p:QuestPDFIntegrationTargetFramework=${targetFramework}`,
    `-p:QuestPDFNetFrameworkTargetFramework=${targetFramework}`,
    `-p:PlatformTarget=${platformTarget}`
  ];

  console.log(chalk.blue(`Building .NET Framework ${targetFramework} / ${platformTarget}`));
  await fs.remove(outputDirectory);
  await fs.ensureDir(outputDirectory);

  await $`dotnet restore ${project} --configfile ${nugetConfig} ${properties}`;
  await $`dotnet build ${project} --configuration Release --framework ${targetFramework} --no-restore ${properties}`;

  await runExecutable(executable, [platformTarget], outputDirectory);
  await validatePdf(`${outputDirectory}/${outputFileNames.skiaPdf}`);
  await validatePdf(`${outputDirectory}/${outputFileNames.qpdfPdf}`);
  await validateXps(`${outputDirectory}/${outputFileNames.xps}`);
});

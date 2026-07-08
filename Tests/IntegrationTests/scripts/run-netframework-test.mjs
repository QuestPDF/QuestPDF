#!/usr/bin/env zx
import {
  $,
  argv,
  chalk,
  cliPath,
  errorMessage,
  fs,
  integrationPath,
  outputFileNames,
  path,
  repoPath,
  runExecutable,
  validatePdf,
  validateXps
} from './integration-test-utils.mjs';

const targetFrameworks = ['net472', 'net481'];
const platformTargets = ['x64', 'x86', 'AnyCPU'];

export async function runNetFrameworkTest({ packageVersion, targetFramework, platformTarget }) {
  if (!packageVersion)
    throw new Error('QuestPDF package version is required.');

  if (!targetFrameworks.includes(targetFramework))
    throw new Error(`Unknown .NET Framework target: ${targetFramework}. Expected one of: ${targetFrameworks.join(', ')}.`);

  if (!platformTargets.includes(platformTarget))
    throw new Error(`Unknown platform target: ${platformTarget}. Expected one of: ${platformTargets.join(', ')}.`);

  const project = integrationPath('QuestPDF.Tests.NetFramework', 'QuestPDF.Tests.NetFramework.csproj');
  const nugetConfig = integrationPath('nuget.config');
  const artifactsRoot = repoPath('artifacts/integration/netframework', targetFramework, platformTarget);
  const outputDirectory = repoPath('artifacts/integration-output/netframework', targetFramework, platformTarget);
  const executable = integrationPath('QuestPDF.Tests.NetFramework/bin/Release', targetFramework, 'QuestPDF.Tests.NetFramework.exe');

  console.log(chalk.blue(`Building .NET Framework ${targetFramework} / ${platformTarget}`));
  await fs.remove(artifactsRoot);
  await fs.remove(outputDirectory);
  await fs.ensureDir(outputDirectory);

  await $`dotnet restore ${cliPath(project)} --configfile ${cliPath(nugetConfig)} -p:QuestPDFIntegrationVersion=${packageVersion} -p:QuestPDFIntegrationTargetFramework=${targetFramework} -p:QuestPDFNetFrameworkTargetFramework=${targetFramework} -p:PlatformTarget=${platformTarget}`;

  await $`dotnet build ${cliPath(project)} --configuration Release --framework ${targetFramework} --no-restore -p:QuestPDFIntegrationVersion=${packageVersion} -p:QuestPDFIntegrationTargetFramework=${targetFramework} -p:QuestPDFNetFrameworkTargetFramework=${targetFramework} -p:PlatformTarget=${platformTarget}`;

  await runExecutable(executable, [platformTarget], { cwd: outputDirectory });
  await validatePdf(path.join(outputDirectory, outputFileNames.skiaPdf));
  await validatePdf(path.join(outputDirectory, outputFileNames.qpdfPdf));
  await validateXps(path.join(outputDirectory, outputFileNames.xps));
}

try {
  const [packageVersionArg, targetFrameworkArg, platformTargetArg] = argv._;

  await runNetFrameworkTest({
    packageVersion: argv['package-version'] ?? argv.version ?? packageVersionArg,
    targetFramework: argv.framework ?? argv['target-framework'] ?? targetFrameworkArg ?? 'net472',
    platformTarget: argv.platform ?? argv['platform-target'] ?? platformTargetArg ?? 'AnyCPU'
  });
} catch (error) {
  console.error(chalk.red(errorMessage(error)));
  process.exit(1);
}

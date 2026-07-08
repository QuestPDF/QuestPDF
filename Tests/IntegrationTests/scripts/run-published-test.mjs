#!/usr/bin/env zx
import {
  $,
  argv,
  chalk,
  errorMessage,
  fetchToFile,
  fs,
  isMain,
  integrationPath,
  isWindowsRuntime,
  kill,
  outputFileNames,
  path,
  repoPath,
  runExecutable,
  validatePdf,
  validateXps,
  waitForHttpServer
} from './integration-test-utils.mjs';

const appConfigurations = new Map([
  ['console', { projectName: 'QuestPDF.Tests.Console', selfContained: false }],
  ['console-aot', { projectName: 'QuestPDF.Tests.Console.Aot', selfContained: true }],
  ['console-singlefile', { projectName: 'QuestPDF.Tests.Console.SingleFile', selfContained: true }],
  ['webapi', { projectName: 'QuestPDF.Tests.WebApi', selfContained: false }],
  ['worker', { projectName: 'QuestPDF.Tests.Worker', selfContained: false }]
]);

export async function runPublishedTest({ appType, targetFramework, rid, packageVersion }) {
  if (!appType)
    throw new Error('App type is required.');

  if (!targetFramework)
    throw new Error('Target framework is required.');

  if (!rid)
    throw new Error('Runtime identifier is required.');

  if (!packageVersion)
    throw new Error('QuestPDF package version is required.');

  const appConfiguration = appConfigurations.get(appType);

  if (!appConfiguration)
    throw new Error(`Unknown app type: ${appType}. Expected one of: ${[...appConfigurations.keys()].join(', ')}.`);

  const projectName = appConfiguration.projectName;
  const project = integrationPath(projectName, `${projectName}.csproj`);
  const nugetConfig = integrationPath('nuget.config');
  const artifactsRoot = repoPath('artifacts/integration', appType, targetFramework, rid);
  const publishDirectory = path.join(artifactsRoot, 'publish');
  const outputDirectory = repoPath('artifacts/integration-output', appType, targetFramework, rid);
  const logDirectory = path.join(artifactsRoot, 'logs');
  const isWindows = isWindowsRuntime(rid);

  console.log(chalk.blue(`Publishing ${appType} for ${targetFramework} / ${rid}`));
  await fs.remove(artifactsRoot);
  await fs.remove(outputDirectory);
  await fs.ensureDir(publishDirectory);
  await fs.ensureDir(outputDirectory);
  await fs.ensureDir(logDirectory);

  await $`dotnet restore ${project} --configfile ${nugetConfig} --runtime ${rid} -p:QuestPDFIntegrationVersion=${packageVersion} -p:QuestPDFIntegrationTargetFramework=${targetFramework}`;

  await $`dotnet publish ${project} --configuration Release --framework ${targetFramework} --runtime ${rid} --self-contained ${String(appConfiguration.selfContained)} --output ${publishDirectory} --no-restore -p:QuestPDFIntegrationVersion=${packageVersion} -p:QuestPDFIntegrationTargetFramework=${targetFramework}`;

  const executable = path.join(publishDirectory, isWindows ? `${projectName}.exe` : projectName);

  if (appType === 'webapi')
    await runWebApiTest({ executable, outputDirectory, logDirectory, isWindows });
  else
    await runConsoleStyleTest({ executable, outputDirectory, isWindows });
}

async function runWebApiTest({ executable, outputDirectory, logDirectory, isWindows }) {
  const port = process.env.QUESTPDF_TEST_PORT ?? '5087';
  const baseUrl = `http://127.0.0.1:${port}`;
  const logFile = path.join(logDirectory, 'webapi.log');

  console.log(chalk.cyan(`Starting Web API at ${baseUrl}`));

  const serverPid = (await $({ cwd: path.dirname(executable) })`ASPNETCORE_URLS=${baseUrl} ${executable} > ${logFile} 2>&1 & echo $!`.quiet()).stdout.trim();

  try {
    await waitForHttpServer(`${baseUrl}/health`, logFile);

    const skiaPdfResponse = path.join(outputDirectory, outputFileNames.skiaPdf);
    const qpdfPdfResponse = path.join(outputDirectory, outputFileNames.qpdfPdf);
    const xpsResponse = path.join(outputDirectory, outputFileNames.xps);

    await fetchToFile(`${baseUrl}/generate-skia-pdf`, skiaPdfResponse);
    await validatePdf(skiaPdfResponse);

    await fetchToFile(`${baseUrl}/generate-pdf`, qpdfPdfResponse);
    await validatePdf(qpdfPdfResponse);

    if (isWindows) {
      await fetchToFile(`${baseUrl}/generate-xps`, xpsResponse);
      await validateXps(xpsResponse);
    }
  } finally {
    try {
      await kill(serverPid);
    } catch {
      // The server may have already stopped after a failing request.
    }
  }
}

async function runConsoleStyleTest({ executable, outputDirectory, isWindows }) {
  await runExecutable(executable, [], { cwd: outputDirectory });
  await validatePdf(path.join(outputDirectory, outputFileNames.skiaPdf));
  await validatePdf(path.join(outputDirectory, outputFileNames.qpdfPdf));

  if (isWindows)
    await validateXps(path.join(outputDirectory, outputFileNames.xps));
}

if (isMain(import.meta.url)) {
  try {
    const [appTypeArg, targetFrameworkArg, ridArg, packageVersionArg] = argv._;

    await runPublishedTest({
      appType: argv.app ?? appTypeArg,
      targetFramework: argv.framework ?? argv['target-framework'] ?? targetFrameworkArg,
      rid: argv.runtime ?? argv.rid ?? argv['runtime-identifier'] ?? ridArg,
      packageVersion: argv['package-version'] ?? argv.version ?? packageVersionArg
    });
  } catch (error) {
    console.error(chalk.red(errorMessage(error)));
    process.exit(1);
  }
}

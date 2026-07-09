#!/usr/bin/env zx

// Publishes every package-test app against a locally packed QuestPDF NuGet package,
// runs the resulting binaries, and verifies that the produced documents are valid.
//
// Usage:
//   zx run-tests.mjs [--version <package-version>] --framework <net6.0|net8.0|net10.0> --runtime <rid> [--app <name>]
//   zx run-tests.mjs [--version <package-version>] --framework <net472|net481> --platform <x64|x86|AnyCPU>
//
// Without --version, the script packs the QuestPDF library into <repo>/artifacts/nupkg with a unique
// version, so local changes are always tested. With --version, it uses the already packed package (CI).

import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';

$.verbose = true;

// Forward slashes work on every supported OS and avoid backslash escaping issues in zx commands.
const testsRoot = path.dirname(fileURLToPath(import.meta.url)).replaceAll('\\', '/');
const repoRoot = path.resolve(testsRoot, '../../../..').replaceAll('\\', '/');
const nugetConfig = `${testsRoot}/nuget.config`;
const packageFeed = `${repoRoot}/artifacts/nupkg`;
const testOutputRoot = `${repoRoot}/artifacts/test-output`;

// Must match TestRunner.OutputFolder in QuestPDF.PackageTests.Shared.
const outputFolderName = 'TestOutput';

const apps = {
  'console': { project: 'QuestPDF.PackageTests.Console' },
  'aot': { project: 'QuestPDF.PackageTests.AOT', selfContained: true },
  'single-file': { project: 'QuestPDF.PackageTests.SingleFile', selfContained: true },
  'webapi': { project: 'QuestPDF.PackageTests.WebAPI' },
  'worker': { project: 'QuestPDF.PackageTests.Worker' },
  'netframework': { project: 'QuestPDF.PackageTests.NetFramework' }
};

const { framework, runtime, platform, app: appFilter } = argv;
let version = argv.version;
const isNetFramework = /^net4/.test(framework ?? '');
const isWindows = isNetFramework || (runtime ?? '').startsWith('win-');

try {
  await main();
} catch (error) {
  console.error(chalk.red(error?.message ?? String(error)));
  process.exitCode = 1;
}

async function main() {
  if (!framework || (isNetFramework ? !platform : !runtime))
    throw new Error(
      'Usage:\n' +
      '  zx run-tests.mjs [--version <package-version>] --framework <net6.0|net8.0|net10.0> --runtime <rid> [--app <name>]\n' +
      '  zx run-tests.mjs [--version <package-version>] --framework <net472|net481> --platform <x64|x86|AnyCPU>');

  if (!version)
    version = await packLibrary();
  else if (!await fs.pathExists(`${packageFeed}/QuestPDF.${version}.nupkg`))
    throw new Error(`QuestPDF ${version} was not found in ${packageFeed}. Omit --version to pack the library automatically.`);

  // Intermediates left by a previous run with a different target framework can poison the restore.
  const projects = [...Object.values(apps).map(app => app.project), 'QuestPDF.PackageTests.Shared'];

  for (const project of projects)
    await Promise.all([fs.remove(`${testsRoot}/${project}/obj`), fs.remove(`${testsRoot}/${project}/bin`)]);

  const failures = [];

  for (const name of selectApps()) {
    console.log(`::group::${name}`);

    try {
      await runApp(name);
      console.log(chalk.green(`${name} passed`));
    } catch (error) {
      failures.push(name);
      console.error(chalk.red(`${name} failed: ${error?.message ?? error}`));
    } finally {
      console.log('::endgroup::');
    }
  }

  if (failures.length > 0)
    throw new Error(`${failures.length} app(s) failed: ${failures.join(', ')}.`);

  console.log(chalk.green('All package tests passed.'));
}

async function packLibrary() {
  const projectFile = `${repoRoot}/src/dotnet/library/QuestPDF/QuestPDF.csproj`;
  const baseVersion = (await fs.readFile(projectFile, 'utf8')).match(/<Version>([^<]+)<\/Version>/)?.[1];

  if (!baseVersion)
    throw new Error(`The <Version> property was not found in ${projectFile}.`);

  // A unique version prevents NuGet from silently reusing a previously cached package.
  const packVersion = `${baseVersion}-local.${new Date().toISOString().replaceAll(/\D/g, '').slice(0, 14)}`;

  // Previous local packages are no longer reachable and would only grow the caches.
  const cachedPackages = `${process.env.NUGET_PACKAGES ?? `${os.homedir()}/.nuget/packages`}/questpdf`;

  for (const cached of await fs.readdir(cachedPackages).catch(() => []))
    if (cached.includes('-local.'))
      await fs.remove(`${cachedPackages}/${cached}`);

  await fs.remove(packageFeed);

  console.log(chalk.blue(`Packing QuestPDF ${packVersion}`));
  await $({ cwd: path.dirname(projectFile) })`dotnet build QuestPDF.csproj --configuration Release -p:BUILD_PACKAGE=true -p:Version=${packVersion} -p:PackageOutputPath=${packageFeed} -p:WarningLevel=0`;

  return packVersion;
}

function selectApps() {
  if (isNetFramework)
    return ['netframework'];

  const available = Object.keys(apps).filter(name => name !== 'netframework');

  if (appFilter) {
    if (!available.includes(appFilter))
      throw new Error(`Unknown app: ${appFilter}. Expected one of: ${available.join(', ')}.`);

    return [appFilter];
  }

  // Native AOT is not available for net6.0 apps or the win-x86 runtime.
  const supportsAot = framework !== 'net6.0' && runtime !== 'win-x86';
  return available.filter(name => name !== 'aot' || supportsAot);
}

async function runApp(name) {
  const binDirectory = isNetFramework ? await buildNetFrameworkApp(name) : await publishApp(name);
  const executable = `${binDirectory}/${apps[name].project}${isWindows ? '.exe' : ''}`;

  // Apps read resources from and write documents to their working directory,
  // so they must run from the directory that contains the executable.
  if (name === 'webapi')
    await runWebApiApp(executable);
  else
    await runConsoleApp(executable);

  const outputDirectory = `${binDirectory}/${outputFolderName}`;
  await validateDocuments(outputDirectory);

  const collectedOutput = `${testOutputRoot}/${name}`;
  await fs.remove(collectedOutput);
  await fs.copy(outputDirectory, collectedOutput);
}

async function publishApp(name) {
  const { project, selfContained = false } = apps[name];
  const projectFile = `${testsRoot}/${project}/${project}.csproj`;
  const publishDirectory = `${repoRoot}/artifacts/publish/${name}`;

  const properties = [
    `-p:QuestPDFIntegrationVersion=${version}`,
    `-p:QuestPDFIntegrationTargetFramework=${framework}`
  ];

  await fs.remove(publishDirectory);
  await $`dotnet restore ${projectFile} --configfile ${nugetConfig} --runtime ${runtime} ${properties}`;
  await $`dotnet publish ${projectFile} --configuration Release --runtime ${runtime} --self-contained ${String(selfContained)} --output ${publishDirectory} --no-restore ${properties}`;

  return publishDirectory;
}

async function buildNetFrameworkApp(name) {
  const { project } = apps[name];
  const projectFile = `${testsRoot}/${project}/${project}.csproj`;
  const binDirectory = `${testsRoot}/${project}/bin/Release/${framework}`;

  const properties = [
    `-p:QuestPDFIntegrationVersion=${version}`,
    `-p:QuestPDFIntegrationTargetFramework=${framework}`,
    `-p:QuestPDFNetFrameworkTargetFramework=${framework}`,
    `-p:PlatformTarget=${platform}`
  ];

  await $`dotnet restore ${projectFile} --configfile ${nugetConfig} ${properties}`;
  await $`dotnet build ${projectFile} --configuration Release --no-restore ${properties}`;

  return binDirectory;
}

async function runConsoleApp(executable) {
  console.log(chalk.cyan(`Running ${path.basename(executable)}`));
  await $({ cwd: path.dirname(executable), stdio: 'inherit' })`${executable}`;
}

async function runWebApiApp(executable) {
  const baseUrl = 'http://127.0.0.1:5087';
  console.log(chalk.cyan(`Starting ${path.basename(executable)} at ${baseUrl}`));

  const server = spawn(executable, {
    cwd: path.dirname(executable),
    env: { ...process.env, ASPNETCORE_URLS: baseUrl },
    stdio: ['ignore', 'inherit', 'inherit']
  });

  const serverFailure = new Promise((resolve, reject) => {
    server.once('error', reject);
    server.once('exit', (code, signal) => reject(new Error(`Web API exited prematurely (code: ${code}, signal: ${signal}).`)));
  });

  // The rejection is expected when the server is stopped after a successful test.
  serverFailure.catch(() => {});

  try {
    await Promise.race([serverFailure, waitForServer(`${baseUrl}/health`)]);

    const response = await fetch(`${baseUrl}/generate`);

    if (!response.ok)
      throw new Error(`GET /generate returned HTTP ${response.status}.`);

    const archive = Buffer.from(await response.arrayBuffer());

    if (!hasHeader(archive, 'PK\x03\x04'))
      throw new Error('The /generate endpoint did not return a valid ZIP archive.');

    console.log(chalk.green(`Downloaded document archive (${archive.length} bytes)`));
  } finally {
    await stopServer(server);
  }
}

async function waitForServer(url) {
  await retry(60, '1s', async () => {
    const response = await fetch(url);

    if (!response.ok)
      throw new Error(`Health check returned HTTP ${response.status}.`);
  });

  console.log(chalk.green(`Server is ready at ${url}`));
}

async function stopServer(server) {
  for (const signal of ['SIGTERM', 'SIGKILL']) {
    if (server.exitCode !== null || server.signalCode !== null)
      return;

    server.kill(signal);
    await Promise.race([new Promise(resolve => server.once('exit', resolve)), sleep(5000)]);
  }
}

async function validateDocuments(outputDirectory) {
  await validatePdf(`${outputDirectory}/skia.pdf`);
  await validatePdf(`${outputDirectory}/qpdf.pdf`);

  if (isWindows)
    await validateXps(`${outputDirectory}/skia.xps`);
}

async function validatePdf(filePath) {
  const buffer = await readDocument(filePath, '%PDF-');

  if (!buffer.subarray(-2048).includes('%%EOF'))
    throw new Error(`PDF EOF marker was not found: ${filePath}`);
}

async function validateXps(filePath) {
  await readDocument(filePath, 'PK\x03\x04');
}

async function readDocument(filePath, expectedHeader) {
  if (!await fs.pathExists(filePath))
    throw new Error(`Expected output file was not created: ${filePath}`);

  const buffer = await fs.readFile(filePath);

  if (buffer.length < 1024)
    throw new Error(`Output file is suspiciously small: ${filePath} (${buffer.length} bytes).`);

  if (!hasHeader(buffer, expectedHeader))
    throw new Error(`Output file has an invalid header: ${filePath}`);

  console.log(chalk.green(`Validated ${filePath} (${buffer.length} bytes)`));
  return buffer;
}

function hasHeader(buffer, header) {
  return buffer.subarray(0, header.length).toString('latin1') === header;
}

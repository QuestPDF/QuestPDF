// Publishes one integration app against the packed QuestPDF package and runs its published output.
// Invoked by run-published-suite.mjs; not a standalone CLI.
import { spawn } from 'node:child_process';

import {
  fetchToFile,
  integrationPath,
  outputFileNames,
  repoPath,
  runExecutable,
  validatePdf,
  validateXps,
  waitForHttpServer
} from './integration-test-utils.mjs';

const appConfigurations = {
  'console': { projectName: 'QuestPDF.Tests.Console', selfContained: false },
  'console-aot': { projectName: 'QuestPDF.Tests.Console.Aot', selfContained: true },
  'console-singlefile': { projectName: 'QuestPDF.Tests.Console.SingleFile', selfContained: true },
  'webapi': { projectName: 'QuestPDF.Tests.WebApi', selfContained: false },
  'worker': { projectName: 'QuestPDF.Tests.Worker', selfContained: false }
};

export const appTypes = Object.keys(appConfigurations);

export async function runPublishedTest({ appType, targetFramework, rid, packageVersion }) {
  const { projectName, selfContained } = appConfigurations[appType];
  const project = integrationPath(projectName, `${projectName}.csproj`);
  const nugetConfig = integrationPath('nuget.config');
  const artifactsRoot = repoPath('artifacts/integration', appType, targetFramework, rid);
  const publishDirectory = `${artifactsRoot}/publish`;
  const logDirectory = `${artifactsRoot}/logs`;
  const outputDirectory = repoPath('artifacts/integration-output', appType, targetFramework, rid);
  const isWindows = rid.startsWith('win-');

  console.log(chalk.blue(`Publishing ${appType} for ${targetFramework} / ${rid}`));
  await fs.remove(artifactsRoot);
  await fs.remove(outputDirectory);
  await fs.ensureDir(outputDirectory);
  await fs.ensureDir(logDirectory);

  const properties = [
    `-p:QuestPDFIntegrationVersion=${packageVersion}`,
    `-p:QuestPDFIntegrationTargetFramework=${targetFramework}`
  ];

  await $`dotnet restore ${project} --configfile ${nugetConfig} --runtime ${rid} ${properties}`;
  await $`dotnet publish ${project} --configuration Release --framework ${targetFramework} --runtime ${rid} --self-contained ${String(selfContained)} --output ${publishDirectory} --no-restore ${properties}`;

  const executable = `${publishDirectory}/${projectName}${isWindows ? '.exe' : ''}`;

  if (appType === 'webapi')
    await runWebApiTest({ executable, outputDirectory, logDirectory, isWindows });
  else
    await runConsoleStyleTest({ executable, outputDirectory, isWindows });
}

async function runConsoleStyleTest({ executable, outputDirectory, isWindows }) {
  await runExecutable(executable, [], outputDirectory);
  await validatePdf(`${outputDirectory}/${outputFileNames.skiaPdf}`);
  await validatePdf(`${outputDirectory}/${outputFileNames.qpdfPdf}`);

  if (isWindows)
    await validateXps(`${outputDirectory}/${outputFileNames.xps}`);
}

async function runWebApiTest({ executable, outputDirectory, logDirectory, isWindows }) {
  const baseUrl = 'http://127.0.0.1:5087';
  const logFile = `${logDirectory}/webapi.log`;

  console.log(chalk.cyan(`Starting Web API at ${baseUrl}`));

  const logStream = fs.createWriteStream(logFile, { flags: 'w' });
  const server = spawn(executable, [], {
    cwd: path.dirname(executable),
    env: { ...process.env, ASPNETCORE_URLS: baseUrl },
    stdio: ['ignore', 'pipe', 'pipe']
  });

  server.stdout.pipe(logStream);
  server.stderr.pipe(logStream);

  try {
    const serverExit = new Promise((resolve, reject) => {
      server.once('error', reject);
      server.once('exit', (code, signal) => resolve({ code, signal }));
    });

    await Promise.race([
      waitForHttpServer(`${baseUrl}/health`, logFile),
      serverExit.then(({ code, signal }) => {
        throw new Error(`Web API exited before becoming ready (code: ${code}, signal: ${signal}).`);
      })
    ]);

    const skiaPdfPath = `${outputDirectory}/${outputFileNames.skiaPdf}`;
    const qpdfPdfPath = `${outputDirectory}/${outputFileNames.qpdfPdf}`;

    await fetchToFile(`${baseUrl}/generate-skia-pdf`, skiaPdfPath);
    await validatePdf(skiaPdfPath);

    await fetchToFile(`${baseUrl}/generate-pdf`, qpdfPdfPath);
    await validatePdf(qpdfPdfPath);

    if (isWindows) {
      const xpsPath = `${outputDirectory}/${outputFileNames.xps}`;
      await fetchToFile(`${baseUrl}/generate-xps`, xpsPath);
      await validateXps(xpsPath);
    }
  } finally {
    await stopServer(server, logStream);
  }
}

async function stopServer(server, logStream) {
  for (const signal of ['SIGTERM', 'SIGKILL']) {
    if (server.exitCode !== null || server.signalCode !== null)
      break;

    server.kill(signal);

    await Promise.race([
      new Promise(resolve => server.once('exit', resolve)),
      new Promise(resolve => setTimeout(resolve, 5000))
    ]);
  }

  await new Promise(resolve => logStream.end(resolve));
}

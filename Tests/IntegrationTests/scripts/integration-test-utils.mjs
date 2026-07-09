// Shared helpers for the integration test scripts.
// All scripts run through the zx CLI, which provides $, argv, chalk, fs, path, and retry as globals.
import { fileURLToPath } from 'node:url';

const scriptsRoot = path.dirname(fileURLToPath(import.meta.url));
const integrationRoot = path.resolve(scriptsRoot, '..');
const repoRoot = path.resolve(scriptsRoot, '../../..');

export const outputFileNames = {
  skiaPdf: 'skia.pdf',
  qpdfPdf: 'qpdf.pdf',
  xps: 'skia.xps'
};

// Forward slashes work on every supported OS and avoid backslash escaping issues in zx commands.
export function integrationPath(...segments) {
  return path.join(integrationRoot, ...segments).replaceAll('\\', '/');
}

export function repoPath(...segments) {
  return path.join(repoRoot, ...segments).replaceAll('\\', '/');
}

export async function runCli(main) {
  try {
    await main();
  } catch (error) {
    console.error(chalk.red(errorMessage(error)));
    process.exitCode = 1;
  }
}

export function errorMessage(error) {
  return error?.message ?? String(error);
}

export async function runExecutable(executable, args, cwd) {
  console.log(chalk.cyan(`Running ${path.basename(executable)} ${args.join(' ')}`.trimEnd()));
  await $({ cwd, stdio: 'inherit' })`${executable} ${args}`;
}

export async function waitForHttpServer(url, logFile) {
  try {
    await retry(60, '1s', async () => {
      const response = await fetch(url);

      if (!response.ok)
        throw new Error(`Health check returned HTTP ${response.status}.`);
    });

    console.log(chalk.green(`Server is ready at ${url}`));
  } catch (error) {
    throw new Error(`Server did not become ready at ${url}: ${errorMessage(error)}${await readLog(logFile)}`);
  }
}

async function readLog(filePath) {
  if (!filePath || !await fs.pathExists(filePath))
    return '';

  return `\nServer log:\n${await fs.readFile(filePath, 'utf8')}`;
}

export async function fetchToFile(url, outputPath) {
  const response = await fetch(url);

  if (!response.ok)
    throw new Error(`GET ${url} returned HTTP ${response.status}.`);

  await fs.ensureDir(path.dirname(outputPath));
  await fs.writeFile(outputPath, Buffer.from(await response.arrayBuffer()));
  console.log(chalk.green(`Saved ${outputPath}`));
}

export async function validatePdf(pdfPath) {
  const buffer = await readOutputFile(pdfPath, 'PDF');

  if (buffer.subarray(0, 5).toString('latin1') !== '%PDF-')
    throw new Error(`PDF header is invalid: ${pdfPath}.`);

  if (!buffer.subarray(-2048).toString('latin1').includes('%%EOF'))
    throw new Error(`PDF EOF marker was not found: ${pdfPath}.`);

  console.log(chalk.green(`Validated PDF ${pdfPath} (${buffer.length} bytes)`));
}

export async function validateXps(xpsPath) {
  const buffer = await readOutputFile(xpsPath, 'XPS');

  if (buffer.readUInt32BE(0) !== 0x504b0304)
    throw new Error(`XPS file does not start with the ZIP package header: ${xpsPath}.`);

  console.log(chalk.green(`Validated XPS ${xpsPath} (${buffer.length} bytes)`));
}

async function readOutputFile(filePath, kind) {
  if (!await fs.pathExists(filePath))
    throw new Error(`Expected ${kind} file was not created: ${filePath}`);

  const buffer = await fs.readFile(filePath);

  if (buffer.length < 1024)
    throw new Error(`${kind} file is too small to be meaningful: ${filePath} (${buffer.length} bytes).`);

  return buffer;
}

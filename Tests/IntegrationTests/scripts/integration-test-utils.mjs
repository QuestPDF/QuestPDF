import { fileURLToPath } from 'node:url';

export const $ = globalThis.$;
export const argv = globalThis.argv;
export const chalk = globalThis.chalk;
export const fetch = globalThis.fetch;
export const fs = globalThis.fs;
export const kill = globalThis.kill;
export const path = globalThis.path;
export const retry = globalThis.retry;

export const scriptsRoot = path.dirname(fileURLToPath(import.meta.url));
export const integrationRoot = path.resolve(scriptsRoot, '..');
export const repoRoot = path.resolve(scriptsRoot, '../../..');

export const outputFileNames = {
  skiaPdf: 'skia.pdf',
  qpdfPdf: 'qpdf.pdf',
  xps: 'skia.xps'
};

export function integrationPath(...segments) {
  return path.join(integrationRoot, ...segments);
}

export function repoPath(...segments) {
  return path.join(repoRoot, ...segments);
}

export function isWindowsRuntime(rid) {
  return rid.startsWith('win-');
}

export function isMain(moduleUrl) {
  const modulePath = path.resolve(fileURLToPath(moduleUrl));
  const candidatePaths = process.argv
    .slice(1, 3)
    .filter(Boolean)
    .map(x => path.resolve(x));

  return candidatePaths.includes(modulePath);
}

export async function runExecutable(executable, args = [], options = {}) {
  const displayArgs = args.length > 0 ? ` ${args.join(' ')}` : '';
  console.log(chalk.cyan(`Running ${path.basename(executable)}${displayArgs}`));

  await $({ cwd: options.cwd, env: { ...process.env, ...options.env } })`${executable} ${args}`;
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

export async function fetchToFile(url, outputPath) {
  const response = await fetch(url);

  if (!response.ok)
    throw new Error(`GET ${url} returned HTTP ${response.status}.`);

  await fs.ensureDir(path.dirname(outputPath));
  await fs.writeFile(outputPath, Buffer.from(await response.arrayBuffer()));
  console.log(chalk.green(`Saved ${outputPath}`));
}

export async function validatePdf(pdfPath) {
  if (!await fs.pathExists(pdfPath))
    throw new Error(`PDF does not exist: ${pdfPath}`);

  const buffer = await fs.readFile(pdfPath);

  if (buffer.length < 1024)
    throw new Error(`PDF is too small: ${pdfPath} (${buffer.length} bytes).`);

  const header = buffer.subarray(0, 5).toString('latin1');

  if (header !== '%PDF-')
    throw new Error(`PDF header is invalid: ${pdfPath}.`);

  const tail = buffer.subarray(Math.max(0, buffer.length - 2048)).toString('latin1');

  if (!tail.includes('%%EOF'))
    throw new Error(`PDF EOF marker was not found: ${pdfPath}.`);

  console.log(chalk.green(`Validated PDF ${pdfPath} (${buffer.length} bytes)`));
}

export async function validateXps(xpsPath) {
  if (!await fs.pathExists(xpsPath))
    throw new Error(`Expected XPS file was not created: ${xpsPath}`);

  const buffer = await fs.readFile(xpsPath);

  if (buffer.length < 1024)
    throw new Error(`XPS file is too small to be meaningful: ${xpsPath} (${buffer.length} bytes).`);

  const hasZipHeader = buffer[0] === 0x50 && buffer[1] === 0x4b && buffer[2] === 0x03 && buffer[3] === 0x04;

  if (!hasZipHeader)
    throw new Error(`XPS file does not start with the ZIP package header: ${xpsPath}.`);

  console.log(chalk.green(`Validated XPS ${xpsPath} (${buffer.length} bytes)`));
}

async function readLog(filePath) {
  if (!filePath || !await fs.pathExists(filePath))
    return '';

  return `\nServer log:\n${await fs.readFile(filePath, 'utf8')}`;
}

export function errorMessage(error) {
  return error?.message ?? String(error);
}

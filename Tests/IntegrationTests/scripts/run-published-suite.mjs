#!/usr/bin/env zx
import { argv, chalk, errorMessage } from './integration-test-utils.mjs';
import { runPublishedTest } from './run-published-test.mjs';

export async function runPublishedSuite({ targetFramework, rid, packageVersion }) {
  if (!targetFramework)
    throw new Error('Target framework is required.');

  if (!rid)
    throw new Error('Runtime identifier is required.');

  if (!packageVersion)
    throw new Error('QuestPDF package version is required.');

  const apps = ['console', 'console-singlefile', 'webapi', 'worker'];

  if (targetFramework !== 'net6.0' && rid !== 'win-x86')
    apps.push('console-aot');

  const failures = [];

  for (const app of apps) {
    console.log(`::group::${app}`);

    try {
      await runPublishedTest({ appType: app, targetFramework, rid, packageVersion });
    } catch (error) {
      failures.push({ app, error });
      console.error(chalk.red(`${app} failed: ${errorMessage(error)}`));
    } finally {
      console.log('::endgroup::');
    }
  }

  if (failures.length > 0)
    throw new Error(`${failures.length} published integration app(s) failed: ${failures.map(x => x.app).join(', ')}.`);
}

try {
  const [targetFrameworkArg, ridArg, packageVersionArg] = argv._;

  await runPublishedSuite({
    targetFramework: argv.framework ?? argv['target-framework'] ?? targetFrameworkArg,
    rid: argv.runtime ?? argv.rid ?? argv['runtime-identifier'] ?? ridArg,
    packageVersion: argv['package-version'] ?? argv.version ?? packageVersionArg
  });
} catch (error) {
  console.error(chalk.red(errorMessage(error)));
  process.exit(1);
}

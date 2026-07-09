#!/usr/bin/env zx
// Publishes and runs every integration app for one target framework / runtime combination.
import { errorMessage, runCli } from './integration-test-utils.mjs';
import { appTypes, runPublishedTest } from './run-published-test.mjs';

await runCli(async () => {
  const [targetFramework, rid, packageVersion, appFilter] = argv._;

  if (!targetFramework || !rid || !packageVersion)
    throw new Error('Usage: zx run-published-suite.mjs <target-framework> <runtime-identifier> <package-version> [app]');

  if (appFilter && !appTypes.includes(appFilter))
    throw new Error(`Unknown app type: ${appFilter}. Expected one of: ${appTypes.join(', ')}.`);

  // Native AOT is not available for net6.0 console apps or the win-x86 runtime.
  const supportsAot = targetFramework !== 'net6.0' && rid !== 'win-x86';
  const apps = (appFilter ? [appFilter] : appTypes).filter(app => app !== 'console-aot' || supportsAot);

  if (apps.length === 0)
    throw new Error(`console-aot is not supported for ${targetFramework} / ${rid}.`);

  const failures = [];

  for (const app of apps) {
    console.log(`::group::${app}`);

    try {
      await runPublishedTest({ appType: app, targetFramework, rid, packageVersion });
    } catch (error) {
      failures.push(app);
      console.error(chalk.red(`${app} failed: ${errorMessage(error)}`));
    } finally {
      console.log('::endgroup::');
    }
  }

  if (failures.length > 0)
    throw new Error(`${failures.length} published integration app(s) failed: ${failures.join(', ')}.`);
});

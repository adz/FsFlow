#!/usr/bin/env node

import fs from 'node:fs';
import path from 'node:path';

const repoRoot = path.resolve(path.dirname(new URL(import.meta.url).pathname), '..');
const githubBase = 'https://github.com/adz/FsFlow/blob/main';

const targets = [
  path.join(repoRoot, 'docs', 'reference'),
];

const pageSpecs = [
  {
    outPath: ['fsflow', 'flow.md'],
    title: 'Flow',
    description: 'Source-documented synchronous workflow surface in FsFlow.',
    intro:
      "This page shows the source-documented `Flow` surface: the core type, the module functions, and the `flow { }` builder.",
    sourceFiles: ['src/FsFlow/Flow.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:Flow'],
      },
      {
        title: 'Module functions',
        symbols: ['module:Flow', 'Flow.run', 'Flow.succeed', 'Flow.value', 'Flow.fail', 'Flow.fromResult', 'Flow.fromOption', 'Flow.fromValueOption', 'Flow.orElseFlow', 'Flow.env', 'Flow.read', 'Flow.map', 'Flow.bind', 'Flow.tap', 'Flow.tapError', 'Flow.mapError', 'Flow.catch', 'Flow.orElse', 'Flow.zip', 'Flow.map2', 'Flow.localEnv', 'Flow.delay', 'Flow.traverse', 'Flow.sequence'],
      },
      {
        title: 'Builder',
        symbols: ['Builders.flow'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'asyncflow.md'],
    title: 'AsyncFlow',
    description: 'Source-documented async workflow surface in FsFlow.',
    intro:
      "This page shows the source-documented `AsyncFlow` surface: the core type, the module functions, and the `asyncFlow { }` builder.",
    sourceFiles: ['src/FsFlow/Flow.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:AsyncFlow'],
      },
      {
        title: 'Module functions',
        symbols: ['module:AsyncFlow', 'AsyncFlow.run', 'AsyncFlow.toAsync', 'AsyncFlow.succeed', 'AsyncFlow.fail', 'AsyncFlow.fromResult', 'AsyncFlow.fromOption', 'AsyncFlow.fromValueOption', 'AsyncFlow.orElseAsync', 'AsyncFlow.orElseAsyncFlow', 'AsyncFlow.fromFlow', 'AsyncFlow.fromAsync', 'AsyncFlow.fromAsyncResult', 'AsyncFlow.env', 'AsyncFlow.read', 'AsyncFlow.map', 'AsyncFlow.bind', 'AsyncFlow.tap', 'AsyncFlow.tapError', 'AsyncFlow.mapError', 'AsyncFlow.catch', 'AsyncFlow.orElse', 'AsyncFlow.zip', 'AsyncFlow.map2', 'AsyncFlow.localEnv', 'AsyncFlow.delay', 'AsyncFlow.traverse', 'AsyncFlow.sequence'],
      },
      {
        title: 'Builder',
        symbols: ['Builders.asyncFlow'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'check.md'],
    title: 'Check',
    description: 'Source-documented pure predicate helpers for FsFlow.',
    intro:
      'This page shows the source-documented `Check` surface: the unit-failure result type and the reusable predicate helpers.',
    sourceFiles: ['src/FsFlow/Validate.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:Check'],
      },
      {
        title: 'Module functions',
        symbols: ['module:Check', 'Check.fromPredicate', 'Check.not', 'Check.and', 'Check.or', 'Check.all', 'Check.any', 'Check.okIf', 'Check.failIf', 'Check.okIfSome', 'Check.okIfNone', 'Check.failIfSome', 'Check.failIfNone', 'Check.okIfValueSome', 'Check.okIfValueNone', 'Check.failIfValueSome', 'Check.failIfValueNone', 'Check.okIfNotNull', 'Check.okIfNull', 'Check.failIfNotNull', 'Check.failIfNull', 'Check.okIfNotEmpty', 'Check.okIfEmpty', 'Check.failIfNotEmpty', 'Check.failIfEmpty', 'Check.okIfEqual', 'Check.okIfNotEqual', 'Check.failIfEqual', 'Check.failIfNotEqual', 'Check.okIfNonEmptyStr', 'Check.okIfEmptyStr', 'Check.failIfNonEmptyStr', 'Check.failIfEmptyStr', 'Check.okIfNotBlank', 'Check.notBlank', 'Check.okIfBlank', 'Check.blank', 'Check.failIfNotBlank', 'Check.failIfBlank', 'Check.orElse', 'Check.orElseWith', 'Check.notNull', 'Check.notEmpty', 'Check.equal', 'Check.notEqual'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'diagnostics.md'],
    title: 'Diagnostics',
    description: 'Source-documented validation diagnostics graph for FsFlow.',
    intro:
      'This page shows the source-documented `Diagnostics` surface: the path-aware graph types and the merge/flatten helpers.',
    sourceFiles: ['src/FsFlow/Validate.fs'],
    sections: [
      {
        title: 'Graph types',
        symbols: ['type:PathSegment', 'type:Path', 'type:Diagnostic', 'type:Diagnostics'],
      },
      {
        title: 'Module functions',
        symbols: ['module:Diagnostics', 'Diagnostics.empty', 'Diagnostics.singleton', 'Diagnostics.merge', 'Diagnostics.flatten'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'validation.md'],
    title: 'Validation',
    description: 'Source-documented accumulating validation for FsFlow.',
    intro:
      'This page shows the source-documented `Validation` surface: the accumulating result type, the module functions, and the `validate { }` builder.',
    sourceFiles: ['src/FsFlow/Validate.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:Validation'],
      },
      {
        title: 'Module functions',
        symbols: ['module:Validation', 'Validation.toResult', 'Validation.succeed', 'Validation.fail', 'Validation.fromResult', 'Validation.map', 'Validation.bind', 'Validation.mapError', 'Validation.map2', 'Validation.apply', 'Validation.collect', 'Validation.sequence', 'Validation.merge'],
      },
      {
        title: 'Builder',
        symbols: ['src/FsFlow/Flow.fs::Builders.validate'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'result.md'],
    title: 'Result',
    description: 'Source-documented fail-fast result helpers for FsFlow.',
    intro:
      'This page shows the source-documented `Result` surface: the module functions and the `result { }` builder.',
    sourceFiles: ['src/FsFlow/Validate.fs'],
    sections: [
      {
        title: 'Module functions',
        symbols: ['module:Result', 'Result.map', 'Result.bind', 'Result.mapError', 'Result.mapErrorTo', 'Result.sequence', 'Result.traverse'],
      },
      {
        title: 'Builder',
        symbols: ['src/FsFlow/Flow.fs::Builders.result'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'runtime.md'],
    title: 'Runtime',
    description: 'Source-documented runtime support and helpers for FsFlow.',
    intro:
      'This page shows the source-documented runtime surface: logging, retry policies, and async operational helpers.',
    sourceFiles: ['src/FsFlow/Flow.fs'],
    sections: [
      {
        title: 'Logging',
        symbols: ['type:LogLevel', 'type:LogEntry'],
      },
      {
        title: 'Retry policy',
        symbols: ['type:RetryPolicy', 'module:RetryPolicy', 'RetryPolicy.noDelay'],
      },
      {
        title: 'Async operational helpers',
        symbols: ['module:AsyncFlow.Runtime', 'AsyncFlow.Runtime.cancellationToken', 'AsyncFlow.Runtime.catchCancellation', 'AsyncFlow.Runtime.ensureNotCanceled', 'AsyncFlow.Runtime.sleep', 'AsyncFlow.Runtime.log', 'AsyncFlow.Runtime.logWith', 'AsyncFlow.Runtime.useWithAcquireRelease', 'AsyncFlow.Runtime.timeout', 'AsyncFlow.Runtime.timeoutToOk', 'AsyncFlow.Runtime.timeoutToError', 'AsyncFlow.Runtime.timeoutWith', 'AsyncFlow.Runtime.retry'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'taskflow.md'],
    title: 'TaskFlow',
    description: 'Source-documented task workflow surface in FsFlow.',
    intro:
      'This page shows the source-documented `TaskFlow` surface: the core type, the module functions, and the `taskFlow { }` builder.',
    sourceFiles: ['src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:TaskFlow'],
      },
      {
        title: 'Module functions',
        symbols: ['module:TaskFlow', 'TaskFlow.run', 'TaskFlow.runContext', 'TaskFlow.toTask', 'TaskFlow.succeed', 'TaskFlow.fail', 'TaskFlow.fromResult', 'TaskFlow.fromOption', 'TaskFlow.fromValueOption', 'TaskFlow.orElseTask', 'TaskFlow.orElseAsync', 'TaskFlow.orElseFlow', 'TaskFlow.orElseAsyncFlow', 'TaskFlow.orElseTaskFlow', 'TaskFlow.fromFlow', 'TaskFlow.fromAsyncFlow', 'TaskFlow.fromTask', 'TaskFlow.fromTaskResult', 'TaskFlow.env', 'TaskFlow.read', 'TaskFlow.readRuntime', 'TaskFlow.readEnvironment', 'TaskFlow.map', 'TaskFlow.bind', 'TaskFlow.tap', 'TaskFlow.tapError', 'TaskFlow.mapError', 'TaskFlow.catch', 'TaskFlow.orElse', 'TaskFlow.zip', 'TaskFlow.map2', 'TaskFlow.localEnv', 'TaskFlow.delay', 'TaskFlow.traverse', 'TaskFlow.sequence'],
      },
      {
        title: 'Builder',
        symbols: ['TaskBuilders.taskFlow'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'taskflow-runtime.md'],
    title: 'TaskFlow Runtime',
    description: 'Source-documented task runtime helpers for FsFlow.',
    intro:
      'This page shows the source-documented task runtime surface: the runtime context and the task-specific operational helpers.',
    sourceFiles: ['src/FsFlow/Runtime.fs', 'src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'Runtime context',
        symbols: ['type:RuntimeContext', 'module:RuntimeContext', 'RuntimeContext.create', 'RuntimeContext.runtime', 'RuntimeContext.environment', 'RuntimeContext.cancellationToken', 'RuntimeContext.mapRuntime', 'RuntimeContext.mapEnvironment', 'RuntimeContext.withRuntime', 'RuntimeContext.withEnvironment'],
      },
      {
        title: 'Runtime helpers',
        symbols: ['src/FsFlow/TaskFlow.fs::module:TaskFlow.Runtime', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.cancellationToken', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.catchCancellation', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.ensureNotCanceled', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.sleep', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.log', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.logWith', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.useWithAcquireRelease', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.timeout', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.timeoutToOk', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.timeoutToError', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.timeoutWith', 'src/FsFlow/TaskFlow.fs::TaskFlow.Runtime.retry'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'taskflow-spec.md'],
    title: 'TaskFlowSpec',
    description: 'Source-documented task workflow specification for FsFlow.',
    intro:
      'This page shows the source-documented `TaskFlowSpec` surface, used for defining and running task workflows with explicit configurations.',
    sourceFiles: ['src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:TaskFlowSpec'],
      },
      {
        title: 'Module functions',
        symbols: ['module:TaskFlowSpec', 'TaskFlowSpec.create', 'TaskFlowSpec.run'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'capability.md'],
    title: 'Capability',
    description: 'Source-documented capabilities and layers for FsFlow.',
    intro:
      'This page shows the source-documented capability and layer surface, used for dependency injection and environment management in task workflows.',
    sourceFiles: ['src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'Capabilities',
        symbols: ['module:Capability', 'Capability.MissingCapability', 'Capability.service', 'Capability.runtime', 'Capability.environment', 'Capability.serviceFromProvider'],
      },
      {
        title: 'Layers',
        symbols: ['type:Layer'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'coldtask.md'],
    title: 'ColdTask',
    description: 'Source-documented delayed task helpers used by FsFlow.',
    intro:
      'This page shows the source-documented `ColdTask` surface: the delayed task helper used to anchor execution to the runtime context.',
    sourceFiles: ['src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'Core type',
        symbols: ['type:ColdTask'],
      },
      {
        title: 'Module functions',
        symbols: ['module:ColdTask', 'ColdTask.run', 'ColdTask.create', 'ColdTask.fromTaskFactory', 'ColdTask.fromTask', 'ColdTask.fromValueTaskFactory', 'ColdTask.fromValueTaskFactoryWithoutCancellation', 'ColdTask.fromValueTask'],
      },
    ],
  },
  {
    outPath: ['fsflow', 'interop.md'],
    title: 'Interop',
    description: 'Source-documented task and async interop helpers for FsFlow.',
    intro:
      'This page shows the interop helpers that bridge task, async, and synchronous boundaries in FsFlow.',
    sourceFiles: ['src/FsFlow/TaskFlow.fs'],
    sections: [
      {
        title: 'TaskFlow bridges',
        symbols: ['TaskFlow.fromFlow', 'TaskFlow.fromAsyncFlow', 'TaskFlow.orElseTask', 'TaskFlow.orElseAsync', 'TaskFlow.orElseFlow', 'TaskFlow.orElseAsyncFlow', 'TaskFlow.orElseTaskFlow'],
      },
      {
        title: 'Builder extensions',
        symbols: ['module:TaskFlowBuilderExtensions', 'module:AsyncFlowBuilderExtensions'],
      },
    ],
  },
];

function ensureDir(dirPath) {
  fs.mkdirSync(dirPath, { recursive: true });
}

function readLines(filePath) {
  return fs.readFileSync(filePath, 'utf8').split(/\r?\n/);
}

function cleanXmlDocText(text) {
  return text
    .replace(/<c>([\s\S]*?)<\/c>/gi, '`$1`')
    .replace(/<paramref name="([^"]+)"\s*\/>/gi, '`$1`')
    .replace(/<see cref="([^"]+)"\s*\/>/gi, (_match, cref) => {
      const withoutPrefix = cref.replace(/^[A-Z]:/, '');
      const lastSegment = withoutPrefix.split(/[.:]/).pop() ?? withoutPrefix;
      return `\`${lastSegment.replace(/`[0-9]+/g, '')}\``;
    })
    .replace(/<[^>]+>/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

function extractSummary(commentLines) {
  if (commentLines.length === 0) {
    return '';
  }

  const raw = commentLines
    .map((line) => line.replace(/^\s*\/\/\/\s?/, ''))
    .join('\n');

  const summaryMatch = raw.match(/<summary>([\s\S]*?)<\/summary>/i);
  const content = summaryMatch ? summaryMatch[1] : raw;
  return cleanXmlDocText(content);
}

function isAttributeLine(line) {
  return /^\s*\[<[^>]+>\]\s*$/.test(line);
}

function extractLetName(line) {
  const simple = line.match(/^\s*let\s+(?:inline\s+)?(?:private\s+)?(?:rec\s+)?(?:(``([^`]+)``)|([A-Za-z_][A-Za-z0-9_']*))(?=\s|$|[:(=<])/);
  if (simple) {
    return simple[2] || simple[3] || null;
  }

  const parenthesized = line.match(/^\s*let\s+(?:inline\s+)?(?:private\s+)?(?:rec\s+)?\(\s*(?:(``([^`]+)``)|([A-Za-z_][A-Za-z0-9_']*))\s*\)(?=\s|$|[:(=<])/);
  if (parenthesized) {
    return parenthesized[2] || parenthesized[3] || null;
  }

  return null;
}

function extractSymbols(filePath) {
  const lines = readLines(filePath);
  const symbols = new Map();
  const moduleStack = [];
  let pendingComments = [];
  let sawExclude = false;

  const popForIndent = (indent) => {
    while (moduleStack.length && moduleStack[moduleStack.length - 1].indent >= indent) {
      moduleStack.pop();
    }
  };

  const currentPrefix = () => moduleStack.map((entry) => entry.name).join('.');

  const record = (name, kind, lineNumber) => {
    if (sawExclude) {
      pendingComments = [];
      sawExclude = false;
      return;
    }

    const summary = extractSummary(pendingComments);
    const fullName = currentPrefix() ? `${currentPrefix()}.${name}` : name;
    const existing = symbols.get(fullName) ?? [];
    existing.push({
      kind,
      line: lineNumber,
      summary,
      filePath,
    });
    symbols.set(fullName, existing);
    pendingComments = [];
    sawExclude = false;
  };

  for (let index = 0; index < lines.length; index += 1) {
    const line = lines[index];
    const trimmed = line.trim();
    const indent = line.match(/^ */)?.[0].length ?? 0;

    if (trimmed.startsWith('///')) {
      pendingComments.push(line);
      if (trimmed.includes('<exclude/>')) {
        sawExclude = true;
      }
      continue;
    }

    if (trimmed === '') {
      continue;
    }

    if (isAttributeLine(line)) {
      continue;
    }

    popForIndent(indent);

    const moduleMatch = line.match(/^(\s*)module\s+([A-Za-z_][A-Za-z0-9_']*)\s*=/);
    if (moduleMatch) {
      const moduleIndent = moduleMatch[1].length;
      popForIndent(moduleIndent);
      record(moduleMatch[2], 'module', index + 1);
      moduleStack.push({ name: moduleMatch[2], indent: moduleIndent });
      continue;
    }

    const typeMatch = line.match(/^(\s*)type\s+([A-Za-z_][A-Za-z0-9_']*)/);
    if (typeMatch) {
      record(typeMatch[2], 'type', index + 1);
      continue;
    }

    const letName = extractLetName(line);
    const currentModuleIndent = moduleStack.length ? moduleStack[moduleStack.length - 1].indent : -4;
    if (letName && (pendingComments.length > 0 || indent <= currentModuleIndent + 4)) {
      const name = letName;
      record(name, 'let', index + 1);
      continue;
    }

    if (pendingComments.length > 0) {
      pendingComments = [];
      sawExclude = false;
    }
  }

  return symbols;
}

function makeSourceLink(filePath, line) {
  const relPath = path.relative(repoRoot, filePath).split(path.sep).join('/');
  return `${githubBase}/${relPath}#L${line}`;
}

function resolveSymbolDoc(symbols, qualifiedName, kindHint) {
  const docs = symbols.get(qualifiedName);
  if (!docs || docs.length === 0) {
    return null;
  }

  if (kindHint) {
    return docs.find((doc) => doc.kind === kindHint) ?? null;
  }

  if (docs.length === 1) {
    return docs[0];
  }

  return docs.find((doc) => doc.kind === 'module') ?? docs.find((doc) => doc.kind === 'type') ?? docs[0];
}

function renderItem(symbols, sourcePath, symbolRef) {
  const [sourcePrefix, rawSymbol] = symbolRef.includes('::') ? symbolRef.split('::', 2) : [null, symbolRef];
  const sourcePathKey = sourcePrefix && !sourcePrefix.includes(':') ? sourcePrefix : sourcePath;
  const symbolWithKind = sourcePrefix && sourcePrefix.includes(':') ? sourcePrefix : rawSymbol;
  const [kindHint, qualifiedName] = symbolWithKind.includes(':') ? symbolWithKind.split(':', 2) : [null, symbolWithKind];
  const doc = resolveSymbolDoc(symbols, qualifiedName, kindHint);
  if (!doc) {
    throw new Error(`Missing symbol doc for ${symbolRef} in ${sourcePathKey}`);
  }

  const label = kindHint && !qualifiedName.includes('.') ? `${kindHint} \`${qualifiedName}\`` : `\`${qualifiedName}\``;
  const summary = doc.summary ? `: ${doc.summary}` : '';
  return `- ${label}${summary} [source](${makeSourceLink(sourcePath, doc.line)})`;
}

function renderSection(spec, section, symbolsByFile) {
  const lines = [`## ${section.title}`, ''];

  if (section.manual) {
    lines.push(section.manual, '');
  }

  for (const symbol of section.symbols ?? []) {
    const [sourceAlias, symbolName] = symbol.includes('::') ? symbol.split('::', 2) : [null, symbol];
    const sourcePath = sourceAlias && !sourceAlias.includes(':') ? sourceAlias : spec.sourceFiles[0];
    const symbols = symbolsByFile.get(sourcePath);
    if (!symbols) {
      throw new Error(`Missing symbol index for ${sourcePath}`);
    }
    lines.push(renderItem(symbols, path.join(repoRoot, sourcePath), sourceAlias ? `${sourceAlias}::${symbolName}` : symbol));
  }

  if ((section.symbols ?? []).length > 0) {
    lines.push('');
  }

  return lines;
}

function renderPage(spec, symbolsByFile) {
  const lines = [
    '---',
    `title: ${spec.title}`,
    `description: ${spec.description}`,
    '---',
    '',
    `# ${spec.title}`,
    '',
    spec.intro,
    '',
  ];

  for (const section of spec.sections) {
    lines.push(...renderSection(spec, section, symbolsByFile));
  }

  lines.push('## Source', '');
  for (const sourceFile of spec.sourceFiles) {
    const relPath = path.relative(repoRoot, path.join(repoRoot, sourceFile)).split(path.sep).join('/');
    lines.push(`- [${path.basename(sourceFile)}](${githubBase}/${relPath})`);
  }
  lines.push('');

  return lines.join('\n');
}

function generate() {
  const symbolsByFile = new Map();

  for (const sourceFile of new Set(pageSpecs.flatMap((spec) => spec.sourceFiles))) {
    symbolsByFile.set(sourceFile, extractSymbols(path.join(repoRoot, sourceFile)));
  }

  for (const targetRoot of targets) {
    ensureDir(targetRoot);
    for (const spec of pageSpecs) {
      const outPath = path.join(targetRoot, ...spec.outPath);
      ensureDir(path.dirname(outPath));
      fs.writeFileSync(outPath, renderPage(spec, symbolsByFile), 'utf8');
    }
  }
}

generate();

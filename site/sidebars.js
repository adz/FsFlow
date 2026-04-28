const guides = [
  {
    type: 'category',
    label: 'Start',
    items: [
      { type: 'doc', id: 'index', label: 'Home' },
      { type: 'doc', id: 'GETTING_STARTED', label: 'Getting Started' },
      { type: 'doc', id: 'TINY_EXAMPLES', label: 'Tiny Examples' },
      { type: 'doc', id: 'examples/README', label: 'Runnable Examples' },
    ],
  },
  {
    type: 'category',
    label: 'Integrate',
    items: [
      { type: 'doc', id: 'TASK_ASYNC_INTEROP', label: 'Task And Async Interop' },
      {
        type: 'category',
        label: 'Integrations',
        items: [
          { type: 'doc', id: 'INTEGRATIONS', label: 'Overview' },
          { type: 'doc', id: 'INTEGRATIONS_FSTOOLKIT', label: 'FsToolkit.ErrorHandling' },
          { type: 'doc', id: 'INTEGRATIONS_VALIDUS', label: 'Validus' },
          { type: 'doc', id: 'INTEGRATIONS_ICEDTASKS', label: 'IcedTasks' },
          { type: 'doc', id: 'INTEGRATIONS_FSHARPPLUS', label: 'FSharpPlus' },
        ],
      },
    ],
  },
  {
    type: 'category',
    label: 'Understand',
    items: [
      { type: 'doc', id: 'WHY_FSFLOW', label: 'Why FsFlow' },
      { type: 'doc', id: 'SEMANTICS', label: 'Semantics' },
      { type: 'doc', id: 'TROUBLESHOOTING_TYPES', label: 'Troubleshooting Types' },
    ],
  },
  {
    type: 'category',
    label: 'Model',
    items: [
      { type: 'doc', id: 'ENV_SLICING', label: 'Environment Slicing' },
      { type: 'doc', id: 'ARCHITECTURAL_STYLES', label: 'Architectural Styles' },
      { type: 'doc', id: 'EFFECT_TS_COMPARISON', label: 'FsFlow And Effect-TS' },
    ],
  },
  {
    type: 'category',
    label: 'Measure',
    items: [{ type: 'doc', id: 'BENCHMARKS', label: 'Benchmarks' }],
  },
];

const api = [
  { type: 'doc', id: 'reference/index', label: 'API Home' },
  {
    type: 'category',
    label: 'FsFlow',
    items: [
      { type: 'doc', id: 'reference/fsflow/index', label: 'Overview' },
      { type: 'doc', id: 'reference/fsflow/flow', label: 'Flow' },
      { type: 'doc', id: 'reference/fsflow/asyncflow', label: 'AsyncFlow' },
      { type: 'doc', id: 'reference/fsflow/validate', label: 'Validate' },
      { type: 'doc', id: 'reference/fsflow/support-types', label: 'Support Types' },
    ],
  },
  {
    type: 'category',
    label: 'FsFlow.Net',
    items: [
      { type: 'doc', id: 'reference/fsflow-net/index', label: 'Overview' },
      { type: 'doc', id: 'reference/fsflow-net/taskflow', label: 'TaskFlow' },
      { type: 'doc', id: 'reference/fsflow-net/coldtask', label: 'ColdTask' },
      { type: 'doc', id: 'reference/fsflow-net/interop', label: 'Interop' },
    ],
  },
];

module.exports = {
  docs: [
    { type: 'category', label: 'Guides', items: guides },
    {
      type: 'category',
      label: 'API Docs',
      items: api,
    },
  ],
};

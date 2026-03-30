namespace EffectFs

[<RequireQualifiedAccess>]
module AsyncResultCompat =
    let ofAsyncResult
        (operation: Async<Result<'value, 'error>>)
        : Effect<'env, 'error, 'value> =
        Effect.fromAsyncResult operation

    let toAsyncResult
        (environment: 'env)
        (effect: Effect<'env, 'error, 'value>)
        : Async<Result<'value, 'error>> =
        Effect.toAsyncResult environment effect

    let map
        (mapper: 'value -> 'next)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'next> =
        Effect.map mapper effect

    let bind
        (binder: 'value -> Effect<'env, 'error, 'next>)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'next> =
        Effect.bind binder effect

    let mapError
        (mapper: 'error -> 'nextError)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'nextError, 'value> =
        Effect.mapError mapper effect

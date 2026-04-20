import {
	effectScope,
	watch as _watch,
	watchEffect as _watchEffect,
	markRaw,
	type EffectScope,
	type WatchCallback,
	type WatchEffect,
	type WatchOptions,
	type WatchEffectOptions,
	type WatchSource,
	type WatchStopHandle
} from 'vue'

/**
 * A lightweight utility that binds {@link watch} and {@link watchEffect}
 * calls to a single {@link EffectScope}.
 * Stopping the scope halts every watcher created through this instance,
 * ensuring deterministic cleanup and preventing memory leaks.
 *
 * @example
 * ```ts
 * const scope = new ScopedWatch()
 *
 * scope.watch(
 *   () => props.id,
 *   id => console.info('ID changed to', id)
 * )
 *
 * scope.watchEffect(() => {
 *   console.debug('Reactive effect triggered')
 * })
 *
 * // When the watchers are no longer required…
 * scope.dispose()
 * ```
 */
export class ScopedWatch {
	/**
	 * The underlying effect-scope holding all registered watchers.
	 * Marked with {@link markRaw} so that a proxy is never applied to it,
	 * even if the instance itself becomes reactive.
	 *
	 * @internal
	 */
	private readonly _scope: EffectScope

	/**
	 * Creates a new watcher scope.
	 *
	 * @param detached - If `true`, the scope is *detached* (i.e. orphaned) and
	 *                   must be disposed manually.
	 *                   If `false` (default), the new scope is *linked* to the
	 *                   currently active one and will be stopped alongside it.
	 */
	constructor(detached: boolean = false) {
		/** @private */
		this._scope = markRaw(effectScope(detached))
		Object.defineProperty(this, '_scope', { enumerable: false })
	}

	/**
	 * A thin wrapper around {@link _watch} that automatically registers the
	 * watcher within this scope.
	 *
	 * @returns A stop-handle that halts **only** this watcher.
	 *          Invoke {@link dispose} to stop all watchers at once.
	 */
	watch(
		source: WatchSource | WatchSource[],
		cb: WatchCallback,
		options?: WatchOptions | undefined
	): WatchStopHandle {
		return this._scope.run(() => _watch(source, cb, options))!
	}

	/**
	 * Equivalent to {@link _watchEffect}, but lifecycle-managed by this scope.
	 *
	 */
	watchEffect(effect: WatchEffect, options?: WatchEffectOptions | undefined): WatchStopHandle {
		return this._scope.run(() => _watchEffect(effect, options))!
	}

	/**
	 * Stops every watcher created through this instance and
	 * releases all associated resources.
	 */
	dispose(): void {
		this._scope.stop()
	}
}

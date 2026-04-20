// ──────────────────────────────────────────────
// DISCLAIMER
// ──────────────────────────────────────────────
//
// This file contains all type definitions for the module.
// Currently, everything is centralized here for convenience,
// but it should be refactored as the code in the `mixins`
// folder is gradually modularized into separate files.
//
// Future goal: move related types closer to their features
// to reduce this file's size and improve maintainability.
//
// ──────────────────────────────────────────────

// ──────────────────────────────────────────────
// Lists
// ──────────────────────────────────────────────

/** [**WIP**] Configuration for a list. */
export type ListConfig = {
	/** Plural table name for the list. */
	readonly tableNamePlural?: string

	/** Path to resources for the list. */
	readonly resourcesPath?: string

	/** CRUD action definitions. */
	readonly crudActions?: object[]

	/** Custom action definitions. */
	readonly customActions?: object[]

	/** General action definitions. */
	readonly generalActions?: object[]

	/** Action to perform when clicking a row. */
	readonly rowClickAction?: Record<string, object>

	/** Whether to show icons for row actions. */
	readonly showRowActionIcon?: boolean

	/** Whether to show icons for general actions. */
	readonly showGeneralActionIcon?: boolean

	/** Whether to show text for general actions. */
	readonly showGeneralActionText?: boolean

	/** The number of records per page. */
	readonly perPage?: number
}

/**
 * This script is intended to be run manually or automatically before
 * starting the development server (e.g., via the "pre-dev" npm script).
 *
 * It generates a single bundled SVG sprite file by combining all individual
 * SVG icons from the source directory.
 *
 * Why is this needed?
 * - Running this script beforehand ensures the bundled SVG sprite is available
 *   in the public folder immediately when the dev server starts.
 * - Without this step, the sprite file would be missing on first run, causing
 *   missing icons until the server is restarted.
 *
 * This allows a smooth developer experience with consistent SVG assets served
 * from the start.
 */

import { generateSvgSpriteBundle } from '../svgstore.config'

generateSvgSpriteBundle()

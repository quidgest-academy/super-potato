import type { Plugin } from 'vite'
import { generateSvgSpriteBundle } from '../svgstore.config'

/**
 * Vite plugin to generate an SVG sprite bundle at build start.
 *
 * @remarks
 * This plugin integrates with the Vite build process by running during the
 * `buildStart` hook. It invokes `generateSvgSpriteBundle` to combine individual
 * SVG icons into a single sprite file, which is then output to the public folder.
 *
 * @returns The Vite plugin configuration object for SVG sprite bundling.
 */
export function viteSvgSpritePlugin(): Plugin {
	return {
		name: 'vite-svg-sprite-plugin',
		buildStart: generateSvgSpriteBundle
	}
}

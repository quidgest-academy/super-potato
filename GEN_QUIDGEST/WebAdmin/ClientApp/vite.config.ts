import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

import { viteSvgSpritePlugin } from './plugins/svgbundle'

const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)

export default defineConfig(({ mode }) => {
	const isDev = mode === 'development'

	return {
		base: './',
		root: './',
		resolve: {
			alias: {
				'@': resolve(__dirname, './src'),
				'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js'
			},
			extensions: ['.js', '.json', '.vue']
		},
		build: {
			target: 'esnext',
			minify: true,
			manifest: false,
			sourcemap: false,
			outDir: 'dist',
			emptyOutDir: true,
			rollupOptions: {
				cache: true,
				treeshake: false
			},
			cssCodeSplit: false,
			reportCompressedSize: false,
			modulePreload: false
		},
		plugins: [
			vue(),
			...(isDev ? [] : [viteSvgSpritePlugin()])
		],
		server: {
			open: false,
			port: 8202,
			proxy: {
				'/api': {
					target: 'http://localhost:5658/',
					secure: false
				}
			}
		}
	}
})

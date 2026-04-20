import vue from '@vitejs/plugin-vue'
import path from 'path'
import { visualizer } from 'rollup-plugin-visualizer'
import { defineConfig } from 'vite'
import mkcert from 'vite-plugin-mkcert'


import { viteSvgSpritePlugin } from './plugins/svgbundle'

export default defineConfig(({ mode }) => {
	const isDev = mode === 'development'

	return {
		base: './',
		root: './',
		resolve: {
			alias: [
				{ find: 'vue-i18n', replacement: 'vue-i18n/dist/vue-i18n.cjs.js' },
				// we alias to the lib's source files in dev
				// so we don't need to rebuild the lib over and over again
				{
					find: /^@quidgest\/clientapp$/,
					replacement: isDev
						? path.resolve(__dirname, '../../packages/clientapp/src/index.ts')
						: '@quidgest/clientapp',
				},
				{
					find: /^@quidgest\/clientapp\/(.+)$/,
					replacement: isDev
						? (_match, subPath) =>
							path.resolve(__dirname, `../../packages/clientapp/src/${subPath}`)
						: '@quidgest/clientapp/$1',
				},
				{ find: '@', replacement: path.resolve(__dirname, './src') }
			],
			extensions: ['.mjs', '.js', '.ts', '.json', '.vue'],
			dedupe: ['vue']
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
				// JGF 19.05.2025: Reactivated treeshake. If a problem is found and it needs to be deactivated again, it should be thoroughtly documented.
				treeshake: true,
				output: {
					compact: true,
					manualChunks: (id) => {
						/* Manual chunking should be limited to:
						- Bundling files that should be loaded together most of the time but the algorithm failed to bundle together
						- Groups of files that are optional, and are making other bundles bigger
						The rest of the files should be left to the algorithm
						*/
						if (id.includes('quidgest/chatbot')) return 'chatbot'
						if (id.includes('node_modules'))
						{
							if (id.includes('leaflet')) return 'leaflet'
							if (id.includes('ace-builds')) return 'ace'
							if (id.includes('ace-diff')) return 'ace'
							if (id.includes('fullcalendar')) return 'fullcalendar'
							if (id.includes('highcharts')) return 'highcharts'
							return null

						}
						//One file per form
						if (id.includes('views/forms'))
						{
							const match = id.match(/\/forms\/([^/]+)/)
							return match ? match[1] : 'forms'
						}
						//One file per menu
						if (id.includes('views/menus'))
						{
							const match = id.match(/Menu([^/]+)/)
							return match ? 'Menu' + match[1] : null
						}
						if(id.includes('dashboard')) return 'dashboard'
						//Layout and sidebar components were not being merged by the algorithm
						if(id.includes('Footer') || id.includes('NavigationalBar') || id.includes('LanguageItems')) return 'layout'
						if( id.includes("RightSidebar") || id.includes('Alerts.vue') || id.includes('QInfoMessage.vue')
							|| id.includes('QAnchor') || id.includes('FormActionButtons'))
							return 'Sidebar'

						if (id.includes('/components/table')) return 'QTable'

						// decided by the algorithm
						return null;
					}
				}
			},
			chunkSizeWarningLimit: 650,
			cssCodeSplit: false,
			reportCompressedSize: false,
			modulePreload: false
		},
		plugins: [
			vue(),
			visualizer(/*{ open: true, filename: 'bundle-analysis.html' }*/),
			...(isDev ? [mkcert()] : [viteSvgSpritePlugin()])
		],
		css: {
			preprocessorOptions: {
				scss: {
					silenceDeprecations: ['import'],
					quietDeps: true
				}
			}
		},
		server: {
			open: false,
			proxy: {
				'/api': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/chatbotapi': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/telemetryapi': {
					target: 'https://localhost:7015/',
					secure: false
				},
				'/auth': {
					target: 'https://localhost:7015/',
					secure: false
				}
			},
			port: 5173,
			https: true
		}
	}
})

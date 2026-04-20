import ace from 'ace-builds'


// MODES
import modeJavascriptUrl from 'ace-builds/src-noconflict/mode-javascript?url'
ace.config.setModuleUrl('ace/mode/javascript', modeJavascriptUrl)

import modeHtmlUrl from 'ace-builds/src-noconflict/mode-html?url'
ace.config.setModuleUrl('ace/mode/html', modeHtmlUrl)

import modeCssUrl from 'ace-builds/src-noconflict/mode-css?url'
ace.config.setModuleUrl('ace/mode/css', modeCssUrl)

import modeCppUrl from 'ace-builds/src-noconflict/mode-c_cpp?url'
ace.config.setModuleUrl('ace/mode/c_cpp', modeCppUrl)

import modeCSharpUrl from 'ace-builds/src-noconflict/mode-csharp?url'
ace.config.setModuleUrl('ace/mode/csharp', modeCSharpUrl)

import modeXmlUrl from 'ace-builds/src-noconflict/mode-xml?url'
ace.config.setModuleUrl('ace/mode/xml', modeXmlUrl)

import modeJavaUrl from 'ace-builds/src-noconflict/mode-java?url'
ace.config.setModuleUrl('ace/mode/java', modeJavaUrl)

import modeSqlUrl from 'ace-builds/src-noconflict/mode-sql?url'
ace.config.setModuleUrl('ace/mode/sql', modeSqlUrl)

import modeMarkdownUrl from 'ace-builds/src-noconflict/mode-markdown?url'
ace.config.setModuleUrl('ace/mode/markdown', modeMarkdownUrl)

import modePlainTextUrl from 'ace-builds/src-noconflict/mode-plain_text?url'
ace.config.setModuleUrl('ace/mode/plain-text', modePlainTextUrl)


// WORKERS - FOR AUTO-COMPLETE AND SYNTAX CHECKING
import workerBaseUrl from 'ace-builds/src-noconflict/worker-base?url'
ace.config.setModuleUrl('ace/mode/base', workerBaseUrl)

import workerJavascriptUrl from 'ace-builds/src-noconflict/worker-javascript?url'
ace.config.setModuleUrl('ace/mode/javascript_worker', workerJavascriptUrl)

import workerHtmlUrl from 'ace-builds/src-noconflict/worker-html?url'
ace.config.setModuleUrl('ace/mode/html_worker', workerHtmlUrl)

import workerCssUrl from 'ace-builds/src-noconflict/worker-css?url'
ace.config.setModuleUrl('ace/mode/css_worker', workerCssUrl)

import workerXmlUrl from 'ace-builds/src-noconflict/worker-xml.js?url'
ace.config.setModuleUrl('ace/mode/xml_worker', workerXmlUrl)


// THEMES
import themeDawnUrl from 'ace-builds/src-noconflict/theme-dawn?url'
ace.config.setModuleUrl('ace/theme/dawn', themeDawnUrl)

import themeTomorrowNightEightiesUrl from 'ace-builds/src-noconflict/theme-tomorrow_night_eighties?url'
ace.config.setModuleUrl('ace/theme/tomorrow_night_eighties', themeTomorrowNightEightiesUrl)


// LANGUAGE TOOLS - AUTOCOMPLETE / SNIPPETS
import 'ace-builds/src-noconflict/ext-language_tools'
ace.require('ace/ext/language_tools')


// SEARCHBOX
import extSearchboxUrl from 'ace-builds/src-noconflict/ext-searchbox?url'
ace.config.setModuleUrl('ace/ext/searchbox', extSearchboxUrl)

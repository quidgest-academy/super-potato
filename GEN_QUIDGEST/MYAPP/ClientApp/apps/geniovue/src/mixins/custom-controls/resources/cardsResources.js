export default class CardsResources
{
	constructor(fnGetResource)
	{
		this._fnGetResource = typeof fnGetResource !== 'function' ? resId => resId : fnGetResource
		Object.defineProperty(this, '_fnGetResource', { enumerable: false })

		Object.defineProperty(this, 'noRecordsText', {
			get: () => this._fnGetResource('SEM_REGISTOS62529'),
			enumerable: true
		})
		Object.defineProperty(this, 'emptyText', {
			get: () => this._fnGetResource('SEM_DADOS_PARA_MOSTR24928'),
			enumerable: true
		})
		Object.defineProperty(this, 'createText', {
			get: () => this._fnGetResource('CRIAR24836'),
			enumerable: true
		})
		Object.defineProperty(this, 'insertText', {
			get: () => this._fnGetResource('INSERIR43365'),
			enumerable: true
		})
		Object.defineProperty(this, 'cardImage', {
			get: () => this._fnGetResource('IMAGEM_DO_CARTAO15404'),
			enumerable: true
		})
		Object.defineProperty(this, 'loading', {
			get: () => this._fnGetResource('A_CARREGAR___34906'),
			enumerable: true
		})
	}
}

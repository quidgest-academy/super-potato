/**
 * Class responsible for bookmarks actions (Read, add and remove).
 */
import { postData } from '@quidgest/clientapp/network'
import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'

import _isEmpty from 'lodash-es/isEmpty'

export default class Bookmarks
{
	/**
	 * Construct method
	 * @param _cb {function} call back function to update vue component model
	 */
	constructor(_cb)
	{
		this.updateData = _cb
		this.isActivated = false
	}

	/**
	 * Read bookmarks form server call callback function to update vue component model
	 */
	fetchData()
	{
		postData('Home', 'Bookmarks', null, (data) => this._updateContent(data))
	}

	/**
	 * Activate menu capture to be added in bookmarks tables
	 */
	activateSelectionMode()
	{
		this.isActivated = true
	}

	/**
	 * Update component Model through callback function
	 * @param data {object} data to be updated
	 */
	_updateContent(data)
	{
		if (this.updateData)
			this.updateData(data)
	}

	/**
	 * Add bookmark
	 * @param module {string} Module ID
	 * @param menuId {string} menu ID
	 */
	addBookmark(module, menuId)
	{
		if (this.isActivated)
		{
			this.isActivated = false
			if (!_isEmpty(module) && !_isEmpty(menuId))
			{
				postData('Home', 'AddBookmark', { module, menuId }, (data) => {
					this._updateContent(data)
				})
			}
		}
	}

	/**
	 * Remove bookmark
	 * @param bookmarkId {string} Primary key column of bookmark table
	 */
	removeBookmark(bookmarkId)
	{
		if (!_isEmpty(bookmarkId))
		{
			postData('Home', 'RemoveBookmark', { bookmarkId }, (data) => {
				if (data.Success)
					this.fetchData()
				else if (!_isEmpty(data.Message))
					displayMessage(data.Message)
			})
		}
	}
}

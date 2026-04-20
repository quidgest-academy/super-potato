import { createFramework } from '@quidgest/clientapp'

import { systemInfo } from '@/systemInfo'

const framework = createFramework({
	locale: systemInfo.locale
})

export default framework

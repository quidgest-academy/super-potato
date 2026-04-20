import '@testing-library/jest-dom'
import { render } from '@testing-library/vue'

import fakeData from '../cases/StaticText.mock'
import StaticText from '@/components/QStaticText.vue'

describe('QStaticText.vue', () => {
	it('Checks if it renders text with HTML', async () => {
		const m = fakeData.simpleUsage().htmlText
		const wrapper = await render(StaticText, {
			props: {
				text: m,
				supportsHtml: true,
				size: 'xxlarge'
			}
		})

		const text =
`<div class="i-static-text input-xxlarge">
  <p class="MsoNormal">
    <span lang="EN-US" style="mso-ansi-language:EN-US"> (<span style="color:red">*</span>) Field
    <b style="mso-bidi-font-weight:normal">
      <i style="mso-bidi-font-style:normal">
        <span style="color:red">required</span>
      </i>
    </b>
    <o:p></o:p>
    </span>
  </p>
</div>`
		expect(wrapper.html()).toBe(text)
	})

	it('Checks if it renders text without HTML', async () => {
		const m = fakeData.simpleUsage().htmlText
		const wrapper = await render(StaticText, {
			props: {
				text: m,
				supportsHtml: false,
				size: 'xxlarge'
			}
		})

		const text =
`<div class="i-static-text input-xxlarge">&lt;p class=MsoNormal&gt;
  &lt;span lang=EN-US style='mso-ansi-language:EN-US'&gt; (&lt;span style='color:red'&gt;*&lt;/span&gt;) Field
  &lt;b style='mso-bidi-font-weight:normal'&gt;
  &lt;i style='mso-bidi-font-style:normal'&gt;
  &lt;span style='color:red'&gt;required&lt;/span&gt;
  &lt;/i&gt;
  &lt;/b&gt;
  &lt;o:p&gt;&lt;/o:p&gt;
  &lt;/span&gt;
  &lt;/p&gt;</div>`
		expect(wrapper.html()).toBe(text)
	})
})

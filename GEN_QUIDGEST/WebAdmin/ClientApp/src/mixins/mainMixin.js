/** Reusable methods and data */
import moment from 'moment';
import { QUtils } from '@/utils/mainUtils';

export const reusableMixin = {
    methods: {
        navigateTo(event, name, hasSubmenu = false, tabId = undefined) { 
            var vm = this;
            vm.isSelected = false;
            vm.isMenuOpen = hasSubmenu ? !vm.isMenuOpen : false;

            // Navigate to the specified route with the tabId as a dynamic route parameter
            vm.$router.push({ 
                name: name, 
                params: { 
                    culture: vm.currentLang, 
                    system: vm.currentYear,
                    tabId // If tabId is provided, add it as a route param, otherwise omit it
                }
            });

            if (vm.currentSelected) {
                vm.currentSelected.classList.remove('selected');
            }

            if (!hasSubmenu) {
                event?.currentTarget.classList.add('selected');
                vm.currentSelected = event?.currentTarget;
            } else {
                event.currentTarget.classList.add('selected');
                vm.currentSelected = event.currentTarget;
                if (vm.Model.Applications.length > 0) {
                    const firstSubItem = vm.Model.Applications[0];
                    vm.currentApp = firstSubItem.Id;
                }
            }
        },
        isEmptyArray(arr) {
            return !(arr && arr.length > 0);
        },
        isEmptyObject(obj) {
            return $.isEmptyObject(obj);
        },
        formatDate(date) {
            if ($.isEmptyObject(date)) return '';
            else if ($.type(date) === 'string') {
                date = QUtils.tryParseDate(date);
            }
            if ($.type(date) === 'date' || moment.isMoment(date)) {
                if (date._isAMomentObject) { date = date.toDate(); }
                var text = date.toLocaleDateString('pt-PT') + ' ' + date.toLocaleTimeString('pt-PT');
                if (text === 'Invalid Date Invalid Date' || date.getFullYear() <= 0) { // IE11 and Null date
                    text = '-';
                }
                return text;
            }
            else return '';
        }
    },
	data()
	{
		var vm = this;
		return {
			Resources: new Proxy({
					__v_skip: false,
					__v_isReactive: true,
					__v_isRef: false,
					__v_isReadonly: false,
					__v_raw: true
				}, {
					get(target, prop, receiver) {
						if (Reflect.has(target, prop))
							return Reflect.get(target, prop, receiver)
						return vm.$tm(prop)
					}
				})
			}
	}
};




export function ReadProviderConfig(type, config, ProviderTypeList) {
	let tempConfig = [];

    if (!type)
        return tempConfig;

    let provider = ProviderTypeList.find(x => x.TypeFullName == type);
    if (!provider)
        return tempConfig;

    //create the temporary value array that the UI will need to supply editors for
    tempConfig = provider.Options.map(o => ({
		PValue: config[o.PropertyName],
        ...o
    }));

    return tempConfig;
}

export function WriteProviderConfig(tempConfig, type, ProviderTypeList) {
    if (!type) return;

    let provider = ProviderTypeList.find(x => x.TypeFullName == type);
    if (!provider) return;
	let obj = {};

	for (const o of provider.Options) {
		let prop = tempConfig.find(x => x.PropertyName == o.PropertyName);
		if (prop.PValue && prop.PValue.trim() !== '')
			obj[o.PropertyName] = prop.PValue;
	}

	return obj;
}

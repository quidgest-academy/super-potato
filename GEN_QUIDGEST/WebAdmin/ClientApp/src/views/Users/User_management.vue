<template>
	<div id="user_management_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.GESTAO_DE_UTILIZADOR20428 }}
			</h1>
		</div>
		<hr>
		<QAlert v-if="alert.isVisible"
			ref="alertBox"
			:type="alert.alertType"
			:text="alert.message"
			:icon="alert.icon"
			@message-dismissed="handleAlertDismissed" />
		<br v-if="alert.isVisible">
		<q-card
			class="q-card--admin-default"
			width="block">
			<q-row-container>
				<row>
					<q-card
						class="q-card--admin-default"
						:title="Resources.CONFIGURACAO_DO_UTIL56540"
						width="block">
						<q-row-container>
							<q-checkbox
								v-model="Model.StatusDisableLogin"
								:label="Resources.DESACTIVAR_CONTA37602"
								:readonly="lockControls" />
							<text-input
								ref="username"
								v-model="Model.Username"
								is-required
								:label="Resources.NOME__48276"
								:isReadOnly="Model.ModForm != '1'" />
							<text-input
								v-model="Model.Email"
								:label="Resources.EMAIL25170"
								:isReadOnly="lockControls" />
							<text-input
								v-model="Model.Phone"
								:label="Resources.TELEFONE37757"
								:isReadOnly="lockControls" />
							<q-checkbox
								v-model="Model.StatusFirstLogin"
								:label="Resources.O_UTILIZADOR_TEM_QUE05121"
								:readonly="lockControls" />
							<q-checkbox
								v-if="Model.ShowInvalidate2FA"
								v-model="Model.Invalidate2FA"
								:label="Resources.INVALIDAR_AUTENTICAC21095"
								:readonly="lockControls || Model.BlockInvalidate2FA" />
							<q-checkbox
								v-model="Model.PasswordChange"
								:label="Resources.ALTERAR_A_PALAVRA_CH54014"
								:readonly="lockControls || !hasQuidgestIDProvider" />
							<password-input
								v-model="Model.PasswordNew"
								:label="Resources.NOVA_15272"
								:isReadOnly="lockControls || !Model.PasswordChange"
								:style='{ "opacity": (!Model.PasswordChange) ? "0.5" : "1" }'
								:disabled="!Model.PasswordChange" />
							<password-input
								v-model="Model.PasswordConfirm"
								:label="Resources.CONFIRMAR_64824"
								:isReadOnly="lockControls  || !Model.PasswordChange"
								:style='{ "opacity": (!Model.PasswordChange) ? "0.5" : "1" }'
								:disabled="!Model.PasswordChange" />
							<div
								v-if="Model.PasswordChange"
								ref="PassMeter"
								id="passMeter">
								<meter
									ref="pswStrengthMeter"
									max="4" id="password-strength-meter"
									value="0" />
								<p
									ref="pswStrengthText"
									id="password-strength-text" />
							</div>
						</q-row-container>
					</q-card>
				</row>
				<row>
					<qtable
						id="PrivilegeTable"
						:rows="Model.Modules"
						:columns="tPrivileges.columns"
						:config="tPrivileges.config"
						:totalRows="totalPrivileges"
						@on-change-query="onChangeQuery"
						class="q-table--borderless">
						<template #permission="props">
							<multiselect
								v-model="Model.AssignedRoles[props.row.Cod]"
								:options="Model.AvaiableRoles[props.row.Cod]"
								:multiple="!Model.OnlyLevels"
								taggable
								:max="props.row.OnlyLevels?1:999"
								:custom-label="roleName"
								label="Designation"
								trackBy="Role" />
						</template>
					</qtable>
				</row>

				<row>
					<div class="q-button-container">
						<q-button
							ref="submitBtn"
							variant="bold"
							:label="Resources[Model.SubmitValue]"
							:disabled="submitBtnLock"
							@click="showDialog('confirm')" />
						<q-button
							:label="Resources.CANCELAR49513"
							@click="showDialog('cancel')" />
					</div>
				</row>
			</q-row-container>
		</q-card>
		
		<q-dialog
			class="alert-dialog"
			v-model="dialog.visible"
			:text="dialog.text"
			:icon="dialog.icon"
			:buttons="dialog.buttons">
		</q-dialog>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import Multiselect from '@/components/multiselect/multiselect_main';

	import _get from "lodash-es/get";

	export default {
		name: 'user_management',

		components: { 'multiselect': Multiselect },

		mixins: [reusableMixin],

		data() {
			return {
				Model: {},
				dialog: {
					visible: false,
					text: '',
					icon: {},
					buttons: []
				},
				dialogText: '',
				alert: {
					isVisible: false,
					alertType: 'info',
					message: '',
					icon: ''
				},
				tPrivileges: {
					columns: [
						{
							label: () => this.$t('SIGLA14738'),
							name: "Cod",
							sort: false
						},
						{
							label: () => this.$t('MODULO43834'),
							name: "Description",
							sort: false
						},
						{
							label: () => this.$t('ROLES61449'),
							slot_name: "permission"
						}],
					config: {
						table_title: () => this.$t('USER_ROLES25359'),
						pagination: false,
						pagination_info: false,
						server_mode: true,
						preservePageOnDataChange: true,
						highlight_row_hover: false,

						global_search: {
							classes: "qtable-global-search",
							placeholder: this.$t('PESQUISAR_MODULO27690'),
							searchOnPressEnter: true,
							showRefreshButton: true,
							searchDebounceRate: 1000
						},
						queryParams: {
							sort: [],
							filters: [],
							global_search: "",
							per_page: 10,
							page: 1
						}
					}
				},
				identityProviders: [],
			};
		},

		computed: {
			lockControls() { 
				return this.$route.params.mod == 3;
			},

			totalPrivileges() {
				if (!$.isEmptyObject(this.Model.Privileges)) return this.Model.Privileges.length; return 0;
			},

			submitBtnLock() {
				return $.isEmptyObject(this.Model.Username) ||
				(this.Model.PasswordChange && ($.isEmptyObject(this.Model.PasswordNew) ||
				$.isEmptyObject(this.Model.PasswordConfirm) ||
				this.Model.PasswordNew !== this.Model.PasswordConfirm));
			},

			passwordStrength() {
				//calcular strenght da password
				score = this.scorePassword(this.Model.PasswordNew),
				scoreStrenght = 0,
				pswStrengthMeter = $(this.$refs.pswStrengthMeter),
				pswStrengthText = $(this.$refs.pswStrengthText);

				if ($.isEmptyObject(this.Model.PasswordNew)) {
					scoreStrenght = 0;
					pswStrengthMeter.text('');
				}
				else {
				if (score > 80) {
					scoreStrenght = 4;
					pswStrengthText.text(this.Resources.FORTE13835);
				} else if (score > 60) {
					scoreStrenght = 3;
					pswStrengthText.text(this.Resources.BOM29058);
				} else if (score >= 30) {
					scoreStrenght = 2;
					pswStrengthText.text(this.Resources.FRACO22195);
				} else if (score < 30) {
					scoreStrenght = 1;
					pswStrengthText.text(this.Resources.POBRE46544);
				}
				}
				pswStrengthMeter.val(scoreStrenght);

				return scoreStrenght;
			},

			hasQuidgestIDProvider() {
				for (var key in this.identityProviders) {
					if (this.identityProviders[key] === "GenioServer.security.QuidgestIdentityProvider") { return true; }
				}
				return false;
			}
		},

		methods: {
			showDialog(type) {
				switch (type) {
					case 'confirm':
						this.dialog.text = this.Resources.DESEJA_GRAVAR_ESTA_F03881;
						this.dialog.icon = { 
							icon: 'alert',
							color: 'warning'
						};
						this.dialog.buttons = [
							{
								id: 'confirm-btn',
								props: {
									label: this.Resources.SIM28552,
									variant: 'bold'
								},
								action: this.submit
							},
							{
								id: 'cancel-btn',
								props: {
									label: this.Resources.NAO06521
								},
							},
						];
						break;

					case 'cancel':
						this.dialog.text = this.Resources.ATENCAO__AS_ALTERACO04365;
						this.dialog.icon = { 
							icon: 'alert',
							color: 'warning'
						};
						this.dialog.buttons = [
							{
								id: 'confirm-btn',
								props: {
									label: this.Resources.SIM28552,
									variant: 'bold'
								},
								action: this.redirectToAllUsers
							},
							{
								id: 'cancel-btn',
								props: {
									label: this.Resources.NAO06521
								},
							},
						];
						break;

					case 'info':
						this.dialog.text = this.Resources.UTILIZADOR_ALTERADO_42131;
						this.dialog.icon = {
							icon: "check-circle-outline",
							color: 'success'
						};
						this.dialog.buttons = [
							{
								id: 'ok-btn',
								props: {
									label: this.Resources.OK57387,
									variant: 'bold'
								},
								action: this.redirectToAllUsers
							},
						];
						break;

					default:
						break;
				}

				this.dialog.visible = true;
			},

			setAlert(type, message) {
				this.alert.isVisible = true;
				this.alert.alertType = type;
				this.alert.message = message;

				this.$nextTick(() => {
					const alertBox = this.$refs.alertBox;
					if (alertBox) {
						alertBox.$el.scrollIntoView({ behavior: 'smooth' });
					}
				});
			},

			handleAlertDismissed() {
				this.alert.isVisible = false;
			},

			redirectToAllUsers() {
				this.$router.replace({ name: 'users', params: { culture: this.currentLang, system: this.currentYear } });
			},

			fetchData() {
				const mod = this.$route.params.mod,
				cod = _get(this.$route.params, 'cod', null);
				QUtils.log("Fetch data - User management");
				QUtils.FetchData(QUtils.apiActionURL('ManageUsers', 'Index', { mod, cod })).done((data) => {
				QUtils.log("Fetch data - OK (User management)", data);

				if (data.Success) {
					this.Model = data.model;

					// Update IdentityProviders list
					this.identityProviders = [];
					$.each(data.model.IdentityProviders, (idx, identityProvider) => {
					this.identityProviders.push(identityProvider);
					});
				}
				else if (data.redirect) {
					this.$router.replace({ name: data.redirect });
				}

				this.getModuleList()
				});
			},

			getModuleList() {
				QUtils.log("Fetching module list...");
				QUtils.FetchData(QUtils.apiActionURL('ManageUsers', 'GetModules', this.tPrivileges.queryParams))
					.done((data) => {
						QUtils.log("Module list fetched successfully:", data);
						this.Model.Modules = data.data || []; 
						this.tPrivileges.total_rows = data.recordsTotal || 0;
					})
					.fail((error) =>  {
						QUtils.log("Error fetching module list:", error);
					});
			},

			onChangeQuery(queryParams) {
				this.tPrivileges.queryParams = queryParams;

				this.fetchData();
			},

			getDialogText(mod) {
				switch (mod) {
					case '1':
						this.dialogText = this.Resources.UTILIZADOR_CRIADO_CO16450;
						break;
					case '2':
						this.dialogText = this.Resources.UTILIZADOR_ALTERADO_42131;
						break;
					default:
						break;
				}
			},

			submit() {
				const mod = this.$route.params.mod,
				cod = _get(this.$route.params, 'cod', null);
				QUtils.postData('ManageUsers', 'SaveConfig', this.Model, { mod, cod }, (data) => {
					if (data.Success) {
						this.getDialogText(this.$route.params.mod);

						if (data.ignoredRoles.length > 0) {
							let ignoredRolesList = data.ignoredRoles.map(pair => {
								const roleName = `<b>${this.Resources[pair.Designation]}</b>`
								const moduleName = `<b>${pair.Module}</b>`
								return roleName + ' ' + this.Resources.DE62476 + ' ' + moduleName;
							});
							let message = this.Resources.ATENCAO__UTILIZADOR_05890 + "<br>";
							message += ignoredRolesList.join("<br>") + ".";
							this.dialog.text = message;
							this.dialog.icon = { 
								icon: 'alert',
								color: 'warning'
							};
						}
						else {
							this.dialog.icon = {
								icon: "check-circle-outline",
								color: 'success'
							};
							this.dialog.text = this.dialogText;
						}

						this.dialog.buttons = [
							{
								id: 'ok-btn',
								props: {
									label: this.Resources.OK57387,
									variant: 'bold'
								},
								action: () => {
									this.redirectToAllUsers()
								}
							}
						];

						this.dialog.visible = true;
					}
					else {
						$.each(data.model, (propName, value) => { this.Model[propName] = value; });
						if (this.Model.ResultMsg) {
							this.setAlert('danger', this.Model.ResultMsg);
						}
					}
				});
			},

			scorePassword(pass) {
				var score = 0;
				if (!pass)
				return score;

				// award every unique letter until 5 repetitions
				var letters = new Object();
				for (var i = 0; i < pass.length; i++) {
				letters[pass[i]] = (letters[pass[i]] || 0) + 1;
				score += 5.0 / letters[pass[i]];
				}

				// bonus points for mixing it up
				var variations = {
				digits: /\d/.test(pass),
				lower: /[a-z]/.test(pass),
				upper: /[A-Z]/.test(pass),
				nonWords: /\W/.test(pass),
				}

				var variationCount = 0;
				for (var check in variations) {
				variationCount += (variations[check] == true) ? 1 : 0;
				}
				score += (variationCount - 1) * 10;

				return parseInt(score);
			},

			roleName (role) {
				return this.Resources[role.Designation];
			}
		},

		created() {
			// Ler dados
			this.fetchData();
		},

		updated() {
			this.getModuleList();
		},

		watch: {
			// call again the method if the route changes
			'$route': 'fetchData',

			'Model.PasswordChange': {
				handler() {
					this.Model.PasswordNew = "";
					this.Model.PasswordConfirm = "";
				},
				deep: true
			},
			
			'Model.PasswordNew': {
				handler() {
					//calcular strenght da password
					var score = this.scorePassword(this.Model.PasswordNew),
					scoreStrenght = 0,
					pswStrengthMeter = $(this.$refs.pswStrengthMeter),
					pswStrengthText = $(this.$refs.pswStrengthText);

					if ($.isEmptyObject(this.Model.PasswordNew)) {
						scoreStrenght = 0;
						pswStrengthMeter.text('');
					}
					else {
						if (score > 80) {
							scoreStrenght = 4;
							pswStrengthText.text(this.Resources.FORTE13835);
						} else if (score > 60) {
							scoreStrenght = 3;
							pswStrengthText.text(this.Resources.BOM29058);
						} else if (score >= 30) {
							scoreStrenght = 2;
							pswStrengthText.text(this.Resources.FRACO22195);
						} else if (score < 30) {
							scoreStrenght = 1;
							pswStrengthText.text(this.Resources.POBRE46544);
						}
					}
					pswStrengthMeter.val(scoreStrenght);
				},
				deep: true
			}
		}
	};
</script>

interface VueContext {
	Resources: Record<string, object>
}

class BaseResources
{
	private readonly resources: Record<string, object>

	constructor(vueContext: VueContext)
	{
		this.resources = vueContext.Resources
	}

		get actions() {
			return this.resources.ACOES22599
		}
}

class UsersTexts extends BaseResources
{
	constructor(vueContext: VueContext) {
		super(vueContext);
	}
		get assignRolesQuickly() {
			return this.resources.ATRIBUIR_RAPIDAMENTE15967
		}
		get selectUsers() {
			return this.resources._1__SELECIONE_UTILIZA54069
		}
		get selectRoles() {
			return this.resources._2__SELECIONE_FUNCOES21542
		}
		get reviewAndConfirm() {
			return this.resources._3__REVEJA_E_CONFIRME45679
		}
		get allUsers() {
			return this.resources.TODOS_OS_UTILIZADORE41512
		}
		get searchUser() {
			return this.resources.PESQUISAR_UTILIZADOR60804
		}
		get userRoles() {
			return this.resources.USER_ROLES25359
		}
		get roleAlreadyAssigned() {
			return this.resources.FUNCAO_AT_ROLE_JA_TINH60074
		}
		get roleNotAssigned() {
			return this.resources.FUNCAO_AT_ROLE_NAO_EST14526
		}
		get accessManagementReport() {
			return this.resources.RELATORIO_DE_GESTAO_29557
		}
		get redundantPermissionsWarning() {
			return this.resources.ATENCAO__ALGUMAS_PER50140
		}
}

class SystemConfigTexts extends BaseResources
{
	constructor(vueContext: VueContext) {
		super(vueContext);
	}
		get assignRolesQuickly() {
			return this.resources.ATRIBUIR_RAPIDAMENTE15967
		}
		get selectUsers() {
			return this.resources._1__SELECIONE_UTILIZA54069
		}
		get selectRoles() {
			return this.resources._2__SELECIONE_FUNCOES21542
		}
		get integrationSettingsAI() {
			return this.resources.CONFIGURACOES_DE_INT56161
		}
		get urlAPIBackendLabel() {
			return this.resources.URL_DO_BACKEND_DA_AP53038
		}
		get urlAPIBackendInfo() {
			return this.resources.DEVERA_COLOCAR_O_END10058
		}
		get saveConfigurationButton() {
			return this.resources.GRAVAR_CONFIGURACAO36308
		}
		get reportsTitle() {
			return this.resources.RELATORIOS37339
		}
		get reportsPathLabel() {
			return this.resources.CAMINHO_PARA_RELATOR05547
		}
		get reportsPathInfo() {
			return this.resources.ESPECIFICA_O_CAMINHO51946
		}
		get crystalReportsLabel() {
			return this.resources.CRYSTAL_REPORTS15382
		}
		get crystalReportsInfo() {
			return this.resources.UTILIZADO_PARA_CARRE49850
		}
		get reportingServicesLabel() {
			return this.resources.REPORTING_SERVICES45145
		}
		get reportingServicesInfo() {
			return this.resources.PERMITE_VERIFICAR_NO39154
		}
		get sqlServerReportingServicesTitle() {
			return this.resources.SQL_SERVER_REPORTING62106
		}
		get isLocalReportsLabel() {
			return this.resources.SAO_OS_RELATORIOS_LO04230
		}
		get displayTexts() {
			return this.resources.EXIBICAO16445
		}
		get defaultDataSystem() {
			return this.resources.SISTEMA_DE_DADOS_PAD13413
		}
		get defaultDataSystemInfo() {
			return this.resources.O_SISTEMA_DE_DADOS_U47595
		}
		get hideDataSystems() {
			return this.resources.OCULTAR_SISTEMAS_DE_60940
		}
		get hideDataSystemsInfo() {
			return this.resources.QUANDO_SELECIONADO__16895
		}
		get invalidDataSystems() {
			return this.resources.SISTEMAS_DE_DADOS_IN53544
		}
		get invalidDataSystemsAlert() {
			return this.resources.DEVE_CONCLUIR_A_CONF15894
		}
		get dataSystems() {
			return this.resources.SISTEMAS_DE_DADOS45551
		}
		get createNewDataSystem() {
			return this.resources.CRIAR_UM_NOVO_SISTEM49777
		}
		get dataSystemName() {
			return this.resources.NOME_DO_SISTEMA_DE_D18974
		}
		get dataSystemNameUniqueInfo() {
			return this.resources.OS_NOMES_DOS_SISTEMA01515
		}
		get databaseName() {
			return this.resources.NOME_DA_BASE_DE_DADO25105
		}
		get databaseNameUniqueInfo() {
			return this.resources.OS_NOMES_DAS_BASES_D40646
		}
		get serverName() {
			return this.resources.NOME_DO_SERVIDOR13641
		}
		get databaseVersion() {
			return this.resources.DATABASE_VERSION15344
		}
		get dataSystemDeletedSuccess() {
			return this.resources.O_SISTEMA_DE_DADOS_F39849
		}
		get currentDataSystemDefault() {
			return this.resources.O_SISTEMA_DE_DADOS_A48279
		}
		get confirmDeleteDataSystem() {
			return this.resources.TEM_A_CERTEZA_QUE_QU16920
		}
		get schedulerTitle() {
			return this.resources.AGENDADOR40611
		}
		get enabledLabel() {
			return this.resources.ATIVO_00196
		}
		get scheduledTasksTitle() {
			return this.resources.TAREFAS_AGENDADAS24414
		}
		get scheduledTaskTitle() {
			return this.resources.TAREFA_AGENDADA03399
		}
		get cronInfoText() {
			return this.resources._SEGUNDO_MINUTO_HORA37214
		}
		get auditTitle() {
			return this.resources.AUDITORIA29703
		}
		get auditLoginLabel() {
			return this.resources.AUDITORIA_DE_LOGIN_D00905
		}
		get auditActionsLabel() {
			return this.resources.AUDITORIA_DE_ACOES_D42106
		}
		get auditSystemLabel() {
			return this.resources.AUDITORIA_DO_SISTEMA08460
		}
		get eventTrackingLabel() {
			return this.resources.REGISTO_DE_EVENTOS65341
		}
		get currentDataSystemTitle() {
			return this.resources.SISTEMA_DE_DADOS_ATU09110
		}
		get serverNameInfo() {
			return this.resources.O_NOME_DO_SERVIDOR_E58624
		}
		get databaseNameInfo() {
			return this.resources._SISTEMA__ANO__E_G__40394
		}
		get databaseServerTypeLabel() {
			return this.resources.TIPO_DE_SERVIDOR_DE_25581
		}
		get serviceIdentifierLabel() {
			return this.resources.IDENTIFICADOR_DO_SER22713
		}
		get serviceNameLabel() {
			return this.resources.NOME_DO_SERVICO32188
		}
		get databaseLoginLabel() {
			return this.resources.LOGIN_DE_ACESSO_A_BA52816
		}
		get databaseConnectionTitle() {
			return this.resources.AUTENTICACAO_NA_BASE39084
		}
		get encryptConnectionLabel() {
			return this.resources.ENCRIPTAR_LIGACAO12834
		}
		get domainUserLabel() {
			return this.resources.UTILIZADOR_DE_DOMINI41043
		}
		get testServerConnectionButton() {
			return this.resources.TESTAR_CONEXAO_COM_O06434
		}
		get sharedTablesLabel() {
			return this.resources.TABELAS_PARTILHADAS29704
		}
		get dataSystemLog() {
			return this.resources.SISTEMA_DE_DADOS_DE_45948
		}
		get logDataSystemTitle() {
			return this.resources.SISTEMA_DE_DADOS_DE_45948
		}
		get qualityAssuranceTitle() {
			return this.resources.GARANTIA_DE_QUALIDAD19784
		}
		get qaEnvironmentLabel() {
			return this.resources.AMBIENTE_DE_QA_09940
		}
		get qaEnvironmentInfo() {
			return this.resources.SELECIONE_PARA_MOSTR59643
		}
		get dateFormatTitle() {
			return this.resources.FORMATO_DAS_DATAS11781
		}
		get numberFormatTitle() {
			return this.resources.FORMATO_DE_NUMERO58330
		}
		get decimalSeparatorLabel() {
			return this.resources.SEPARADOR_DECIMAL14173
		}
		get groupSeparatorLabel() {
			return this.resources.SEPARADOR_DE_GRUPO26735
		}
		get negativeNumberFormatLabel() {
			return this.resources.FORMATO_DE_NUMERO_NE41581
		}
		get reportLabel() {
			return this.resources.RELATORIO62426
		}
		get reportsByLanguageTitle() {
			return this.resources.RELATORIOS_POR_LINGU09488
		}
		get elasticsearchTitle() {
			return this.resources.ELASTICSEARCH49143
		}
		get elasticsearchSearchEngineTitle() {
			return this.resources.MOTOR_DE_PESQUISA__E50766
		}
		get fscrawlerLabel() {
			return this.resources.FSCRAWLER01982
		}
		get messagingSystemTitle() {
			return this.resources.SISTEMA_DE_MENSAGENS07077
		}
		get messagingQueueServerTitle() {
			return this.resources.MENSAGENS_QUEUE_SERV62690
		}
		get messageBrokerTitle() {
			return this.resources.CORRETOR_DE_MENSAGEN22044
		}
		get publishTitle() {
			return this.resources.PUBLICAR52698
		}
		get subscribeTitle() {
			return this.resources.INSCREVER_SE07499
		}
		get journalTimeoutLabel() {
			return this.resources.JOURNAL_TIMEOUT__MIN38634
		}
		get maxSendNumberLabel() {
			return this.resources.NUMERO_MAXIMO_DE_TEN51201
		}
		get messageListTitle() {
			return this.resources.LISTA_DE_MENSAGENS31887
		}
		get queueTitle() {
			return this.resources.QUEUE45251
		}
		get queueNameLabel() {
			return this.resources.NOME_DA_QUEUE56594
		}
		get queueChannelLabel() {
			return this.resources.CANAL_DA_QUEUE34934
		}
		get queuePathLabel() {
			return this.resources.TRAJETO_DA_QUEUE07185
		}
		get blockSizeLabel() {
			return this.resources.TAMANHO_DO_BLOCO42316
		}
		get unicodeLabel() {
			return this.resources.UNICODE63246
		}
		get usesMsmqLabel() {
			return this.resources.USA_MSMQ18528
		}
		get journalLabel() {
			return this.resources.JOURNAL20931
		}
		get acksConfigTitle() {
			return this.resources.CONFIGURACAO_DE_ACKS49550
		}
		get sourceQueueLabel() {
			return this.resources.QUEUE_ORIGEM31278
		}
		get ackQueueLabel() {
			return this.resources.QUEUE_ACK30680
		}
		get property() {
			return this.resources.PROPERTY43977
		}
		get insertNewKey() {
			return this.resources.INSERT_NEW_KEY15186
		}
		get listDefaultKeys() {
			return this.resources.LIST_DEFAULT_KEYS58194
		}
		get advancedProperties() {
			return this.resources.PROPRIEDADES_AVANCAD23972
		}
		get report() {
			return this.resources.RELATORIO62426
		}
		get reportsByLanguage() {
			return this.resources.RELATORIOS_POR_LINGU09488
		}
		get configuracaoDoSistema() {
			return this.resources.CONFIGURACAO_DO_SIST39343
		}
		get estadoDaOperacao() {
			return this.resources.ESTADO_DA_OPERACAO38065
		}
		get sistemasDeDados() {
			return this.resources.SISTEMAS_DE_DADOS45551
		}
		get definicoesDoEcra() {
			return this.resources.DEFINICOES_DO_ECRA09420
		}
		get iaEServicosExternos() {
			return this.resources.IA_E_SERVICOS_EXTERN49145
		}
		get integracao() {
			return this.resources.INTEGRACAO28978
		}
		get propriedadesExtra() {
			return this.resources.PROPRIEDADES_EXTRA18082
		}
		get connectionSuccess() {
			return this.resources.CONEXAO_BEM_SUCEDIDA50537
		}
		get connectionFailed() {
			return this.resources.FALHA_NA_CONEXAO29916
		}
		get mcpSecurityMode() {
			return this.resources.MODO_DE_SEGURANCA_MC18759
		}
		get mcpSecurityModeHelp() {
			return this.resources.DEFINE_O_MODO_DE_SEG18180
		}
		get jwtEncryptionKey() {
			return this.resources.CHAVE_DE_ENCRIPTACAO37781
		}
		get jwtEncryptionKeyHelp() {
			return this.resources.CHAVE_SECRETA_UTILIZ48448
		}
		get urlMCPLabel() {
			return this.resources.URL_DO_SERVIDOR_MCP19003
		}
		get urlMCPInfo() {
			return this.resources.ENDPOINT_DO_SERVIDOR41381
		}
}

class AppConfigTexts extends BaseResources
{
	constructor(vueContext: VueContext) {
		super(vueContext);
	}
		get appConfigurationTitle() {
			return this.resources.CONFIGURACAO_DA_APLI59110
		}
		get authenticationMode() {
			return this.resources.MODO_DE_AUTENTICACAO19339
		}
		get concurrentSessionsPolicy() {
			return this.resources.POLITICA_DE_SESSOES_19368
		}
		get allowAuthenticationRecovery() {
			return this.resources.PERMITE_RECUPERACAO_41959
		}
		get activateTwoFactorAuth() {
			return this.resources.ATIVAR_AUTENTICACAO_40943
		}
		get mandatoryTwoFactorAuth() {
			return this.resources.OBRIGATORIO_A_UTILIZ32451
		}
		get sessionTimeout() {
			return this.resources.TIME_OUT_DA_SESSAO36825
		}
		get passwordPolicy() {
			return this.resources.POLITICA_DE_PASSWORD17131
		}
		get minCharacters() {
			return this.resources.MINIMO_DE_CARACTERES10869
		}
		get passwordStrength() {
			return this.resources.MODO_DE_AUTENTICACAO19339
		}
		get maxLoginAttempts() {
			return this.resources.NUMERO_MAXIMO_TENTAT34521
		}
		get passwordExpirationDays() {
			return this.resources.EXPIRACAO_DA_PASSWOR46052
		}
		get daysToExpiration() {
			return this.resources.DIAS_PARA_A_EXPIRACA24916
		}
		get encryptionAlgorithm() {
			return this.resources.ALGORITMO_DE_ENCRIPT09649
		}
		get usePasswordBlacklist() {
			return this.resources.USE_PASSWORD_BLACKLI22314
		}
		get managePasswordBlacklist() {
			return this.resources.MANAGE_PASSWORD_BLAC01612
		}
		get operationStatus() {
			return this.resources.ESTADO_DA_OPERACAO38065
		}
		get blacklistedPasswordsInDb() {
			return this.resources.BLACKLISTED_PASSWORD46582
		}
		get deleteAllBlacklistedPasswords() {
			return this.resources.DELETE_ALL_BLACKLIST01597
		}
		get identityProvider() {
			return this.resources.FORNECEDOR_DE_IDENTI58587
		}
		get roleProvider() {
			return this.resources.FORNECEDOR_DE_AUTORI36867
		}
		get precondition() {
			return this.resources.PRECONDICAO44917
		}
		get fixedUser() {
			return this.resources.UTILIZADOR_FIXO32336
		}
		get thisNameAlreadyExists() {
			return this.resources.ESTE_NOME_JA_EXISTE_51368
		}
		get autoLogin() {
			return this.resources.LOGIN_AUTOMATICO22707
		}
		get identityProvidersTitle() {
			return this.resources.FORNECEDORES_DE_IDEN35608
		}
		get roleProvidersTitle() {
			return this.resources.FORNECEDORES_DE_AUTO29899
		}
		get fixedUsersTitle() {
			return this.resources.UTILIZADORES_FIXOS00716
		}
		get pathAppLabel() {
			return this.resources.CAMINHO_PARA_A_APLIC44450
		}
		get pathDocumentsLabel() {
			return this.resources.CAMINHO_PARA_DOCUMEN18456
		}
		get downloadConfigFile() {
			return this.resources.DESCARREGAR_FICHEIRO61580
		}
}

export {
	UsersTexts,
	SystemConfigTexts,
	AppConfigTexts
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.framework
{
	/// <summary>
	/// Classe que representa o par (Status, Message).
	/// Que representa a resposta de pedidos de INS, ALT, DUP, CANC, ELI
	/// </summary>
	public class StatusMessage
	{
		private Status status;
		private string mensagem;
		private string origin;

		/// <summary>
		/// Agregação de todos os StatusMessage.
		/// </summary>
		private readonly Stack<StatusMessage> _statusMessageStack = new Stack<StatusMessage>();

		/// <summary>
		/// Constructor da classe (Aggregator)
		/// </summary>
		public StatusMessage()
		{
			this.status = Status.OK;
			this.mensagem = string.Empty;
		}

		/// <summary>
		/// Constructor da classe
		/// </summary>
		public StatusMessage(Status status, string mensagem, string origin = "")
		{
			this.status = status;
			this.mensagem = mensagem;
			this.origin = origin;
		}

		/// <summary>
		/// Método que permite fazer uma copia do objecto sem ser por referencia. Este método nao copia a stack.
		/// </summary>
		/// <param name="obj">objecto a copiar</param>
		public void Clone(StatusMessage msg)
		{
			this.status = msg.Status;
			this.mensagem = msg.Message;
			this.origin = msg.Origin;
		}

		/// <summary>
		/// Implementação do método equals to o tipoStatusMensagem
		/// </summary>
		/// <param name="obj">objecto a comparar</param>
		/// <returns>true se forem iguais e false caso contrário</returns>
		public override bool Equals(Object obj)
		{
			if (obj is StatusMessage n)
				return (n.Status == this.Status && n.Message == this.Message);

			//TODO: Why return True ???
			return true;
		}

		/// <summary>
		/// Override do método GetHashCode do tipo Object
		/// </summary>
		/// <returns>devolve o hashcode do objecto Qfield</returns>
		public override int GetHashCode()
		{
            unchecked // Allow arithmetic overflow, numbers will just "wrap around"
            {
                int hashcode = 1430287;
                hashcode = hashcode * 7302013 ^ Status.GetHashCode();
                hashcode = hashcode * 7302013 ^ Message.GetHashCode();
                return hashcode;
            }
		}

		/// <summary>
		/// True if there's an error among the status messages, false otherwise
		/// </summary>
		public bool HasError => this.ContainsStatus(Status.E);

		/// <summary>
		/// True if there's a warning among the status messages, false otherwise
		/// </summary>
		public bool HasWarning => this.ContainsStatus(Status.W);

		/// <summary>
		/// True if there's no errors or warnings among the status messages, false otherwise
		/// </summary>
		public bool IsOk => !HasError && !HasWarning;

		/// <summary>
		/// Status da resposta
		/// </summary>
		public Status Status
		{
			get
			{
				// Se tiver um E -> Erro
				// Se foram todos W -> Warning
				// Se só OK, então OK
				if (HasError)
					return Status.E;
				else if (HasWarning) // Basta um "W" ou devem ser todos !?
					return Status.W;
				return this.status;
			}
		}

		/// <summary>
		/// Message description
		/// </summary>
		public string Message
		{
			get
			{
				var curStatus = this.Status;
				var lastSM = _statusMessageStack.FirstOrDefault(sm => sm.Status.Equals(curStatus));
				if (lastSM != null)
					return lastSM.Message;
				else
					return this.mensagem;
			}
		}

		/// <summary>
		/// A list with all the warnings in the stack.
		/// </summary>
		public IList<StatusMessage> Warnings
		{
			get
			{
				return _statusMessageStack.Where(m => m.Status == Status.W || m.Status == Status.OK_MAIS_W).ToList();
			}
		}

		/// <summary>
		/// A list with all the warning messages in the stack.
		/// </summary>
		public IList<string> WarningMessages
		{
			get
			{
				IList<string> warningList = new List<string>(Warnings.Count);
				foreach (StatusMessage m in Warnings)
					warningList.Add(m.Message);
				return warningList;
			}
		}

		/// <summary>
		/// Message origin information
		/// </summary>
		public string Origin
		{
			get
			{
				var curStatus = this.Status;
				var lastSM = _statusMessageStack.FirstOrDefault(sm => sm.Status.Equals(curStatus));
				if (lastSM != null)
					return lastSM.Origin;
				else
					return this.origin;
			}
		}

		/// <summary>
		/// Merge a status message with multiple message into one. Only merges first level for now.
		/// </summary>
		/// <param name="statusMensagem"></param>
		public StatusMessage MergeStatusMessage(StatusMessage messageStatus)
		{
			if (messageStatus == null)
				throw new ArgumentNullException(nameof(messageStatus));

			while (messageStatus._statusMessageStack.Count > 1)
			{
				var msg = messageStatus._statusMessageStack.Pop();
				_statusMessageStack.Push(msg);
			}
			if(!_statusMessageStack.Contains(messageStatus))
				_statusMessageStack.Push(messageStatus);

			return this;
		}

		/// <summary>
		/// Verificar se exists StatusMessage com determinado status
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public bool ContainsStatus(Status status)
		{
			return _statusMessageStack.Any(sm => sm.ContainsStatus(status)) || this.status == status;
		}

		/// <summary>
		/// Returns the list of errors in this status message;
		/// </summary>
		public List<StatusMessage> GetErrorList()
		{
			return _statusMessageStack.Where(sm => sm.Status == Status.E).ToList();
		}

		/// <summary>
		/// Concatenate all status messages into one string using a separator.
		/// </summary>
		/// <returns>string containing all status messages concatenated</returns>
		public string PrintMessages(string separator="; ")
		{
			return string.Join(separator, _statusMessageStack
				.Where(sm => !string.IsNullOrEmpty(sm.Message))
                .Select(sm => sm.Message) );
		}

		/// <summary>
		/// Gets all status messages.
		/// </summary>
		/// <returns>Array containing all status messages concatenated</returns>
		public string[] GetStackMessages()
		{
			return _statusMessageStack
				.Where(sm => !string.IsNullOrEmpty(sm.Message))
				.Select(sm => sm.Message).ToArray();
		}

		#region Static functions

		public static StatusMessage GetAggregator()
		{
			return new StatusMessage();
		}
		public static StatusMessage OK(string msg = null)
		{
			return new StatusMessage(Status.OK, msg ?? string.Empty);
		}
		public static StatusMessage Error(string msg = null, string origin = "")
		{
			return new StatusMessage(Status.E, msg ?? "Error", origin);
		}
		public static StatusMessage Warning(string msg = null, string origin = "")
		{
			return new StatusMessage(Status.W, msg ?? "Warning", origin);
		}

		#endregion
	}
}

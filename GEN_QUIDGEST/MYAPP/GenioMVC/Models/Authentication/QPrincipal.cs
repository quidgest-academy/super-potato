using System;
using System.Security.Principal;

using CSGenio.framework;

namespace GenioMVC.Models
{
	[Serializable]
	public class QPrincipal : GenericPrincipal
	{
		public User User { get; set; }

		public QPrincipal(IIdentity identity, string[] roles, User user) : base(identity, roles)
		{
			this.User = user;
		}

		public QPrincipal(User user) : base(new GenericIdentity(user.Name), new string[] { })
		{
			this.User = user;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading;

namespace GenioMVC.Models
{
	public static class UserCache
	{
		// Dicionário de sessões indexado por um token de sessão
		private static Dictionary<string, QPrincipal> cache = new Dictionary<string, QPrincipal>();

		// Semáforo que garante a exclusividade de leitura e escrita sobre o dicionário das sessões
		private static ReaderWriterLockSlim s_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		/// <summary>
		/// Obtem o user associado ao token de sessão, se existir
		/// </summary>
		/// <param name="token">Token</param>
		/// <param name="user">User associado se existir em sessão</param>
		/// <returns>True se o token é um token válido de sessão</returns>
		public static bool GetSessionUser(string token, out QPrincipal user)
		{
			s_lock.EnterReadLock();
			bool res = cache.TryGetValue(token, out user);
			s_lock.ExitReadLock();
			return res;
		}

		/// <summary>
		/// Associa um token de sessão a um user
		/// </summary>
		/// <param name="user">User</param>
		/// <returns>Token de sessão</returns>
		public static string CreateCache(QPrincipal user)
		{
			string token = Guid.NewGuid().ToString().Replace("-", "");
			s_lock.EnterWriteLock();
			cache.Add(token, user);
			s_lock.ExitWriteLock();
			return token;
		}
	}
}

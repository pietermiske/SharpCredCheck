using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Net;

namespace SharpCredCheck
{
	class Program
	{

		private static bool ValidateMachineCredentials(string username, string password)
		{
			PrincipalContext context = new PrincipalContext(ContextType.Machine);
			bool result = context.ValidateCredentials(username, password);
			return result;
		}


		private static bool ValidateDomainCredentials(string username, string password, string domain)
		{

			PrincipalContext context = new PrincipalContext(ContextType.Domain, domain);
			bool result = context.ValidateCredentials(username, password);
			return result;
		}



		private static bool ValidateDomainCredentialsDC(string username, string password, string domain, string dc)
		{
			string ldapBase = dc + ":389";
			bool result = true;

			var credentials = new NetworkCredential(username, password, domain);
			var domainController = new LdapDirectoryIdentifier(ldapBase);
			var connection = new LdapConnection(domainController, credentials);

			try
			{
				connection.Bind();
			}
			catch (Exception)
			{
				result = false;
			}

			connection.Dispose();
			return result;
		}




		public static void logo()
		{
			Console.WriteLine(@"
  ___ _                   ___            _  ___ _           _   
 / __| |_  __ _ _ _ _ __ / __|_ _ ___ __| |/ __| |_  ___ __| |__
 \__ \ ' \/ _` | '_| '_ \ (__| '_/ -_) _` | (__| ' \/ -_) _| / /
 |___/_||_\__,_|_| | .__/\___|_| \___\__,_|\___|_||_\___\__|_\_\
                   |_|                                                                                               
");
		}


		public static void help()
		{
			string help = @"
Required arguments:               Discription:
-------------------               ------------    
/user:<username>                  Username of the account you want to verify 
/pass:<password>                  Password of the account you want to verify 
/local                            Verify credentials on the local machine (use either this one or /ad)
/ad                               Verify credentials in the domain (use either this one or /local)

Optional arguments:               Discription:
-------------------               ------------
/help                             This help menu
/domain:<domain>                  Domain name of the account you want to verify (auto detects current domain name)
/dc:<FQDN or IP>                  FQDN or IP of the domain controller used for authentication (auto detects main DC in current domain)

Usage examples:
---------------
Verify local credentials:         SharpCredCheck.exe /username:john /password:Password123 /local
Verify domain credentials:        SharpCredCheck.exe /username:john /password:Password123 /ad
Verify creds in other domain:     SharpCredCheck.exe /username:john /password:Password123 /domain:contoso.com /dc:dc01.contoso.com /ad
";

			Console.WriteLine(help + "\n");
		}




		static void Main(string[] args)
		{
			try
			{
				string username = "";
				string password = "";
				string domain = "";
				string dc = "";
				bool credCheck;

				logo();

				var arguments = new Dictionary<string, string>();
				foreach (var argument in args)
				{
					var idx = argument.IndexOf(':');
					if (idx > 0)
						arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
					else
						arguments[argument] = string.Empty;
				}

				if (arguments.ContainsKey("/help") || arguments.ContainsKey("-h"))
				{
					help();
				}

				else
				{
					if (arguments.ContainsKey("/user"))
					{
						username = (arguments["/user"]);
					}
					if (arguments.ContainsKey("/pass"))
					{
						password = (arguments["/pass"]);
					}
					if (arguments.ContainsKey("/domain"))
					{
						domain = (arguments["/domain"]);
					}

					if (arguments.ContainsKey("/local") || arguments.ContainsKey("/ad"))
					{
						//checks if the local user credentials are valid
						if (arguments.ContainsKey("/local"))
						{
							if (username.Length != 0 && password.Length != 0)
							{
								credCheck = ValidateMachineCredentials(username, password);
								if (credCheck == true)
								{
									Console.WriteLine($"[+] Valid local user credentials: {username} : {password}\n");
								}
								else
								{
									Console.WriteLine($"[-] Invalid credentials");
								}
							}
							else
							{
								help();
							}
						}

						//checks if the domain credentials are valid
						if (arguments.ContainsKey("/ad"))
						{
							if (username.Length != 0 && password.Length != 0)
							{
								if (domain.Length == 0)
								{
									domain = Domain.GetCurrentDomain().ToString();
								}

								if (arguments.ContainsKey("/dc"))
								{
									dc = (arguments["/dc"]);
									credCheck = ValidateDomainCredentialsDC(username, password, domain, dc);
								}
								else
								{
									credCheck = ValidateDomainCredentials(username, password, domain);
								}

								if (credCheck == true)
								{
									Console.WriteLine($"[+] Valid domain user credentials: {domain}\\{username} : {password}\n");
								}
								else
								{
									Console.WriteLine($"[-] Invalid domain credentials");
								}

							}
							else
							{
								help();
							}
						}
					}
					else
					{
						help();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("[-] " + ex.Message);
			}
		}
	}
}

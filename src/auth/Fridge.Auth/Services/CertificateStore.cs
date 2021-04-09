using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Fridge.Auth.Services
{
	/// <summary>
	/// Provides access to X.509 certificate for token signing.
	/// </summary>
	public class CertificateStore
	{
		/// <summary>
		/// Retrieves X.509 certificate for token signing.
		/// </summary>
		/// <returns>The X.509 certificate.</returns>
		public X509Certificate2 GetCertificate()
		{
			var certificateDirectoryLocation = Directory.GetParent(typeof(LambdaEntryPoint).Assembly.Location).FullName;

			var certificateLocation = Path.Combine(certificateDirectoryLocation, "identityserver-signing-certificate.pfx");

			return new X509Certificate2(certificateLocation);
		}
	}
}

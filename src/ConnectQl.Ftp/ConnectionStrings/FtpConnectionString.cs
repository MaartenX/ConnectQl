// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpConnectionString.cs" company="Winvision bv">
//   Copyright (c) 2016 Winvision bv. All rights reserved.
// </copyright>
// <summary>
//   The FTP connection string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ConnectQl.Ftp.ConnectionStrings
{
    using System;

    /// <summary>
    /// The FTP connection string.
    /// </summary>
    internal class FtpConnectionString : ConnectionString<FtpConnectionString>
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [ConnectionStringField("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        [ConnectionStringField("Uri")]
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [ConnectionStringField("User name", "User")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL.
        /// </summary>
        [ConnectionStringField("Use SSL", "UseSSL")]
        public bool? UseSsl { get; set; }
    }
}
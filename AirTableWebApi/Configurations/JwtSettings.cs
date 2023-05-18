namespace AirTableWebApi.Configurations
{
    public class JwtSettings
    {

        /// <summary>Gets or sets the key.</summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>Gets or sets the issuer.</summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>Gets or sets the access expiration.</summary>
        /// <value>The access expiration.</value>
        public int AccessExpiration { get; set; }

    }
}

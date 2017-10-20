using DiscogsClient.Internal;

namespace MusicInfoLib.API
{
    public class DiscogsAPI
    {
        public DiscogsClient.DiscogsClient client;
        public static DiscogsAPI Instance { get; } = new DiscogsAPI();
        
        private DiscogsAPI()
        {
            // If want to use it, Singleton it
            
            
            //var discogsConfig = ConfigurationManager.GetSection("Discogs") as NameValueCollection;
            
            //Create authentication object using private and public keys: you should fournish real keys here
            /*var oAuthCompleteInformation = new OAuthCompleteInformation(discogsConfig["key"], 
                discogsConfig["secret"], , );
            //Create discogs client using the authentication
            var discogsClient = new DiscogsClient.DiscogsClient(oAuthCompleteInformation);*/
            
            //Create authentication based on Discogs token
            var tokenInformation = new TokenAuthenticationInformation("uiHViDZbDPlVQZNZgvluZypCMBjXoKIUVAYaJbmj");
            //Create discogs client using the authentication
            client = new DiscogsClient.DiscogsClient(tokenInformation);
        }
    }
}
namespace Template.Domain.Constants
{
    public class Errors
    {
        public static readonly string AUTHENTICATION_HEADER_BAD_FORMAT = "Le header Authorization est mal formaté. Il doit commencer par Bearer";
        public static readonly string AUTHENTICATION_HEADER_EMPTY = "Aucun paramètre d'authentification présent dans le header";

        public static readonly string AUTHENTICATION_TOKEN_INVALID_AUDIENCE = "L'audience présente dans le token n'est pas valide";
        public static readonly string AUTHENTICATION_TOKEN_BAD_ENCRYPTION = "Impossible de décrypter le token";
        public static readonly string AUTHENTICATION_TOKEN_INVALID = "Le token est invalide";
        public static readonly string AUTHENTICATION_TOKEN_EXPIRED = "Le token est expiré";

        public static readonly string AUTHENTICATION_UNKNOWN_ERROR = "Erreur inconnue";

        public static readonly string POST_EMPTY_REQUEST = "Aucun paramètre n'a été envoyé à la fonction Azure";
        public static readonly string POST_BAD_REQUEST_CONTENT = "Erreur lors de la vérification du modèle de données";

        public static readonly string BUS_EMPTY_ITEM = "Aucun paramètre n'a été envoyé à la file d'attente";
        public static readonly string BUS_DESERIALIZATION = "Erreur lors de la deserialisation";
    }
}

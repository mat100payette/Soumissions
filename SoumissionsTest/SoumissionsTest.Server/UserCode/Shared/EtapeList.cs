using System.Collections.Generic;

namespace LightSwitchApplication.UserCode.Shared
{
    public static class EtapeList
    {
        public const string PROSPECTION = "PROSPECTION";
        public const string BUDGET = "BUDGET";
        public const string SPECIFICATION = "SPÉCIFICATION";
        public const string SOUMISSION = "SOUMISSION";
        public const string PREVISION = "PRÉVISION";
        public const string DESSIN = "DESSIN D'ATELIER";
        public const string ATTENTE = "ATTENTE D'APPROBATION";
        public const string PRODUCTION = "PRODUCTION";
        public const string LIVRE = "LIVRÉ";
        public const string PERDU = "PERDU";
        public const string ANNULE = "ANNULÉ";

        public static List<string> GetEtapes()
        {
            return new List<string> {
                PROSPECTION,
                BUDGET,
                SPECIFICATION,
                SOUMISSION,
                PREVISION,
                DESSIN,
                ATTENTE,
                PRODUCTION,
                LIVRE,
                PERDU,
                ANNULE
            };
        }

    }
}
CREATE PROCEDURE [dbo].[CleanProduits]
AS BEGIN

	DELETE prod FROM dbo.Produits AS prod WHERE NOT EXISTS (
		SELECT 1
		FROM   dbo.ProjetProduits pp
		WHERE  pp.ProjetProduit_Produit = prod.Id
	);

END

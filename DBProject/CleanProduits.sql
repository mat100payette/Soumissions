CREATE PROCEDURE [dbo].[CleanProduits]
AS BEGIN

	DELETE FROM dbo.Produits WHERE 
		(SELECT COUNT(*) FROM dbo.ProjetProduits WHERE ProjetProduit_Produit = Id) = 0;

END

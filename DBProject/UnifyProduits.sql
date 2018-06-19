CREATE PROCEDURE [dbo].[UnifyProduits]
	@ProduitId int = 0
AS BEGIN

	WHILE(1 = 1)
	BEGIN
	  SELECT @ProduitId = MIN(Id)
	  FROM dbo.Produits WHERE Id > @ProduitId
	  IF @ProduitId IS NULL BREAK

	  UPDATE dbo.ProjetProduits SET ProjetProduit_Produit = @ProduitId 
	  WHERE (SELECT TOP 1 Nom FROM dbo.Produits WHERE Id = ProjetProduit_Produit) = (SELECT TOP 1 Nom FROM dbo.Produits WHERE Id = @ProduitId) 

	  DELETE FROM dbo.Produits WHERE Id > @ProduitId AND Nom LIKE (SELECT TOP 1 Nom FROM dbo.Produits WHERE Id = @ProduitId)
	END

END
CREATE PROCEDURE [dbo].[DeleteAll]
AS BEGIN 
SET NOCOUNT ON;

DELETE FROM [dbo].[Produits]
DELETE FROM [dbo].[Projets]
DELETE FROM [dbo].[ProjetProduits]
DELETE FROM [dbo].[ProjetEtapes]
DELETE FROM [dbo].[ProduitsProduction]

END 

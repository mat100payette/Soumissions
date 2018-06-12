CREATE PROCEDURE [dbo].[UpdateModeles]
AS BEGIN

	UPDATE dbo.Modeles SET NomComplet = CONCAT(Nom, ' ', CAST(Capacite AS VARCHAR));

END
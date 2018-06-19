CREATE PROCEDURE [dbo].[UpdateModeles]
AS BEGIN

	UPDATE dbo.Modeles SET NomComplet = CAST(Nom AS VARCHAR(MAX)) + ' ' + CAST(Capacite AS VARCHAR(MAX));

END
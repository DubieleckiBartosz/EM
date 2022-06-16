IF NOT EXISTS (SELECT * FROM sys.triggers 
           WHERE Name = 'TRG_PerformanceProposalDelete')
BEGIN
EXECUTE ('CREATE TRIGGER [dbo].[TRG_PerformanceProposalDelete] ON [dbo].[PerformanceProposals] INSTEAD OF DELETE
	AS 
	BEGIN
		SET NOCOUNT ON
	
		UPDATE PerformanceProposals
		SET Deleted = 1, DateDeletion = GETDATE()
		FROM PerformanceProposals JOIN deleted
		ON PerformanceProposals.Id = deleted.Id
	
	END'); 
END
-- Checks if the trigger exists and if it does, it
-- should be dropped and recreated.
IF EXISTS(
  SELECT *
    FROM sys.triggers
   WHERE name = N'UpdateImageProtocol'
     AND parent_class_desc = N'OBJECT_OR_COLUMN'
)
	DROP TRIGGER UpdateImageProtocol
GO

CREATE TRIGGER UpdateImageProtocol ON [dbo].[Images]
AFTER UPDATE, INSERT
AS
	BEGIN
		-- Replace all image URL protocols from HTTP to HTTPS on insert and update.
		UPDATE Images
		SET Url = REPLACE(Images.Url, 'http://', 'https://')
		FROM [dbo].[Images] Images
		INNER JOIN inserted I
		on Images.ImageId = I.ImageId
	END
GO
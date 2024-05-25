CREATE PROCEDURE GetActiveNodes
AS
BEGIN
    SELECT * 
    FROM Nodes 
    WHERE IsActive = 1;
END
